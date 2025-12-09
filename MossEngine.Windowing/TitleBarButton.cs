using MossEngine.Windowing.UI;
using MossEngine.Windowing.UI.Components;
using MossEngine.Windowing.UI.Yoga;
using SkiaSharp;

namespace MossEngine.Windowing;

public sealed class TitleBarButton : Panel
{
	private readonly Text _text;

	public char Icon
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;

			_text.Value = value.ToString();
		}
	}

	public TitleBarButton( char icon )
	{
		Width = Length.Point( 48 );
		Height = Length.Point( 40 );
		Background = SKColors.Black;
		FlexShrink = 0;
		JustifyContent = YogaJustify.Center;
		AlignItems = YogaAlign.Center;

		_text = new Text
		{
			Foreground = SKColors.White,
			FontSize = 10,
			FontFamily = FontFamily.FromFile( "Segoe Fluent Icons", @"C:\Windows\Fonts\SegoeIcons.ttf" ),
			Value = Icon.ToString(),
			IsHitTestVisible = false
		};

		Icon = icon;
		AddChild( _text );
	}

	protected override void OnUpdate()
	{
		Background = SKColor.FromHsl( 0, 0, IsInside() ? 10 : 0 );
	}
}
