using MossEngine.UI;
using MossEngine.Windowing;
using Silk.NET.Maths;
using SkiaSharp;

namespace MossEngine.Editor;

public abstract partial class EngineWindow( string title ) : MossWindow( title, 1366, 768 )
{
	private RootPanelRenderer _rootPanelRenderer = null!;

	public RootPanel RootPanel { get; private set; } = null!;

	protected sealed override void OnInitialized()
	{
		RootPanel = new RootPanel();
		RootPanel.Resize( Window.Size.X, Window.Size.Y );

		_rootPanelRenderer = new RootPanelRenderer( RootPanel );

		OnReady();
	}

	protected virtual void OnReady()
	{
	}

	protected override void OnDraw( SKCanvas canvas, Vector2D<int> size )
	{
		RootPanel.Resize( size.X, size.Y );
		_rootPanelRenderer.Render( canvas );
	}

	protected sealed override void OnBeforeRender( double deltaTime )
	{
		base.OnBeforeRender( deltaTime );
	}

	// protected sealed override void OnRender( double deltaTime )
	// {
	// 	base.OnRender( deltaTime );
	// }

	protected sealed override void OnAfterRender( double deltaTime )
	{
		base.OnAfterRender( deltaTime );
	}
}
