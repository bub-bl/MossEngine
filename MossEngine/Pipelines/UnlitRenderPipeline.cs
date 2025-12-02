using System.Runtime.InteropServices;
using MossEngine.Windowing;
using Silk.NET.WebGPU;

namespace MossEngine.Pipelines;

public class UnlitRenderPipeline( BaseWindow window )
{
	private unsafe RenderPipeline* _renderPipeline;

	public void Initialize()
	{
		var shader = Shader.Load( "Shaders/unlit.wgsl", window.Device );

		unsafe
		{
			var vertexState = new VertexState
			{
				Module = shader.Handle,
				EntryPoint = (byte*)Marshal.StringToHGlobalAnsi( "main_vs" ).ToPointer()
			};

			var blendState = stackalloc BlendState[1];
			blendState[0].Color = new BlendComponent
			{
				SrcFactor = BlendFactor.One,
				DstFactor = BlendFactor.OneMinusSrcAlpha,
				Operation = BlendOperation.Add
			};
			blendState[0].Alpha = new BlendComponent
			{
				SrcFactor = BlendFactor.One,
				DstFactor = BlendFactor.OneMinusSrcAlpha,
				Operation = BlendOperation.Add
			};

			var colorTargetState = stackalloc ColorTargetState[1];
			colorTargetState[0].WriteMask = ColorWriteMask.All;
			colorTargetState[0].Format = window.SwapChainFormat;
			colorTargetState[0].Blend = blendState;

			var fragmentState = new FragmentState
			{
				Module = shader.Handle,
				EntryPoint = (byte*)Marshal.StringToHGlobalAnsi( "main_fs" ).ToPointer(),
				Targets = colorTargetState,
				TargetCount = 1
			};

			var renderPipelineDescriptor = new RenderPipelineDescriptor
			{
				Vertex = vertexState,
				Fragment = &fragmentState,
				Multisample = new MultisampleState { Mask = 0xFFFFFFFF, Count = 1, AlphaToCoverageEnabled = false },
				Primitive = new PrimitiveState
				{
					CullMode = CullMode.Back,
					FrontFace = FrontFace.Ccw,
					Topology = PrimitiveTopology.TriangleList
				}
			};

			_renderPipeline = WebGpu.Wgpu.DeviceCreateRenderPipeline( window.Device, in renderPipelineDescriptor );
		}

		Console.WriteLine( "Created render pipeline." );
	}

	public void Render()
	{
		unsafe
		{
			WebGpu.Wgpu.RenderPassEncoderSetPipeline( window.RenderPassEncoder, _renderPipeline );
			WebGpu.Wgpu.RenderPassEncoderDraw( window.RenderPassEncoder, 3, 1, 0, 0 );
		}
	}
}
