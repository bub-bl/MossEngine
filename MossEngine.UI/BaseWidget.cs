using System.Numerics;
using MossEngine.UI.Yoga;
using SkiaSharp;

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

	public YogaDirection Direction
	{
		get => YogaNode.Direction;
		set => YogaNode.Direction = value;
	}

	public Vector2 Center => new(YogaNode.LayoutLeft + YogaNode.Width / 2, YogaNode.LayoutTop + YogaNode.Height / 2);

	public bool IsDirty { get; protected set; } = true;

	protected BaseWidget()
	{
		YogaNode = new YogaNode();
		YogaNode.Context = this;
		YogaNode.FlexDirection = YogaFlexDirection.Row;
	}

	public void ComputeLayout()
	{
		YogaNode.CalculateLayout();
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
