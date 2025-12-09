using MossEngine.Windowing;

namespace MossEngine.Editor;

public static class Editor
{
	public static MossWindow MainWindow { get; set; } = null!;
	public static StatusBar StatusBar { get; } = new();
}
