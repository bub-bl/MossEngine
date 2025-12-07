using System.Numerics;

namespace MossEngine.WebGpu.ImGui;

internal struct Uniforms
{
	public Matrix4x4 Mvp;
	public float Gamma;
}
