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

	protected override void OnKeyUp( Key key )
	{
		base.OnKeyUp( key );
		RootPanel.ProcessKeyUp( key );
	}

	protected override void OnKeyDown( Key key )
	{
		base.OnKeyDown( key );
		RootPanel.ProcessKeyDown( key );
	}
}
