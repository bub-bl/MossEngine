using System.Numerics;
using MossEngine.UI.Yoga;

namespace MossEngine.UI;

public partial class Panel
{
	public Length MinWidth
	{
		get => YogaNode.MinWidth;
		set => YogaNode.MinWidth = value;
	}
	
	public Length MaxWidth
	{
		get => YogaNode.MaxWidth;
		set => YogaNode.MaxWidth = value;
	}
	
	public Length Width
	{
		get => YogaNode.Width;
		set => YogaNode.Width = value;
	}
	
	public Length MinHeight
	{
		get => YogaNode.MinHeight;
		set => YogaNode.MinHeight = value;
	}
	
	public Length MaxHeight
	{
		get => YogaNode.MaxHeight;
		set => YogaNode.MaxHeight = value;
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
	
	public Length Gap
	{
		get => YogaNode.Gap;
		set => YogaNode.Gap = value;
	}
	
	public Length GapRow
	{
		get => YogaNode.GapRow;
		set => YogaNode.GapRow = value;
	}
	
	public Length GapColumn
	{
		get => YogaNode.GapColumn;
		set => YogaNode.GapColumn = value;
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
	
	public Length FlexBasis
	{
		get => YogaNode.FlexBasis;
		set => YogaNode.FlexBasis = value;
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
}
