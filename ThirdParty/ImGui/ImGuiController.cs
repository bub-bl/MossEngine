using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Game;
using Game.Pipelines;
using Game.ThirdParty.ImGui;
using ImGuiNET;
using Silk.NET.Core.Native;
using Silk.NET.Input;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using Buffer = Silk.NET.WebGPU.Buffer;

public unsafe class ImGuiController : IDisposable
{
	private readonly WebGpuDevice _device;
	private readonly WebGpuQueue _queue;
	private readonly IView _view;
	private readonly IInputContext _inputContext;
	private readonly TextureFormat _swapChainFormat;
	private readonly TextureFormat? _depthFormat;
	private readonly uint _framesInFlight;

	private Shader _shader = null!;

	private Texture* _fontTexture;
	private Sampler* _fontSampler;
	private TextureView* _fontView;

	private BindGroupLayout* _commonBindGroupLayout;
	private BindGroupLayout* _imageBindGroupLayout;
	private RenderPipeline* _renderPipeline;

	private BindGroup* _commonBindGroup;

	private Buffer* _uniformsBuffer;

	private WindowRenderBuffers _windowRenderBuffers;

	private readonly Dictionary<nint, nint> _viewsById = [];
	private readonly List<char> _pressedChars = [];
	private readonly Dictionary<Key, bool> _keyEvents = [];

	public ImGuiController( WebGpuDevice device, IView view, IInputContext inputContext, uint framesInFlight,
		TextureFormat swapChainFormat, TextureFormat? depthFormat )
	{
		_device = device;
		_view = view;
		_inputContext = inputContext;
		_swapChainFormat = swapChainFormat;
		_depthFormat = depthFormat;
		_framesInFlight = framesInFlight;
		_queue = _device.GetQueue();

		Init();
	}

	public void Update( float delta )
	{
		SetPerFrameImGuiData( delta );
		UpdateImGuiInput();
		ImGui.NewFrame();
	}

	public void Render( RenderPassEncoder* encoder )
	{
		ImGui.Render();
		DrawImGui( encoder );
	}

	private BindGroup* BindImGuiTextureView( TextureView* view )
	{
		var id = (nint)view;

		if ( _viewsById.TryGetValue( id, out var ptr ) )
			return (BindGroup*)ptr;

		var imageEntry = new BindGroupEntry
		{
			Binding = 0,
			Buffer = null,
			Offset = 0,
			Size = 0,
			Sampler = null,
			TextureView = view,
		};

		var imageDesc = new BindGroupDescriptor
		{
			Layout = _imageBindGroupLayout,
			EntryCount = 1,
			Entries = &imageEntry
		};

		var bindGroup = WebGpu.Wgpu.DeviceCreateBindGroup( _device, in imageDesc );
		_viewsById[id] = (nint)bindGroup;

		return bindGroup;
	}

	public IntPtr RegisterTexture( TextureView* view )
	{
		BindImGuiTextureView( view );
		return (IntPtr)view;
	}

	private void Init()
	{
		var context = ImGui.CreateContext();
		ImGui.SetCurrentContext( context );

		_inputContext.Keyboards[0].KeyUp += KeyUp;
		_inputContext.Keyboards[0].KeyDown += KeyDown;
		_inputContext.Keyboards[0].KeyChar += KeyChar;

		InitShaders();
		InitFonts();
		InitBindGroupLayouts();
		InitPipeline();
		InitUniformBuffers();
		InitBindGroups();

		SetPerFrameImGuiData( 1f / 60f );
	}

	private void InitShaders()
	{
		_shader = Shader.Load( "Shaders/imgui.wgsl", _device );
	}

