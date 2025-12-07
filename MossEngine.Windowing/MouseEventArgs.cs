using System.Numerics;
using Silk.NET.Input;

namespace MossEngine.Windowing;

public sealed class MouseEventArgs : EventArgs
{
	public Vector2 Position { get; init; }
	public MouseButton Button { get; init; }
}
