using System.Runtime.InteropServices;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using SkiaSharp;

namespace Game.Rendering;

public sealed unsafe class SkiaSurfaceRenderer : IDisposable
{
	private SKSurface? _surface;
	private SKCanvas? _canvas;
	private SKImageInfo _imageInfo;
	private byte[] _pixelBuffer = [];
	private GCHandle _pixelHandle;
	private Texture* _texture;
	private TextureView* _textureView;
	private Vector2D<int> _textureSize;

	public TextureView* TextureView => _textureView;
	public Vector2D<int> TextureSize => _textureSize;

	public bool Render( WebGpuDevice device, WebGpuQueue queue, Vector2D<int> size,
		Action<SKCanvas, Vector2D<int>> draw )
	{
		if ( size.X <= 0 || size.Y <= 0 || !queue.IsValid() ) return false;

		EnsureSurface( size );
		EnsureTexture( device, size );
		if ( _texture is null ) return false;

		_canvas!.Clear( SKColors.Transparent );
		draw( _canvas, size );
		_canvas.Flush();

		UploadToTexture( queue, _texture );
		return _textureView is not null;
	}

	private void EnsureSurface( Vector2D<int> size )
	{
		if ( _surface is not null && _imageInfo.Width == size.X && _imageInfo.Height == size.Y )
			return;

		_surface?.Dispose();
		_canvas = null;

		FreePixelBuffer();

		_imageInfo = new SKImageInfo( size.X, size.Y, SKColorType.Bgra8888, SKAlphaType.Premul );
		_pixelBuffer = new byte[_imageInfo.RowBytes * _imageInfo.Height];
		_pixelHandle = GCHandle.Alloc( _pixelBuffer, GCHandleType.Pinned );

		var pixelPtr = _pixelHandle.AddrOfPinnedObject();
		_surface = SKSurface.Create( _imageInfo, pixelPtr, _imageInfo.RowBytes );
		_canvas = _surface.Canvas;
	}

	private void UploadToTexture( WebGpuQueue queue, Texture* texture )
	{
		var destination = new ImageCopyTexture
		{
			Texture = texture, Aspect = TextureAspect.All, MipLevel = 0, Origin = new Origin3D( 0, 0, 0 )
		};

		var dataLayout = new TextureDataLayout
		{
			BytesPerRow = (uint)_imageInfo.RowBytes, RowsPerImage = (uint)_imageInfo.Height, Offset = 0
		};

		var writeSize = new Extent3D
		{
			Width = (uint)_imageInfo.Width, Height = (uint)_imageInfo.Height, DepthOrArrayLayers = 1
		};

		var queueHandle = (Queue*)queue;
		var pixelPtr = (byte*)_pixelHandle.AddrOfPinnedObject();

		WebGpu.Wgpu.QueueWriteTexture( queueHandle, &destination, pixelPtr, (nuint)_pixelBuffer.Length, &dataLayout,
			&writeSize );
	}

	private void EnsureTexture( WebGpuDevice device, Vector2D<int> size )
	{
		if ( _texture is not null && _textureSize == size ) return;

		ReleaseTexture();

		var desc = new TextureDescriptor
		{
			Dimension = TextureDimension.Dimension2D,
			Format = TextureFormat.Bgra8Unorm,
			Size = new Extent3D { Width = (uint)size.X, Height = (uint)size.Y, DepthOrArrayLayers = 1 },
			MipLevelCount = 1,
			SampleCount = 1,
			Usage = TextureUsage.TextureBinding | TextureUsage.CopyDst
		};

		_texture = WebGpu.Wgpu.DeviceCreateTexture( device, desc );
		_textureView = WebGpu.Wgpu.TextureCreateView( _texture, null );
		_textureSize = size;
	}

	private void ReleaseTexture()
	{
		if ( _textureView is not null )
		{
			WebGpu.Wgpu.TextureViewRelease( _textureView );
			_textureView = null;
		}

		if ( _texture is not null )
		{
			WebGpu.Wgpu.TextureRelease( _texture );
			_texture = null;
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
		FreePixelBuffer();
	}
}
