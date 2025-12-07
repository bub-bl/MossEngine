using System.Numerics;
using Silk.NET.Input;

namespace MossEngine.Windowing;

public sealed class PointerEventArgs( object target, Vector2 screenPosition, MouseButton? button ) : EventArgs
{
	public object Target { get; } = target;
	public Vector2 ScreenPosition { get; } = screenPosition;
	public MouseButton? Button { get; } = button;
	public bool Handled { get; set; }
}
