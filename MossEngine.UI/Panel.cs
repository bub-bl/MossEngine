using System.Diagnostics;
using System.Numerics;
using MossEngine.UI.Yoga;
using SkiaSharp;
using Yoga;

namespace MossEngine.UI;

public class Panel
{
	public Panel? Parent { get; private set; }
	public List<Panel> Children { get; } = [];

	// Yoga node for layout
	internal YogaNode YogaNode { get; }

	// Style-ish properties (very minimal)
	public SKColor Background { get; set; } = SKColors.Transparent;
	public SKColor Foreground { get; set; } = SKColors.Black;

	public string Text { get; set; } = string.Empty;

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
		get => YogaNode.PositionType;
		set => YogaNode.PositionType = value;
	}

	public Vector2 BorderRadius { get; set; }

	// public float LayoutLeft => YogaNode.LayoutLeft;
	// public float LayoutTop => YogaNode.LayoutTop;
	// public float LayoutRight => YogaNode.LayoutRight;
	// public float LayoutBottom => YogaNode.LayoutBottom;
	//
	// public float AbsoluteLeft => (Parent?.AbsoluteLeft ?? 0f) + LayoutLeft;
	// public float AbsoluteTop => (Parent?.AbsoluteTop ?? 0f) + LayoutTop;
	// public float AbsoluteRight => AbsoluteLeft + (YogaNode.LayoutRight - LayoutLeft);
	// public float AbsoluteBottom => AbsoluteTop + (YogaNode.LayoutBottom - LayoutTop);

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

	public string DebugLabel { get; set; }

	public Panel()
	{
		YogaNode = new YogaNode();
		YogaNode.Context = this;
		YogaNode.AlignItems = YogaAlign.FlexStart;
		YogaNode.JustifyContent = YogaJustify.FlexStart;
		YogaNode.FlexDirection = YogaFlexDirection.Row;
		YogaNode.PositionType = YogaPositionType.Relative;
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

	public bool HasParent => Parent is not null;

	public Length LayoutLeft => YogaNode.LayoutLeft;
	public Length LayoutTop => YogaNode.LayoutTop;

	private Vector2 GetFinalPosition()
	{
		var parentLeft = Parent?.LayoutLeft ?? Length.Zero;
		var parentTop = Parent?.LayoutTop ?? Length.Zero;

		var finalLeft = parentLeft + LayoutLeft;
		var finalTop = parentTop + LayoutTop;
		
		unsafe
		{
			if ( HasParent )
			{
				var parentNodeLayoutLeft = YG.NodeLayoutGetLeft( Parent.YogaNode );
				var parentNodeLayoutTop = YG.NodeLayoutGetTop( Parent.YogaNode );

				Console.WriteLine( "Name: " + DebugLabel + ", " + LayoutLeft + ", " + LayoutTop + ", " +
				                   Parent?.LayoutLeft + ", " + Parent?.LayoutTop + ", " + parentNodeLayoutLeft + ", " +
				                   parentNodeLayoutTop );
			}
		}

		return new Vector2( finalLeft, finalTop );
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

	public virtual void MarkDirty()
	{
		IsDirty = true;
		Parent?.MarkDirty();
	}

	// Called after Yoga layout computed. x,y are absolute positions in root space.
	public virtual void Draw( SKCanvas canvas )
	{
		DrawBackground( canvas );
		DrawText( canvas );

		foreach ( var c in Children )
		{
			c.Draw( canvas );
		}

		IsDirty = false;
	}
}
