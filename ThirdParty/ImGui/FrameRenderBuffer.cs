using Silk.NET.Core.Native;
using Buffer = Silk.NET.WebGPU.Buffer;

namespace Game.ThirdParty.ImGui;

internal unsafe struct FrameRenderBuffer
{
	public ulong VertexBufferSize;
	public ulong IndexBufferSize;
	public Buffer* VertexBufferGpu;
	public Buffer* IndexBufferGpu;
	public GlobalMemory VertexBufferMemory;
	public GlobalMemory IndexBufferMemory;
};
