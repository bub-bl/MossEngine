using SkiaSharp;

namespace MossEngine.UI;

public abstract class BaseSkiaBuilder<TBuilder>( SKCanvas canvas ) where TBuilder : BaseSkiaBuilder<TBuilder>
{
	protected readonly SKCanvas Canvas = canvas;
	protected readonly SKPaint Paint = new();

	private float _x;
	private float _y;
	private float _width;
	private float _height;
	private bool _sizeSet;
	private bool _positionSet;
	private bool _rectExplicitlySet;
	private SKRect _explicitRect;

	public TBuilder WithRect( SKRect rect )
	{
		_explicitRect = rect;
		_rectExplicitlySet = true;

		return (TBuilder)this;
	}

	public TBuilder WithRect( float x, float y, float width, float height )
	{
		return WithRect( new SKRect( x, y, x + width, y + height ) );
	}

	public TBuilder At( float x, float y )
	{
		_x = x;
		_y = y;
		_positionSet = true;
		_rectExplicitlySet = false;

		return (TBuilder)this;
	}

	public TBuilder WithSize( float width, float height )
	{
		width = Math.Max( 0, width );
		height = Math.Max( 0, height );

		_width = width;
		_height = height;
		_sizeSet = true;
		_rectExplicitlySet = false;

		return (TBuilder)this;
	}

	protected SKRect BuildRect()
	{
		if ( _rectExplicitlySet )
			return _explicitRect;

		// if ( !_sizeSet )
		// 	throw new InvalidOperationException( "Size must be specified via WithSize or WithRect before drawing." );

		var x = _positionSet ? _x : 0f;
		var y = _positionSet ? _y : 0f;

		return new SKRect( x, y, x + _width, y + _height );
	}

	public abstract void Draw();
}
