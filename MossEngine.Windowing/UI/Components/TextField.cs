using System.Numerics;
using MossEngine.Windowing.UI.Yoga;
using NLog.Fluent;
using Silk.NET.Input;
using SkiaSharp;

namespace MossEngine.Windowing.UI.Components;

public class TextField : Panel
{
	private string _text = string.Empty;
	private int _cursorPosition;
	private int _selectionStart = -1;
	private int _selectionEnd = -1;
	private float _scrollX;
	private bool _isFocused;
	private DateTime _lastCursorBlink = DateTime.Now;
	private bool _cursorVisible = true;
	private const double CursorBlinkInterval = 0.5;

	// Suppression continue (Backspace)
	private bool _deleteKeyDown;
	private DateTime _deleteStartTime = DateTime.MinValue;
	private DateTime _nextDeleteTime = DateTime.MinValue;
	private const double DeleteInitialDelay = 0.4; // délai initial avant répétition
	private const double DeleteRepeatDelay = 0.025; // délai entre répétitions

	// Déplacement curseur répétitif (flèches, Home, End)
	private bool _arrowKeyDown;
	private Key _currentArrowKey;
	private bool _arrowSelecting; // si Shift était pressé lors du keydown
	private DateTime _arrowStartTime = DateTime.MinValue;
	private DateTime _nextArrowTime = DateTime.MinValue;
	private const double ArrowInitialDelay = DeleteInitialDelay;
	private const double ArrowRepeatDelay = DeleteRepeatDelay;

	public SKColor TextColor { get; set; } = SKColors.Black;
	public SKColor PlaceholderColor { get; set; } = new(128, 128, 128);
	public SKColor SelectionColor { get; set; } = new(0, 120, 215, 100);
	public SKColor CursorColor { get; set; } = SKColors.Black;
	public SKColor FocusBorderColor { get; set; } = new(0, 120, 215);
	public string Placeholder { get; set; } = string.Empty;
	public float FontSize { get; set; } = 14f;
	public string FontFamily { get; set; } = "Arial";
	public int MaxLength { get; set; } = int.MaxValue;
	public bool IsPassword { get; set; } = false;
	public bool IsReadOnly { get; set; } = false;

	public event EventHandler<TextChangedEventArgs>? TextChanged;
	public event EventHandler? FocusGained;
	public event EventHandler? FocusLost;

	private SKPaint? _textPaint;
	private SKPaint? _placeholderPaint;

	public string Text
	{
		get => _text;
		set
		{
			if ( _text != value )
			{
				_text = value;
				_cursorPosition = Math.Clamp( _cursorPosition, 0, _text.Length );

				ClearSelection();
				MarkDirty();

				TextChanged?.Invoke( this, new TextChangedEventArgs( _text ) );
			}
		}
	}

	public bool IsFocused
	{
		get => _isFocused;
		private set
		{
			if ( _isFocused != value )
			{
				_isFocused = value;

				if ( _isFocused )
				{
					_cursorVisible = true;
					_lastCursorBlink = DateTime.Now;

					FocusGained?.Invoke( this, EventArgs.Empty );
				}
				else
				{
					ClearSelection();
					FocusLost?.Invoke( this, EventArgs.Empty );
				}

				MarkDirty();
			}
		}
	}

	public TextField()
	{
		// Default styling
		Background = SKColors.White;
		StrokeColor = new SKColor( 200, 200, 200 );
		StrokeWidth = 1f;
		BorderRadius = new Vector2( 4f, 4f );
		Padding = new Padding( 8f );
		Height = Length.Point( 32f );
		MinWidth = Length.Point( 100f );

		InitializePaints();
	}

	private void InitializePaints()
	{
		_textPaint = new SKPaint
		{
			Color = TextColor,
			TextSize = FontSize,
			IsAntialias = true,
			Typeface = SKTypeface.FromFamilyName( FontFamily )
		};

		_placeholderPaint = new SKPaint
		{
			Color = PlaceholderColor,
			TextSize = FontSize,
			IsAntialias = true,
			Typeface = SKTypeface.FromFamilyName( FontFamily )
		};
	}

