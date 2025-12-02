using Silk.NET.WebGPU;

namespace Game;

public sealed class Texture2D : IDisposable
{
	private unsafe Texture* _texture;
	public TextureFormat Format { get; }
	public uint Width { get; }
	public uint Height { get; }
	public uint MipLevels { get; }
	public TextureUsage Usage { get; }
	public TextureAspect Aspect { get; }

	public static unsafe implicit operator Texture*( Texture2D texture )
	{
		return texture._texture;
	}

	public void Dispose()
	{
		unsafe
		{
			WebGpu.Wgpu.TextureDestroy( _texture );
			WebGpu.Wgpu.TextureRelease( _texture );
		}
	}
}
