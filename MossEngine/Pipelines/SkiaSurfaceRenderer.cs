using System.Runtime.InteropServices;
using MossEngine.System;
using Silk.NET.Core.Native;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using SkiaSharp;

namespace MossEngine.Pipelines;

public sealed class SkiaRenderPipeline : IDisposable
{
	private SKSurface? _surface;
	private SKCanvas? _canvas;
	private SKImageInfo _imageInfo;
	private byte[] _pixelBuffer = [];
	private GCHandle _pixelHandle;
	private unsafe Texture* _texture;
	private Shader? _shader;
	private unsafe RenderPipeline* _pipeline;
	private unsafe Sampler* _sampler;
	private unsafe BindGroupLayout* _bindGroupLayout;
	private unsafe PipelineLayout* _pipelineLayout;
	private unsafe BindGroup* _activeBindGroup;
	private unsafe TextureView* _boundTextureView;
	private TextureFormat _currentTargetFormat;
	private bool _pipelineInitialized;

	public unsafe TextureView* TextureView { get; private set; }

	public Vector2D<int> TextureSize { get; private set; }

	public unsafe void RenderOverlay( WebGpuDevice device, WebGpuQueue queue, RenderPassEncoder* encoder,
		TextureFormat targetFormat, Vector2D<int> size, Action<SKCanvas, Vector2D<int>> draw )
	{
		if ( !IsQueueValid( queue ) ) return;
		if ( !RenderToTexture( device, queue, size, draw ) ) return;

		EnsurePipeline( device, targetFormat );
		EnsureBindGroup( device, TextureView );

		WebGpu.Wgpu.RenderPassEncoderSetPipeline( encoder, _pipeline );
		WebGpu.Wgpu.RenderPassEncoderSetBindGroup( encoder, 0, _activeBindGroup, 0, null );
		WebGpu.Wgpu.RenderPassEncoderDraw( encoder, 6, 1, 0, 0 );
	}

	private static bool IsQueueValid( WebGpuQueue queue ) => queue.IsValid();
	
	private unsafe bool RenderToTexture( WebGpuDevice device, WebGpuQueue queue, Vector2D<int> size,
		Action<SKCanvas, Vector2D<int>> draw )
	{
		// TODO - Find a way to avoid useless rendering
		
		if ( size.X <= 0 || size.Y <= 0 ) return false;

		EnsureSurface( size );
		EnsureTexture( device, size );

		if ( _texture is null ) return false;
		
		_canvas!.Clear( SKColors.Transparent );
		draw( _canvas, size );
		_canvas.Flush();

		UploadToTexture( queue, _texture );
		return TextureView is not null;
	}

	private void EnsureSurface( Vector2D<int> size )
	{
		if ( _surface is not null && _imageInfo.Width == size.X && _imageInfo.Height == size.Y )
			return;

		_surface?.Dispose();
		_canvas = null;

		FreePixelBuffer();

		_imageInfo = new SKImageInfo( size.X, size.Y, SKColorType.Bgra8888, SKAlphaType.Premul,
			SKColorSpace.CreateSrgb() );
		_pixelBuffer = new byte[_imageInfo.RowBytes * _imageInfo.Height];
		_pixelHandle = GCHandle.Alloc( _pixelBuffer, GCHandleType.Pinned );

		var pixelPtr = _pixelHandle.AddrOfPinnedObject();
		_surface = SKSurface.Create( _imageInfo, pixelPtr, _imageInfo.RowBytes );
		_canvas = _surface.Canvas;
	}

	private unsafe void UploadToTexture( WebGpuQueue queue, Texture* texture )
	{
		var destination = new ImageCopyTexture
		{
			Texture = texture,
			Aspect = TextureAspect.All,
			MipLevel = 0,
			Origin = new Origin3D( 0, 0, 0 )
		};

		var dataLayout = new TextureDataLayout
		{
			BytesPerRow = (uint)_imageInfo.RowBytes,
			RowsPerImage = (uint)_imageInfo.Height,
			Offset = 0
		};

		var writeSize = new Extent3D
		{
			Width = (uint)_imageInfo.Width,
			Height = (uint)_imageInfo.Height,
			DepthOrArrayLayers = 1
		};

		var queueHandle = (Queue*)queue;
		var pixelPtr = (byte*)_pixelHandle.AddrOfPinnedObject();

		WebGpu.Wgpu.QueueWriteTexture( queueHandle, &destination, pixelPtr, (nuint)_pixelBuffer.Length, &dataLayout,
			&writeSize );
	}

