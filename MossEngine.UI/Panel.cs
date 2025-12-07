using System.Numerics;
using System.Runtime.CompilerServices;
using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI;

public partial class Panel
{
	public Guid Id { get; } = Guid.NewGuid();
	public Panel? Parent { get; private set; }
	public List<Panel> Children { get; } = [];
	public object? Tag { get; set; }

	// Yoga node for layout
	internal YogaNode YogaNode { get; }

	// Style-ish properties (very minimal)
	public SKColor Background { get; set; } = SKColors.Transparent;
	public SKColor Foreground { get; set; } = SKColors.Black;

	// Propriétés de la bordure
	public SKColor StrokeColor { get; set; } = SKColors.Transparent;
	public float StrokeWidth { get; set; } = 0f;
	public StrokeStyle StrokeStyle { get; set; } = StrokeStyle.Solid;
	public bool IsHitTestVisible { get; set; } = true;
	public bool IsFocusable { get; set; } = true;

	public float LayoutWidth => YogaNode.LayoutWidth;
	public float LayoutHeight => YogaNode.LayoutHeight;

	public bool IsDirty { get; protected set; } = true;

	public string DebugLabel { get; set; } = null!;

	public Length LayoutLeft => YogaNode.LayoutLeft;
	public Length LayoutTop => YogaNode.LayoutTop;
	public Length LayoutRight => YogaNode.LayoutRight;
	public Length LayoutBottom => YogaNode.LayoutBottom;

	public event EventHandler? Update;

	public Panel()
	{
		YogaNode = new YogaNode();
		YogaNode.Context = this;
		YogaNode.AlignItems = YogaAlign.FlexStart;
		YogaNode.JustifyContent = YogaJustify.FlexStart;
		YogaNode.FlexDirection = YogaFlexDirection.Row;
		YogaNode.Position = YogaPositionType.Relative;

		// UpdateMeasurement();
	}

	public void ComputeLayout()
	{
		// if ( !IsDirty ) return;
		YogaNode.CalculateLayout();
	}

	internal void InternalOnUpdate()
	{
		OnUpdate();
	}

	protected virtual void OnUpdate()
	{
		Update?.Invoke( this, EventArgs.Empty );

		foreach ( var child in Children )
		{
			child.InternalOnUpdate();
		}
	}

	public void AddChild( Panel child )
	{
		if ( child.Parent is not null )
			throw new InvalidOperationException( "Already has parent" );

		child.Parent = this;

		Children.Add( child );

		var idx = Children.Count - 1;
		YogaNode.InsertChildAt( child.YogaNode, idx );

		// UpdateMeasurement();
		OnAddChild( child );
		MarkDirty();
	}

	public void ClearChildren()
	{
		for ( var i = Children.Count - 1; i >= 0; i-- )
		{
			var child = Children[i];
			Children.RemoveAt( i );
			YogaNode.RemoveChildAt( i );
			child.Parent = null;
		}

		MarkDirty();
	}

	public void RemoveChild( Panel child )
	{
		if ( child.Parent != this ) return;

		var idx = Children.IndexOf( child );
		if ( idx < 0 ) return;

		Children.RemoveAt( idx );
		YogaNode.RemoveChildAt( idx );

		child.Parent = null;

		OnRemoveChild( child );
		MarkDirty();
	}

	protected virtual void OnAddChild( Panel child )
	{
	}

	protected virtual void OnRemoveChild( Panel child )
	{
	}

	protected internal Vector2 GetFinalPosition()
	{
		var local = new Vector2( YogaNode.LayoutLeft, YogaNode.LayoutTop );

		var parentAbs = YogaNode.Position is YogaPositionType.Absolute
			? Vector2.Zero
			: Parent?.GetFinalPosition() ?? Vector2.Zero;

		return parentAbs + local;
	}

