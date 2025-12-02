using System.Runtime.InteropServices;
using Silk.NET.WebGPU;

namespace MossEngine;

internal static class WebGpu
{
	public static WebGPU Wgpu { get; private set; } = null!;
	public static WebGpuInstance Instance { get; private set; } = null!;

	public static void Initialize()
	{
		Wgpu = WebGPU.GetApi();
		Console.WriteLine( "Created WebGPU api." );

		Instance = new WebGpuInstance();
	}

	public static void ConfigureDebugCallback( WebGpuDevice device )
	{
		unsafe
		{
			var callback = PfnErrorCallback.From( ( type, msgPtr, userDataPtr ) =>
			{
				var message = Marshal.PtrToStringUTF8( (IntPtr)msgPtr );
				Console.WriteLine( $"WGPU Unhandled Error: {type} -> {message}" );
			} );

			Wgpu.DeviceSetUncapturedErrorCallback( device, callback, null );
			Console.WriteLine( "Created WebGPU debug callback." );
		}
	}

	public static void Dispose()
	{
		Wgpu.Dispose();
		Console.WriteLine( "Disposed WebGPU." );
	}
}
