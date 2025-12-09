using MossEngine.UI;
using MossEngine.UI.Components;
using MossEngine.UI.Yoga;
using MossEngine.Windowing;
using SkiaSharp;

namespace MossEngine.Editor;

public sealed class TitleBarButton : Panel
{
	public char Icon { get; set; }

	public TitleBarButton( char icon )
	{
		Width = Length.Point( 48 );
		Height = Length.Point( 40 );
		Background = SKColors.Black;
		FlexShrink = 0;
		JustifyContent = YogaJustify.Center;
		AlignItems = YogaAlign.Center;
		IsHitTestVisible = true;

		Icon = icon;

		var text = new Text
		{
			Foreground = SKColors.White,
			FontSize = 10,
			FontFamily = FontFamily.FromFile( "Segoe Fluent Icons", @"C:\Windows\Fonts\SegoeIcons.ttf" ),
			Value = Icon.ToString()
		};
		AddChild( text );
	}

	protected override void OnPointerEnter( PointerEventArgs args )
	{
		Background = SKColor.FromHsl( 0, 0, 10 );
	}

	protected override void OnPointerLeave( PointerEventArgs args )
	{
		Background = SKColors.Black;
	}
}

public sealed class TitleBar : Panel
{
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

		var buttons = new Panel
		{
			AlignItems = YogaAlign.Center
		};
		AddChild( buttons );

		var minimizeButton = new TitleBarButton( '\uE921' );
		buttons.AddChild( minimizeButton );

		var maximizeButton = new TitleBarButton( '\uE922' ); // uE923 -> Restore
		buttons.AddChild( maximizeButton );

		var closeButton = new TitleBarButton( '\uE8bb' );
		buttons.AddChild( closeButton );
	}

	// public override void Draw( SKCanvas canvas )
	// {
	// 	Width = Length.Percent( 100 );
	// 	Height = Length.Point( 32 );
	// 	FlexShrink = 0;
	// 	
	// 	var paint = new SKPaint
	// 	{
	// 		Color = SKColors.Red,
	// 		Style = SKPaintStyle.Fill,
	// 		IsAntialias = true,
	// 		StrokeWidth = 1,
	// 	};
	// 	canvas.DrawRect( 0, 0, Width, Height, paint );
	//
	// 	// var textPaint = new SKPaint
	// 	// {
	// 	// 	Color = SKColors.White,
	// 	// 	Style = SKPaintStyle.Fill,
	// 	// 	IsAntialias = true,
	// 	// 	StrokeWidth = 1,
	// 	// 	TextAlign = SKTextAlign.Left,
	// 	// 	// TextBaseline = SKTextBaseline.Alphabetic,
	// 	// 	TextEncoding = SKTextEncoding.Utf8,
	// 	// 	TextSize = 16,
	// 	// };
	// 	// canvas.DrawText( Title, 8, 24, textPaint );
	// }
}
