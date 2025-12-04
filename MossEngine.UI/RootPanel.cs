using SkiaSharp;

namespace MossEngine.UI;

public class RootPanel : Panel
{
	public float ScreenWidth { get; set; }
	public float ScreenHeight { get; set; }

	public void Resize( float width, float height )
	{
		ScreenWidth = width;
		ScreenHeight = height;

		YogaNode.Width = width;
		YogaNode.Height = height;
	}
}