	private unsafe void EnsureTexture( WebGpuDevice device, Vector2D<int> size )
	{
		if ( _texture is not null && TextureSize == size ) return;

		ReleaseTexture();

		var desc = new TextureDescriptor
		{
			Dimension = TextureDimension.Dimension2D,
			Format = TextureFormat.Bgra8UnormSrgb,
			Size = new Extent3D { Width = (uint)size.X, Height = (uint)size.Y, DepthOrArrayLayers = 1 },
			MipLevelCount = 1,
			SampleCount = 1,
			Usage = TextureUsage.TextureBinding | TextureUsage.CopyDst
		};

		_texture = WebGpu.Wgpu.DeviceCreateTexture( device, desc );
		TextureView = WebGpu.Wgpu.TextureCreateView( _texture, null );
		TextureSize = size;
	}

	private unsafe void ReleaseTexture()
	{
		if ( TextureView is not null )
		{
			WebGpu.Wgpu.TextureViewRelease( TextureView );
			TextureView = null;
		}

		if ( _texture is not null )
		{
			WebGpu.Wgpu.TextureRelease( _texture );
			_texture = null;
		}
	}

	private unsafe void EnsurePipeline( WebGpuDevice device, TextureFormat targetFormat )
	{
		if ( _pipelineInitialized && _pipeline is not null && _currentTargetFormat == targetFormat ) return;

		DestroyPipelineResources();
		_shader ??= Shader.Load( "Shaders/skia_overlay.wgsl", device );
		_currentTargetFormat = targetFormat;

		var samplerDesc = new SamplerDescriptor
		{
			MinFilter = FilterMode.Linear,
			MagFilter = FilterMode.Linear,
			MipmapFilter = MipmapFilterMode.Linear,
			AddressModeU = AddressMode.ClampToEdge,
			AddressModeV = AddressMode.ClampToEdge,
			AddressModeW = AddressMode.ClampToEdge,
			MaxAnisotropy = 1
		};

		_sampler = WebGpu.Wgpu.DeviceCreateSampler( device, samplerDesc );

		var layoutEntries = stackalloc BindGroupLayoutEntry[2];
		layoutEntries[0].Binding = 0;
		layoutEntries[0].Visibility = ShaderStage.Fragment;
		layoutEntries[0].Sampler.Type = SamplerBindingType.Filtering;

		layoutEntries[1].Binding = 1;
		layoutEntries[1].Visibility = ShaderStage.Fragment;
		layoutEntries[1].Texture.SampleType = TextureSampleType.Float;
		layoutEntries[1].Texture.ViewDimension = TextureViewDimension.Dimension2D;
		layoutEntries[1].Texture.Multisampled = false;

		var layoutDesc = new BindGroupLayoutDescriptor { EntryCount = 2, Entries = layoutEntries };
		_bindGroupLayout = WebGpu.Wgpu.DeviceCreateBindGroupLayout( device, layoutDesc );

		var layoutPtr = stackalloc BindGroupLayout*[1];
		layoutPtr[0] = _bindGroupLayout;

		var pipelineLayoutDesc = new PipelineLayoutDescriptor
		{
			BindGroupLayoutCount = 1,
			BindGroupLayouts = layoutPtr
		};

		_pipelineLayout = WebGpu.Wgpu.DeviceCreatePipelineLayout( device, pipelineLayoutDesc );

		var vsEntry = SilkMarshal.StringToPtr( "vs_main" );
		var fsEntry = SilkMarshal.StringToPtr( "fs_main" );

		var vertexState = new VertexState
		{
			Module = _shader.Handle,
			EntryPoint = (byte*)vsEntry,
			BufferCount = 0,
			Buffers = null
		};

		var blendState = new BlendState
		{
			Color = new BlendComponent
			{
				SrcFactor = BlendFactor.SrcAlpha,
				DstFactor = BlendFactor.OneMinusSrcAlpha,
				Operation = BlendOperation.Add
			},
			Alpha = new BlendComponent
			{
				SrcFactor = BlendFactor.One,
				DstFactor = BlendFactor.OneMinusSrcAlpha,
				Operation = BlendOperation.Add
			}
		};

		var colorTargetState = new ColorTargetState
		{
			Format = targetFormat,
			Blend = &blendState,
			WriteMask = ColorWriteMask.All
		};

		var fragmentState = new FragmentState
		{
			Module = _shader.Handle,
			EntryPoint = (byte*)fsEntry,
			TargetCount = 1,
			Targets = &colorTargetState
		};

		var pipelineDesc = new RenderPipelineDescriptor
		{
			Vertex = vertexState,
			Fragment = &fragmentState,
			Primitive = new PrimitiveState
			{
				Topology = PrimitiveTopology.TriangleList,
				StripIndexFormat = IndexFormat.Undefined,
				FrontFace = FrontFace.Ccw,
				CullMode = CullMode.None
			},
			Multisample = new MultisampleState { Count = 1, Mask = uint.MaxValue, AlphaToCoverageEnabled = false },
			Layout = _pipelineLayout
		};

		_pipeline = WebGpu.Wgpu.DeviceCreateRenderPipeline( device, pipelineDesc );

		SilkMarshal.Free( vsEntry );
		SilkMarshal.Free( fsEntry );

		_pipelineInitialized = true;
	}

