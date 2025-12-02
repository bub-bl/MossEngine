using System.Runtime.InteropServices;
using Silk.NET.WebGPU;

namespace MossEngine.Pipelines;

public sealed class Shader : IDisposable
{
	public readonly ShaderHandle Handle;

	public string Name { get; private set; }

	private Shader( string name, string code, WebGpuDevice device )
	{
		Name = name;
		Handle = ShaderHandle.FromCode( code, device );
	}

	public static Shader Load( string path, WebGpuDevice device )
	{
		var name = Path.GetFileNameWithoutExtension( path );
		var code = File.ReadAllText( path );

		return new Shader( name, code, device );
	}

	public static unsafe implicit operator ShaderModule*( Shader shader )
	{
		return shader.Handle;
	}

	public void Dispose()
	{
		Handle.Dispose();
	}
}

public readonly unsafe struct ShaderHandle : IDisposable
{
	private readonly ShaderModule* _module;

	private ShaderHandle( string code, WebGpuDevice device )
	{
		_module = ShaderUtility.CreateShaderModule( code, device );
	}

	public static ShaderHandle FromCode( string code, WebGpuDevice device )
	{
		return new ShaderHandle( code, device );
	}

	public static implicit operator ShaderModule*( ShaderHandle handle )
	{
		return handle._module;
	}

	public void Dispose()
	{
		WebGpu.Wgpu.ShaderModuleRelease( _module );
	}
}

internal static class ShaderUtility
{
	public static unsafe ShaderModule* CreateShaderModule( string shaderCode, WebGpuDevice device )
	{
		var shader = new ShaderModuleWGSLDescriptor
		{
			Code = (byte*)Marshal.StringToHGlobalAnsi( shaderCode ).ToPointer()
		};
		shader.Chain.SType = SType.ShaderModuleWgslDescriptor;

		var shaderModuleDescriptor = new ShaderModuleDescriptor { NextInChain = (ChainedStruct*)&shader };

		var shaderModule = WebGpu.Wgpu.DeviceCreateShaderModule( device, in shaderModuleDescriptor );
		Console.WriteLine( "Created shader module." );

		return shaderModule;
	}
}
