using System.Numerics;
using MossEngine.UI.Yoga;
using MossEngine.Windowing;

namespace MossEngine.UI;

public partial class Panel
{
	public event EventHandler<PointerEventArgs>? PointerEnter;
	public event EventHandler<PointerEventArgs>? PointerLeave;
	public event EventHandler<PointerEventArgs>? PointerMove;
	public event EventHandler<PointerEventArgs>? PointerDown;
	public event EventHandler<PointerEventArgs>? PointerUp;
	public event EventHandler<PointerEventArgs>? PointerClick;
	public event EventHandler<KeyEventArgs>? KeyDown;
	public event EventHandler<KeyEventArgs>? KeyUp;

	internal bool ContainsPoint( Vector2 point )
	{
		if ( !IsHitTestVisible || Display is YogaDisplay.None )
			return false;

		var position = GetFinalPosition();
		var withinX = point.X >= position.X && point.X <= position.X + LayoutWidth;
		var withinY = point.Y >= position.Y && point.Y <= position.Y + LayoutHeight;

		return withinX && withinY;
	}

	internal Panel? HitTest( Vector2 point )
	{
		if ( !ContainsPoint( point ) )
			return null;

		for ( var i = Children.Count - 1; i >= 0; i-- )
		{
			var child = Children[i];
			var childHit = child.HitTest( point );
			if ( childHit is not null )
				return childHit;
		}

		return this;
	}

	internal void RaisePointerEnter( PointerEventArgs args )
	{
		PointerEnter?.Invoke( this, args );
		OnPointerEnter( args );
	}

	internal void RaisePointerLeave( PointerEventArgs args )
	{
		PointerLeave?.Invoke( this, args );
		OnPointerLeave( args );
	}

	internal void RaisePointerMove( PointerEventArgs args )
	{
		PointerMove?.Invoke( this, args );
		OnPointerMove( args );
	}

	internal void RaisePointerDown( PointerEventArgs args )
	{
		PointerDown?.Invoke( this, args );
		OnPointerDown( args );
	}

	internal void RaisePointerUp( PointerEventArgs args )
	{
		PointerUp?.Invoke( this, args );
		OnPointerUp( args );
	}

	internal void RaisePointerClick( PointerEventArgs args )
	{
		PointerClick?.Invoke( this, args );
		OnPointerClick( args );
	}

	internal void RaiseKeyDown( KeyEventArgs args )
	{
		KeyDown?.Invoke( this, args );
		OnKeyDown( args );
	}

	internal void RaiseKeyUp( KeyEventArgs args )
	{
		KeyUp?.Invoke( this, args );
		OnKeyUp( args );
	}

	protected virtual void OnPointerEnter( PointerEventArgs args )
	{
	}

	protected virtual void OnPointerLeave( PointerEventArgs args )
	{
	}

	protected virtual void OnPointerMove( PointerEventArgs args )
	{
	}

	protected virtual void OnPointerDown( PointerEventArgs args )
	{
	}

	protected virtual void OnPointerUp( PointerEventArgs args )
	{
	}

	protected virtual void OnPointerClick( PointerEventArgs args )
	{
	}

	protected virtual void OnKeyDown( KeyEventArgs args )
	{
	}

	protected virtual void OnKeyUp( KeyEventArgs args )
	{
	}
}
