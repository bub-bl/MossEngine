using Silk.NET.Input;

namespace MossEngine.UI;

public partial class RootPanel : Panel
{
	private Panel? _hoveredPanel;
	private Panel? _pressedPanel;
	private Panel? _focusedPanel;
	private MouseButton? _activeButton;

	public float ScreenWidth { get; set; }
	public float ScreenHeight { get; set; }

	public Panel? FocusedPanel => _focusedPanel;

	public void Resize( float width, float height )
	{
		ScreenWidth = width;
		ScreenHeight = height;

		YogaNode.Width = width;
		YogaNode.Height = height;
	}

	private void FocusPanel( Panel panel )
	{
		if ( !panel.IsFocusable ) return;
		if ( ReferenceEquals( _focusedPanel, panel ) ) return;

		_focusedPanel = panel;
	}
}
