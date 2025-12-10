using System.Numerics;
using Silk.NET.Input;

namespace MossEngine.Windowing;

public abstract partial class MossWindow
{
	private IInputContext _input = null!;

	private void InitializeInput()
	{
		_input = Window.CreateInput();

		foreach ( var keyboard in _input.Keyboards )
			RegisterKeyboardEvents( keyboard );

		foreach ( var mouse in _input.Mice )
			RegisterMouseEvents( mouse );
	}

	private void RegisterKeyboardEvents( IKeyboard keyboard )
	{
		keyboard.KeyDown += InternalOnKeyDown;
		keyboard.KeyUp += InternalOnKeyUp;
		keyboard.KeyChar += InternalOnKeyChar;
	}

	private void UnregisterKeyboardEvents( IKeyboard keyboard )
	{
		keyboard.KeyDown -= InternalOnKeyDown;
		keyboard.KeyUp -= InternalOnKeyUp;
		keyboard.KeyChar -= InternalOnKeyChar;
	}

	private void RegisterMouseEvents( IMouse mouse )
	{
		mouse.DoubleClick += InternalOnDoubleClick;
		mouse.MouseDown += InternalOnMouseDown;
		mouse.MouseUp += InternalOnMouseUp;
		mouse.MouseMove += InternalOnMouseMove;
		mouse.Scroll += InternalOnScroll;
	}

	private void UnregisterMouseEvents( IMouse mouse )
	{
		mouse.DoubleClick -= InternalOnDoubleClick;
		mouse.MouseDown -= InternalOnMouseDown;
		mouse.MouseUp -= InternalOnMouseUp;
		mouse.MouseMove -= InternalOnMouseMove;
		mouse.Scroll -= InternalOnScroll;
	}

	private void InternalOnScroll( IMouse arg1, ScrollWheel arg2 )
	{
		OnScroll( arg2 );
	}

	private void InternalOnMouseMove( IMouse arg1, Vector2 arg2 )
	{
		OnMouseMove( arg2 );
	}

	private void InternalOnMouseUp( IMouse arg1, MouseButton arg2 )
	{
		var e = new MouseEventArgs { Button = arg2, Position = arg1.Position };
		OnMouseUp( e );
	}

	private void InternalOnMouseDown( IMouse arg1, MouseButton arg2 )
	{
		var e = new MouseEventArgs { Button = arg2, Position = arg1.Position };
		OnMouseDown( e );
	}

	private void InternalOnDoubleClick( IMouse arg1, MouseButton arg2, Vector2 arg3 )
	{
		var e = new MouseEventArgs { Button = arg2, Position = arg3 };
		OnMouseDoubleClick( e );
	}

	private void InternalOnKeyUp( IKeyboard arg1, Key arg2, int arg3 )
	{
		OnKeyUp( arg1, arg2 );
	}

	private void InternalOnKeyDown( IKeyboard arg1, Key arg2, int arg3 )
	{
		OnKeyDown( arg1, arg2 );
	}
	
	private void InternalOnKeyChar( IKeyboard arg1, char arg2 )
	{
		OnKeyChar( arg1, arg2 );
	}

	protected virtual void OnKeyUp( IKeyboard keyboard, Key key )
	{
	}

	protected virtual void OnKeyDown( IKeyboard keyboard, Key key )
	{
	}
	
	protected virtual void OnKeyChar( IKeyboard keyboard, char key )
	{
	}

	protected virtual void OnMouseUp( MouseEventArgs e )
	{
	}

	protected virtual void OnMouseDown( MouseEventArgs e )
	{
	}

	protected virtual void OnMouseDoubleClick( MouseEventArgs e )
	{
	}

	protected virtual void OnMouseMove( Vector2 position )
	{
	}

	protected virtual void OnScroll( ScrollWheel wheel )
	{
	}

	private void DisposeInput()
	{
		foreach ( var keyboard in _input.Keyboards )
			UnregisterKeyboardEvents( keyboard );

		foreach ( var mouse in _input.Mice )
			UnregisterMouseEvents( mouse );

		_input.Dispose();
	}
}
