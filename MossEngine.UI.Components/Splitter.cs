using System.Numerics;
using MossEngine.UI.Yoga;
using MossEngine.Windowing;
using SkiaSharp;

namespace MossEngine.UI.Components;

public sealed class Splitter : Panel, IDisposable
{
	private readonly Panel _firstHost;
	private readonly Panel _secondHost;
	private readonly Panel _splitterHandle;
	private Vector2 _dragStartPointer;
	private float _dragStartSplit;
	private float _dragAvailableSize;
	private bool _isDragging;
	private float _split = 0.5f;

	public Splitter( SplitterOrientation orientation = SplitterOrientation.Horizontal )
	{
		Orientation = orientation;
		FlexDirection = orientation is SplitterOrientation.Horizontal
			? YogaFlexDirection.Row
			: YogaFlexDirection.Column;
		Gap = 0;
		IsFocusable = false;

		_firstHost = CreateContentHost();
		_secondHost = CreateContentHost();
		_splitterHandle = CreateHandle();

		AddChild( _firstHost );
		AddChild( _splitterHandle );
		AddChild( _secondHost );

		UpdateChildSizes();
	}

	public SplitterOrientation Orientation { get; }

	public Panel First => _firstHost;
	public Panel Second => _secondHost;

	public float Split
	{
		get => _split;
		set
		{
			var clamped = Math.Clamp( value, 0.01f, 0.99f );
			if ( Math.Abs( _split - clamped ) < 0.0001f ) return;

			_split = clamped;
			UpdateChildSizes();
		}
	}

	public float SplitterThickness { get; set; } = 4f;
	public float MinFirstSize { get; set; } = 120f;
	public float MinSecondSize { get; set; } = 120f;

	public SKColor SplitterColor
	{
		get => field;
		set
		{
			_splitterHandle.Background = value;
			
			field = value;
			MarkDirty();
		}
	}
	
	private Panel CreateContentHost()
	{
		return new Panel
		{
			Flex = 0,
			FlexGrow = 0,
			FlexShrink = 0,
			Background = SKColors.Transparent,
			IsHitTestVisible = false
		};
	}

	private Panel CreateHandle()
	{
		var handle = new Panel { Background = SplitterColor, IsFocusable = false, IsHitTestVisible = true };

		if ( Orientation is SplitterOrientation.Horizontal )
		{
			handle.Width = Length.Point( SplitterThickness );
			handle.Height = Length.Percent( 100 );
		}
		else
		{
			handle.Width = Length.Percent( 100 );
			handle.Height = Length.Point( SplitterThickness );
		}

		handle.PointerEnter += HandleOnPointerEnter;
		handle.PointerDown += HandleOnPointerDown;
		handle.PointerMove += HandleOnPointerMove;
		handle.PointerUp += HandleOnPointerUp;
		handle.PointerLeave += HandleOnPointerLeave;

		return handle;
	}

	private void HandleOnPointerDown( object? sender, PointerEventArgs e )
	{
		_isDragging = true;
		_dragStartPointer = e.ScreenPosition;
		_dragStartSplit = _split;
		_dragAvailableSize = Orientation is SplitterOrientation.Horizontal ? LayoutWidth : LayoutHeight;

		e.Handled = true;
	}

	private void HandleOnPointerMove( object? sender, PointerEventArgs e )
	{
		if ( !_isDragging || _dragAvailableSize <= 0 ) return;

		var delta = Orientation is SplitterOrientation.Horizontal
			? e.ScreenPosition.X - _dragStartPointer.X
			: e.ScreenPosition.Y - _dragStartPointer.Y;

		var firstPixels = (_dragStartSplit * _dragAvailableSize) + delta;
		var minFirst = MinFirstSize;
		var minSecond = MinSecondSize;
		var clampedPixels = Math.Clamp( firstPixels, minFirst, Math.Max( minFirst, _dragAvailableSize - minSecond ) );
		var ratio = clampedPixels / _dragAvailableSize;

		Split = ratio;
		e.Handled = true;
	}

	private void HandleOnPointerUp( object? sender, PointerEventArgs e )
	{
		if ( !_isDragging ) return;
		_isDragging = false;

		e.Handled = true;
	}

	private void HandleOnPointerEnter( object? sender, PointerEventArgs e )
	{
	}

	private void HandleOnPointerLeave( object? sender, PointerEventArgs e )
	{
		if ( _isDragging ) return;
		_isDragging = false;

		e.Handled = true;
	}

	private void UpdateChildSizes()
	{
		var firstPercent = _split * 100f;
		var secondPercent = (1f - _split) * 100f;

		if ( Orientation is SplitterOrientation.Horizontal )
		{
			_firstHost.Width = Length.Percent( firstPercent );
			_firstHost.Height = Length.Percent( 100 );
			_secondHost.Width = Length.Percent( secondPercent );
			_secondHost.Height = Length.Percent( 100 );
		}
		else
		{
			_firstHost.Height = Length.Percent( firstPercent );
			_firstHost.Width = Length.Percent( 100 );
			_secondHost.Height = Length.Percent( secondPercent );
			_secondHost.Width = Length.Percent( 100 );
		}

		MarkDirty();
	}

	public void Dispose()
	{
		_splitterHandle.PointerEnter -= HandleOnPointerEnter;
		_splitterHandle.PointerDown -= HandleOnPointerDown;
		_splitterHandle.PointerMove -= HandleOnPointerMove;
		_splitterHandle.PointerUp -= HandleOnPointerUp;
		_splitterHandle.PointerLeave -= HandleOnPointerLeave;
	}
}
