namespace MossEngine.Windowing.UI.Components;

public class TextChangedEventArgs( string text ) : EventArgs
{
	public string Text => text;
}
