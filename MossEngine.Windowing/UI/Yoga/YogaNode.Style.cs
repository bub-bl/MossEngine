using Yoga;

namespace MossEngine.Windowing.UI.Yoga;

public unsafe partial class YogaNode
{
	public bool HadOverflow => YG.NodeLayoutGetHadOverflow( this ) is not 0;

	public Length MinWidth
	{
		get => YG.NodeStyleGetMinWidth( this );
		set
		{
			switch ( value.Unit )
			{
				case YogaUnit.Point:
					YG.NodeStyleSetMinWidth( this, value );
					break;
				case YogaUnit.Percent:
					YG.NodeStyleSetMinWidthPercent( this, value );
					break;
				case YogaUnit.Auto:
					throw new NotSupportedException( "Auto is not supported" );
				case YogaUnit.FitContent:
					YG.NodeStyleSetMinWidthFitContent( this );
					break;
				case YogaUnit.MaxContent:
					YG.NodeStyleSetMinWidthMaxContent( this );
					break;
				case YogaUnit.Stretch:
					YG.NodeStyleSetMinWidthStretch( this );
					break;
			}
		}
	}

	public Length MaxWidth
	{
		get => YG.NodeStyleGetMaxWidth( this );
		set
		{
			switch ( value.Unit )
			{
				case YogaUnit.Point:
					YG.NodeStyleSetMaxWidth( this, value );
					break;
				case YogaUnit.Percent:
					YG.NodeStyleSetMaxWidthPercent( this, value );
					break;
				case YogaUnit.Auto:
					throw new NotSupportedException( "Auto is not supported" );
				case YogaUnit.FitContent:
					YG.NodeStyleSetMaxWidthFitContent( this );
					break;
				case YogaUnit.MaxContent:
					YG.NodeStyleSetMaxWidthMaxContent( this );
					break;
				case YogaUnit.Stretch:
					YG.NodeStyleSetMaxWidthStretch( this );
					break;
			}
		}
	}

	public Length Width
	{
		get => YG.NodeStyleGetWidth( this );
		set
		{
			switch ( value.Unit )
			{
				case YogaUnit.Point:
					YG.NodeStyleSetWidth( this, value );
					break;
				case YogaUnit.Percent:
					YG.NodeStyleSetWidthPercent( this, value );
					break;
				case YogaUnit.Auto:
					YG.NodeStyleSetWidthAuto( this );
					break;
				case YogaUnit.FitContent:
					YG.NodeStyleSetWidthFitContent( this );
					break;
				case YogaUnit.MaxContent:
					YG.NodeStyleSetWidthMaxContent( this );
					break;
				case YogaUnit.Stretch:
					YG.NodeStyleSetWidthStretch( this );
					break;
			}
		}
	}

	public Length MinHeight
	{
		get => YG.NodeStyleGetMinHeight( this );
		set
		{
			switch ( value.Unit )
			{
				case YogaUnit.Point:
					YG.NodeStyleSetMinHeight( this, value );
					break;
				case YogaUnit.Percent:
					YG.NodeStyleSetMinHeightPercent( this, value );
					break;
				case YogaUnit.Auto:
					throw new NotSupportedException( "Auto is not supported" );
				case YogaUnit.FitContent:
					YG.NodeStyleSetMinHeightFitContent( this );
					break;
				case YogaUnit.MaxContent:
					YG.NodeStyleSetMinHeightMaxContent( this );
					break;
				case YogaUnit.Stretch:
					YG.NodeStyleSetMinHeightStretch( this );
					break;
			}
		}
	}

