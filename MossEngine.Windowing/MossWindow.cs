using ImGuiNET;
using MossEngine.WebGpu;
using MossEngine.WebGpu.ImGui;
using MossEngine.WebGpu.Skia;
using Silk.NET.Maths;
using Silk.NET.WebGPU;
using Silk.NET.Windowing;
using SkiaSharp;

namespace MossEngine.Windowing;

public abstract unsafe partial class MossWindow( string title, int width, int height ) : IDisposable
{
	private WebGpuSurface _surface = null!;
	private WebGpuAdapter _adapter = null!;
	private WebGpuQueue _queue = null!;
	private CommandEncoder* _commandEncoder;
	private SurfaceTexture _surfaceTexture;
	private TextureView* _surfaceTextureView;
	private ImGuiController _imGuiController = null!;
	private readonly SkiaRenderPipeline _skiaRenderPipeline = new();

	// private RootPanelRenderer _rootPanelRenderer = null!;
	//
	// public RootPanel RootPanel { get; private set; } = null!;

	public RenderPassEncoder* RenderPassEncoder { get; private set; }
	public WebGpuDevice Device { get; private set; } = null!;
	public IWindow Window { get; private set; } = null!;

	public TextureFormat SwapChainFormat { get; private set; }

	public void Run()
	{
		var options = WindowOptions.DefaultVulkan;
		options.Size = new Vector2D<int>( width, height );
		options.ShouldSwapAutomatically = false;
		options.IsContextControlDisabled = true;
		options.Title = title;
		options.VSync = true;
		// options.FramesPerSecond = 60;
		// options.API = GraphicsAPI.None;

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

		// --- activate custom titlebar: keep animations, draw your own
		// var hwnd = Window.Native!.Win32!.Value.Hwnd;
		// var hwnd = window.Native!.Win32!.Handle;
		// CustomWindowFrame.ApplyCustomFrame(hwnd);

		InitializeWebGpu();
		ConfigureSurface();
		InitializeImGui();

		using ( var _ = Cursor.Scope( _input ) )
		{
			OnInitialized();
			Window.Run();
		}
	}

	private void InitializeWebGpu()
	{
		_surface = WebGpuApi.Instance.CreateSurface( Window );
		_adapter = new WebGpuAdapter( WebGpuApi.Instance, _surface );

		Device = _adapter.CreateDevice();
		WebGpuApi.ConfigureDebugCallback( Device );
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
		SwapChainFormat = WebGpuApi.Wgpu.SurfaceGetPreferredFormat( _surface, _adapter );

		var surfaceConfiguration = new SurfaceConfiguration
		{
			Device = Device,
			Width = (uint)Window.Size.X,
			Height = (uint)Window.Size.Y,
			Format = SwapChainFormat,
			PresentMode = PresentMode.Fifo,
			Usage = TextureUsage.RenderAttachment | TextureUsage.CopyDst | TextureUsage.CopySrc,
		};

		WebGpuApi.Wgpu.SurfaceConfigure( _surface, surfaceConfiguration );
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
		OnDraw( canvas, size );
		OnTitleBarDraw( canvas, size );
	}

	protected virtual void OnDraw( SKCanvas canvas, Vector2D<int> size )
	{
	}
	
	protected virtual void OnTitleBarDraw( SKCanvas canvas, Vector2D<int> size )
	{
		// var paint = new SKPaint
		// {
		// 	Color = SKColors.Red,
		// 	Style = SKPaintStyle.Fill,
		// 	IsAntialias = true,
		// 	StrokeWidth = 1,
		// };
		//
		// // canvas.DrawColor( SKColors.Black );
		// canvas.DrawRect(0, 0, size.X, 32, paint);
	}

	public void Dispose()
	{
		// WindowsTitlebarHelper.DisableCustomTitlebar(); // restore if possible

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

		WebGpuApi.Instance.Dispose();
		WebGpuApi.Dispose();

		DisposeInput();
		Window.Dispose();
	}
}