	private void InitFonts()
	{
		ImGui.GetIO().Fonts.GetTexDataAsRGBA32( out byte* pixels, out var width, out var height, out var sizePerPixel );

		var textureDescriptor = new TextureDescriptor
		{
			Dimension = TextureDimension.Dimension2D,
			Size = new Extent3D { Width = (uint)width, Height = (uint)height, DepthOrArrayLayers = 1, },
			SampleCount = 1,
			Format = TextureFormat.Rgba8Unorm,
			MipLevelCount = 1,
			Usage = TextureUsage.CopyDst | TextureUsage.TextureBinding
		};

		_fontTexture = WebGpu.Wgpu.DeviceCreateTexture( _device, in textureDescriptor );

		var textureViewDescriptor = new TextureViewDescriptor
		{
			Dimension = TextureViewDimension.Dimension2D,
			Format = TextureFormat.Rgba8Unorm,
			BaseMipLevel = 0,
			MipLevelCount = 1,
			BaseArrayLayer = 0,
			ArrayLayerCount = 1,
			Aspect = TextureAspect.All
		};

		_fontView = WebGpu.Wgpu.TextureCreateView( _fontTexture, in textureViewDescriptor );

		var imageCopyTexture =
			new ImageCopyTexture { Texture = _fontTexture, MipLevel = 0, Aspect = TextureAspect.All, };

		var textureDataLayout = new TextureDataLayout
		{
			Offset = 0,
			BytesPerRow = (uint)(width * sizePerPixel),
			RowsPerImage = (uint)height,
		};

		var extent = new Extent3D { Height = (uint)height, Width = (uint)width, DepthOrArrayLayers = 1, };

		WebGpu.Wgpu.QueueWriteTexture( _queue, &imageCopyTexture, pixels, (nuint)(width * height * sizePerPixel),
			in textureDataLayout, in extent );

		var samplerDescriptor = new SamplerDescriptor
		{
			MinFilter = FilterMode.Linear,
			MagFilter = FilterMode.Linear,
			MipmapFilter = MipmapFilterMode.Linear,
			AddressModeU = AddressMode.Repeat,
			AddressModeV = AddressMode.Repeat,
			AddressModeW = AddressMode.Repeat,
			MaxAnisotropy = 1,
		};

		_fontSampler = WebGpu.Wgpu.DeviceCreateSampler( _device, in samplerDescriptor );

		ImGui.GetIO().Fonts.SetTexID( (nint)_fontView );
	}

	private void InitBindGroupLayouts()
	{
		var commonBgLayoutEntries = stackalloc BindGroupLayoutEntry[2];
		commonBgLayoutEntries[0].Binding = 0;
		commonBgLayoutEntries[0].Visibility = ShaderStage.Vertex | ShaderStage.Fragment;
		commonBgLayoutEntries[0].Buffer.Type = BufferBindingType.Uniform;
		commonBgLayoutEntries[1].Binding = 1;
		commonBgLayoutEntries[1].Visibility = ShaderStage.Fragment;
		commonBgLayoutEntries[1].Sampler.Type = SamplerBindingType.Filtering;

		var imageBgLayoutEntries = stackalloc BindGroupLayoutEntry[1];
		imageBgLayoutEntries[0].Binding = 0;
		imageBgLayoutEntries[0].Visibility = ShaderStage.Fragment;
		imageBgLayoutEntries[0].Texture.SampleType = TextureSampleType.Float;
		imageBgLayoutEntries[0].Texture.ViewDimension = TextureViewDimension.Dimension2D;

		var commonBgLayoutDesc = new BindGroupLayoutDescriptor { EntryCount = 2, Entries = commonBgLayoutEntries, };
		var imageBgLayoutDesc = new BindGroupLayoutDescriptor { EntryCount = 1, Entries = imageBgLayoutEntries, };

		_commonBindGroupLayout = WebGpu.Wgpu.DeviceCreateBindGroupLayout( _device, in commonBgLayoutDesc );
		_imageBindGroupLayout = WebGpu.Wgpu.DeviceCreateBindGroupLayout( _device, in imageBgLayoutDesc );
	}

