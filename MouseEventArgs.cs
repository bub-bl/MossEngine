using System.Numerics;
using Silk.NET.Input;

namespace Game;

public sealed class MouseEventArgs : EventArgs
{
	public Vector2 Position { get; init; }
	public MouseButton Button { get; init; }
}
