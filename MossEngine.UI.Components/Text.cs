using System.Drawing;
using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI.Components;

public class Text : Panel
{
	private static readonly SKPaint MeasurementPaint = new() { IsAntialias = true, TextSize = 20 };

	public string Value
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;

			UpdateMeasurement();
			MarkDirty();
		}
	} = null!;

	public FontFamily FontFamily
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;

			MeasurementPaint.Typeface = value.Typeface;
			MarkDirty();
		}
	}

	public float FontSize
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;
			
			MeasurementPaint.TextSize = value;
			MarkDirty();
		}
	}
	
	public SKColor Foreground
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;
			
			MeasurementPaint.Color = value;
			MarkDirty();
		}
	}

	public Text()
	{
		UpdateMeasurement();
	}

	private void DrawText( SKCanvas canvas )
	{
		var position = GetFinalPosition();
		MeasurementPaint.Color = Foreground;

		var bounds = new SKRect();
		MeasurementPaint.MeasureText( Value, ref bounds );
		var metrics = MeasurementPaint.FontMetrics;

		float textX;

		// Calcul de la position X en fonction de l'alignement
		// var textX = position.X;

		// // Ajustement pour le texte aligné à droite
		// if ( YogaNode.AlignSelf == YogaAlign.FlexEnd ||
		//      (YogaNode.Parent?.JustifyContent == YogaJustify.FlexEnd && YogaNode.AlignSelf == YogaAlign.Auto) )
		// {
		// 	textX = position.X + LayoutWidth - bounds.Width - bounds.Left;
		// }
		// else
		// {
		// 	textX = position.X - bounds.Left;
		// }

		// Ajustement pour le texte aligné à droite
		// TODO - Nous ne devrions pas avoir besoin de ce code
		if ( Parent?.JustifyContent is YogaJustify.FlexEnd )
		{
			textX = position.X + LayoutWidth - bounds.Width - bounds.Left;
		}
		else
		{
			textX = position.X - bounds.Left;
		}

		var containerCenter = position.Y + LayoutHeight / 2f;
		var baseline = containerCenter - (metrics.Ascent + metrics.Descent) / 2f;

		// // Dessiner le fond avec la largeur réelle du texte
		// using ( var paint = new SKPaint() )
		// {
		// 	paint.Color = Background;
		// 	canvas.DrawRect(
		// 		new SKRect(
		// 			textX + bounds.Left,
		// 			position.Y,
		// 			textX + bounds.Width + bounds.Left,
		// 			position.Y + LayoutHeight
		// 		),
		// 		paint
		// 	);
		// }

		new SkiaRectBuilder( canvas )
			.WithRect( new SKRect( textX + bounds.Left, position.Y, textX + bounds.Width + bounds.Left,
				position.Y + LayoutHeight ) )
			.WithBorderRadius( BorderRadius.X, BorderRadius.Y )
			.WithFill( Background )
			.Draw();

		MeasurementPaint.TextSize = FontSize;
		canvas.DrawText( Value, textX, baseline, MeasurementPaint );
	}

	private void UpdateMeasurement()
	{
		YogaNode.MeasureFunction = ShouldMeasureText() ? MeasureText : null;
	}

	private bool ShouldMeasureText()
	{
		return !string.IsNullOrEmpty( Value ) && Children.Count is 0;
	}

	private SizeF MeasureText( YogaNode node, float width, YogaMeasureMode widthMode, float height,
		YogaMeasureMode heightMode )
	{
		var bounds = new SKRect();
		MeasurementPaint.MeasureText( Value, ref bounds );

		// Utiliser la largeur calculée à partir des bords
		var measuredWidth = bounds.Right - bounds.Left;
		var metrics = MeasurementPaint.FontMetrics;
		var measuredHeight = (metrics.Descent - metrics.Ascent) + metrics.Leading;

		return new SizeF(
			Resolve( measuredWidth, width, widthMode ),
			Resolve( measuredHeight, height, heightMode )
		);

		static float Resolve( float measured, float available, YogaMeasureMode mode ) => mode switch
		{
			YogaMeasureMode.Exactly => available,
			YogaMeasureMode.AtMost => MathF.Min( available, measured ),
			_ => measured
		};
	}

	protected override void OnChildrenAdded( Panel child )
	{
		UpdateMeasurement();
	}

	// Called after Yoga layout computed. x,y are absolute positions in root space.
	public override void Draw( SKCanvas canvas )
	{
		if ( Display is YogaDisplay.None ) return;

		// DrawBackground( canvas );
		DrawText( canvas );

		ClipOverflow( canvas );
		DrawChildren( canvas );

		IsDirty = false;
	}
}
