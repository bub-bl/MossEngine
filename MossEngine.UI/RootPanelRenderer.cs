using SkiaSharp;

namespace MossEngine.UI;

public sealed class RootPanelRenderer( RootPanel root )
{
	// call from your engine render loop, with an Skia canvas that maps to the game viewport.
	public void Render( SKCanvas canvas )
	{
		// optional: clear
		canvas.Clear( SKColors.Transparent );

		// layout
		root.ComputeLayout();

		// update
		root.InternalOnUpdate();

		// draw tree
		root.Draw( canvas );
	}
}