	protected override void OnUpdate()
	{
		// Curseur : ne pas clignoter quand on maintient une flèche (ou potentiellement une suppression)
		if ( _isFocused && !_arrowKeyDown && (DateTime.Now - _lastCursorBlink).TotalSeconds >= CursorBlinkInterval )
		{
			_cursorVisible = !_cursorVisible;
			_lastCursorBlink = DateTime.Now;
			MarkDirty();
		}

		var now = DateTime.Now;

		// Suppression continue (Backspace): initial immediate handled in OnKeyDown, puis répétitions après delay
		if ( _deleteKeyDown && _isFocused )
		{
			if ( now >= _nextDeleteTime )
			{
				HandleDelete( false );
				_nextDeleteTime = now + TimeSpan.FromSeconds( DeleteRepeatDelay );
				// pendant suppression continue, garder curseur visible et reset blink timestamp
				_cursorVisible = true;
				_lastCursorBlink = DateTime.Now;
				MarkDirty();
			}
		}

		// Déplacement curseur répétitif (flèches, Home, End)
		if ( _arrowKeyDown && _isFocused )
		{
			if ( now >= _nextArrowTime )
			{
				PerformArrowAction( _currentArrowKey, _arrowSelecting );
				_nextArrowTime = now + TimeSpan.FromSeconds( ArrowRepeatDelay );

				// pendant déplacement continu, curseur visible et ne doit pas clignoter
				_cursorVisible = true;
				_lastCursorBlink = DateTime.Now;
				MarkDirty();
			}
		}
	}

	// Effectue l'action pour une touche flèche/Home/End (appui immédiat ou répétition)
	private void PerformArrowAction( Key key, bool selecting )
	{
		switch ( key )
		{
			case Key.Left:
				MoveCursor( -1, selecting );
				break;
			case Key.Right:
				MoveCursor( 1, selecting );
				break;
			case Key.Home:
				_cursorPosition = 0;
				if ( !selecting ) ClearSelection();
				else UpdateSelection();
				break;
			case Key.End:
				_cursorPosition = _text.Length;
				if ( !selecting ) ClearSelection();
				else UpdateSelection();
				break;
		}

		EnsureCursorVisible();
	}

	public override void Draw( SKCanvas canvas )
	{
		if ( Display is YogaDisplay.None ) return;

		// Update paints
		if ( _textPaint is not null )
		{
			_textPaint.Color = TextColor;
			_textPaint.TextSize = FontSize;
		}

		if ( _placeholderPaint is not null )
		{
			_placeholderPaint.Color = PlaceholderColor;
			_placeholderPaint.TextSize = FontSize;
		}

		// Draw background with focus border
		DrawTextFieldBackground( canvas );

		// Clip to content area
		var position = GetFinalPosition();
		var paddingLeft = Padding.Left;
		var paddingTop = Padding.Top;
		var paddingRight = Padding.Right;
		var paddingBottom = Padding.Bottom;

		var contentX = position.X + paddingLeft;
		var contentY = position.Y + paddingTop;
		var contentWidth = LayoutWidth - paddingLeft - paddingRight;
		var contentHeight = LayoutHeight - paddingTop - paddingBottom;

		canvas.Save();
		canvas.ClipRect( new SKRect( contentX, contentY, contentX + contentWidth, contentY + contentHeight ) );

		// Selection
		if ( HasSelection() && _isFocused )
		{
			DrawSelection( canvas, contentX, contentY, contentHeight );
		}

		// Text / Placeholder
		if ( string.IsNullOrEmpty( _text ) && !string.IsNullOrEmpty( Placeholder ) && !_isFocused )
		{
			DrawPlaceholder( canvas, contentX, contentY, contentHeight );
		}
		else
		{
			DrawText( canvas, contentX, contentY, contentHeight );
		}

		// Cursor
		if ( _isFocused && _cursorVisible )
		{
			float cursorPos = _cursorPosition;

			// Si une sélection est active, placer curseur à la fin de la sélection
			if ( HasSelection() )
			{
				var (_, end) = GetOrderedSelection();
				cursorPos = end;
			}

			DrawCursor( canvas, contentX, contentY, contentHeight, cursorPos );
		}

		canvas.Restore();
		DrawChildren( canvas );
	}

