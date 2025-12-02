using System.Runtime.InteropServices;
using Silk.NET.WebGPU;

namespace Game;

public sealed class WebGpuAdapter : IDisposable
{
	private unsafe Adapter* _adapter;

	public WebGpuAdapter( WebGpuInstance instance, WebGpuSurface surface )
	{
		unsafe
		{
			var adapterOptions = new RequestAdapterOptions
			{
				CompatibleSurface = surface,
				BackendType = BackendType.Vulkan,
				PowerPreference = PowerPreference.HighPerformance
			};

			var callback = PfnRequestAdapterCallback.From( ( status, adapter, msgPtr, userDataPtr ) =>
			{
				if ( status is RequestAdapterStatus.Success )
				{
					_adapter = adapter;

					Console.WriteLine( "Retrieved WebGPU adapter." );
					return;
				}

				var message = Marshal.PtrToStringUTF8( (IntPtr)msgPtr );
				Console.WriteLine( $"Failed to create WebGPU adapter: {message}" );
			} );

			WebGpu.Wgpu.InstanceRequestAdapter( instance, adapterOptions, callback, null );
		}

		Console.WriteLine( "Created WebGPU adapter." );
	}

	public WebGpuDevice CreateDevice()
	{
		return new WebGpuDevice( this );
	}

	public static unsafe implicit operator Adapter*( WebGpuAdapter adapter )
	{
		return adapter._adapter;
	}

	public void Dispose()
	{
		unsafe
		{
			WebGpu.Wgpu.AdapterRelease( _adapter );
		}
	}
}
