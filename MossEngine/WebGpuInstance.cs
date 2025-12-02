using Silk.NET.WebGPU;
using Silk.NET.Windowing;

namespace MossEngine;

public sealed class WebGpuInstance : IDisposable
{
	private readonly unsafe Instance* _instance;

	public WebGpuInstance()
	{
		unsafe
		{
			var instanceDescriptor = new InstanceDescriptor();
			_instance = WebGpu.Wgpu.CreateInstance( instanceDescriptor );
		}

		Console.WriteLine( "Created WebGPU instance." );
	}

	public WebGpuSurface CreateSurface( IWindow window )
	{
		return new WebGpuSurface( this, window );
	}

	public static unsafe implicit operator Instance*( WebGpuInstance instance )
	{
		return instance._instance;
	}

	public void Dispose()
	{
		unsafe
		{
			WebGpu.Wgpu.InstanceRelease( _instance );
		}
	}
}
