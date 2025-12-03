using System;
using System.Numerics;
using Yoga;

namespace MossEngine.UI.Yoga;

public unsafe partial class YogaNode
{
	private const float LayoutEdgeTolerance = 0.0001f;

	public float Width
	{
		get => YG.NodeLayoutGetWidth( this );
		set => YG.NodeStyleSetWidth( this, value );
	}

	public float Height
	{
		get => YG.NodeLayoutGetHeight( this );
		set => YG.NodeStyleSetHeight( this, value );
	}

	public Vector2 Position
	{
		get => new( YG.NodeLayoutGetLeft( this ), YG.NodeLayoutGetTop( this ) );
		set
		{
			YG.NodeStyleSetPosition( this, (YGEdge)YogaEdge.Left, value.X );
			YG.NodeStyleSetPosition( this, (YGEdge)YogaEdge.Top, value.Y );
		}
	}

	public float Left => YG.NodeLayoutGetLeft( this );
	public float Top => YG.NodeLayoutGetTop( this );
	public float Right => YG.NodeLayoutGetRight( this );
	public float Bottom => YG.NodeLayoutGetBottom( this );

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
