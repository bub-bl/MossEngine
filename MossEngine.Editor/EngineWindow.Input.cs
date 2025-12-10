using System.Numerics;
using MossEngine.Windowing;
using Silk.NET.Input;

namespace MossEngine.Editor;

public partial class EngineWindow
{
	protected override void OnMouseMove( Vector2 position )
	{
		base.OnMouseMove( position );
		RootPanel.ProcessPointerMove( position );
	}

	protected override void OnMouseUp( MouseEventArgs e )
	{
		base.OnMouseUp( e );
		RootPanel.ProcessPointerUp( e );
	}

	protected override void OnMouseDown( MouseEventArgs e )
	{
		base.OnMouseDown( e );
		RootPanel.ProcessPointerDown( e );
	}

	protected override void OnKeyUp( IKeyboard keyboard, Key key )
	{
		base.OnKeyUp( keyboard, key );
		RootPanel.ProcessKeyUp( keyboard, key );
	}

	protected override void OnKeyDown( IKeyboard keyboard, Key key )
	{
		base.OnKeyDown( keyboard, key );
		RootPanel.ProcessKeyDown( keyboard, key );
	}
	
	protected override void OnKeyChar( IKeyboard keyboard, char key )
	{
		base.OnKeyChar( keyboard, key );
		RootPanel.ProcessKeyChar( keyboard, key );
	}
}
