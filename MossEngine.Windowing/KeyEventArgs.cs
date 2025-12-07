using Silk.NET.Input;

namespace MossEngine.Windowing;

public sealed class KeyEventArgs( object target, Key key ) : EventArgs
{
	public object Target { get; } = target;
	public Key Key { get; } = key;
	public bool Handled { get; set; }
}
