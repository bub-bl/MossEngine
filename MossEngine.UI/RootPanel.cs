using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI;

public class RootPanel : BaseWidget
{
	public float ScreenWidth { get; set; }
	public float ScreenHeight { get; set; }

	public RootPanel( float width, float height )
	{
		Resize( width, height );
		YogaNode.PositionType = YogaPositionType.Relative;
	}

	public void Resize( float width, float height )
	{
		ScreenWidth = width;
		ScreenHeight = height;

		YogaNode.Width = width;
		YogaNode.Height = height;
	}

	public override void Draw( SKCanvas canvas )
	{
		// Draw root background if needed
		foreach ( var c in Children )
		{
			c.Draw( canvas );
		}
	}
}