	public Length MaxHeight
	{
		get => YG.NodeStyleGetMaxHeight( this );
		set
		{
			switch ( value.Unit )
			{
				case YogaUnit.Point:
					YG.NodeStyleSetMaxHeight( this, value );
					break;
				case YogaUnit.Percent:
					YG.NodeStyleSetMaxHeightPercent( this, value );
					break;
				case YogaUnit.Auto:
					throw new NotSupportedException( "Auto is not supported" );
				case YogaUnit.FitContent:
					YG.NodeStyleSetMaxHeightFitContent( this );
					break;
				case YogaUnit.MaxContent:
					YG.NodeStyleSetMaxHeightMaxContent( this );
					break;
				case YogaUnit.Stretch:
					YG.NodeStyleSetMaxHeightStretch( this );
					break;
			}
		}
	}

	public Length Height
	{
		get => YG.NodeStyleGetHeight( this );
		set
		{
			switch ( value.Unit )
			{
				case YogaUnit.Point:
					YG.NodeStyleSetHeight( this, value );
					break;
				case YogaUnit.Percent:
					YG.NodeStyleSetHeightPercent( this, value );
					break;
				case YogaUnit.Auto:
					YG.NodeStyleSetHeightAuto( this );
					break;
				case YogaUnit.FitContent:
					YG.NodeStyleSetHeightFitContent( this );
					break;
				case YogaUnit.MaxContent:
					YG.NodeStyleSetHeightMaxContent( this );
					break;
				case YogaUnit.Stretch:
					YG.NodeStyleSetHeightStretch( this );
					break;
			}
		}
	}

