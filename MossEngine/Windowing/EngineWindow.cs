using ImGuiNET;
using MossEngine.Pipelines;
using MossEngine.UI;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using SkiaSharp;

namespace MossEngine.Windowing;

public abstract unsafe partial class EngineWindow( string title ) : IDisposable
{
	private WebGpuSurface _surface = null!;
	private WebGpuAdapter _adapter = null!;
	private WebGpuQueue _queue = null!;
	private CommandEncoder* _commandEncoder;
	private SurfaceTexture _surfaceTexture;
	private TextureView* _surfaceTextureView;
	private ImGuiController _imGuiController = null!;
	private readonly SkiaRenderPipeline _skiaRenderPipeline = new();

	private RootPanelRenderer _rootPanelRenderer = null!;

	public RootPanel RootPanel { get; private set; } = null!;

	public RenderPassEncoder* RenderPassEncoder { get; private set; }
	public WebGpuDevice Device { get; private set; } = null!;
	public IWindow Window { get; private set; } = null!;

	public TextureFormat SwapChainFormat { get; private set; }

	public void Run()
	{
		//Create a window.
		var options = WindowOptions.Default;
		options.Size = new Vector2D<int>( 800, 600 );
		options.ShouldSwapAutomatically = false;
		options.IsContextControlDisabled = true;
		options.Title = title;
		options.VSync = true;
		options.FramesPerSecond = 60;
		options.API = GraphicsAPI.None;

		Window = Silk.NET.Windowing.Window.Create( options );

		Window.Load += InternalOnWindowLoad;
		Window.Update += InternalOnWindowUpdate;
		Window.Render += OnWindowRender;
		Window.Resize += InternalOnWindowFramebufferResize;
		Window.FramebufferResize += InternalOnWindowFramebufferResize;
		Window.Closing += OnWindowClose;
		Window.FileDrop += OnWindowFileDrop;
		Window.Move += OnWindowMove;
		Window.StateChanged += OnWindowStateChanged;

		Window.Initialize();

		InitializeWebGpu();
		ConfigureSurface();
		InitializeImGui();

		RootPanel = new RootPanel();
		RootPanel.Resize( Window.Size.X, Window.Size.Y );

		_rootPanelRenderer = new RootPanelRenderer( RootPanel );

		OnInitialized();
		Window.Run();
	}

	private void InitializeWebGpu()
	{
		_surface = WebGpu.Instance.CreateSurface( Window );
		_adapter = new WebGpuAdapter( WebGpu.Instance, _surface );

		Device = _adapter.CreateDevice();
		WebGpu.ConfigureDebugCallback( Device );
	}

	private void InitializeImGui()
	{
		_imGuiController =
			new ImGuiController( Device, Window, _input, 2, SwapChainFormat, null );

		var io = ImGui.GetIO();
		io.ConfigFlags = ImGuiConfigFlags.DockingEnable | ImGuiConfigFlags.ViewportsEnable;
		io.WantSaveIniSettings = true;
	}

	private void ConfigureSurface()
	{
		SwapChainFormat = WebGpu.Wgpu.SurfaceGetPreferredFormat( _surface, _adapter );

		var surfaceConfiguration = new SurfaceConfiguration
		{
			Device = Device,
			Width = (uint)Window.Size.X,
			Height = (uint)Window.Size.Y,
			Format = SwapChainFormat,
			PresentMode = PresentMode.Fifo,
			Usage = TextureUsage.RenderAttachment | TextureUsage.CopyDst | TextureUsage.CopySrc,
		};

		WebGpu.Wgpu.SurfaceConfigure( _surface, surfaceConfiguration );
	}

	protected virtual void OnInitialized()
	{
	}

	protected virtual void OnRender( double deltaTime )
	{
		var framebufferSize = Window.FramebufferSize;

		_skiaRenderPipeline.RenderOverlay( Device, _queue, RenderPassEncoder, SwapChainFormat, framebufferSize,
			InternalOnSkiaDraw );

		_imGuiController.Update( (float)deltaTime );
		OnImGuiDraw();
		_imGuiController.Render( RenderPassEncoder );
	}

	protected virtual void OnImGuiDraw()
	{
	}

	private void InternalOnSkiaDraw( SKCanvas canvas, Vector2D<int> size )
	{
		RootPanel.Resize( size.X, size.Y );
		_rootPanelRenderer.Render( canvas );

		OnDraw( canvas, size );
	}

	protected virtual void OnDraw( SKCanvas canvas, Vector2D<int> size )
	{
	}

	public void Dispose()
	{
		_skiaRenderPipeline.Dispose();
		_imGuiController.Dispose();

		Window.Load -= InternalOnWindowLoad;
		Window.Update -= InternalOnWindowUpdate;
		Window.Render -= OnWindowRender;
		Window.Resize -= InternalOnWindowFramebufferResize;
		Window.FramebufferResize -= InternalOnWindowFramebufferResize;
		Window.Closing -= OnWindowClose;
		Window.FileDrop -= OnWindowFileDrop;
		Window.Move -= OnWindowMove;
		Window.StateChanged -= OnWindowStateChanged;

		Device.Dispose();

		_surface.Dispose();
		_adapter.Dispose();

		WebGpu.Instance.Dispose();
		WebGpu.Dispose();
		DisposeInput();
		Window.Dispose();
	}
}
