using System.Numerics;
using Silk.NET.Input;

namespace MossEngine.UI;

public sealed class PointerEventArgs : EventArgs
{
    internal PointerEventArgs( Panel target, Vector2 screenPosition, MouseButton? button )
    {
        Target = target;
        ScreenPosition = screenPosition;
        LocalPosition = screenPosition - target.GetFinalPosition();
        Button = button;
    }

    public Panel Target { get; }
    public Vector2 ScreenPosition { get; }
    public Vector2 LocalPosition { get; }
    public MouseButton? Button { get; }
    public bool Handled { get; set; }
}
