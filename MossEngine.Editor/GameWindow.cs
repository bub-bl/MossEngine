// using System.Net.Mime;
// using System.Numerics;
// using MossEngine.UI;
// using MossEngine.UI.Components;
// using MossEngine.UI.Yoga;
// using Silk.NET.Maths;
// using SkiaSharp;
//
// namespace MossEngine.Editor;
//
// public sealed class GameWindow() : BaseEditorWindow( "Editor - Game" )
// {
// 	private UnlitRenderPipeline _unlitRenderPipeline = null!;
//
// 	protected override void OnInitialized()
// 	{
// 		_unlitRenderPipeline = new UnlitRenderPipeline( this );
// 		_unlitRenderPipeline.Initialize();
//
// 		var rectangle = new Panel
// 		{
// 			DebugLabel = "Rectangle",
// 			Background = SKColors.Red,
// 			Foreground = SKColors.White,
// 			Width = Length.Point( 200 ),
// 			Height = Length.Point( 200 ),
// 			Position = YogaPositionType.Relative,
// 		};
//
// 		var subRectangle = new Panel
// 		{
// 			DebugLabel = "SubRectangle",
// 			Background = SKColors.Blue,
// 			Width = Length.Point( 100 ),
// 			Height = Length.Point( 50 ),
// 			Position = YogaPositionType.Relative
// 		};
//
// 		var text = new Text
// 		{
// 			DebugLabel = "Text",
// 			Value = "Hello World",
// 			Background = SKColors.Green,
// 			Foreground = SKColors.White,
// 			Left = Length.Point( 0 ),
// 			Top = Length.Point( 0 ),
// 			Position = YogaPositionType.Relative
// 		};
//
// 		rectangle.AddChild( subRectangle );
// 		subRectangle.AddChild( text );
//
// 		var image = new Image
// 		{
// 			DebugLabel = "Image",
// 			Background = SKColors.Black,
// 			Width = Length.Point( 400 ),
// 			Height = Length.Point( 400 ),
// 			BorderRadius = new Vector2( 20, 20 ),
// 			Overflow = YogaOverflow.Hidden,
// 			ObjectFit = ObjectFit.ScaleDown,
// 			Src = @"C:\Users\bubbl\Pictures\gta-modding.jpg"
// 		};
//
// 		RootPanel.DebugLabel = "RootPanel";
// 		RootPanel.Width = Length.Percent( 90 );
// 		RootPanel.Height = Length.Percent( 90 );
// 		RootPanel.Background = SKColors.Yellow;
// 		RootPanel.AddChild( rectangle );
// 		RootPanel.AddChild( image );
//
// 		Console.WriteLine( $"RootPanel children count: {RootPanel.Children.Count}" );
// 	}
//
// 	protected override void OnRender( double deltaTime )
// 	{
// 		_unlitRenderPipeline.Render();
// 		base.OnRender( deltaTime );
// 	}
//
// 	protected override void OnDraw( SKCanvas canvas, Vector2D<int> size )
// 	{
// 		// using var backgroundPaint = new SKPaint();
// 		// backgroundPaint.Color = SKColor.Empty;
// 		//
// 		// using var accentPaint = new SKPaint();
// 		// accentPaint.Color = new SKColor( 0xFF, 0x00, 0x00, 0xFF );
// 		// accentPaint.IsAntialias = true;
// 		//
// 		// canvas.DrawRect( new SKRect( 0, 0, size.X, size.Y ), backgroundPaint );
// 		// canvas.DrawCircle( size.X / 2f, size.Y / 2f, Math.Min( size.X, size.Y ) / 4f, accentPaint );
// 	}
//
// 	protected override void OnImGuiDraw()
// 	{
// 		// ImGui.Begin( "Hello World" );
// 		// ImGui.ShowDemoWindow();
// 		// ImGui.End();
// 	}
// }
