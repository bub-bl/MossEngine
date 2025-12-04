using SkiaSharp;

namespace MossEngine.UI;

public class SkiaRectBuilder : BaseSkiaBuilder<SkiaRectBuilder>
{
	private float _radiusX;
	private float _radiusY;
	private bool _fillEnabled = true;
	private bool _strokeEnabled;
	private SKColor _fillColor = SKColors.White;
	private SKColor _strokeColor = SKColors.Transparent;
	private float _strokeWidth = 1f;

	public SkiaRectBuilder( SKCanvas canvas ) : base( canvas )
	{
		Paint.IsAntialias = true;
	}

	public SkiaRectBuilder WithBorderRadius( float radius )
	{
		return WithBorderRadius( radius, radius );
	}

	public SkiaRectBuilder WithBorderRadius( float radiusX, float radiusY )
	{
		_radiusX = Math.Max( 0, radiusX );
		_radiusY = Math.Max( 0, radiusY );

		return this;
	}

	public SkiaRectBuilder WithFill( SKColor color )
	{
		_fillColor = color;
		_fillEnabled = true;

		return this;
	}

	public SkiaRectBuilder WithStroke( SKColor color, float strokeWidth = 1f )
	{
		strokeWidth = Math.Max( 0, strokeWidth );

		_strokeColor = color;
		_strokeWidth = strokeWidth;
		_strokeEnabled = true;

		return this;
	}

	private void DrawRectGeometry( SKRect rect )
	{
		if ( _radiusX <= 0 && _radiusY <= 0 )
		{
			Canvas.DrawRect( rect, Paint );
			return;
		}

		using var roundRect = new SKRoundRect( rect, _radiusX, _radiusY );
		Canvas.DrawRoundRect( roundRect, Paint );
	}

	public override void Draw()
	{
		var rect = BuildRect();
		if ( rect.Width <= 0 || rect.Height <= 0 ) return;

		if ( _fillEnabled && _fillColor != SKColors.Transparent )
		{
			Paint.Style = SKPaintStyle.Fill;
			Paint.StrokeWidth = 0;
			Paint.Color = _fillColor;

			DrawRectGeometry( rect );
		}

		if ( _strokeEnabled && _strokeColor != SKColors.Transparent )
		{
			Paint.Style = SKPaintStyle.Stroke;
			Paint.StrokeWidth = _strokeWidth;
			Paint.Color = _strokeColor;

			DrawRectGeometry( rect );
		}
	}
}
