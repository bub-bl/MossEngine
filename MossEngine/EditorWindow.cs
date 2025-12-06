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
			GapColumn = 8
		};
		mainLayout.AddChild( main );

		var left = new Panel
		{
			Width = Length.Point( 340 ),
			Height = Length.Percent( 100 ),
			Background = SKColor.FromHsl( 0, 0, 10 ),
			BorderRadius = new Vector2( 8 )
		};
		main.AddChild( left );

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
		main.AddChild( center );

		var right = new Panel
		{
			Width = Length.Point( 340 ),
			Height = Length.Percent( 100 ),
			Background = SKColor.FromHsl( 0, 0, 10 ),
			BorderRadius = new Vector2( 8 )
		};
		main.AddChild( right );

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
	}
}

public static class StatusBar
{
	public static List<Item> Items { get; } = [];
	
	public static void AddItem( Item item )
	{
		Items.Add( item );
	}
	
	public struct Item
	{
		
	}
}

public sealed class StatusBarPanel : Panel
{
	public StatusBarPanel()
	{
		Width = Length.Percent( 100 );
		Height = Length.Point( 32 );
		Flex = 1;
		FlexShrink = 0;
		Padding = new Padding { Horizontal = Length.Point( 8 ) };
		AlignItems = YogaAlign.Center;

		var fps = 1f / Time.Delta;
		var ms = Time.Delta * 1000f;
		var fpsText = $"{fps:F0} FPS ({ms:F0} ms)";

		var text = new Text
		{
			Value = fpsText,
			FontSize = 12,
			Foreground = SKColors.White,
			// Width = Length.Percent( 100 ),
			// Height = Length.Point( 40 ),
			Position = YogaPositionType.Relative,
		};
		text.Update += ( sender, args ) =>
		{
			var fps = 1f / Time.Delta;
			var ms = Time.Delta * 1000f;
			var fpsText = $"{fps:F0} FPS ({ms:F0} ms)";

			text.Value = fpsText;
		};
		AddChild( text );
	}
}

public sealed class ConsolePanel : Panel
{
	public ConsolePanel()
	{
		Width = Length.Percent( 100 );
		Height = Length.Percent( 100 );
		Background = SKColors.Black;
		Flex = 1;
		FlexGrow = 1;
		FlexShrink = 0;
		FlexDirection = YogaFlexDirection.Column;
		GapRow = Length.Point( 10 );

		ConsoleInterceptor.OnWrite += ConsoleInterceptorOnOnWrite;
	}

	private void ConsoleInterceptorOnOnWrite( string text )
	{
		var logText = new Text
		{
			Value = text,
			FontSize = 12,
			Foreground = SKColors.White,
			Background = SKColors.Red,
			Width = Length.Percent( 100 ),
			Height = Length.Point( 60 ),
			Position = YogaPositionType.Relative,
		};

		logText.PointerEnter += ( sender, args ) =>
		{
			if ( sender is not Panel panel ) return;
			panel.Background = SKColors.Gray;
		};

		logText.PointerLeave += ( sender, args ) =>
		{
			if ( sender is not Panel panel ) return;
			panel.Background = SKColors.Black;
		};

		AddChild( logText );
		// Console.WriteLine( "Added child to console panel" );
	}
}
