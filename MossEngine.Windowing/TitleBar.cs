using MossEngine.Windowing.UI;
using MossEngine.Windowing.UI.Components;
using MossEngine.Windowing.UI.Yoga;
using Silk.NET.Windowing;
using SkiaSharp;

namespace MossEngine.Windowing;

public sealed class TitleBar : Panel
{
	private Panel _buttons = new();
	
	public string Title { get; set; } = "Engine";

	public TitleBar()
	{
		Width = Length.Percent( 100 );
		Height = Length.Point( 40 );
		Background = SKColors.Black;
		FlexShrink = 0;
		Padding = new Padding { Left = 12 };
		JustifyContent = YogaJustify.SpaceBetween;
		AlignItems = YogaAlign.Center;

		var title = new Text
		{
			Foreground = SKColors.White, FontFamily = FontFamily.FromFontName( "Segoe UI" ), Value = Title
		};
		AddChild( title );

		_buttons = new Panel { AlignItems = YogaAlign.Center };
		AddChild( _buttons );

		var minimizeButton = new TitleBarButton( '\uE921' );
		minimizeButton.PointerUp += ( sender, args ) =>
		{
			ParentWindow?.WindowState = WindowState.Minimized;
		};
		_buttons.AddChild( minimizeButton );

		var maximizeButton = new TitleBarButton( '\uE922' );
		maximizeButton.PointerUp += ( sender, args ) =>
		{
			switch ( ParentWindow?.WindowState )
			{
				case WindowState.Maximized:
					maximizeButton.Icon = '\uE922';
					ParentWindow.WindowState = WindowState.Normal;
					break;
				case WindowState.Normal:
					maximizeButton.Icon = '\uE923';
					ParentWindow.WindowState = WindowState.Maximized;
					break;
			}
		};
		_buttons.AddChild( maximizeButton );

		var closeButton = new TitleBarButton( '\uE8bb' );
		closeButton.PointerUp += ( sender, args ) =>
		{
			ParentWindow?.Close();
		};
		_buttons.AddChild( closeButton );
	}

	public bool ShouldHit()
	{
		return _buttons.ContainsPoint( Cursor.MousePosition );
	}

	// protected override void OnPointerDown( PointerEventArgs args )
	// {
	// 	var window = Editor.MainWindow;
	//
	// 	if ( args.Button is not MouseButton.Left ) return;
	//
	// 	window.BeginMoveDrag();
	// 	args.Handled = true;
	// }
	//
	// protected override void OnPointerMove( PointerEventArgs args )
	// {
	// 	var window = Editor.MainWindow;
	//
	// 	if ( window.IsDragging )
	// 	{
	// 		var currentMousePosition = window.MousePosition;
	// 		var delta = currentMousePosition - window.DragStartPosition;
	//
	// 		if ( delta != Vector2.Zero )
	// 		{
	// 			window.Window.Position = new Vector2D<int>(
	// 				(int)(window.Window.Position.X + delta.X),
	// 				(int)(window.Window.Position.Y + delta.Y)
	// 			);
	// 		}
	//
	// 		args.Handled = true;
	// 	}
	// }
	//
	// protected override void OnPointerUp( PointerEventArgs args )
	// {
	// 	var window = Editor.MainWindow;
	// 	
	// 	if ( !window.IsDragging || args.Button is not MouseButton.Left ) return;
	//
	// 	window.EndMoveDrag();
	// 	args.Handled = true;
	// }
}
