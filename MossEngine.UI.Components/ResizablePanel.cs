using System.Numerics;
using MossEngine.UI.Yoga;
using MossEngine.Windowing;
using Silk.NET.Input;
using SkiaSharp;

namespace MossEngine.UI.Components;

[Flags]
public enum ResizeEdges
{
	None = 0,
	Left = 1 << 0,
	Right = 1 << 1,
	Top = 1 << 2,
	Bottom = 1 << 3,

	Horizontal = Left | Right,
	Vertical = Top | Bottom,
	All = Left | Right | Top | Bottom
}

public sealed class ResizablePanel : Panel, IDisposable
{
	private readonly Dictionary<ResizeEdges, Panel> _grips = new();

	private bool _isDragging;
	private ResizeEdges _dragEdge;
	private Vector2 _dragStart;
	private float _sizeAtDragStart;

	public ResizablePanel( ResizeEdges resizableEdges = ResizeEdges.None )
	{
		IsFocusable = false;

		Content = new Panel
		{
			Width = Length.Percent( 100 ),
			Height = Length.Percent( 100 ),
			Flex = 1,
			FlexGrow = 1,
			FlexShrink = 1,
			Background = SKColors.Red,
			IsHitTestVisible = false
		};

		ResizableEdges = resizableEdges;
	}

	#region Properties

	public ResizeEdges ResizableEdges
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;

