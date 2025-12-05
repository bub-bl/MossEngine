using System.Drawing;
using System.Numerics;
using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI;

public class Panel
{
	private static readonly SKPaint MeasurementPaint = new() { IsAntialias = true, TextSize = 20 };

	public Panel? Parent { get; private set; }
	public List<Panel> Children { get; } = [];

	// Yoga node for layout
	internal YogaNode YogaNode { get; }

	// Style-ish properties (very minimal)
	public SKColor Background { get; set; } = SKColors.Transparent;
	public SKColor Foreground { get; set; } = SKColors.Black;

	public string Text
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;

			UpdateMeasurement();
			MarkDirty();
		}
	}

	public float LayoutWidth => YogaNode.LayoutWidth;
	public float LayoutHeight => YogaNode.LayoutHeight;

	public Length Width
	{
		get => YogaNode.Width;
		set => YogaNode.Width = value;
	}

	public Length Height
	{
		get => YogaNode.Height;
		set => YogaNode.Height = value;
	}

	public Margin Margin
	{
		get => YogaNode.Margin;
		set => YogaNode.Margin = value;
	}

	public Padding Padding
	{
		get => YogaNode.Padding;
		set => YogaNode.Padding = value;
	}

	public YogaPositionType Position
	{
		get => YogaNode.Position;
		set => YogaNode.Position = value;
	}

	public YogaFlexDirection FlexDirection
	{
		get => YogaNode.FlexDirection;
		set => YogaNode.FlexDirection = value;
	}

	public YogaAlign AlignItems
	{
		get => YogaNode.AlignItems;
		set => YogaNode.AlignItems = value;
	}

	public YogaJustify JustifyContent
	{
		get => YogaNode.JustifyContent;
		set => YogaNode.JustifyContent = value;
	}

	public YogaAlign AlignContent
	{
		get => YogaNode.AlignContent;
		set => YogaNode.AlignContent = value;
	}

	public YogaAlign AlignSelf
	{
		get => YogaNode.AlignSelf;
		set => YogaNode.AlignSelf = value;
	}

	public YogaWrap FlexWrap
	{
		get => YogaNode.FlexWrap;
		set => YogaNode.FlexWrap = value;
	}

	public YogaOverflow Overflow
	{
		get => YogaNode.Overflow;
		set => YogaNode.Overflow = value;
	}

	public YogaDisplay Display
	{
		get => YogaNode.Display;
		set => YogaNode.Display = value;
	}

	public float Flex
	{
		get => YogaNode.Flex;
		set => YogaNode.Flex = value;
	}

	public float FlexGrow
	{
		get => YogaNode.FlexGrow;
		set => YogaNode.FlexGrow = value;
	}

	public float FlexShrink
	{
		get => YogaNode.FlexShrink;
		set => YogaNode.FlexShrink = value;
	}

	public float AspectRatio
	{
		get => YogaNode.AspectRatio;
		set => YogaNode.AspectRatio = value;
	}

	public Vector2 BorderRadius { get; set; }

	public Length Left
	{
		get => YogaNode.Left;
		set => YogaNode.Left = value;
	}

	public Length Top
	{
		get => YogaNode.Top;
		set => YogaNode.Top = value;
	}

	public Length Right
	{
		get => YogaNode.Right;
		set => YogaNode.Right = value;
	}

	public Length Bottom
	{
		get => YogaNode.Bottom;
		set => YogaNode.Bottom = value;
	}

	public Length Start
	{
		get => YogaNode.Start;
		set => YogaNode.Start = value;
	}

	public Length End
	{
		get => YogaNode.End;
		set => YogaNode.End = value;
	}

	public Length Horizontal
	{
		get => YogaNode.Horizontal;
		set => YogaNode.Horizontal = value;
	}

	public Length Vertical
	{
		get => YogaNode.Vertical;
		set => YogaNode.Vertical = value;
	}

	public YogaDirection Direction
	{
		get => YogaNode.Direction;
		set => YogaNode.Direction = value;
	}

	public Vector2 Center => new(YogaNode.LayoutLeft + YogaNode.Width / 2, YogaNode.LayoutTop + YogaNode.Height / 2);

	public bool IsDirty { get; protected set; } = true;

	public string DebugLabel { get; set; } = null!;

	public Panel()
	{
		YogaNode = new YogaNode();
		YogaNode.Context = this;
		YogaNode.AlignItems = YogaAlign.FlexStart;
		YogaNode.JustifyContent = YogaJustify.FlexStart;
		YogaNode.FlexDirection = YogaFlexDirection.Row;
		YogaNode.Position = YogaPositionType.Relative;

		UpdateMeasurement();
	}

	public void ComputeLayout()
	{
		// if ( !IsDirty ) return;
		YogaNode.CalculateLayout();
	}

	public void AddChild( Panel child )
	{
		if ( child.Parent is not null )
			throw new InvalidOperationException( "Already has parent" );

		child.Parent = this;

		Children.Add( child );

		var idx = Children.Count - 1;
		YogaNode.InsertChildAt( child.YogaNode, idx );

		UpdateMeasurement();
		MarkDirty();
	}

	public void RemoveChild( Panel child )
	{
		if ( child.Parent != this ) return;

		var idx = Children.IndexOf( child );
		if ( idx < 0 ) return;

		Children.RemoveAt( idx );
		YogaNode.RemoveChildAt( idx );

		child.Parent = null;
		MarkDirty();
	}

	protected Vector2 GetFinalPosition()
	{
		var local = new Vector2( YogaNode.LayoutLeft, YogaNode.LayoutTop );

		var parentAbs = YogaNode.Position is YogaPositionType.Absolute
			? Vector2.Zero
			: Parent?.GetFinalPosition() ?? Vector2.Zero;

		return parentAbs + local;
	}

	protected void DrawBackground( SKCanvas canvas )
	{
		var position = GetFinalPosition();

		new SkiaRectBuilder( canvas )
			.At( position.X, position.Y )
			.WithSize( LayoutWidth, LayoutHeight )
			.WithBorderRadius( BorderRadius.X, BorderRadius.Y )
			.WithFill( Background )
			.Draw();
	}

	protected void DrawText( SKCanvas canvas )
	{
		var position = GetFinalPosition();

		new SkiaTextBuilder( canvas )
			.At( position.X, position.Y )
			.WithSize( LayoutWidth, LayoutHeight )
			.WithText( Text )
			.WithColor( Foreground )
			.WithFontSize( 20 )
			.Draw();
	}

	private IDisposable? PushOverflowClip( SKCanvas canvas )
	{
		if ( Overflow is YogaOverflow.Visible )
			return null;

		var position = GetFinalPosition();
		var clipRect = new SKRect( position.X, position.Y, position.X + LayoutWidth, position.Y + LayoutHeight );
		var hasRadius = BorderRadius.LengthSquared() > 0.001f;

		canvas.Save();

		if ( hasRadius )
		{
			var roundRect = new SKRoundRect( clipRect, BorderRadius.X, BorderRadius.Y );
			canvas.ClipRoundRect( roundRect, SKClipOperation.Intersect, antialias: true );
		}
		else
		{
			canvas.ClipRect( clipRect, SKClipOperation.Intersect, antialias: true );
		}

		// if ( Overflow is YogaOverflow.Scroll )
		// {
		// 	var scrollY = MathF.Max( 0, YogaNode.ScrollY );
		// 	var scrollX = MathF.Max( 0, YogaNode.ScrollX );
		// 	canvas.Translate( -scrollX, -scrollY );
		//
		// 	return new CanvasRestore( canvas, translateBackX: scrollX, translateBackY: scrollY );
		// }

		return new CanvasRestore( canvas, translateBackX: 0, translateBackY: 0 );
	}
	
	protected void ClipOverflow( SKCanvas canvas )
	{
		using var clip = PushOverflowClip( canvas );
	}
	
	protected void DrawChildren( SKCanvas canvas )
	{
		foreach ( var c in Children )
		{
			c.Draw( canvas );
		}
	}

	private void UpdateMeasurement()
	{
		YogaNode.MeasureFunction = ShouldMeasureText() ? MeasureText : null;
	}

	private bool ShouldMeasureText()
	{
		return !string.IsNullOrEmpty( Text ) && Children.Count is 0;
	}

	private SizeF MeasureText( YogaNode node, float width, YogaMeasureMode widthMode, float height,
		YogaMeasureMode heightMode )
	{
		var bounds = new SKRect();
		MeasurementPaint.MeasureText( Text, ref bounds );

		var measuredWidth = bounds.Width;
		var metrics = MeasurementPaint.FontMetrics;
		var measuredHeight = MathF.Abs( metrics.Ascent ) + MathF.Abs( metrics.Descent );

		return new SizeF( Resolve( measuredWidth, width, widthMode ),
			Resolve( measuredHeight, height, heightMode ) );

		static float Resolve( float measured, float available, YogaMeasureMode mode ) => mode switch
		{
			YogaMeasureMode.Exactly => available,
			YogaMeasureMode.AtMost => MathF.Min( available, measured ),
			_ => measured
		};
	}

	public virtual void MarkDirty()
	{
		IsDirty = true;
		Parent?.MarkDirty();
	}

	// Called after Yoga layout computed. x,y are absolute positions in root space.
	public virtual void Draw( SKCanvas canvas )
	{
		if ( Display is YogaDisplay.None ) return;

		DrawBackground( canvas );
		DrawText( canvas );
		
		ClipOverflow( canvas );
		DrawChildren( canvas );

		IsDirty = false;
	}
}
