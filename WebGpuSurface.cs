using Silk.NET.WebGPU;
using Silk.NET.Windowing;

namespace Game;

public sealed class WebGpuSurface : IDisposable
{
	private readonly unsafe Surface* _surface;

	public WebGpuSurface( WebGpuInstance instance, IWindow window )
	{
		unsafe
		{
			_surface = window.CreateWebGPUSurface( WebGpu.Wgpu, instance );
		}

		Console.WriteLine( "Created WebGPU surface." );
	}

	public static unsafe implicit operator Surface*( WebGpuSurface surface )
	{
		return surface._surface;
	}

	public void Dispose()
	{
		unsafe
		{
			WebGpu.Wgpu.SurfaceRelease( _surface );
		}
	}
}