	private void InitPipeline()
	{
		var bgLayouts = stackalloc BindGroupLayout*[2];
		bgLayouts[0] = _commonBindGroupLayout;
		bgLayouts[1] = _imageBindGroupLayout;

		var layoutDesc = new PipelineLayoutDescriptor { BindGroupLayoutCount = 2, BindGroupLayouts = bgLayouts };
		var layout = WebGpu.Wgpu.DeviceCreatePipelineLayout( _device, in layoutDesc );

		var vertexEntry = SilkMarshal.StringToPtr( "vs_main" );
		var fragmentEntry = SilkMarshal.StringToPtr( "fs_main" );

		var vertexAttrib = stackalloc VertexAttribute[3];
		vertexAttrib[0].Format = VertexFormat.Float32x2;
		vertexAttrib[0].Offset = (ulong)Marshal.OffsetOf<ImDrawVert>( nameof( ImDrawVert.pos ) );
		vertexAttrib[0].ShaderLocation = 0;
		vertexAttrib[1].Format = VertexFormat.Float32x2;
		vertexAttrib[1].Offset = (ulong)Marshal.OffsetOf<ImDrawVert>( nameof( ImDrawVert.uv ) );
		vertexAttrib[1].ShaderLocation = 1;
		vertexAttrib[2].Format = VertexFormat.Unorm8x4;
		vertexAttrib[2].Offset = (ulong)Marshal.OffsetOf<ImDrawVert>( nameof( ImDrawVert.col ) );
		vertexAttrib[2].ShaderLocation = 2;

		var vbLayout = new VertexBufferLayout
		{
			ArrayStride = (ulong)sizeof( ImDrawVert ),
			StepMode = VertexStepMode.Vertex,
			AttributeCount = 3,
			Attributes = vertexAttrib
		};

		var blendState = new BlendState();
		blendState.Alpha.Operation = BlendOperation.Add;
		blendState.Alpha.SrcFactor = BlendFactor.One;
		blendState.Alpha.DstFactor = BlendFactor.OneMinusSrcAlpha;
		blendState.Color.Operation = BlendOperation.Add;
		blendState.Color.SrcFactor = BlendFactor.SrcAlpha;
		blendState.Color.DstFactor = BlendFactor.OneMinusSrcAlpha;

		var colorTargetState = new ColorTargetState
		{
			Blend = &blendState,
			Format = _swapChainFormat,
			WriteMask = ColorWriteMask.All
		};

		var fragmentState = new FragmentState
		{
			Module = _shader,
			EntryPoint = (byte*)fragmentEntry,
			TargetCount = 1,
			Targets = &colorTargetState
		};

		var renderPipelineDescriptor = new RenderPipelineDescriptor
		{
			Vertex =
				new VertexState
				{
					Module = _shader,
					EntryPoint = (byte*)vertexEntry,
					Buffers = &vbLayout,
					BufferCount = 1
				},
			Primitive = new PrimitiveState
			{
				Topology = PrimitiveTopology.TriangleList,
				StripIndexFormat = IndexFormat.Undefined,
				FrontFace = FrontFace.Ccw,
				CullMode = CullMode.None
			},
			Multisample = new MultisampleState { Count = 1, Mask = ~0u, AlphaToCoverageEnabled = false },
			Fragment = &fragmentState,
			Layout = layout
		};

		if ( _depthFormat is not null )
		{
			var depthStencilState = new DepthStencilState
			{
				Format = (TextureFormat)_depthFormat,
				DepthWriteEnabled = false,
				DepthCompare = CompareFunction.Always,
				StencilFront = new StencilFaceState { Compare = CompareFunction.Always },
				StencilBack = new StencilFaceState { Compare = CompareFunction.Always }
			};

			renderPipelineDescriptor.DepthStencil = &depthStencilState;
		}

		_renderPipeline = WebGpu.Wgpu.DeviceCreateRenderPipeline( _device, in renderPipelineDescriptor );

		SilkMarshal.Free( vertexEntry );
		SilkMarshal.Free( fragmentEntry );

		WebGpu.Wgpu.PipelineLayoutRelease( layout );
	}

	private void InitUniformBuffers()
	{
		var bufferDescriptor = new BufferDescriptor
		{
			Usage = BufferUsage.CopyDst | BufferUsage.Uniform,
			Size = (ulong)Helpers.Align( sizeof( Uniforms ), 16 )
		};

		_uniformsBuffer = WebGpu.Wgpu.DeviceCreateBuffer( _device, in bufferDescriptor );
	}