	private void DrawTextFieldBackground( SKCanvas canvas )
	{
		var position = GetFinalPosition();
		var borderColor = _isFocused ? FocusBorderColor : StrokeColor;
		var borderWidth = _isFocused ? 2f : StrokeWidth;

		// Draw background
		new SkiaRectBuilder( canvas )
			.At( position.X, position.Y )
			.WithSize( LayoutWidth, LayoutHeight )
			.WithBorderRadius( BorderRadius.X, BorderRadius.Y )
			.WithFill( Background )
			.Draw();

		// Draw border
		if ( borderWidth > 0 )
		{
			var halfStroke = borderWidth * 0.5f;

			var strokeRect = SKRect.Create(
				position.X + halfStroke,
				position.Y + halfStroke,
				Math.Max( 0, LayoutWidth - borderWidth ),
				Math.Max( 0, LayoutHeight - borderWidth )
			);

			using var paint = new SKPaint();
			paint.Color = borderColor;
			paint.Style = SKPaintStyle.Stroke;
			paint.StrokeWidth = borderWidth;
			paint.IsAntialias = true;

			if ( BorderRadius.X > 0 || BorderRadius.Y > 0 )
			{
				var radiusX = Math.Max( 0, BorderRadius.X - halfStroke );
				var radiusY = Math.Max( 0, BorderRadius.Y - halfStroke );

				canvas.DrawRoundRect( strokeRect, radiusX, radiusY, paint );
			}
			else
			{
				canvas.DrawRect( strokeRect, paint );
			}
		}
	}

	private void DrawSelection( SKCanvas canvas, float x, float y, float height )
	{
		if ( _textPaint is null ) return;

		var displayText = GetDisplayText();
		var (start, end) = GetOrderedSelection();

		var textBeforeSelection = displayText[..start];
		var selectedText = displayText.Substring( start, end - start );

		var startX = x - _scrollX + _textPaint.MeasureText( textBeforeSelection );
		var selectionWidth = _textPaint.MeasureText( selectedText );

		using var selectionPaint = new SKPaint();
		selectionPaint.Color = SelectionColor;
		selectionPaint.Style = SKPaintStyle.Fill;

		var metrics = _textPaint.FontMetrics;
		var textHeight = metrics.Descent - metrics.Ascent;

		// Aligner le rectangle sur le haut du texte
		var selectionY = y + (height - textHeight) / 2;

		canvas.DrawRect( startX, selectionY, selectionWidth, textHeight, selectionPaint );
	}

	private void DrawText( SKCanvas canvas, float x, float y, float height )
	{
		if ( _textPaint is null ) return;

		var displayText = GetDisplayText();
		if ( string.IsNullOrEmpty( displayText ) ) return;

		var metrics = _textPaint.FontMetrics;
		var textY = y + (height - (metrics.Descent - metrics.Ascent)) / 2 - metrics.Ascent;

		canvas.DrawText( displayText, x - _scrollX, textY, _textPaint );
	}

	private void DrawPlaceholder( SKCanvas canvas, float x, float y, float height )
	{
		if ( _placeholderPaint is null || string.IsNullOrEmpty( Placeholder ) ) return;

		var metrics = _placeholderPaint.FontMetrics;
		var textY = y + (height - (metrics.Descent - metrics.Ascent)) / 2 - metrics.Ascent;

		canvas.DrawText( Placeholder, x, textY, _placeholderPaint );
	}

	private void DrawCursor( SKCanvas canvas, float x, float y, float height, float cursorPosition )
	{
		if ( _textPaint is null ) return;

		var displayText = GetDisplayText();
		var textBeforeCursor = displayText[..(int)cursorPosition];
		var cursorX = x - _scrollX + _textPaint.MeasureText( textBeforeCursor );

		using var cursorPaint = new SKPaint();
		cursorPaint.Color = CursorColor;
		cursorPaint.Style = SKPaintStyle.Fill;
		cursorPaint.StrokeWidth = 1.5f;

		var metrics = _textPaint.FontMetrics;
		var textHeight = metrics.Descent - metrics.Ascent;
		var cursorY = y + (height - textHeight) / 2;

		canvas.DrawLine( cursorX, cursorY, cursorX, cursorY + textHeight, cursorPaint );
	}


	private string GetDisplayText()
	{
		if ( IsPassword && !string.IsNullOrEmpty( _text ) )
			return new string( '•', _text.Length );

		return _text;
	}

	protected override void OnPointerClick( PointerEventArgs args )
	{
		base.OnPointerClick( args );
		IsFocused = true;

		// Position cursor at click location
		var clickX = args.ScreenPosition.X - GetFinalPosition().X - Padding.Left + _scrollX;

		_cursorPosition = GetCharacterIndexAtX( clickX );
		_cursorVisible = true;
		_lastCursorBlink = DateTime.Now;

		MarkDirty();
	}

	protected override void OnPointerDown( PointerEventArgs args )
	{
		base.OnPointerDown( args );

		if ( !_isFocused ) return;

		var clickX = args.ScreenPosition.X - GetFinalPosition().X - Padding.Left + _scrollX;
		var clickPos = GetCharacterIndexAtX( clickX );

		_selectionStart = clickPos;
		_cursorPosition = clickPos;
		_selectionEnd = -1;

		MarkDirty();
	}

