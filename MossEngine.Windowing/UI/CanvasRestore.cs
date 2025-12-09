using SkiaSharp;

namespace MossEngine.Windowing.UI;

public readonly struct CanvasRestore : IDisposable
{
	private readonly SKCanvas _canvas;
	private readonly float _translateBackX;
	private readonly float _translateBackY;

	public CanvasRestore( SKCanvas canvas, float translateBackX, float translateBackY )
	{
		_canvas = canvas;
		_translateBackX = translateBackX;
		_translateBackY = translateBackY;
	}

	public void Dispose()
	{
		if ( _translateBackX != 0 || _translateBackY != 0 )
		{
			_canvas.Translate( _translateBackX, _translateBackY );
		}

		_canvas.Restore();
	}
}