	private void InitBindGroups()
	{
		var bindGroupEntries = stackalloc BindGroupEntry[2];
		bindGroupEntries[0].Binding = 0;
		bindGroupEntries[0].Buffer = _uniformsBuffer;
		bindGroupEntries[0].Offset = 0;
		bindGroupEntries[0].Size = (ulong)Helpers.Align( sizeof( Uniforms ), 16 );
		bindGroupEntries[0].Sampler = null;
		bindGroupEntries[1].Binding = 1;
		bindGroupEntries[1].Buffer = null;
		bindGroupEntries[1].Offset = 0;
		bindGroupEntries[1].Size = 0;
		bindGroupEntries[1].Sampler = _fontSampler;

		var bgCommonDesc = new BindGroupDescriptor
		{
			Layout = _commonBindGroupLayout,
			EntryCount = 2,
			Entries = bindGroupEntries
		};

		_commonBindGroup = WebGpu.Wgpu.DeviceCreateBindGroup( _device, in bgCommonDesc );

		BindImGuiTextureView( _fontView );
	}

	private static bool TryMapKeys( Key key, out ImGuiKey imGuiKey )
	{
		imGuiKey = key switch
		{
			Key.Backspace => ImGuiKey.Backspace,
			Key.Tab => ImGuiKey.Tab,
			Key.Enter => ImGuiKey.Enter,
			Key.CapsLock => ImGuiKey.CapsLock,
			Key.Escape => ImGuiKey.Escape,
			Key.Space => ImGuiKey.Space,
			Key.PageUp => ImGuiKey.PageUp,
			Key.PageDown => ImGuiKey.PageDown,
			Key.End => ImGuiKey.End,
			Key.Home => ImGuiKey.Home,
			Key.Left => ImGuiKey.LeftArrow,
			Key.Right => ImGuiKey.RightArrow,
			Key.Up => ImGuiKey.UpArrow,
			Key.Down => ImGuiKey.DownArrow,
			Key.PrintScreen => ImGuiKey.PrintScreen,
			Key.Insert => ImGuiKey.Insert,
			Key.Delete => ImGuiKey.Delete,
			>= Key.Number0 and <= Key.Number9 => ImGuiKey._0 + (key - Key.Number0),
			>= Key.A and <= Key.Z => ImGuiKey.A + (key - Key.A),
			>= Key.Keypad0 and <= Key.Keypad9 => ImGuiKey.Keypad0 + (key - Key.Keypad0),
			Key.KeypadMultiply => ImGuiKey.KeypadMultiply,
			Key.KeypadAdd => ImGuiKey.KeypadAdd,
			Key.KeypadSubtract => ImGuiKey.KeypadSubtract,
			Key.KeypadDecimal => ImGuiKey.KeypadDecimal,
			Key.KeypadDivide => ImGuiKey.KeypadDivide,
			Key.KeypadEqual => ImGuiKey.KeypadEqual,
			>= Key.F1 and <= Key.F1 => ImGuiKey.F1 + (key - Key.F1),
			Key.NumLock => ImGuiKey.NumLock,
			Key.ScrollLock => ImGuiKey.ScrollLock,
			Key.ShiftLeft or Key.ShiftRight => ImGuiKey.ModShift,
			Key.ControlLeft or Key.ControlRight => ImGuiKey.ModCtrl,
			Key.SuperLeft or Key.SuperRight => ImGuiKey.ModSuper,
			Key.AltLeft or Key.AltRight => ImGuiKey.ModAlt,
			Key.Semicolon => ImGuiKey.Semicolon,
			Key.Equal => ImGuiKey.Equal,
			Key.Comma => ImGuiKey.Comma,
			Key.Minus => ImGuiKey.Minus,
			Key.Period => ImGuiKey.Period,
			Key.GraveAccent => ImGuiKey.GraveAccent,
			Key.LeftBracket => ImGuiKey.LeftBracket,
			Key.RightBracket => ImGuiKey.RightBracket,
			Key.Apostrophe => ImGuiKey.Apostrophe,
			Key.Slash => ImGuiKey.Slash,
			Key.BackSlash => ImGuiKey.Backslash,
			Key.Pause => ImGuiKey.Pause,
			_ => ImGuiKey.None,
		};

		return imGuiKey != ImGuiKey.None;
	}

