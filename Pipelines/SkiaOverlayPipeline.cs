using System.Runtime.InteropServices;
using Silk.NET.Core.Native;
using Silk.NET.WebGPU;

namespace Game.Pipelines;

public sealed unsafe class SkiaOverlayPipeline : IDisposable
{
	private readonly WebGpuDevice _device;
	private readonly Shader _shader;
	private RenderPipeline* _pipeline;
	private Sampler* _sampler;
	private BindGroupLayout* _bindGroupLayout;
	private PipelineLayout* _pipelineLayout;
	private readonly TextureFormat _targetFormat;
	private BindGroup* _activeBindGroup;
	private TextureView* _boundTextureView;

	public SkiaOverlayPipeline( WebGpuDevice device, TextureFormat targetFormat )
	{
		_device = device;
		_targetFormat = targetFormat;
		_shader = Shader.Load( "Shaders/skia_overlay.wgsl", device );

		CreateResources();
	}

	private void CreateResources()
	{
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

		_sampler = WebGpu.Wgpu.DeviceCreateSampler( _device, samplerDesc );

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
		_bindGroupLayout = WebGpu.Wgpu.DeviceCreateBindGroupLayout( _device, layoutDesc );

		var layoutPtr = stackalloc BindGroupLayout*[1];
		layoutPtr[0] = _bindGroupLayout;
		var pipelineLayoutDesc = new PipelineLayoutDescriptor
		{
			BindGroupLayoutCount = 1,
			BindGroupLayouts = layoutPtr
		};

		_pipelineLayout = WebGpu.Wgpu.DeviceCreatePipelineLayout( _device, pipelineLayoutDesc );

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
			Format = _targetFormat,
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
			Multisample = new MultisampleState
			{
				Count = 1,
				Mask = uint.MaxValue,
				AlphaToCoverageEnabled = false
			},
			Layout = _pipelineLayout
		};

		_pipeline = WebGpu.Wgpu.DeviceCreateRenderPipeline( _device, pipelineDesc );

		SilkMarshal.Free( vsEntry );
		SilkMarshal.Free( fsEntry );
	}

	public void Render( RenderPassEncoder* encoder, TextureView* textureView )
	{
		if ( textureView is null ) return;

		if ( _activeBindGroup is null || textureView != _boundTextureView )
		{
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

			_activeBindGroup = WebGpu.Wgpu.DeviceCreateBindGroup( _device, bindGroupDesc );
			_boundTextureView = textureView;
		}

		WebGpu.Wgpu.RenderPassEncoderSetPipeline( encoder, _pipeline );
		WebGpu.Wgpu.RenderPassEncoderSetBindGroup( encoder, 0, _activeBindGroup, 0, null );
		WebGpu.Wgpu.RenderPassEncoderDraw( encoder, 6, 1, 0, 0 );
	}

	public void Dispose()
	{
		_shader.Dispose();

		if ( _activeBindGroup is not null )
		{
			WebGpu.Wgpu.BindGroupRelease( _activeBindGroup );
			_activeBindGroup = null;
		}

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

		if ( _bindGroupLayout is not null )
		{
			WebGpu.Wgpu.BindGroupLayoutRelease( _bindGroupLayout );
			_bindGroupLayout = null;
		}

		if ( _pipelineLayout is not null )
		{
			WebGpu.Wgpu.PipelineLayoutRelease( _pipelineLayout );
			_pipelineLayout = null;
		}
	}
}
