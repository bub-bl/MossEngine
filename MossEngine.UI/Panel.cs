using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI;

public class Panel : BaseWidget
{
	public string Text { get; set; } = string.Empty;

	public Panel()
	{
		// default flex behaviour for panels
		YogaNode.AlignItems = YogaAlign.FlexStart;
		YogaNode.JustifyContent = YogaJustify.FlexStart;
		YogaNode.FlexDirection = YogaFlexDirection.Row;
		// YogaNode.Width = YogaValue.Auto.Value;
		// YogaNode.Height = YogaValue.Auto.Value;
	}

	private void DrawBackground( SKCanvas canvas )
	{
		// var parentLeft = Parent?.Left ?? Length.Zero;
		// var parentTop = Parent?.Top ?? Length.Zero;
		// var left = parentLeft + Left;
		// var top = parentTop + Top;

		var parentLeft = Parent?.LayoutLeft ?? Length.Zero;
		var parentTop = Parent?.LayoutTop ?? Length.Zero;
		var left = parentLeft + LayoutLeft;
		var top = parentTop + LayoutTop;
		
		new SkiaRectBuilder( canvas )
			.At( left, top )
			.WithSize( Size.X, Size.Y )
			.WithBorderRadius( BorderRadius.X, BorderRadius.Y )
			.WithFill( Background )
			.Draw();
	}

	private void DrawText( SKCanvas canvas )
	{
		// var parentLeft = Parent?.Left ?? Length.Zero;
		// var parentTop = Parent?.Top ?? Length.Zero;
		// var left = parentLeft + Left;
		// var top = parentTop + Top;

		var parentLeft = Parent?.LayoutLeft ?? Length.Zero;
		var parentTop = Parent?.LayoutTop ?? Length.Zero;
		var left = parentLeft + LayoutLeft;
		var top = parentTop + LayoutTop;

		new SkiaTextBuilder( canvas )
			.At( left, top )
			.WithText( Text )
			.WithColor( Foreground )
			.WithFontSize( 20 )
			.Draw();
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

public abstract class BaseSkiaBuilder<TBuilder>( SKCanvas canvas ) where TBuilder : BaseSkiaBuilder<TBuilder>
{
	protected readonly SKCanvas Canvas = canvas;
	protected readonly SKPaint Paint = new();

	private float _x;
	private float _y;
	private float _width;
	private float _height;
	private bool _sizeSet;
	private bool _positionSet;
	private bool _rectExplicitlySet;
	private SKRect _explicitRect;

	public TBuilder WithRect( SKRect rect )
	{
		_explicitRect = rect;
		_rectExplicitlySet = true;

		return (TBuilder)this;
	}

	public TBuilder WithRect( float x, float y, float width, float height )
	{
		return WithRect( new SKRect( x, y, x + width, y + height ) );
	}

	public TBuilder At( float x, float y )
	{
		_x = x;
		_y = y;
		_positionSet = true;
		_rectExplicitlySet = false;

		return (TBuilder)this;
	}

	public TBuilder WithSize( float width, float height )
	{
		width = Math.Max( 0, width );
		height = Math.Max( 0, height );

		_width = width;
		_height = height;
		_sizeSet = true;
		_rectExplicitlySet = false;

		return (TBuilder)this;
	}

	protected SKRect BuildRect()
	{
		if ( _rectExplicitlySet )
			return _explicitRect;

		// if ( !_sizeSet )
		// 	throw new InvalidOperationException( "Size must be specified via WithSize or WithRect before drawing." );

		var x = _positionSet ? _x : 0f;
		var y = _positionSet ? _y : 0f;

		return new SKRect( x, y, x + _width, y + _height );
	}

	public abstract void Draw();
}

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