	private void UpdateImGuiInput()
	{
		var io = ImGui.GetIO();

		var mouseState = _inputContext.Mice[0];
		_ = _inputContext.Keyboards[0];

		io.MouseDown[0] = mouseState.IsButtonPressed( MouseButton.Left );
		io.MouseDown[1] = mouseState.IsButtonPressed( MouseButton.Right );
		io.MouseDown[2] = mouseState.IsButtonPressed( MouseButton.Middle );

		io.MousePos = new Vector2( mouseState.Position.X, mouseState.Position.Y );

		var wheel = mouseState.ScrollWheels[0];
		io.MouseWheel = wheel.Y;
		io.MouseWheelH = wheel.X;

		io.AddInputCharactersUTF8( CollectionsMarshal.AsSpan( _pressedChars ) );
		_pressedChars.Clear();

		foreach ( var evt in _keyEvents )
		{
			if ( TryMapKeys( evt.Key, out var imGuiKey ) )
				io.AddKeyEvent( imGuiKey, evt.Value );
		}

		_keyEvents.Clear();
	}

	private void SetPerFrameImGuiData( float deltaSeconds )
	{
		var io = ImGui.GetIO();
		var windowSize = _view.Size;

		io.DisplaySize = new Vector2( windowSize.X, windowSize.Y );

		if ( windowSize is { X: > 0, Y: > 0 } )
		{
			io.DisplayFramebufferScale = new Vector2( _view.FramebufferSize.X / windowSize.X,
				_view.FramebufferSize.Y / windowSize.Y );
		}

		io.DeltaTime = deltaSeconds;
	}

