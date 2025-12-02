using Silk.NET.WebGPU;

namespace Game;

public sealed class WebGpuQueue : IDisposable
{
	private readonly unsafe Queue* _queue;

	public WebGpuQueue( WebGpuDevice device )
	{
		unsafe
		{
			_queue = WebGpu.Wgpu.DeviceGetQueue( device );
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
			WebGpu.Wgpu.QueueRelease( _queue );
		}
	}
}
