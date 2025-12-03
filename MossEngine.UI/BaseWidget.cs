using System.Numerics;
using MossEngine.UI.Yoga;
using SkiaSharp;
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

	public float Padding
	{
		get => YogaNode.GetPadding( YogaEdge.All );
		set => YogaNode.StyleSetPadding( YogaEdge.All, value );
	}

	public float Margin
	{
		get => YogaNode.GetMargin( YogaEdge.All );
		set => YogaNode.StyleSetMargin( YogaEdge.All, value );
	}

	public Vector2 BorderRadius { get; set; }

	public Vector2 Size
	{
		get => new(YogaNode.Width, YogaNode.Height);
		set
		{
			YogaNode.Width = value.X;
			YogaNode.Height = value.Y;
		}
	}

	public Vector2 Position
	{
		get => new(YogaNode.Left, YogaNode.Top);
		set => YogaNode.Position = value;
	}

	public YogaDirection Direction { get; set; } = YogaDirection.Inherit;

	public Vector2 Center => new(YogaNode.Left + YogaNode.Width / 2, YogaNode.Top + YogaNode.Height / 2);

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
		YogaNode.Width = Size.X;
		YogaNode.Height = Size.Y;
		YogaNode.Position = Position;
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
