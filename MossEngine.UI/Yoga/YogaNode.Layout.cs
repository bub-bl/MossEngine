using System;
using System.Numerics;
using Yoga;

namespace MossEngine.UI.Yoga;

public unsafe partial class YogaNode
{
	private const float LayoutEdgeTolerance = 0.0001f;

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

	private void SetNodeStylePosition(YogaEdge edge, Length value)
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
	
	public YogaDirection Direction
	{
		get => (YogaDirection)YG.NodeLayoutGetDirection( this );
		set => YG.NodeStyleSetDirection( this, (YGDirection)value );
	}

	public bool HadOverflow => YG.NodeLayoutGetHadOverflow( this ) is not 0;

	public float GetMargin( YogaEdge edge )
	{
		return ResolveCompoundLayoutEdge( edge,
			static ( node, e ) => YG.NodeLayoutGetMargin( node, (YGEdge)e ),
			name: nameof( GetMargin ) );
	}

	public float GetBorder( YogaEdge edge )
	{
		return ResolveCompoundLayoutEdge( edge,
			static ( node, e ) => YG.NodeLayoutGetBorder( node, (YGEdge)e ),
			name: nameof( GetBorder ) );
	}

	public float GetPadding( YogaEdge edge )
	{
		return ResolveCompoundLayoutEdge( edge,
			static ( node, e ) => YG.NodeLayoutGetPadding( node, (YGEdge)e ),
			name: nameof( GetPadding ) );
	}

	private float ResolveCompoundLayoutEdge( YogaEdge requestedEdge,
		Func<YogaNode, YogaEdge, float> accessor,
		string name )
	{
		return requestedEdge switch
		{
			YogaEdge.All => RequireUniformValue( requestedEdge, accessor, name,
				YogaEdge.Left, YogaEdge.Top, YogaEdge.Right, YogaEdge.Bottom ),
			YogaEdge.Horizontal => RequireUniformValue( requestedEdge, accessor, name, YogaEdge.Left, YogaEdge.Right ),
			YogaEdge.Vertical => RequireUniformValue( requestedEdge, accessor, name, YogaEdge.Top, YogaEdge.Bottom ),
			_ => accessor( this, requestedEdge )
		};
	}

	private float RequireUniformValue( YogaEdge requestedEdge,
		Func<YogaNode, YogaEdge, float> accessor,
		string name,
		params YogaEdge[] expandedEdges )
	{
		var reference = accessor( this, expandedEdges[0] );
		for ( var i = 1; i < expandedEdges.Length; i++ )
		{
			var candidate = accessor( this, expandedEdges[i] );
			if ( Math.Abs( reference - candidate ) > LayoutEdgeTolerance )
			{
				throw new InvalidOperationException(
					$"Cannot resolve {name} for '{requestedEdge}' because the layout values differ between {string.Join( ", ", expandedEdges )}. Query concrete edges instead." );
			}
		}

		return reference;
	}
}
