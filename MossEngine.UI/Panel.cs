using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI;

public class Panel : BaseWidget
{
	private readonly SKPaint _textPaint;

	public string Text { get; set; } = string.Empty;

	public Panel()
	{
		// default flex behaviour for panels
		YogaNode.AlignItems = YogaAlign.FlexStart;
		YogaNode.JustifyContent = YogaJustify.FlexStart;
		YogaNode.FlexDirection = YogaFlexDirection.Row;
		// YogaNode.Width = YogaValue.Auto.Value;
		// YogaNode.Height = YogaValue.Auto.Value;

		_textPaint = new SKPaint { IsAntialias = true, TextSize = 14, Color = Foreground, IsStroke = false };
	}

	private void DrawBackground( SKCanvas canvas )
	{
		new SkiaRectBuilder( canvas )
			.At( Position.X, Position.Y )
			.WithSize( Size.X, Size.Y )
			.WithBorderRadius( BorderRadius.X, BorderRadius.Y )
			.WithFill( Background )
			.Draw();
	}

	private void DrawText( SKCanvas canvas )
	{
		if ( string.IsNullOrEmpty( Text ) ) return;

		_textPaint.Color = Foreground;
		DrawingHelper.DrawText( canvas, Position.X + Padding, Position.Y + Padding, Text, _textPaint );
	}

	public override void Draw( SKCanvas canvas )
	{
		DrawBackground( canvas );
		DrawText( canvas );

		// children
		foreach ( var c in Children )
		{
			c.Draw( canvas );
		}

		IsDirty = false;
	}
}

public static class DrawingHelper
{
	public static void DrawText( SKCanvas canvas, float x, float y, string text, SKPaint paint )
	{
		var px = x;
		var py = y + Math.Abs( paint.FontMetrics.Ascent );

		canvas.DrawText( text, px, py, paint );
	}
}

public abstract class BaseSkiaBuilder( SKCanvas canvas )
{
	protected readonly SKCanvas Canvas = canvas;
	protected readonly SKPaint Paint = new();
}

public class SkiaRectBuilder : BaseSkiaBuilder
{
	private SKRect _explicitRect;
	private bool _rectExplicitlySet;
	private float _x;
	private float _y;
	private float _width;
	private float _height;
	private bool _positionSet;
	private bool _sizeSet;
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

	public SkiaRectBuilder WithRect( SKRect rect )
	{
		_explicitRect = rect;
		_rectExplicitlySet = true;

		return this;
	}

	public SkiaRectBuilder WithRect( float x, float y, float width, float height )
	{
		return WithRect( new SKRect( x, y, x + width, y + height ) );
	}

	public SkiaRectBuilder At( float x, float y )
	{
		_x = x;
		_y = y;
		_positionSet = true;
		_rectExplicitlySet = false;

		return this;
	}

	public SkiaRectBuilder WithSize( float width, float height )
	{
		width = Math.Max( 0, width );
		height = Math.Max( 0, height );

		_width = width;
		_height = height;
		_sizeSet = true;
		_rectExplicitlySet = false;

		return this;
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

	public void Draw()
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

	private SKRect BuildRect()
	{
		if ( _rectExplicitlySet )
			return _explicitRect;

		if ( !_sizeSet )
			throw new InvalidOperationException( "Size must be specified via WithSize or WithRect before drawing." );

		var x = _positionSet ? _x : 0f;
		var y = _positionSet ? _y : 0f;

		return new SKRect( x, y, x + _width, y + _height );
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
}