	private void DrawImGui( RenderPassEncoder* encoder )
	{
		var drawData = ImGui.GetDrawData();
		drawData.ScaleClipRects( ImGui.GetIO().DisplayFramebufferScale );

		var framebufferWidth = (int)(drawData.DisplaySize.X * drawData.FramebufferScale.X);
		var framebufferHeight = (int)(drawData.DisplaySize.Y * drawData.FramebufferScale.Y);

		if ( framebufferWidth <= 0 || framebufferHeight <= 0 )
			return;

		if ( _windowRenderBuffers.FrameRenderBuffers is null || _windowRenderBuffers.FrameRenderBuffers.Length is 0 )
		{
			_windowRenderBuffers.Index = 0;
			_windowRenderBuffers.Count = _framesInFlight;
			_windowRenderBuffers.FrameRenderBuffers = new FrameRenderBuffer[_windowRenderBuffers.Count];
		}

		_windowRenderBuffers.Index = (_windowRenderBuffers.Index + 1) % _windowRenderBuffers.Count;

		ref var frameRenderBuffer =
			ref _windowRenderBuffers.FrameRenderBuffers[_windowRenderBuffers.Index];

		if ( drawData.TotalVtxCount > 0 )
		{
			var vertSize = (ulong)Helpers.Align( drawData.TotalVtxCount * sizeof( ImDrawVert ), 4 );
			var indexSize = (ulong)Helpers.Align( drawData.TotalIdxCount * sizeof( ushort ), 4 );

			CreateOrUpdateBuffers( ref frameRenderBuffer, vertSize, indexSize );

			var vtxDst = frameRenderBuffer.VertexBufferMemory.AsPtr<ImDrawVert>();
			var idxDst = frameRenderBuffer.IndexBufferMemory.AsPtr<ushort>();

			for ( var n = 0; n < drawData.CmdListsCount; n++ )
			{
				var cmdList = drawData.CmdLists[n];

				Unsafe.CopyBlock( vtxDst, cmdList.VtxBuffer.Data.ToPointer(),
					(uint)cmdList.VtxBuffer.Size * (uint)sizeof( ImDrawVert ) );
				Unsafe.CopyBlock( idxDst, cmdList.IdxBuffer.Data.ToPointer(),
					(uint)cmdList.IdxBuffer.Size * sizeof( ushort ) );

				vtxDst += cmdList.VtxBuffer.Size;
				idxDst += cmdList.IdxBuffer.Size;
			}

			// Mapping might be better?
			WebGpu.Wgpu.QueueWriteBuffer( _queue, frameRenderBuffer.VertexBufferGpu, 0,
				frameRenderBuffer.VertexBufferMemory, (nuint)vertSize );
			WebGpu.Wgpu.QueueWriteBuffer( _queue, frameRenderBuffer.IndexBufferGpu, 0,
				frameRenderBuffer.IndexBufferMemory,
				(nuint)indexSize );
		}

		var io = ImGui.GetIO();

		var uniforms = new Uniforms
		{
			Mvp = Matrix4x4.CreateOrthographicOffCenter(
				0f,
				io.DisplaySize.X,
				io.DisplaySize.Y,
				0.0f,
				-1.0f,
				1.0f
			),
			Gamma = 2.0f
		};

		WebGpu.Wgpu.QueueWriteBuffer( _queue, _uniformsBuffer, 0, &uniforms, (nuint)sizeof( Uniforms ) );
		WebGpu.Wgpu.RenderPassEncoderSetPipeline( encoder, _renderPipeline );

		if ( drawData.TotalVtxCount > 0 )
		{
			WebGpu.Wgpu.RenderPassEncoderSetVertexBuffer( encoder, 0, frameRenderBuffer.VertexBufferGpu, 0,
				frameRenderBuffer.VertexBufferSize );
			WebGpu.Wgpu.RenderPassEncoderSetIndexBuffer( encoder, frameRenderBuffer.IndexBufferGpu, IndexFormat.Uint16,
				0,
				frameRenderBuffer.IndexBufferSize );
			uint dynamicOffsets = 0;
			WebGpu.Wgpu.RenderPassEncoderSetBindGroup( encoder, 0, _commonBindGroup, 0, in dynamicOffsets );
		}

		WebGpu.Wgpu.RenderPassEncoderSetViewport( encoder, 0, 0, drawData.FramebufferScale.X * drawData.DisplaySize.X,
			drawData.FramebufferScale.Y * drawData.DisplaySize.Y, 0, 1 );

		var vtxOffset = 0;
		var idxOffset = 0;

		for ( var n = 0; n < drawData.CmdListsCount; n++ )
		{
			var cmdList = drawData.CmdLists[n];

			for ( var i = 0; i < cmdList.CmdBuffer.Size; i++ )
			{
				var cmd = cmdList.CmdBuffer[i];

				if ( cmd.UserCallback != IntPtr.Zero )
				{
				}
				else
				{
					var texId = cmd.TextureId;

					if ( texId != IntPtr.Zero )
					{
						if ( _viewsById.TryGetValue( texId, out var value ) )
						{
							uint dynamicOffsets = 0;

							WebGpu.Wgpu.RenderPassEncoderSetBindGroup( encoder, 1, (BindGroup*)value, 0,
								in dynamicOffsets );
						}
					}
				}

				var clipMin = new Vector2( cmd.ClipRect.X, cmd.ClipRect.Y );
				var clipMax = new Vector2( cmd.ClipRect.Z, cmd.ClipRect.W );

				if ( clipMax.X <= clipMin.X || clipMax.Y <= clipMin.Y )
					continue;

				WebGpu.Wgpu.RenderPassEncoderSetScissorRect( encoder, (uint)clipMin.X, (uint)clipMin.Y,
					(uint)(clipMax.X - clipMin.X), (uint)(clipMax.Y - clipMin.Y) );
				WebGpu.Wgpu.RenderPassEncoderDrawIndexed( encoder, cmd.ElemCount, 1, (uint)(idxOffset + cmd.IdxOffset),
					(int)(vtxOffset + cmd.VtxOffset), 0 );
			}

			vtxOffset += cmdList.VtxBuffer.Size;
			idxOffset += cmdList.IdxBuffer.Size;
		}
	}

