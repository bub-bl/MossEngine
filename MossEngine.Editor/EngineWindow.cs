using MossEngine.Windowing;
using MossEngine.Windowing.UI;
using MossEngine.Windowing.UI.Yoga;
using Silk.NET.Maths;
using SkiaSharp;

namespace MossEngine.Editor;

public abstract partial class EngineWindow( string title ) : MossWindow( title, 1366, 768 )
{
	// private RootPanelRenderer _rootPanelRenderer = null!;

	// public RootPanel RootPanel { get; private set; } = null!;
	// // public TitleBar TitleBar { get; private set; } = null!;
	// public Panel FrameContent { get; private set; } = null!;

	// protected sealed override void OnInitialized()
	// {
	// 	// RootPanel = new RootPanel { Flex = 1, FlexDirection = YogaFlexDirection.Column, Background = SKColors.Black };
	// 	// RootPanel.Resize( Window.Size.X, Window.Size.Y );
	// 	//
	// 	// _rootPanelRenderer = new RootPanelRenderer( Window, RootPanel );
	// 	//
	// 	// TitleBar = new TitleBar();
	// 	// RootPanel.AddChild( TitleBar );
	// 	//
	// 	// FrameContent = new Panel
	// 	// {
	// 	// 	Width = Length.Percent( 100 ),
	// 	// 	Height = Length.Percent( 100 ),
	// 	// 	Flex = 1,
	// 	// 	FlexGrow = 1,
	// 	// 	Background = SKColors.Black
	// 	// };
	// 	// RootPanel.AddChild( FrameContent );
	//
	// 	OnReady();
	// }
	//
	// protected virtual void OnReady()
	// {
	// }

	// protected override void OnDraw( SKCanvas canvas, Vector2D<int> size )
	// {
	// 	RootPanel.Resize( size.X, size.Y );
	// 	_rootPanelRenderer.Render( canvas );
	// }

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
