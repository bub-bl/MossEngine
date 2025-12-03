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

	private void DrawBackground( SKCanvas canvas, float x, float y )
	{
		if ( Background == SKColors.Transparent ) return;
		DrawingHelper.DrawRect( canvas, x + Position.X, y + Position.Y, Size.X, Size.Y, Background );
	}
	
	private void DrawText( SKCanvas canvas, float x, float y )
	{
		if ( string.IsNullOrEmpty( Text ) ) return;
		
		_textPaint.Color = Foreground;
		DrawingHelper.DrawText( canvas, x + Position.X + Padding, y + Position.Y + Padding, Text, _textPaint );
	}

	public override void Draw( SKCanvas canvas, float x, float y )
	{
		DrawBackground( canvas, x, y );
		DrawText( canvas, x, y );

		// children
		foreach ( var c in Children )
		{
			c.Draw( canvas, x + Position.X, y + Position.Y );
		}

		IsDirty = false;
	}
}

public static class DrawingHelper
{
	public static void DrawRect( SKCanvas canvas, float x, float y, float width, float height, SKColor color )
	{
		if ( color == SKColors.Transparent ) return;

		using var p = new SKPaint();
		p.Color = color;

		canvas.DrawRect( new SKRect( x, y, x + width, y + height ), p );
	}
	
	public static void DrawText( SKCanvas canvas, float x, float y, string text, SKPaint paint )
	{
		var px = x;
		var py = y + Math.Abs( paint.FontMetrics.Ascent );
		
		canvas.DrawText( text, px, py, paint );
	}
}
