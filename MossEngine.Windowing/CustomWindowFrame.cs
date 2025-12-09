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
	
	[StructLayout( LayoutKind.Sequential )]
	private struct Rect
	{
		public int Left, Top, Right, Bottom;
	}
	
	public static void ApplyCustomFrame( IntPtr hwnd, int titlebarHeight, Func<bool> shouldHit )
	{
		var style = GetWindowLongPtr( hwnd, GWL_STYLE ).ToInt64();
		style |= WS_THICKFRAME;

		SetWindowLongPtr( hwnd, GWL_STYLE, new IntPtr( style ) );

		_customWndProc = ( hWnd, msg, wParam, lParam ) =>
		{
			switch ( msg )
			{
				// Remove the non-client area
				case WM_NCCALCSIZE:
					return IntPtr.Zero;

				// Handle hit testing
				case WM_NCHITTEST:
					return HitTestCustom( hWnd, lParam, titlebarHeight, shouldHit );
			}

			return CallWindowProc( _originalWndProc, hWnd, msg, wParam, lParam );
		};

		_originalWndProc = GetWindowLongPtr( hwnd, GWLP_WNDPROC );
		
		SetWindowLongPtr( hwnd, GWLP_WNDPROC, Marshal.GetFunctionPointerForDelegate( _customWndProc ) );
		SetWindowPos( hwnd, IntPtr.Zero, 0, 0, 0, 0, SWP_FRAMECHANGED | SWP_NOMOVE | SWP_NOSIZE );
	}
	
	public static void RemoveTopBorder( IntPtr hwnd )
	{
		var margins = new Margins
		{
			Left = 0,
			Right = 0,
			Top = -1, // <- Remove the white border
			Bottom = 0
		};

		DwmExtendFrameIntoClientArea( hwnd, ref margins );
	}

	private static IntPtr HitTestCustom( IntPtr hWnd, IntPtr lParam, int titlebarHeight, Func<bool> shouldHit )
	{
		int x = (short)(lParam.ToInt32() & 0xFFFF);
		int y = (short)((lParam.ToInt32() >> 16) & 0xFFFF);

		GetWindowRect( hWnd, out var rect );

		const int border = 8;

		var left = x >= rect.Left && x < rect.Left + border;
		var right = x <= rect.Right && x > rect.Right - border;
		var top = y >= rect.Top && y < rect.Top + border;
		var bottom = y <= rect.Bottom && y > rect.Bottom - border;

		if ( left && top ) return HTTOPLEFT;
		if ( right && top ) return HTTOPRIGHT;
		if ( left && bottom ) return HTBOTTOMLEFT;
		if ( right && bottom ) return HTBOTTOMRIGHT;
		if ( top ) return HTTOP;
		if ( bottom ) return HTBOTTOM;
		if ( left ) return HTLEFT;
		if ( right ) return HTRIGHT;

		// Draggable zone
		if ( !shouldHit() && y - rect.Top < titlebarHeight )
			return 2; // HTCAPTION

		return 1; // HTCLIENT
	}
}
