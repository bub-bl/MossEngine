using Game.Pipelines;
using Game.Rendering;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using SkiaSharp;

namespace Game.Windowing;

public abstract unsafe partial class BaseWindow( string title ) : IDisposable
{
	private WebGpuSurface _surface = null!;
	private WebGpuAdapter _adapter = null!;
	private WebGpuQueue _queue = null!;
	private CommandEncoder* _commandEncoder;
	private SurfaceTexture _surfaceTexture;
	private TextureView* _surfaceTextureView;
	private ImGuiController _imGuiController = null!;
	private readonly SkiaSurfaceRenderer _skiaRenderer = new();
	private SkiaOverlayPipeline? _skiaOverlayPipeline;

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
		Window.Initialize();

		InitializeWebGpu();
		ConfigureSurface();
		InitializeImGui();

		Window.Load += OnWindowLoad;
		Window.Update += OnWindowUpdate;
		Window.Render += OnWindowRender;
		Window.Resize += OnWindowFramebufferResize;
		Window.FramebufferResize += OnWindowFramebufferResize;
		Window.Closing += OnWindowClose;
		Window.FileDrop += OnWindowFileDrop;
		Window.Move += OnWindowMove;
		Window.StateChanged += OnWindowStateChanged;

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
			new ImGuiController( Device, Window, Window.CreateInput(), 2, SwapChainFormat, null );

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

		_skiaOverlayPipeline ??= new SkiaOverlayPipeline( Device, SwapChainFormat );
	}

	protected virtual void OnInitialized()
	{
	}

	protected virtual void OnRender( double deltaTime )
	{
		var framebufferSize = Window.FramebufferSize;
		var hasSkiaOverlay = _skiaRenderer.Render( Device, _queue, framebufferSize, OnSkiaDraw );

		if ( hasSkiaOverlay )
		{
			var textureView = _skiaRenderer.TextureView;
			_skiaOverlayPipeline?.Render( RenderPassEncoder, textureView );
		}

		_imGuiController.Update( (float)deltaTime );
		OnImGuiDraw();
		_imGuiController.Render( RenderPassEncoder );
	}

	protected virtual void OnImGuiDraw()
	{
	}

	protected virtual void OnSkiaDraw( SKCanvas canvas, Vector2D<int> size )
	{
	}

	public void Dispose()
	{
		_skiaOverlayPipeline?.Dispose();
		_skiaRenderer.Dispose();
		_imGuiController.Dispose();

		Window.Load -= OnWindowLoad;
		Window.Update -= OnWindowUpdate;
		Window.Render -= OnWindowRender;
		Window.Resize -= OnWindowFramebufferResize;
		Window.FramebufferResize -= OnWindowFramebufferResize;
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
