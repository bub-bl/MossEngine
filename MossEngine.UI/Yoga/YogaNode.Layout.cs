using Yoga;

namespace MossEngine.UI.Yoga;

public unsafe partial class YogaNode
{
	public Length Width
	{
		get => YG.NodeLayoutGetWidth( this );
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
					throw new NotSupportedException( "FitContent is not supported" );
				case YogaUnit.MaxContent:
					throw new NotSupportedException( "MaxContent is not supported" );
				case YogaUnit.Stretch:
					throw new NotSupportedException( "Stretch is not supported" );
			}
		}
	}

	public Length Height
	{
		get => YG.NodeLayoutGetHeight( this );
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
					throw new NotSupportedException( "FitContent is not supported" );
				case YogaUnit.MaxContent:
					throw new NotSupportedException( "MaxContent is not supported" );
				case YogaUnit.Stretch:
					throw new NotSupportedException( "Stretch is not supported" );
			}
		}
	}

	public YogaPositionType Position
	{
		get => (YogaPositionType)YG.NodeStyleGetPositionType( this );
		set => YG.NodeStyleSetPositionType( this, (YGPositionType)value );
	}

	public float LayoutLeft => YG.NodeLayoutGetLeft( this );
	public float LayoutTop => YG.NodeLayoutGetTop( this );
	public float LayoutRight => YG.NodeLayoutGetRight( this );
	public float LayoutBottom => YG.NodeLayoutGetBottom( this );

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

	public YogaDirection Direction
	{
		get => (YogaDirection)YG.NodeLayoutGetDirection( this );
		set => YG.NodeStyleSetDirection( this, (YGDirection)value );
	}

	public bool HadOverflow => YG.NodeLayoutGetHadOverflow( this ) is not 0;

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
			case YogaUnit.Undefined:
				YG.NodeStyleSetPadding( this, (YGEdge)edge, YG.YGUndefined );
				break;
			case YogaUnit.Auto:
			case YogaUnit.FitContent:
			case YogaUnit.MaxContent:
			case YogaUnit.Stretch:
				throw new NotSupportedException( "Padding does not support the specified YogaUnit" );
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
			case YogaUnit.Undefined:
				YG.NodeStyleSetMargin( this, (YGEdge)edge, YG.YGUndefined );
				break;
			case YogaUnit.Auto:
			case YogaUnit.FitContent:
			case YogaUnit.MaxContent:
			case YogaUnit.Stretch:
				throw new NotSupportedException( "Margin does not support the specified YogaUnit" );
		}
	}
}
