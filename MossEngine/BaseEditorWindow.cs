using MossEngine.Windowing;

namespace MossEngine;

public abstract class BaseEditorWindow( string title ) : EngineWindow( title, 1366, 768 )
{
	protected sealed override void OnBeforeRender( double deltaTime )
	{
		base.OnBeforeRender( deltaTime );
	}

	// protected sealed override void OnRender( double deltaTime )
	// {
	// 	base.OnRender( deltaTime );
	// }

	protected sealed override void OnAfterRender( double deltaTime )
	{
		base.OnAfterRender( deltaTime );
	}
}
