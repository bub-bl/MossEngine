using System.Numerics;
using MossEngine.Core.Utility;
using Silk.NET.Input;

namespace MossEngine.Windowing;

public static class Cursor
{
	private static IInputContext? _input;

	public static Vector2 MousePosition
	{
		get
		{
			if ( _input is null || _input.Mice.Count is 0 ) 
				return Vector2.Zero;

			return _input.Mice[0].Position;
		}
	}

	public static StandardCursor Current
	{
		get => field;
		set
		{
			field = value;

			if ( _input is null ) return;

			foreach ( var mouse in _input.Mice )
			{
				mouse.Cursor.StandardCursor = value;
			}
		}
	}

	public static CursorMode Visibility
	{
		get => field;
		set
		{
			field = value;

			if ( _input is null ) return;

			foreach ( var mouse in _input.Mice )
			{
				mouse.Cursor.CursorMode = value;
			}
		}
	}

	public static IDisposable Scope( IInputContext context )
	{
		_input = context;

		return DisposeAction.Create( () =>
		{
			Current = StandardCursor.Arrow;
			Visibility = CursorMode.Normal;

			_input = null!;
		} );
	}
}
