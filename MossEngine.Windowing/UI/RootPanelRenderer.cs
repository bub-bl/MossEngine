using Silk.NET.Windowing;
using SkiaSharp;

namespace MossEngine.Windowing.UI;

public sealed class RootPanelRenderer
{
	private readonly RootPanel _root;

	public RootPanelRenderer( IWindow window, RootPanel root )
	{
		_root = root;
		_root.ParentWindow = window;
	}

	// Call from our engine render loop, with an Skia canvas that maps to the game viewport.
	public void Render( SKCanvas canvas )
	{
		// optional: clear
		canvas.Clear( SKColors.Transparent );

		// layout
		_root.ComputeLayout();

		// update
		_root.InternalOnUpdate();

		// draw tree
		_root.Draw( canvas );
	}
}
