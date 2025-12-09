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
	private static extern bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy,
		uint uFlags );

	[DllImport( "user32.dll" )]
	private static extern bool GetWindowRect( IntPtr hWnd, out Rect rect );

	[DllImport( "user32.dll" )]
	private static extern IntPtr CallWindowProc( IntPtr lpPrevWndFunc, IntPtr hWnd, uint msg, IntPtr wParam,
		IntPtr lParam );

	[StructLayout( LayoutKind.Sequential )]
	private struct Rect
	{
		public int Left, Top, Right, Bottom;
	}

	public static void ApplyCustomFrame( IntPtr hwnd, int titlebarHeight, Func<bool> shouldHit )
	{
		// ------------------------------------------------------------
		// 🔧 Nettoyage du style : borderless mais redimensionnable
		// ------------------------------------------------------------
		long style = GetWindowLongPtr( hwnd, GWL_STYLE ).ToInt64();

		// style &= ~WS_CAPTION;
		// style &= ~WS_BORDER;

		// garder WS_THICKFRAME → resize natif mais plus de bordure visuelle
		style |= WS_THICKFRAME;

		SetWindowLongPtr( hwnd, GWL_STYLE, new IntPtr( style ) );

		// ------------------------------------------------------------
		// WndProc custom
		// ------------------------------------------------------------
		_customWndProc = ( hWnd, msg, wParam, lParam ) =>
		{
			switch ( msg )
			{
				// Indispensable : supprime totalement la zone non-client
				case WM_NCCALCSIZE:
					return IntPtr.Zero;

				case WM_NCHITTEST:
					return HitTestCustom( hWnd, lParam, titlebarHeight, shouldHit );
			}

			return CallWindowProc( _originalWndProc, hWnd, msg, wParam, lParam );
		};

		_originalWndProc = GetWindowLongPtr( hwnd, GWLP_WNDPROC );
		SetWindowLongPtr( hwnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate( _customWndProc ) );

		SetWindowPos( hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE );
	}

	private static IntPtr HitTestCustom( IntPtr hWnd, IntPtr lParam, int titlebarHeight, Func<bool> shouldHit )
	{
		int x = (short)(lParam.ToInt32() & 0xFFFF);
		int y = (short)((lParam.ToInt32() >> 16) & 0xFFFF);

		GetWindowRect( hWnd, out var rect );

		const int border = 8;

		bool left = x >= rect.Left && x < rect.Left + border;
		bool right = x <= rect.Right && x > rect.Right - border;
		bool top = y >= rect.Top && y < rect.Top + border;
		bool bottom = y <= rect.Bottom && y > rect.Bottom - border;

		if ( left && top ) return (IntPtr)HTTOPLEFT;
		if ( right && top ) return (IntPtr)HTTOPRIGHT;
		if ( left && bottom ) return (IntPtr)HTBOTTOMLEFT;
		if ( right && bottom ) return (IntPtr)HTBOTTOMRIGHT;
		if ( top ) return (IntPtr)HTTOP;
		if ( bottom ) return (IntPtr)HTBOTTOM;
		if ( left ) return (IntPtr)HTLEFT;
		if ( right ) return (IntPtr)HTRIGHT;

		// Zone draggable
		if ( !shouldHit() && y - rect.Top < titlebarHeight )
			return (IntPtr)2; // HTCAPTION

		return (IntPtr)1; // HTCLIENT
	}

	// Dwm attributes (inchangés)
	public enum WindowAttribute { WindowCornerPreference = 33 }

	public enum WindowCornerPreference { Default = 0, DoNotRound = 1, Round = 2, RoundSmall = 3 }

	[DllImport( "dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false )]
	internal static extern void DwmSetWindowAttribute(
		IntPtr hwnd, WindowAttribute attribute, ref WindowCornerPreference pvAttribute, uint cbAttribute );
}