	public Margin Margin
	{
		get => new()
		{
			Left = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.Left ),
			Top = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.Top ),
			Right = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.Right ),
			Bottom = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.Bottom ),
			Start = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.Start ),
			End = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.End ),
			Horizontal = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.Horizontal ),
			Vertical = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.Vertical ),
			All = YG.NodeStyleGetMargin( this, (YGEdge)YogaEdge.All )
		};
		set
		{
			SetNodeStyleMargin( YogaEdge.Left, value.Left );
			SetNodeStyleMargin( YogaEdge.Top, value.Top );
			SetNodeStyleMargin( YogaEdge.Right, value.Right );
			SetNodeStyleMargin( YogaEdge.Bottom, value.Bottom );
			SetNodeStyleMargin( YogaEdge.Start, value.Start );
			SetNodeStyleMargin( YogaEdge.End, value.End );
			SetNodeStyleMargin( YogaEdge.Horizontal, value.Horizontal );
			SetNodeStyleMargin( YogaEdge.Vertical, value.Vertical );
			SetNodeStyleMargin( YogaEdge.All, value.All );
		}
	}

	public Padding Padding
	{
		get => new()
		{
			Left = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.Left ),
			Top = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.Top ),
			Right = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.Right ),
			Bottom = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.Bottom ),
			Start = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.Start ),
			End = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.End ),
			Horizontal = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.Horizontal ),
			Vertical = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.Vertical ),
			All = YG.NodeStyleGetPadding( this, (YGEdge)YogaEdge.All )
		};
		set
		{
			SetNodeStylePadding( YogaEdge.Left, value.Left );
			SetNodeStylePadding( YogaEdge.Top, value.Top );
			SetNodeStylePadding( YogaEdge.Right, value.Right );
			SetNodeStylePadding( YogaEdge.Bottom, value.Bottom );
			SetNodeStylePadding( YogaEdge.Start, value.Start );
			SetNodeStylePadding( YogaEdge.End, value.End );
			SetNodeStylePadding( YogaEdge.Horizontal, value.Horizontal );
			SetNodeStylePadding( YogaEdge.Vertical, value.Vertical );
			SetNodeStylePadding( YogaEdge.All, value.All );
		}
	}

	public Length Left
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.Left );
		set => SetNodeStylePosition( YogaEdge.Left, value );
	}

	public Length Top
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.Top );
		set => SetNodeStylePosition( YogaEdge.Top, value );
	}

	public Length Right
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.Right );
		set => SetNodeStylePosition( YogaEdge.Right, value );
	}

	public Length Bottom
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.Bottom );
		set => SetNodeStylePosition( YogaEdge.Bottom, value );
	}

	public Length Start
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.Start );
		set => SetNodeStylePosition( YogaEdge.Start, value );
	}

	public Length End
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.End );
		set => SetNodeStylePosition( YogaEdge.End, value );
	}

	public Length Horizontal
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.Horizontal );
		set => SetNodeStylePosition( YogaEdge.Horizontal, value );
	}

	public Length Vertical
	{
		get => YG.NodeStyleGetPosition( this, (YGEdge)YogaEdge.Vertical );
		set => SetNodeStylePosition( YogaEdge.Vertical, value );
	}

	public YogaPositionType Position
	{
		get => (YogaPositionType)YG.NodeStyleGetPositionType( this );
		set => YG.NodeStyleSetPositionType( this, (YGPositionType)value );
	}

	public YogaDirection Direction
	{
		get => (YogaDirection)YG.NodeLayoutGetDirection( this );
		set => YG.NodeStyleSetDirection( this, (YGDirection)value );
	}

	public YogaFlexDirection FlexDirection
	{
		get => (YogaFlexDirection)YG.NodeStyleGetFlexDirection( this );
		set => YG.NodeStyleSetFlexDirection( this, (YGFlexDirection)value );
	}

	public YogaAlign AlignItems
	{
		get => (YogaAlign)YG.NodeStyleGetAlignItems( this );
		set => YG.NodeStyleSetAlignItems( this, (YGAlign)value );
	}

	public YogaJustify JustifyContent
	{
		get => (YogaJustify)YG.NodeStyleGetJustifyContent( this );
		set => YG.NodeStyleSetJustifyContent( this, (YGJustify)value );
	}

	public YogaAlign AlignContent
	{
		get => (YogaAlign)YG.NodeStyleGetAlignContent( this );
		set => YG.NodeStyleSetAlignContent( this, (YGAlign)value );
	}

	public YogaAlign AlignSelf
	{
		get => (YogaAlign)YG.NodeStyleGetAlignSelf( this );
		set => YG.NodeStyleSetAlignSelf( this, (YGAlign)value );
	}

	public YogaWrap FlexWrap
	{
		get => (YogaWrap)YG.NodeStyleGetFlexWrap( this );
		set => YG.NodeStyleSetFlexWrap( this, (YGWrap)value );
	}

	public YogaOverflow Overflow
	{
		get => (YogaOverflow)YG.NodeStyleGetOverflow( this );
		set => YG.NodeStyleSetOverflow( this, (YGOverflow)value );
	}

	public YogaDisplay Display
	{
		get => (YogaDisplay)YG.NodeStyleGetDisplay( this );
		set => YG.NodeStyleSetDisplay( this, (YGDisplay)value );
	}

	public float Flex
	{
		get => YG.NodeStyleGetFlex( this );
		set => YG.NodeStyleSetFlex( this, value );
	}

	public float FlexGrow
	{
		get => YG.NodeStyleGetFlexGrow( this );
		set => YG.NodeStyleSetFlexGrow( this, value );
	}

	public float FlexShrink
	{
		get => YG.NodeStyleGetFlexShrink( this );
		set => YG.NodeStyleSetFlexShrink( this, value );
	}

	public Length FlexBasis
	{
		get => YG.NodeStyleGetFlexBasis( this );
		set
		{
			switch ( value.Unit )
			{
				case YogaUnit.Point:
					YG.NodeStyleSetFlexBasis( this, value.Value );
					break;
				case YogaUnit.Percent:
					YG.NodeStyleSetFlexBasisPercent( this, value.Value );
					break;
				case YogaUnit.Auto:
					YG.NodeStyleSetFlexBasisAuto( this );
					break;
				case YogaUnit.FitContent:
					YG.NodeStyleSetFlexBasisFitContent( this );
					break;
				case YogaUnit.MaxContent:
					YG.NodeStyleSetFlexBasisMaxContent( this );
					break;
				case YogaUnit.Stretch:
					YG.NodeStyleSetFlexBasisStretch( this );
					break;
			}
		}
	}

	public float AspectRatio
	{
		get => YG.NodeStyleGetAspectRatio( this );
		set => YG.NodeStyleSetAspectRatio( this, value );
	}

	public Length Gap
	{
		get => YG.NodeStyleGetGap( this, (YGGutter)YogaGutter.All );
		set => SetNodeStyleGap( YogaGutter.All, value );
	}

	public Length GapColumn
	{
		get => YG.NodeStyleGetGap( this, (YGGutter)YogaGutter.Column );
		set => SetNodeStyleGap( YogaGutter.Column, value );
	}

	public Length GapRow
	{
		get => YG.NodeStyleGetGap( this, (YGGutter)YogaGutter.Row );
		set => SetNodeStyleGap( YogaGutter.Row, value );
	}

	private void SetNodeStylePosition( YogaEdge edge, Length value )
	{
		switch ( value.Unit )
		{
			case YogaUnit.Point:
				YG.NodeStyleSetPosition( this, (YGEdge)edge, value.Value );
				break;
			case YogaUnit.Percent:
				YG.NodeStyleSetPositionPercent( this, (YGEdge)edge, value.Value );
				break;
			case YogaUnit.Auto:
				YG.NodeStyleSetPositionAuto( this, (YGEdge)edge );
				break;
			case YogaUnit.FitContent:
				throw new NotSupportedException( "FitContent is not supported" );
			case YogaUnit.MaxContent:
				throw new NotSupportedException( "MaxContent is not supported" );
			case YogaUnit.Stretch:
				throw new NotSupportedException( "Stretch is not supported" );
		}
	}

	private void SetNodeStylePadding( YogaEdge edge, Length value )
	{
		switch ( value.Unit )
		{
			case YogaUnit.Point:
				YG.NodeStyleSetPadding( this, (YGEdge)edge, value.Value );
				break;
			case YogaUnit.Percent:
				YG.NodeStyleSetPaddingPercent( this, (YGEdge)edge, value.Value );
				break;
			case YogaUnit.Auto:
				throw new NotSupportedException( "Auto is not supported" );
			case YogaUnit.FitContent:
				throw new NotSupportedException( "FitContent is not supported" );
			case YogaUnit.MaxContent:
				throw new NotSupportedException( "MaxContent is not supported" );
			case YogaUnit.Stretch:
				throw new NotSupportedException( "Stretch is not supported" );
		}
	}

	private void SetNodeStyleMargin( YogaEdge edge, Length value )
	{
		switch ( value.Unit )
		{
			case YogaUnit.Point:
				YG.NodeStyleSetMargin( this, (YGEdge)edge, value.Value );
				break;
			case YogaUnit.Percent:
				YG.NodeStyleSetMarginPercent( this, (YGEdge)edge, value.Value );
				break;
			case YogaUnit.Auto:
				YG.NodeStyleSetMarginAuto( this, (YGEdge)edge );
				break;
			case YogaUnit.FitContent:
				throw new NotSupportedException( "FitContent is not supported" );
			case YogaUnit.MaxContent:
				throw new NotSupportedException( "MaxContent is not supported" );
			case YogaUnit.Stretch:
				throw new NotSupportedException( "Stretch is not supported" );
		}
	}

	private void SetNodeStyleGap( YogaGutter gutter, Length gap )
	{
		switch ( gap.Unit )
		{
			case YogaUnit.Point:
				YG.NodeStyleSetGap( this, (YGGutter)gutter, gap.Value );
				break;
			case YogaUnit.Percent:
				YG.NodeStyleSetGapPercent( this, (YGGutter)gutter, gap.Value );
				break;
			default:
				throw new NotSupportedException( "Unsupported unit" );
		}
	}
}
