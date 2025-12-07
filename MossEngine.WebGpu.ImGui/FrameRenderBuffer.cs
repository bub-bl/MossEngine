using Silk.NET.Core.Native;

namespace MossEngine.WebGpu.ImGui;

internal unsafe struct FrameRenderBuffer
{
	public ulong VertexBufferSize;
	public ulong IndexBufferSize;
	public Silk.NET.WebGPU.Buffer* VertexBufferGpu;
	public Silk.NET.WebGPU.Buffer* IndexBufferGpu;
	public GlobalMemory VertexBufferMemory;
	public GlobalMemory IndexBufferMemory;
};
