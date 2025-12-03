using Yoga;

namespace MossEngine.UI.Yoga;

public unsafe partial class YogaNode
{
	public YogaPositionType PositionType
	{
		get => (YogaPositionType)YG.NodeStyleGetPositionType( this );
		set => YG.NodeStyleSetPositionType( this, (YGPositionType)value );
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
	
	public float AspectRatio
	{
		get => YG.NodeStyleGetAspectRatio( this );
		set => YG.NodeStyleSetAspectRatio( this, value );
	}

	public YogaValue StyleGetPosition( YogaEdge edge )
	{
		return YG.NodeStyleGetPosition( this, (YGEdge)edge ).ToYogaValue();
	}
	public void StyleSetPosition( YogaEdge edge, float position )
	{
		YG.NodeStyleSetPosition( this, (YGEdge)edge, position );
	}
	public void StyleSetPositionPercent( YogaEdge edge, float position )
	{
		YG.NodeStyleSetPositionPercent( this, (YGEdge)edge, position );
	}

	public YogaValue StyleGetMargin( YogaEdge edge )
	{
		return YG.NodeStyleGetMargin( this, (YGEdge)edge ).ToYogaValue();
	}
	public void StyleSetMargin( YogaEdge edge, float margin )
	{
		YG.NodeStyleSetMargin( this, (YGEdge)edge, margin );
	}
	public void StyleSetMarginAuto( YogaEdge edge )
	{
		YG.NodeStyleSetMarginAuto( this, (YGEdge)edge );
	}
	public void StyleSetMarginPercent( YogaEdge edge, float margin )
	{
		YG.NodeStyleSetMarginPercent( this, (YGEdge)edge, margin );
	}
	public YogaValue StyleGetPadding( YogaEdge edge )
	{
		return YG.NodeStyleGetPadding( this, (YGEdge)edge ).ToYogaValue();
	}
	public void StyleSetPadding( YogaEdge edge, float padding )
	{
		YG.NodeStyleSetPadding( this, (YGEdge)edge, padding );
	}
	public void StyleSetPaddingPercent( YogaEdge edge, float padding )
	{
		YG.NodeStyleSetPaddingPercent( this, (YGEdge)edge, padding );
	}
	public float StyleGetBorder( YogaEdge edge )
	{
		return YG.NodeStyleGetBorder( this, (YGEdge)edge );
	}
	public void StyleSetBorder( YogaEdge edge, float border )
	{
		YG.NodeStyleSetBorder( this, (YGEdge)edge, border );
	}
	public void StyleSetWidthPercent( float percent )
	{
		YG.NodeStyleSetWidthPercent( this, percent );
	}
	public void StyleSetWidthAuto()
	{
		YG.NodeStyleSetWidthAuto( this );
	}
	public void StyleGetWidth()
	{
		YG.NodeStyleGetWidth( this );
	}
	public void StyleSetHeightPercent( float percent )
	{
		YG.NodeStyleSetHeightPercent( this, percent );
	}
	public void StyleSetHeightAuto()
	{
		YG.NodeStyleSetHeightAuto( this );
	}
	public void StyleGetHeight()
	{
		YG.NodeStyleGetHeight( this );
	}

	public void CopyStyleFrom( YogaNode node )
	{
		YG.NodeCopyStyle( this, node );
	}

	public YogaDirection StyleGetDirection()
	{
		return (YogaDirection)YG.NodeStyleGetDirection( this );
	}

	public YGValue StyleGetGap( YogaGutter gutter = YogaGutter.All )
	{
		return YG.NodeStyleGetGap( this, (YGGutter)gutter );
	}

	public void StyleSetGap( float gapLength, YogaGutter gutter = YogaGutter.All )
	{
		YG.NodeStyleSetGap( this, (YGGutter)gutter, gapLength );
	}

	public YogaValue StyleGetMinWidth()
	{
		return YG.NodeStyleGetMinWidth( this ).ToYogaValue();
	}

	public void StyleSetMinHeight( float minHeight )
	{
		YG.NodeStyleSetMinHeight( this, minHeight );
	}

	public void StyleSetMinHeightPercent( float minHeightPercent )
	{
		YG.NodeStyleSetMinWidthPercent( this, minHeightPercent );
	}

	public YogaValue StyleGetMinHeight()
	{
		return YG.NodeStyleGetMinHeight( this ).ToYogaValue();
	}

	public void StyleSetMinWidth( float minWidth )
	{
		YG.NodeStyleSetMinWidth( this, minWidth );
	}

	public void StyleSetMinWidthPercent( float minWidthPercent )
	{
		YG.NodeStyleSetMinWidthPercent( this, minWidthPercent );
	}

	public YogaValue StyleGetMaxWidth()
	{
		return YG.NodeStyleGetMaxWidth( this ).ToYogaValue();
	}

	public void StyleSetMaxHeight( float minHeight )
	{
		YG.NodeStyleSetMaxHeight( this, minHeight );
	}

	public void StyleSetMaxHeightPercent( float minHeightPercent )
	{
		YG.NodeStyleSetMaxWidthPercent( this, minHeightPercent );
	}

	public YogaValue StyleGetMaxHeight()
	{
		return YG.NodeStyleGetMaxHeight( this ).ToYogaValue();
	}

	public void StyleSetMaxWidth( float minWidth )
	{
		YG.NodeStyleSetMaxWidth( this, minWidth );
	}

	public void StyleSetMaxWidthPercent( float minWidthPercent )
	{
		YG.NodeStyleSetMaxWidthPercent( this, minWidthPercent );
	}
}