	private void CreateOrUpdateBuffers( ref FrameRenderBuffer frameRenderBuffer, ulong vertSize, ulong indexSize )
	{
		if ( frameRenderBuffer.VertexBufferGpu is null || frameRenderBuffer.VertexBufferSize < vertSize )
		{
			frameRenderBuffer.VertexBufferMemory?.Dispose();

			if ( frameRenderBuffer.VertexBufferGpu is not null )
			{
				WebGpu.Wgpu.BufferDestroy( frameRenderBuffer.VertexBufferGpu );
				WebGpu.Wgpu.BufferRelease( frameRenderBuffer.VertexBufferGpu );
			}

			var desc = new BufferDescriptor { Size = vertSize, Usage = BufferUsage.Vertex | BufferUsage.CopyDst, };

			frameRenderBuffer.VertexBufferGpu = WebGpu.Wgpu.DeviceCreateBuffer( _device, in desc );
			frameRenderBuffer.VertexBufferSize = vertSize;
			frameRenderBuffer.VertexBufferMemory = GlobalMemory.Allocate( (int)vertSize );
		}

		if ( frameRenderBuffer.IndexBufferGpu is null || frameRenderBuffer.IndexBufferSize < indexSize )
		{
			frameRenderBuffer.IndexBufferMemory?.Dispose();

			if ( frameRenderBuffer.IndexBufferGpu is not null )
			{
				WebGpu.Wgpu.BufferDestroy( frameRenderBuffer.IndexBufferGpu );
				WebGpu.Wgpu.BufferRelease( frameRenderBuffer.IndexBufferGpu );
			}

			var desc = new BufferDescriptor { Size = indexSize, Usage = BufferUsage.Index | BufferUsage.CopyDst, };

			frameRenderBuffer.IndexBufferGpu = WebGpu.Wgpu.DeviceCreateBuffer( _device, in desc );
			frameRenderBuffer.IndexBufferSize = indexSize;
			frameRenderBuffer.IndexBufferMemory = GlobalMemory.Allocate( (int)indexSize );
		}
	}

	private void KeyChar( IKeyboard arg1, char arg2 )
	{
		_pressedChars.Add( arg2 );
	}

	private void KeyDown( IKeyboard arg1, Key arg2, int arg3 )
	{
		_keyEvents[arg2] = true;
	}

	private void KeyUp( IKeyboard arg1, Key arg2, int arg3 )
	{
		_keyEvents[arg2] = false;
	}

	public void Dispose()
	{
		if ( _windowRenderBuffers.FrameRenderBuffers is not null )
		{
			foreach ( var renderBuffer in _windowRenderBuffers.FrameRenderBuffers )
			{
				WebGpu.Wgpu.BufferDestroy( renderBuffer.VertexBufferGpu );
				WebGpu.Wgpu.BufferRelease( renderBuffer.VertexBufferGpu );
				WebGpu.Wgpu.BufferDestroy( renderBuffer.IndexBufferGpu );
				WebGpu.Wgpu.BufferRelease( renderBuffer.IndexBufferGpu );

				renderBuffer.IndexBufferMemory?.Dispose();
				renderBuffer.VertexBufferMemory?.Dispose();
			}
		}

		foreach ( var bg in _viewsById )
		{
			WebGpu.Wgpu.BindGroupRelease( (BindGroup*)bg.Value );
		}

		WebGpu.Wgpu.BindGroupRelease( _commonBindGroup );

		WebGpu.Wgpu.BufferDestroy( _uniformsBuffer );
		WebGpu.Wgpu.BufferRelease( _uniformsBuffer );

		WebGpu.Wgpu.RenderPipelineRelease( _renderPipeline );

		WebGpu.Wgpu.BindGroupLayoutRelease( _commonBindGroupLayout );
		WebGpu.Wgpu.BindGroupLayoutRelease( _imageBindGroupLayout );

		WebGpu.Wgpu.SamplerRelease( _fontSampler );
		WebGpu.Wgpu.TextureViewRelease( _fontView );
		WebGpu.Wgpu.TextureDestroy( _fontTexture );
		WebGpu.Wgpu.TextureRelease( _fontTexture );

		_shader.Dispose();

		_inputContext.Keyboards[0].KeyChar -= KeyChar;
		_inputContext.Keyboards[0].KeyUp -= KeyUp;
		_inputContext.Keyboards[0].KeyDown -= KeyDown;
	}
}
