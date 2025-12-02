using Silk.NET.WebGPU;

namespace MossEngine;

public sealed class WebGpuQueue : IValid, IDisposable
{
	private readonly unsafe Queue* _queue;
	private readonly unsafe Device* _device;

	public bool IsValid
	{
		get
		{
			unsafe
			{
				return _queue is not null;
			}
		}
	}

	public WebGpuQueue( WebGpuDevice device )
	{
		unsafe
		{
			_device = device;
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
