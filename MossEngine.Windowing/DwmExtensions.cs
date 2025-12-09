using System.Runtime.InteropServices;

namespace MossEngine.Windowing;

public static class DwmExtensions
{
	[DllImport( "dwmapi.dll", PreserveSig = true )]
	private static extern int DwmExtendFrameIntoClientArea( IntPtr hwnd, ref Margins margins );

	[StructLayout( LayoutKind.Sequential )]
	private struct Margins
	{
		public int Left;
		public int Right;
		public int Top;
		public int Bottom;
	}

	public static void RemoveTopBorder( IntPtr hwnd )
	{
		// Astuce : Top = -1 = étend le frame *à l’intérieur* du client
		var margins = new Margins
		{
			Left = 0,
			Right = 0,
			Top = -1, // <- enlève totalement la bordure blanche
			Bottom = 0
		};

		DwmExtendFrameIntoClientArea( hwnd, ref margins );
	}
}
