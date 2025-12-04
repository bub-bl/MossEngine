using SkiaSharp;

namespace MossEngine.UI;

public class SkiaTextBuilder : BaseSkiaBuilder<SkiaTextBuilder>
{
	private string _text;

	public SkiaTextBuilder( SKCanvas canvas ) : base( canvas )
	{
		Paint.IsAntialias = true;
	}

	public SkiaTextBuilder WithText( string text )
	{
		_text = text;
		return this;
	}

	public SkiaTextBuilder WithColor( SKColor color )
	{
		Paint.Color = color;
		return this;
	}

	public SkiaTextBuilder WithFont( SKTypeface typeface )
	{
		Paint.Typeface = typeface;
		return this;
	}

	public SkiaTextBuilder WithFontSize( float fontSize )
	{
		Paint.TextSize = fontSize;
		return this;
	}

	public override void Draw()
	{
		if ( string.IsNullOrEmpty( _text ) ) return;

		var rect = BuildRect();
		var top = rect.Top + Math.Abs( Paint.FontMetrics.Ascent );

		Canvas.DrawText( _text, rect.Left, top, Paint );
	}
}
