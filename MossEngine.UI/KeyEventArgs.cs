using Silk.NET.Input;

namespace MossEngine.UI;

public sealed class KeyEventArgs : EventArgs
{
	public Panel Target { get; }
	public Key Key { get; }
	public bool Handled { get; set; }

	internal KeyEventArgs( Panel target, Key key )
	{
		Target = target;
		Key = key;
	}
}