			RecreateGrips();
		}
	}

	public Panel Content { get; }

	public int GripThickness
	{
		get => field;
		set
		{
			if ( field == value ) return;
			field = value;

			UpdateGripSizes();
		}
	} = 6;

	public SKColor GripColor
	{
		get => field;
		set
		{
			field = value;
			UpdateGripColors();
		}
	}

	public SKColor GripHoverColor { get; set; } = SKColors.Transparent;

	#endregion

	#region Grip Management

	private void CreateGrips()
	{
		ClearChildren();
		_grips.Clear();

		var hasLeft = ResizableEdges.HasFlag( ResizeEdges.Left );
		var hasRight = ResizableEdges.HasFlag( ResizeEdges.Right );
		var hasTop = ResizableEdges.HasFlag( ResizeEdges.Top );
		var hasBottom = ResizableEdges.HasFlag( ResizeEdges.Bottom );

		if ( (hasLeft || hasRight) && !(hasTop || hasBottom) )
		{
			FlexDirection = YogaFlexDirection.Row;
			// Gap = 0;
		}
		else if ( (hasTop || hasBottom) && !(hasLeft || hasRight) )
		{
			FlexDirection = YogaFlexDirection.Column;
			// Gap = 0;
		}
		else
		{
			FlexDirection = YogaFlexDirection.Row;
			// Gap = 0;
		}

		if ( hasLeft )
			CreateGrip( ResizeEdges.Left );

		if ( hasTop )
			CreateGrip( ResizeEdges.Top );

		AddChild( Content );

		if ( hasRight )
			CreateGrip( ResizeEdges.Right );

		if ( hasBottom )
			CreateGrip( ResizeEdges.Bottom );
	}

	private void RecreateGrips()
	{
		foreach ( var grip in _grips.Values )
		{
			RemoveChild( grip );
			DetachGripEvents( grip );
		}

		_grips.Clear();
		CreateGrips();
	}

	private void CreateGrip( ResizeEdges edge )
	{
		var grip = new Panel
		{
			Background = GripColor,
			IsFocusable = false,
			IsHitTestVisible = true,
			Flex = 0,
			FlexGrow = 0,
			FlexShrink = 0,
			Tag = edge
		};

		UpdateGripSize( grip, edge );
		AttachGripEvents( grip );

		_grips[edge] = grip;
		AddChild( grip );
	}

	private void UpdateGripSize( Panel grip, ResizeEdges edge )
	{
		switch ( edge )
		{
			case ResizeEdges.Left:
			case ResizeEdges.Right:
				grip.Width = Length.Point( GripThickness );
				grip.Height = Length.Percent( 100 );
				break;
			case ResizeEdges.Top:
			case ResizeEdges.Bottom:
				grip.Width = Length.Percent( 100 );
				grip.Height = Length.Point( GripThickness );
				break;
		}
	}

	private void UpdateGripSizes()
	{
		foreach ( var (edge, grip) in _grips )
		{
			UpdateGripSize( grip, edge );
		}
	}

	private void UpdateGripColors()
	{
		foreach ( var grip in _grips.Values )
		{
			if ( !_isDragging || grip.Tag as ResizeEdges? != _dragEdge )
			{
				grip.Background = GripColor;
			}
		}
	}

	#endregion

	#region Event Handling

	private void AttachGripEvents( Panel grip )
	{
		grip.PointerDown += OnGripPointerDown;
		grip.PointerMove += OnGripPointerMove;
		grip.PointerUp += OnGripPointerUp;
		grip.PointerEnter += OnGripPointerEnter;
		grip.PointerLeave += OnGripPointerLeave;
	}

	private void DetachGripEvents( Panel grip )
	{
		grip.PointerDown -= OnGripPointerDown;
		grip.PointerMove -= OnGripPointerMove;
		grip.PointerUp -= OnGripPointerUp;
		grip.PointerEnter -= OnGripPointerEnter;
		grip.PointerLeave -= OnGripPointerLeave;
	}

	private void OnGripPointerDown( object? sender, PointerEventArgs e )
	{
		if ( sender is not Panel { Tag: ResizeEdges edge } ) return;

		_isDragging = true;
		_dragEdge = edge;
		_dragStart = e.ScreenPosition;

		_sizeAtDragStart = edge switch
		{
			ResizeEdges.Left or ResizeEdges.Right => LayoutWidth,
			ResizeEdges.Top or ResizeEdges.Bottom => LayoutHeight,
			_ => 0
		};

		e.Handled = true;
	}

	private void OnGripPointerMove( object? sender, PointerEventArgs e )
	{
		if ( !_isDragging ) return;

		var delta = e.ScreenPosition - _dragStart;
		float newSize;

		switch ( _dragEdge )
		{
			case ResizeEdges.Right:
				newSize = _sizeAtDragStart + delta.X;
				newSize = Math.Clamp( newSize, MinWidth, MaxWidth );
				YogaNode.Width = Length.Point( newSize );
				break;

			case ResizeEdges.Left:
				newSize = _sizeAtDragStart - delta.X;
				newSize = Math.Clamp( newSize, MinWidth, MaxWidth );
				YogaNode.Width = Length.Point( newSize );
				break;

			case ResizeEdges.Bottom:
				newSize = _sizeAtDragStart + delta.Y;
				newSize = Math.Clamp( newSize, MinHeight, MaxHeight );
				YogaNode.Height = Length.Point( newSize );
				break;

			case ResizeEdges.Top:
				newSize = _sizeAtDragStart - delta.Y;
				newSize = Math.Clamp( newSize, MinHeight, MaxHeight );
				YogaNode.Height = Length.Point( newSize );
				break;
		}

		MarkDirty();
		e.Handled = true;
	}

	private void OnGripPointerUp( object? sender, PointerEventArgs e )
	{
		if ( !_isDragging ) return;

		_isDragging = false;

		if ( sender is Panel grip )
		{
			grip.Background = GripColor;
		}

		Cursor.Current = StandardCursor.Arrow;
		e.Handled = true;
	}

	private void OnGripPointerEnter( object? sender, PointerEventArgs e )
	{
		if ( sender is not Panel { Tag: ResizeEdges edge } grip ) return;

		grip.Background = GripHoverColor;

		Cursor.Current = edge switch
		{
			ResizeEdges.Left or ResizeEdges.Right => StandardCursor.HResize,
			ResizeEdges.Top or ResizeEdges.Bottom => StandardCursor.VResize,
			_ => StandardCursor.Arrow
		};
	}

	private void OnGripPointerLeave( object? sender, PointerEventArgs e )
	{
		if ( _isDragging ) return;

		if ( sender is Panel grip )
		{
			grip.Background = GripColor;
		}

		Cursor.Current = StandardCursor.Arrow;
	}

	#endregion

	#region Public API

	public void SetWidthConstraints( float minWidth, float maxWidth = float.MaxValue )
	{
		MinWidth = Math.Max( 0, minWidth );
		MaxWidth = Math.Max( minWidth, maxWidth );
	}

	public void SetHeightConstraints( float minHeight, float maxHeight = float.MaxValue )
	{
		MinHeight = Math.Max( 0, minHeight );
		MaxHeight = Math.Max( minHeight, maxHeight );
	}

	#endregion

	public void Dispose()
	{
		foreach ( var grip in _grips.Values )
		{
			DetachGripEvents( grip );
		}
	}
}
