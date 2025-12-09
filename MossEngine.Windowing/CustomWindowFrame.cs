using System.Runtime.InteropServices;

namespace MossEngine.Windowing;

public static class CustomWindowFrame
{
	private delegate IntPtr WndProcDelegate( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam );

	private static WndProcDelegate _customWndProc;
	private static IntPtr _originalWndProc;

	private const int GWL_STYLE = -16;
	private const int GWLP_WNDPROC = -4;

	private const uint WM_NCCALCSIZE = 0x0083;
	private const uint WM_NCPAINT = 0x0085;
	private const uint WM_NCHITTEST = 0x0084;

	private const int WS_CAPTION = 0x00C00000;
	private const int WS_THICKFRAME = 0x00040000;
	private const int WS_SIZEBOX = 0x00040000;
	private const int WS_MINIMIZEBOX = 0x00020000;
	private const int WS_MAXIMIZEBOX = 0x00010000;
	private const int WS_SYSMENU = 0x00080000;
	private const int WS_BORDER = 0x00800000;

	private const int SWP_NOMOVE = 0x0002;
	private const int SWP_NOSIZE = 0x0001;
	private const int SWP_FRAMECHANGED = 0x0020;

	private const int HTLEFT = 10;
	private const int HTRIGHT = 11;
	private const int HTTOP = 12;
	private const int HTTOPLEFT = 13;
	private const int HTTOPRIGHT = 14;
	private const int HTBOTTOM = 15;
	private const int HTBOTTOMLEFT = 16;
	private const int HTBOTTOMRIGHT = 17;

	[DllImport( "user32.dll" )]
	private static extern IntPtr GetWindowLongPtr( IntPtr hWnd, int nIndex );

	[DllImport( "user32.dll" )]
	private static extern IntPtr SetWindowLongPtr( IntPtr hWnd, int nIndex, IntPtr value );

	[DllImport( "user32.dll" )]
	private static extern bool SetWindowPos(
		IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags );

	[DllImport( "user32.dll" )]
	private static extern bool GetWindowRect( IntPtr hWnd, out Rect rect );

	[DllImport( "user32.dll" )]
	private static extern IntPtr CallWindowProc(
		IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam );

	[StructLayout( LayoutKind.Sequential )]
	private struct Rect
	{
		public int Left, Top, Right, Bottom;
	}

	public static void ApplyCustomFrame( IntPtr hwnd, int titlebarHeight )
	{
		// Supprimer WS_CAPTION pour masquer la titlebar native§
		var style = GetWindowLongPtr( hwnd, GWL_STYLE ).ToInt64();
		// style &= ~WS_CAPTION; // <- important
		// style |= WS_THICKFRAME;
		style &= ~(WS_CAPTION | WS_THICKFRAME | WS_SYSMENU);

		SetWindowLongPtr( hwnd, GWL_STYLE, new IntPtr( style ) );

		_customWndProc = ( hWnd, msg, wParam, lParam ) =>
		{
			switch ( msg )
			{
				case WM_NCHITTEST:
					return HitTestCustom( hWnd, lParam, titlebarHeight );
			}

			return CallWindowProc( _originalWndProc, hWnd, msg, wParam, lParam );
		};

		_originalWndProc = GetWindowLongPtr( hwnd, GWLP_WNDPROC );
		SetWindowLongPtr( hwnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate( _customWndProc ) );

		SetWindowPos( hwnd, IntPtr.Zero, 0, 0, 0, 0,
			SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE );
	}

	private static IntPtr HitTestCustom(IntPtr hWnd, IntPtr lParam, int titlebarHeight)
	{
		// Récupérer la position du curseur
		int x = (short)(lParam.ToInt32() & 0xFFFF);
		int y = (short)((lParam.ToInt32() >> 16) & 0xFFFF);

		// Obtenir les dimensions de la fenêtre
		GetWindowRect(hWnd, out var rect);

		// Convertir les coordonnées d'écran en coordonnées client
		int windowX = x - rect.Left;
		int windowY = y - rect.Top;

		// // Vérifier si le curseur est dans la zone de la barre de titre personnalisée
		// if (windowY < titlebarHeight && windowX < (rect.Right - rect.Left) - 100) // Laisser de l'espace pour les boutons
		// {
		// 	return (IntPtr)2; // HTCAPTION - Permet le déplacement de la fenêtre
		// }

		// Gestion du redimensionnement (8px de bordure)
		const int border = 8;

		var left = x >= rect.Left && x < rect.Left + border;
		var right = x <= rect.Right && x > rect.Right - border;
		var top = y >= rect.Top && y < rect.Top + border;
		var bottom = y <= rect.Bottom && y > rect.Bottom - border;

		if ( left && top ) return (IntPtr)HTTOPLEFT;
		if ( right && top ) return (IntPtr)HTTOPRIGHT;
		if ( left && bottom ) return (IntPtr)HTBOTTOMLEFT;
		if ( right && bottom ) return (IntPtr)HTBOTTOMRIGHT;
		if ( top ) return (IntPtr)HTTOP;
		if ( bottom ) return (IntPtr)HTBOTTOM;
		if ( left ) return (IntPtr)HTLEFT;
		if ( right ) return (IntPtr)HTRIGHT;

		// // Zone draggable de la titlebar custom
		// if ( y - rect.Top < titlebarHeight )
		// 	return (IntPtr)2; // HTCAPTION → Windows va dragger la fenêtre

		return (IntPtr)1; // HTCLIENT → reste client normal
	}

	// The enum flag for DwmSetWindowAttribute's second parameter, which tells the function what attribute to set.
	// Copied from dwmapi.h
	public enum WindowAttribute
	{
		WindowCornerPreference = 33
	}

	// The DWM_WINDOW_CORNER_PREFERENCE enum for DwmSetWindowAttribute's third parameter, which tells the function
	// what value of the enum to set.
	// Copied from dwmapi.h
	public enum WindowCornerPreference
	{
		Default = 0,
		DoNotRound = 1,
		Round = 2,
		RoundSmall = 3
	}

	// Import dwmapi.dll and define DwmSetWindowAttribute in C# corresponding to the native function.
	[DllImport( "dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false )]
	internal static extern void DwmSetWindowAttribute( IntPtr hwnd,
		WindowAttribute attribute,
		ref WindowCornerPreference pvAttribute,
		uint cbAttribute );
}
