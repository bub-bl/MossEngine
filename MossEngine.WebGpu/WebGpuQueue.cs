using Silk.NET.WebGPU;

namespace MossEngine.WebGpu;

public sealed class WebGpuQueue : IDisposable
{
	private readonly unsafe Queue* _queue;
	private readonly unsafe Device* _device;

	public WebGpuQueue( WebGpuDevice device )
	{
		unsafe
		{
			_device = device;
			_queue = WebGpuApi.Wgpu.DeviceGetQueue( device );
		}
	}

	public static unsafe implicit operator Queue*( WebGpuQueue queue )
	{
		return queue._queue;
	}

	public void Dispose()
	{
		unsafe
		{
			WebGpuApi.Wgpu.QueueRelease( _queue );
		}
	}
}
