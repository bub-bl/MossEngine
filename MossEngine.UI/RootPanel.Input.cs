using System.Numerics;
using Silk.NET.Input;

namespace MossEngine.UI;

public partial class RootPanel
{
	public void ProcessPointerMove( Vector2 position )
	{
		var hit = HitTest( position );

		if ( hit != _hoveredPanel )
		{
			if ( _hoveredPanel is not null )
			{
				var leaveArgs = new PointerEventArgs( _hoveredPanel, position, _activeButton );
				_hoveredPanel.RaisePointerLeave( leaveArgs );
			}

			_hoveredPanel = hit;

			if ( _hoveredPanel is not null )
			{
				var enterArgs = new PointerEventArgs( _hoveredPanel, position, _activeButton );
				_hoveredPanel.RaisePointerEnter( enterArgs );
			}
		}

		var target = _pressedPanel ?? _hoveredPanel;

		if ( target is not null )
		{
			var moveArgs = new PointerEventArgs( target, position, _activeButton );
			target.RaisePointerMove( moveArgs );
		}
	}

	public void ProcessPointerDown( MouseEventArgs args )
	{
		var hit = HitTest( args.Position );
		if ( hit is null ) return;

		_pressedPanel = hit;
		_activeButton = args.Button;

		FocusPanel( hit );

		var downArgs = new PointerEventArgs( hit, args.Position, args.Button );
		hit.RaisePointerDown( downArgs );
	}

	public void ProcessPointerUp( MouseEventArgs args )
	{
		var target = _pressedPanel ?? HitTest( args.Position );
		if ( target is null ) return;

		var upArgs = new PointerEventArgs( target, args.Position, args.Button );
		target.RaisePointerUp( upArgs );

		if ( _pressedPanel == target && target.ContainsPoint( args.Position ) )
		{
			var clickArgs = new PointerEventArgs( target, args.Position, args.Button );
			target.RaisePointerClick( clickArgs );
		}

		_pressedPanel = null;
		_activeButton = null;
	}

	public void ProcessKeyDown( Key key )
	{
		if ( _focusedPanel is null ) return;

		var args = new KeyEventArgs( _focusedPanel, key );
		_focusedPanel.RaiseKeyDown( args );
	}

	public void ProcessKeyUp( Key key )
	{
		if ( _focusedPanel is null ) return;

		var args = new KeyEventArgs( _focusedPanel, key );
		_focusedPanel.RaiseKeyUp( args );
	}
}
