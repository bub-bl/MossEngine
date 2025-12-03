using System.Numerics;
using ImGuiNET;
using MossEngine.Pipelines;
using MossEngine.UI;
using Silk.NET.Maths;
using SkiaSharp;

namespace MossEngine;

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

	protected override void OnSkiaDraw( SKCanvas canvas, Vector2D<int> size )
	{
		var rectangle = new Panel
		{
			Background = SKColors.Blue,
			Foreground = SKColors.White,
			Text = "Hello World",
			Size = new Vector2( 200, 200 ),
			Position = new Vector2( 100, 100 ),
			Padding = 20
		};
		RootPanel.AddChild( rectangle );

		Console.WriteLine( $"RootPanel children count: {RootPanel.Children.Count}" );

		// using var backgroundPaint = new SKPaint();
		// backgroundPaint.Color = SKColor.Empty;
		//
		// using var accentPaint = new SKPaint();
		// accentPaint.Color = new SKColor( 0xFF, 0x00, 0x00, 0xFF );
		// accentPaint.IsAntialias = true;
		//
		// canvas.DrawRect( new SKRect( 0, 0, size.X, size.Y ), backgroundPaint );
		// canvas.DrawCircle( size.X / 2f, size.Y / 2f, Math.Min( size.X, size.Y ) / 4f, accentPaint );
	}

	protected override void OnImGuiDraw()
	{
		// ImGui.Begin( "Hello World" );
		// ImGui.ShowDemoWindow();
		// ImGui.End();
	}
}
