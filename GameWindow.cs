using Game.Pipelines;
using ImGuiNET;

namespace Game;

public sealed class GameWindow() : EditorWindow( "Editor - Game" )
{
	private UnlitRenderPipeline _unlitRenderPipeline = null!;

	protected override void OnInitialized()
	{
		_unlitRenderPipeline = new UnlitRenderPipeline( this );
		_unlitRenderPipeline.Initialize();
	}

	protected override void OnRender( double deltaTime )
	{
		_unlitRenderPipeline.Render();
		base.OnRender( deltaTime );
	}

	protected override void OnImGuiDraw()
	{
		ImGui.Begin( "Hello World" );
		ImGui.ShowDemoWindow();
		ImGui.End();
	}
}
