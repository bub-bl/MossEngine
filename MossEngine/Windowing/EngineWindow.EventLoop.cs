using MossEngine.System.Time;
using MossEngine.Utility;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;

namespace MossEngine.Windowing;

public abstract unsafe partial class EngineWindow
{
	private void InternalOnWindowLoad()
	{
		OnWindowLoad();
		InitializeInput();
	}

	private void InternalOnWindowUpdate( double deltaTime )
	{
		OnWindowUpdate( deltaTime );
	}

	private void InternalOnWindowFramebufferResize( Vector2D<int> newSize )
	{
		OnWindowResize( newSize );
	}

	protected virtual void OnWindowClose()
	{
	}

	protected virtual void OnWindowFileDrop( string[] obj )
	{
	}

	protected virtual void OnWindowMove( Vector2D<int> obj )
	{
	}

	protected virtual void OnWindowStateChanged( WindowState state )
	{
	}

	protected virtual void OnWindowRender( double deltaTime )
	{
		// If the window is minimized, don't render to avoid errors.
		if ( Window.WindowState is WindowState.Minimized ) return;

		// using var _ = Time.Scope( RealTime.Now, deltaTime );

		Time.Update( RealTime.Now, deltaTime );

		OnBeforeRender( deltaTime );
		OnRender( deltaTime );
		OnAfterRender( deltaTime );
	}

	protected virtual void OnBeforeRender( double deltaTime )
	{
		_queue = Device.GetQueue();
		_commandEncoder = WebGpu.Wgpu.DeviceCreateCommandEncoder( Device, null );

		WebGpu.Wgpu.SurfaceGetCurrentTexture( _surface, ref _surfaceTexture );

		switch ( _surfaceTexture.Status )
		{
			case SurfaceGetCurrentTextureStatus.Success:
				break;
			case SurfaceGetCurrentTextureStatus.Timeout:
			case SurfaceGetCurrentTextureStatus.Outdated:
			case SurfaceGetCurrentTextureStatus.Lost:
				WebGpu.Wgpu.TextureRelease( _surfaceTexture.Texture );
				ConfigureSurface();
				return;
			default:
				throw new Exception( $"Error... {_surfaceTexture.Status}" );
		}

		_surfaceTextureView = WebGpu.Wgpu.TextureCreateView( _surfaceTexture.Texture, null );

		var colorAttachments = stackalloc RenderPassColorAttachment[1];
		colorAttachments[0].View = _surfaceTextureView;
		colorAttachments[0].LoadOp = LoadOp.Clear;
		colorAttachments[0].StoreOp = StoreOp.Store;
		colorAttachments[0].ClearValue = new Color { R = 0, G = 0, B = 0, A = 1 };

		var renderPassDescriptor = new RenderPassDescriptor
		{
			ColorAttachments = colorAttachments, ColorAttachmentCount = 1
		};

		RenderPassEncoder = WebGpu.Wgpu.CommandEncoderBeginRenderPass( _commandEncoder, renderPassDescriptor );
	}

	protected virtual void OnAfterRender( double deltaTime )
	{
		WebGpu.Wgpu.RenderPassEncoderEnd( RenderPassEncoder );
		WebGpu.Wgpu.RenderPassEncoderRelease( RenderPassEncoder );
		WebGpu.Wgpu.TextureViewRelease( _surfaceTextureView );

		var commandBuffer = WebGpu.Wgpu.CommandEncoderFinish( _commandEncoder, null );

		WebGpu.Wgpu.QueueSubmit( _queue, 1, &commandBuffer );

		WebGpu.Wgpu.CommandEncoderRelease( _commandEncoder );
		WebGpu.Wgpu.CommandBufferReference( commandBuffer );

		WebGpu.Wgpu.SurfacePresent( _surface );
		Window.SwapBuffers();
	}

	protected virtual void OnWindowUpdate( double deltaTime )
	{
	}

	protected virtual void OnWindowLoad()
	{
	}

	protected virtual void OnWindowResize( Vector2D<int> newSize )
	{
		// if ( _surface is null || Device is null || _adapter is null ) return;
		if ( Window.WindowState is WindowState.Minimized ) return;
		if ( newSize.X <= 0 || newSize.Y <= 0 ) return;

		var surfaceConfiguration = new SurfaceConfiguration
		{
			Device = Device,
			Width = (uint)newSize.X,
			Height = (uint)newSize.Y,
			Format = SwapChainFormat,
			PresentMode = PresentMode.Fifo,
			Usage = TextureUsage.RenderAttachment | TextureUsage.CopyDst | TextureUsage.CopySrc
		};

		WebGpu.Wgpu.SurfaceConfigure( _surface, surfaceConfiguration );
		Console.WriteLine( $"Surface reconfigurée pour la taille : {newSize.X}x{newSize.Y}" );
	}
}