	protected override void OnPointerMove( PointerEventArgs args )
	{
		base.OnPointerMove( args );

		if ( _isFocused && _selectionStart >= 0 && args.Button is MouseButton.Left )
		{
			var clickX = args.ScreenPosition.X - GetFinalPosition().X - Padding.Left + _scrollX;
			var currentPos = GetCharacterIndexAtX( clickX );

			if ( currentPos != _cursorPosition )
			{
				_cursorPosition = currentPos;
				_selectionEnd = currentPos;

				MarkDirty();
			}
		}
	}

	protected override void OnKeyDown( KeyEventArgs args )
	{
		base.OnKeyDown( args );

		var shift = args.Keyboard.IsKeyPressed( Key.ShiftLeft );
		var ctrl = args.Keyboard.IsKeyPressed( Key.ControlLeft );

		switch ( args.Key )
		{
			case Key.Backspace:
				if ( !_deleteKeyDown )
				{
					_deleteKeyDown = true;
					_deleteStartTime = DateTime.Now;
					HandleDelete( false );
					_nextDeleteTime = _deleteStartTime + TimeSpan.FromSeconds( DeleteInitialDelay );

					_cursorVisible = true;
					_lastCursorBlink = DateTime.Now;
				}

				break;

			case Key.Left:
			case Key.Right:
			case Key.Home:
			case Key.End:
				if ( !_arrowKeyDown )
				{
					_arrowKeyDown = true;
					_currentArrowKey = args.Key.Value;
					_arrowSelecting = shift;
					_arrowStartTime = DateTime.Now;

					PerformArrowAction( _currentArrowKey, _arrowSelecting );
					_nextArrowTime = _arrowStartTime + TimeSpan.FromSeconds( ArrowInitialDelay );

					_cursorVisible = true;
					_lastCursorBlink = DateTime.Now;
				}

				break;

			case Key.Q when ctrl:
				SelectAll();
				_cursorVisible = true;
				_lastCursorBlink = DateTime.Now;
				MarkDirty();
				break;

			case Key.C when ctrl:
				CopyToClipboard();
				break;

			case Key.X when ctrl:
				CutToClipboard();
				break;

			case Key.V when ctrl:
				PasteFromClipboard();
				break;
		}

		EnsureCursorVisible();
	}

	protected override void OnKeyUp( KeyEventArgs args )
	{
		base.OnKeyUp( args );

		// Backspace released
		if ( args.Key == Key.Backspace )
		{
			_deleteKeyDown = false;
			_deleteStartTime = DateTime.MinValue;
			_nextDeleteTime = DateTime.MinValue;
		}

		// Arrow/Home/End released
		if ( args.Key == _currentArrowKey &&
		     (args.Key == Key.Left || args.Key == Key.Right || args.Key == Key.Home || args.Key == Key.End) )
		{
			_arrowKeyDown = false;
			_arrowStartTime = DateTime.MinValue;
			_nextArrowTime = DateTime.MinValue;
		}
	}

	protected override void OnKeyChar( KeyEventArgs args )
	{
		base.OnKeyChar( args );

		if ( !_isFocused || IsReadOnly || args.Character is null ) return;
		HandleCharacterInput( args.Character.Value );
	}

	private void MoveCursor( int delta, bool selecting )
	{
		var newPos = Math.Clamp( _cursorPosition + delta, 0, _text.Length );
		if ( newPos == _cursorPosition ) return;

		_cursorPosition = newPos;

		if ( selecting )
		{
			UpdateSelection();
		}
		else
		{
			ClearSelection();
		}
	}

	private void UpdateSelection()
	{
		if ( _selectionStart < 0 )
			_selectionStart = _cursorPosition - (_cursorPosition > 0 ? 1 : 0);

		_selectionEnd = _cursorPosition;
	}

	private void InsertText( string textToInsert )
	{
		if ( string.IsNullOrEmpty( textToInsert ) ) return;

		DeleteSelection();

		if ( _text.Length + textToInsert.Length > MaxLength )
		{
			textToInsert = textToInsert[..Math.Max( 0, MaxLength - _text.Length )];
		}

		_text = _text.Insert( _cursorPosition, textToInsert );
		_cursorPosition += textToInsert.Length;

		ClearSelection();
		TextChanged?.Invoke( this, new TextChangedEventArgs( _text ) );
	}

