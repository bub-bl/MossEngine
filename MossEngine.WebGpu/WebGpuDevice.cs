using System.Runtime.InteropServices;
using Silk.NET.WebGPU;

namespace MossEngine.WebGpu;

public sealed class WebGpuDevice : IDisposable
{
	private unsafe Device* _device;

	public WebGpuDevice( WebGpuAdapter adapter )
	{
		unsafe
		{
			var deviceOptions = new DeviceDescriptor();

			var callback = PfnRequestDeviceCallback.From( ( status, device, msgPtr, _ ) =>
			{
				if ( status is RequestDeviceStatus.Success )
				{
					_device = device;
					Console.WriteLine( "Retrieved WebGPU device." );
				}
				else
				{
					var message = Marshal.PtrToStringUTF8( (IntPtr)msgPtr );
					Console.WriteLine( $"Failed to create WebGPU device: {message}" );
				}
			} );

			WebGpuApi.Wgpu.AdapterRequestDevice( adapter, deviceOptions, callback, null );
		}
	}

	public WebGpuQueue GetQueue()
	{
		return new WebGpuQueue( this );
	}

	public static unsafe implicit operator Device*( WebGpuDevice device )
	{
		return device._device;
	}

	public void Dispose()
	{
		unsafe
		{
			WebGpuApi.Wgpu.DeviceRelease( _device );
		}
	}
}