	private unsafe void EnsureBindGroup( WebGpuDevice device, TextureView* textureView )
	{
		if ( textureView is null ) return;
		if ( _activeBindGroup is not null && _boundTextureView == textureView ) return;

		if ( _activeBindGroup is not null )
		{
			WebGpu.Wgpu.BindGroupRelease( _activeBindGroup );
			_activeBindGroup = null;
		}

		var bindGroupEntries = stackalloc BindGroupEntry[2];
		bindGroupEntries[0].Binding = 0;
		bindGroupEntries[0].Sampler = _sampler;

		bindGroupEntries[1].Binding = 1;
		bindGroupEntries[1].TextureView = textureView;

		var bindGroupDesc = new BindGroupDescriptor
		{
			Layout = _bindGroupLayout,
			EntryCount = 2,
			Entries = bindGroupEntries
		};

		_activeBindGroup = WebGpu.Wgpu.DeviceCreateBindGroup( device, bindGroupDesc );
		_boundTextureView = textureView;
	}

	private static TextureFormat GetFormat( TextureFormat format ) => format;

	private unsafe void DestroyPipelineResources()
	{
		if ( _activeBindGroup is not null )
		{
			WebGpu.Wgpu.BindGroupRelease( _activeBindGroup );
			_activeBindGroup = null;
		}

		_boundTextureView = null;
		_pipelineInitialized = false;

		if ( _pipeline is not null )
		{
			WebGpu.Wgpu.RenderPipelineRelease( _pipeline );
			_pipeline = null;
		}

		if ( _sampler is not null )
		{
			WebGpu.Wgpu.SamplerRelease( _sampler );
			_sampler = null;
		}

		if ( _pipelineLayout is not null )
		{
			WebGpu.Wgpu.PipelineLayoutRelease( _pipelineLayout );
			_pipelineLayout = null;
		}

		if ( _bindGroupLayout is not null )
		{
			WebGpu.Wgpu.BindGroupLayoutRelease( _bindGroupLayout );
			_bindGroupLayout = null;
		}
	}

	private void FreePixelBuffer()
	{
		if ( _pixelHandle.IsAllocated )
			_pixelHandle.Free();

		_pixelBuffer = [];
	}

	public void Dispose()
	{
		_surface?.Dispose();
		_canvas = null;

		ReleaseTexture();
		DestroyPipelineResources();

		_shader?.Dispose();

		FreePixelBuffer();
	}
}
