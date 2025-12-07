using System.Numerics;
using ImGuiNET;
using MossEngine.UI;
using MossEngine.UI.Components;
using MossEngine.UI.Yoga;
using MossEngine.Utility;
using SkiaSharp;

namespace MossEngine;

public sealed class EditorWindow() : BaseEditorWindow( "Editor" )
{
	protected override void OnInitialized()
	{
		var mainLayout = new Panel
		{
			Width = Length.Percent( 100 ),
			Height = Length.Percent( 100 ),
			Flex = 1,
			FlexDirection = YogaFlexDirection.Column
		};
		RootPanel.AddChild( mainLayout );

		var main = new Panel
		{
			Width = Length.Percent( 100 ),
			Padding = new Padding { Top = Length.Point( 8 ), Left = Length.Point( 8 ), Right = Length.Point( 8 ) },
			Background = SKColor.FromHsl( 0, 0, .2f ),
			Flex = 1,
			FlexGrow = 1,
			FlexShrink = 0
		};
		mainLayout.AddChild( main );

		var horizontalSplitter = new SplitterPanel( SplitterOrientation.Horizontal )
		{
			Width = Length.Percent( 100 ),
			Height = Length.Percent( 100 ),
			GapColumn = 4,
			Split = 0.25f,
			MinFirstSize = 220f,
			MinSecondSize = 220f,
			SplitterThickness = 6f
		};
		main.AddChild( horizontalSplitter );

		var left = new Panel
		{
			Width = Length.Percent( 100 ),
			Height = Length.Percent( 100 ),
			Background = SKColor.FromHsl( 0, 0, 10 ),
			BorderRadius = new Vector2( 8 )
		};
		horizontalSplitter.First.AddChild( left );

		var rightContainer = new Panel
		{
			Width = Length.Percent( 100 ),
			Height = Length.Percent( 100 ),
			Flex = 1,
			FlexGrow = 1,
			FlexDirection = YogaFlexDirection.Row,
			GapColumn = 8
		};
		horizontalSplitter.Second.AddChild( rightContainer );

		var center = new Panel
		{
			Width = Length.Stretch,
			Height = Length.Percent( 100 ),
			Background = SKColor.FromHsl( 0, 0, 10 ),
			BorderRadius = new Vector2( 8 ),
			Flex = 1,
			FlexGrow = 1,
			FlexShrink = 0
		};
		rightContainer.AddChild( center );

		var right = new Panel
		{
			Width = Length.Point( 340 ),
			Height = Length.Percent( 100 ),
			Background = SKColor.FromHsl( 0, 0, 10 ),
			BorderRadius = new Vector2( 8 )
		};
		rightContainer.AddChild( right );

		var bottom = new Panel { Width = Length.Percent( 100 ), Height = Length.Point( 32 ) };
		mainLayout.AddChild( bottom );

		var statusBar = new StatusBarPanel();
		bottom.AddChild( statusBar );

		// var consolePanel = new ConsolePanel();
		// bottom.AddChild( consolePanel );

		RootPanel.DebugLabel = "RootPanel";
		RootPanel.Width = Length.Percent( 90 );
		RootPanel.Height = Length.Percent( 90 );
		RootPanel.Background = SKColors.Black;

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

		// Nom affiché (fallback)
		var label = panel.GetType().Name;

		// TreeNode retourne true si le nœud est ouvert
		const ImGuiTreeNodeFlags flag = ImGuiTreeNodeFlags.DefaultOpen;

		if ( ImGui.TreeNodeEx( label, flag ) )
		{
			// Affiche les enfants dans le nœud ouvert
			foreach ( var child in panel.Children )
				DrawPanelNode( child );

			ImGui.TreePop();
		}

		ImGui.PopID();
	}
}
