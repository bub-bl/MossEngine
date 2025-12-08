using System.Numerics;
using ImGuiNET;
using MossEngine.Core.Utility;
using MossEngine.UI;
using MossEngine.UI.Components;
using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.Editor;

public sealed class EditorWindow() : EngineWindow( "Editor" )
{
	protected override void OnReady()
	{
		RootPanel.DebugLabel = "RootPanel";
		RootPanel.Background = SKColors.Black;

		var mainLayout = new Panel
		{
			Width = Length.Percent( 100 ), Height = Length.Percent( 100 ), FlexDirection = YogaFlexDirection.Column
		};
		RootPanel.AddChild( mainLayout );

		var main = new Panel
		{
			Width = Length.Percent( 100 ),
			Height = Length.Percent( 100 ),
			Padding = new Padding { Top = Length.Point( 8 ), Left = Length.Point( 8 ), Right = Length.Point( 8 ) },
			Flex = 1,
			FlexShrink = 0
		};
		mainLayout.AddChild( main );

		// ═══════════════════════════════════════════════════════════════
		// LAYOUT PRINCIPAL avec ResizablePanel
		// ═══════════════════════════════════════════════════════════════
		var mainContainer = new Panel
		{
			Width = Length.Percent( 100 ),
			Height = Length.Percent( 100 ),
			FlexDirection = YogaFlexDirection.Row,
			// Gap = 8
		};
		main.AddChild( mainContainer );

		// Panel LEFT (redimensionnable à droite)
		var left = new ResizablePanel( ResizeEdges.Right )
		{
			Width = Length.Point( 300 ),
			Height = Length.Percent( 100 ),
			MinWidth = 240,
			MaxWidth = 340,
			GripThickness = 8,
			// GripColor = SKColor.FromHsl( 0, 0, 20 ),
			// GripHoverColor = SKColor.FromHsl( 0, 0, 40 )
		};
		left.Content.Background = SKColor.FromHsl( 0, 0, 30 );
		left.Content.BorderRadius = new Vector2( 8 );
		mainContainer.AddChild( left );

		// Panel CENTER (flexible, prend l'espace restant)
		var center = new Panel
		{
			Width = Length.Stretch,
			Height = Length.Percent( 100 ),
			Background = SKColor.FromHsl( 0, 0, 10 ),
			BorderRadius = new Vector2( 8 ),
			Flex = 1,
			FlexGrow = 1,
			FlexShrink = 1
		};
		mainContainer.AddChild( center );

		// Panel RIGHT (redimensionnable à gauche)
		var right = new ResizablePanel( ResizeEdges.Left )
		{
			Width = Length.Point( 340 ),
			Height = Length.Percent( 100 ),
			MinWidth = 240,
			MaxWidth = 340,
			GripThickness = 8,
			// GripColor = SKColor.FromHsl( 0, 0, 20 ),
			// GripHoverColor = SKColor.FromHsl( 0, 0, 40 )
		};
		right.Content.Background = SKColor.FromHsl( 0, 0, 40 );
		right.Content.BorderRadius = new Vector2( 8 );
		mainContainer.AddChild( right );

		// ═══════════════════════════════════════════════════════════════
		// BOTTOM BAR
		// ═══════════════════════════════════════════════════════════════
		var bottom = new Panel { Width = Length.Percent( 100 ), Height = Length.Point( 32 ) };
		mainLayout.AddChild( bottom );

		var statusBar = new StatusBarPanel();
		bottom.AddChild( statusBar );

		Console.WriteLine( $"RootPanel children count: {RootPanel.Children.Count}" );
	}

	protected override void OnImGuiDraw()
	{
		var fps = 1f / Time.Delta;
		var ms = Time.Delta * 1000f;
		var fpsText = $"{fps:F0} FPS ({ms:F0} ms)";

		ImGui.Begin( "Debug" );
		ImGui.Text( $"RootPanel children count: {RootPanel.Children.Count}" );
		ImGui.Text( fpsText );
		ImGui.End();

		ImGui.Begin( "Hierarchy" );
		DrawPanelNode( RootPanel );
		ImGui.End();
	}

	private static void DrawPanelNode( Panel panel )
	{
		ImGui.PushID( panel.Id.ToString() );

		var label = panel.GetType().Name;
		const ImGuiTreeNodeFlags flag = ImGuiTreeNodeFlags.DefaultOpen;

		if ( ImGui.TreeNodeEx( label, flag ) )
		{
			foreach ( var child in panel.Children )
				DrawPanelNode( child );

			ImGui.TreePop();
		}

		ImGui.PopID();
	}
}