	protected void DrawBackground( SKCanvas canvas )
	{
		var position = GetFinalPosition();

		// Dessiner le fond
		new SkiaRectBuilder( canvas )
			.At( position.X, position.Y )
			.WithSize( LayoutWidth, LayoutHeight )
			.WithBorderRadius( BorderRadius.X, BorderRadius.Y )
			.WithFill( Background )
			.Draw();

		// Dessiner la bordure si nécessaire
		if ( StrokeWidth > 0 && StrokeColor.Alpha > 0 )
		{
			// Ajuster la position et la taille pour que la bordure soit à l'intérieur du panel
			var halfStroke = StrokeWidth * 0.5f;
			var strokeRect = SKRect.Create(
				position.X + halfStroke,
				position.Y + halfStroke,
				Math.Max( 0, LayoutWidth - StrokeWidth ),
				Math.Max( 0, LayoutHeight - StrokeWidth )
			);

			using var paint = new SKPaint
			{
				Color = StrokeColor, Style = SKPaintStyle.Stroke, StrokeWidth = StrokeWidth, IsAntialias = true
			};

			// Appliquer le style de bordure
			switch ( StrokeStyle )
			{
				case StrokeStyle.Dashed:
					paint.PathEffect = SKPathEffect.CreateDash( new[] { StrokeWidth * 3f, StrokeWidth * 2f }, 0 );
					break;

				case StrokeStyle.Dotted:
					paint.PathEffect = SKPathEffect.CreateDash( new[] { StrokeWidth, StrokeWidth * 2f }, 0 );
					break;

				case StrokeStyle.DashedDotted:
					paint.PathEffect =
						SKPathEffect.CreateDash(
							new[] { StrokeWidth * 4f, StrokeWidth * 2f, StrokeWidth, StrokeWidth * 2f }, 0 );
					break;

				case StrokeStyle.DashedDottedDotted:
					paint.PathEffect = SKPathEffect.CreateDash(
						new[]
						{
							StrokeWidth * 6f, StrokeWidth * 2f, StrokeWidth, StrokeWidth * 2f, StrokeWidth,
							StrokeWidth * 2f
						}, 0 );
					break;
			}

			// Dessiner la bordure avec le même rayon d'arrondi que le fond
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

	private IDisposable? PushOverflowClip( SKCanvas canvas )
	{
		if ( Overflow is YogaOverflow.Visible )
			return null;

		var position = GetFinalPosition();
		var clipRect = new SKRect( position.X, position.Y, position.X + LayoutWidth, position.Y + LayoutHeight );
		var hasRadius = BorderRadius.LengthSquared() > 0.001f;

		canvas.Save();

		if ( hasRadius )
		{
			var roundRect = new SKRoundRect( clipRect, BorderRadius.X, BorderRadius.Y );
			canvas.ClipRoundRect( roundRect, SKClipOperation.Intersect, antialias: true );
		}
		else
		{
			canvas.ClipRect( clipRect, SKClipOperation.Intersect, antialias: true );
		}

		// if ( Overflow is YogaOverflow.Scroll )
		// {
		// 	var scrollY = MathF.Max( 0, YogaNode.ScrollY );
		// 	var scrollX = MathF.Max( 0, YogaNode.ScrollX );
		// 	canvas.Translate( -scrollX, -scrollY );
		//
		// 	return new CanvasRestore( canvas, translateBackX: scrollX, translateBackY: scrollY );
		// }

		return new CanvasRestore( canvas, translateBackX: 0, translateBackY: 0 );
	}

	protected void ClipOverflow( SKCanvas canvas )
	{
		using var clip = PushOverflowClip( canvas );
	}

	protected void DrawChildren( SKCanvas canvas )
	{
		foreach ( var child in Children )
		{
			if ( child.Display is not YogaDisplay.None )
			{
				child.Draw( canvas );
			}
		}
	}

	public virtual void MarkDirty( [CallerMemberName] string? member = null )
	{
		// Console.WriteLine( $"Marking dirty: {member}" );
		IsDirty = true;
		// Parent?.MarkDirty();
	}

	// Called after Yoga layout computed. x,y are absolute positions in root space.
	public virtual void Draw( SKCanvas canvas )
	{
		// if ( !IsDirty ) return;
		if ( Display is YogaDisplay.None ) return;

		DrawBackground( canvas );
		// DrawText( canvas );

		ClipOverflow( canvas );
		DrawChildren( canvas );

		// Console.WriteLine( "Render: " + Id );
		IsDirty = false;
	}

	public void StateHasChanged()
	{
		MarkDirty();
	}

	public virtual int BuildHash()
	{
		return -1;
	}

	public override string ToString()
	{
		return $"{GetType().Name}({Id})";
	}
}