	private void HandleDelete( bool resetTimer = true )
	{
		if ( HasSelection() )
		{
			DeleteSelection();
		}
		else if ( _cursorPosition > 0 )
		{
			_text = _text.Remove( _cursorPosition - 1, 1 );
			_cursorPosition--;

			TextChanged?.Invoke( this, new TextChangedEventArgs( _text ) );
		}

		if ( resetTimer )
		{
			_deleteStartTime = DateTime.MinValue;
			_nextDeleteTime = DateTime.MinValue;
		}
	}

	private void DeleteSelection()
	{
		if ( !HasSelection() ) return;

		var (start, end) = GetOrderedSelection();

		_text = _text.Remove( start, end - start );
		_cursorPosition = start;

		ClearSelection();
		TextChanged?.Invoke( this, new TextChangedEventArgs( _text ) );
	}

	private void SelectAll()
	{
		_selectionStart = 0;
		_selectionEnd = _text.Length;
		_cursorPosition = _text.Length;
	}

	private void CopyToClipboard()
	{
		if ( !HasSelection() ) return;

		var (start, end) = GetOrderedSelection();
		var selectedText = _text.Substring( start, end - start );

		// Note: Clipboard access would need platform-specific implementation
		// This is a placeholder for the actual clipboard API
	}

	private void CutToClipboard()
	{
		if ( !HasSelection() ) return;

		CopyToClipboard();
		DeleteSelection();
	}

	private void PasteFromClipboard()
	{
		// Note: Clipboard access would need platform-specific implementation
		// This is a placeholder - you would get text from clipboard and call InsertText
		// string clipboardText = GetClipboardText();
		// InsertText(clipboardText);
	}

	private int GetCharacterIndexAtX( float x )
	{
		if ( _textPaint is null ) return 0;

		var displayText = GetDisplayText();
		if ( string.IsNullOrEmpty( displayText ) ) return 0;

		for ( var i = 0; i <= displayText.Length; i++ )
		{
			var substr = displayText[..i];
			var width = _textPaint.MeasureText( substr );

			if ( width > x )
			{
				// Check if we're closer to this char or the previous one
				if ( i > 0 )
				{
					var prevWidth = _textPaint.MeasureText( displayText[..(i - 1)] );

					if ( x - prevWidth < width - x )
						return i - 1;
				}

				return i;
			}
		}

		return displayText.Length;
	}

	private void EnsureCursorVisible()
	{
		if ( _textPaint is null ) return;

		var displayText = GetDisplayText();
		var textBeforeCursor = displayText[.._cursorPosition];
		var cursorX = _textPaint.MeasureText( textBeforeCursor );

		var paddingLeft = Padding.Left;
		var paddingRight = Padding.Right;
		var contentWidth = LayoutWidth - paddingLeft - paddingRight;

		// Scroll right if cursor is past visible area
		if ( cursorX - _scrollX > contentWidth - 10 )
		{
			_scrollX = cursorX - contentWidth + 10;
		}
		// Scroll left if cursor is before visible area
		else if ( cursorX - _scrollX < 0 )
		{
			_scrollX = Math.Max( 0, cursorX - 10 );
		}
	}

	private bool HasSelection()
	{
		return _selectionStart >= 0 && _selectionEnd >= 0 && _selectionStart != _selectionEnd;
	}

	private (int start, int end) GetOrderedSelection()
	{
		if ( !HasSelection() )
			return (_cursorPosition, _cursorPosition);

		var start = Math.Min( _selectionStart, _selectionEnd );
		var end = Math.Max( _selectionStart, _selectionEnd );

		return (start, end);
	}

	private void ClearSelection()
	{
		_selectionStart = -1;
		_selectionEnd = -1;
	}

	public void Focus()
	{
		IsFocused = true;
	}

	public void Blur()
	{
		IsFocused = false;
	}

	/// <summary>
	/// Méthode publique pour insérer un caractère directement (appelée par le gestionnaire d'input)
	/// </summary>
	public void HandleCharacterInput( char character )
	{
		if ( !_isFocused || IsReadOnly ) return;

		// Filtrer les caractères de contrôle sauf tabulation
		if ( char.IsControl( character ) && character != '\t' )
			return;

		InsertText( character.ToString() );
		EnsureCursorVisible();

		_cursorVisible = true;
		_lastCursorBlink = DateTime.Now;

		MarkDirty();
	}

	public void Dispose()
	{
		_textPaint?.Dispose();
		_placeholderPaint?.Dispose();
	}
}
