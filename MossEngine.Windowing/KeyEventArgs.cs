using MossEngine.Windowing.UI.Components;
using Silk.NET.Input;

namespace MossEngine.Windowing;

public sealed class KeyEventArgs( object target, IKeyboard keyboard ) : EventArgs
{
	public IKeyboard Keyboard { get; } = keyboard;
	public object Target { get; } = target;
	public Key? Key { get; set; }
	public char? Character { get; set; }
	public KeyModifiers Modifiers { get; set; }
	public bool Handled { get; set; }
}
