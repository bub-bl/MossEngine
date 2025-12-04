using System.Numerics;
using MossEngine.UI.Yoga;
using SkiaSharp;
using Yoga;
using YogaValue = MossEngine.UI.Yoga.YogaValue;

namespace MossEngine.UI;

public abstract class BaseWidget
{
	public BaseWidget? Parent { get; private set; }
	public List<BaseWidget> Children { get; } = [];

	// Yoga node for layout
	internal YogaNode YogaNode { get; }

	// Style-ish properties (very minimal)
	public SKColor Background { get; set; } = SKColors.Transparent;
	public SKColor Foreground { get; set; } = SKColors.Black;

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
	
	public float Padding
	{
		get => YogaNode.GetPadding( YogaEdge.All );
		set => YogaNode.StyleSetPadding( YogaEdge.All, value );
	}
	
	public float PaddingLeft
	{
		get => YogaNode.GetPadding( YogaEdge.Left );
		set => YogaNode.StyleSetPadding( YogaEdge.Left, value );
	}
	
	public float PaddingRight
	{
		get => YogaNode.GetPadding( YogaEdge.Right );
		set => YogaNode.StyleSetPadding( YogaEdge.Right, value );
	}
	
	public float PaddingTop
	{
		get => YogaNode.GetPadding( YogaEdge.Top );
		set => YogaNode.StyleSetPadding( YogaEdge.Top, value );
	}
	
	public float PaddingBottom
	{
		get => YogaNode.GetPadding( YogaEdge.Bottom );
		set => YogaNode.StyleSetPadding( YogaEdge.Bottom, value );
	}
	
	public float PaddingHorizontal
	{
		get => YogaNode.GetPadding( YogaEdge.Horizontal );
		set => YogaNode.StyleSetPadding( YogaEdge.Horizontal, value );
	}

	public float PaddingVertical
	{
		get => YogaNode.GetPadding( YogaEdge.Vertical );
		set => YogaNode.StyleSetPadding( YogaEdge.Vertical, value );
	}

	public float Margin
	{
		get => YogaNode.GetMargin( YogaEdge.All );
		set => YogaNode.StyleSetMargin( YogaEdge.All, value );
	}

	public float MarginLeft
	{
		get => YogaNode.GetMargin( YogaEdge.Left );
		set => YogaNode.StyleSetMargin( YogaEdge.Left, value );
	}

	public float MarginRight
	{
		get => YogaNode.GetMargin( YogaEdge.Right );
		set => YogaNode.StyleSetMargin( YogaEdge.Right, value );
	}

	public float MarginTop
	{
		get => YogaNode.GetMargin( YogaEdge.Top );
		set => YogaNode.StyleSetMargin( YogaEdge.Top, value );
	}

	public float MarginBottom
	{
		get => YogaNode.GetMargin( YogaEdge.Bottom );
		set => YogaNode.StyleSetMargin( YogaEdge.Bottom, value );
	}
	
	public float MarginHorizontal
	{
		get => YogaNode.GetMargin( YogaEdge.Horizontal );
		set => YogaNode.StyleSetMargin( YogaEdge.Horizontal, value );
	}

	public float MarginVertical
	{
		get => YogaNode.GetMargin( YogaEdge.Vertical );
		set => YogaNode.StyleSetMargin( YogaEdge.Vertical, value );
	}

	public Vector2 BorderRadius { get; set; }
	
	public YogaPositionType Position
	{
		get => YogaNode.PositionType;
		set => YogaNode.PositionType = value;
	}
	
	public float LayoutLeft => YogaNode.LayoutLeft;
	public float LayoutTop => YogaNode.LayoutTop;
	public float LayoutRight => YogaNode.LayoutRight;
	public float LayoutBottom => YogaNode.LayoutBottom;

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
	
	public YogaDirection Direction { get; set; } = YogaDirection.Inherit;

	public Vector2 Center => new(YogaNode.LayoutLeft + YogaNode.Width / 2, YogaNode.LayoutTop + YogaNode.Height / 2);

	public bool IsDirty { get; protected set; } = true;

	protected BaseWidget()
	{
		YogaNode = new YogaNode();
		YogaNode.Context = this;
		YogaNode.Width = YogaValue.Auto.Value;
		YogaNode.Height = YogaValue.Auto.Value;
		YogaNode.FlexDirection = YogaFlexDirection.Column;
	}

	public void ComputeLayout()
	{
		// set root size in case it changed
		YogaNode.Direction = Direction;
		
		YogaNode.CalculateLayout( YogaNode.Width, YogaNode.Height, YogaNode.Direction );

		// Recursively compute layout for children - TODO - (I think this is not needed)
		// foreach ( var c in Children )
		// {
		// 	c.ComputeLayout();
		// }
	}

	public void AddChild( BaseWidget child )
	{
		if ( child.Parent is not null )
			throw new InvalidOperationException( "Already has parent" );

		child.Parent = this;

		Children.Add( child );
		YogaNode.InsertChildAt( child.YogaNode, YogaNode.Children.Count );

		MarkDirty();
	}

	public void RemoveChild( BaseWidget child )
	{
		if ( child.Parent != this ) return;

		var idx = Children.IndexOf( child );
		if ( idx < 0 ) return;

		Children.RemoveAt( idx );
		YogaNode.RemoveChildAt( idx );

		child.Parent = null;
		MarkDirty();
	}

	public virtual void MarkDirty()
	{
		IsDirty = true;
		Parent?.MarkDirty();
	}

	// Called after Yoga layout computed. x,y are absolute positions in root space.
	public abstract void Draw( SKCanvas canvas );
}
