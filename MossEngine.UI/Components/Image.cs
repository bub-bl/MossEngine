using System.Numerics;
using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI.Components;

public class Image : Panel
{
	private SKImage? _image;
	private string? _loadedSrc;

	public ObjectFit ObjectFit { get; set; } = ObjectFit.Cover;

	public string? Src
	{
		get;
		set
		{
			if ( field == value ) return;
			field = value;

			UnloadImage();
			MarkDirty();
		}
	}

	private void UnloadImage()
	{
		_loadedSrc = null;
		_image?.Dispose();
		_image = null;
	}

	private void EnsureImageLoaded()
	{
		if ( string.IsNullOrWhiteSpace( Src ) )
		{
			UnloadImage();
			return;
		}

		if ( _loadedSrc == Src ) return;

		UnloadImage();

		if ( !File.Exists( Src ) ) return;

		using var stream = File.OpenRead( Src );
		using var data = SKData.Create( stream );

		_image = SKImage.FromEncodedData( data );
		_loadedSrc = _image is not null ? Src : null;
	}

	private void DrawImage( SKCanvas canvas )
	{
		EnsureImageLoaded();

		if ( _image is null ) return;

		var position = GetFinalPosition();
		var destRect = CreateDestinationRect( position );

		using var paint = new SKPaint();
		paint.FilterQuality = SKFilterQuality.High;
		paint.IsAntialias = true;

		var hasCornerRadius = BorderRadius.LengthSquared() > 0.001f;

		if ( hasCornerRadius )
		{
			var roundRect = new SKRoundRect( destRect, BorderRadius.X, BorderRadius.Y );
			canvas.Save();
			canvas.ClipRoundRect( roundRect, SKClipOperation.Intersect, antialias: true );
			DrawImageFitted( canvas, destRect, paint );
			canvas.Restore();
		}
		else
		{
			DrawImageFitted( canvas, destRect, paint );
		}
	}

	private SKRect CreateDestinationRect( Vector2 position )
	{
		var width = LayoutWidth > 0 ? LayoutWidth : _image!.Width;
		var height = LayoutHeight > 0 ? LayoutHeight : _image!.Height;

		return new SKRect( position.X, position.Y, position.X + width, position.Y + height );
	}

	private void DrawImageFitted( SKCanvas canvas, SKRect availableRect, SKPaint paint )
	{
		var (source, destination) = CalculateSourceAndDestination( availableRect );
		canvas.DrawImage( _image!, source, destination, paint );
	}

	private (SKRect source, SKRect destination) CalculateSourceAndDestination( SKRect availableRect )
	{
		var imageWidth = _image!.Width;
		var imageHeight = _image.Height;
		var srcRect = new SKRect( 0, 0, imageWidth, imageHeight );
		var destRect = availableRect;
		var destWidth = destRect.Width;
		var destHeight = destRect.Height;

		if ( imageWidth <= 0 || imageHeight <= 0 || destWidth <= 0 || destHeight <= 0 )
			return (srcRect, destRect);

		var widthRatio = destWidth / imageWidth;
		var heightRatio = destHeight / imageHeight;

		switch ( ObjectFit )
		{
			case ObjectFit.Fill:
				return (srcRect, destRect);

			case ObjectFit.None:
				{
					var width = MathF.Min( destWidth, imageWidth );
					var height = MathF.Min( destHeight, imageHeight );
					
					return (srcRect,
						new SKRect( destRect.Left, destRect.Top, destRect.Left + width, destRect.Top + height ));
				}

			case ObjectFit.ScaleDown:
				{
					var scale = MathF.Min( 1f, MathF.Min( widthRatio, heightRatio ) );
					return (srcRect, CenterRect( destRect, imageWidth * scale, imageHeight * scale ));
				}

			case ObjectFit.Contain:
				{
					var scale = MathF.Min( widthRatio, heightRatio );
					return (srcRect, CenterRect( destRect, imageWidth * scale, imageHeight * scale ));
				}

			case ObjectFit.Cover:
				{
					var scale = MathF.Max( widthRatio, heightRatio );
					var coverWidth = imageWidth * scale;
					var coverHeight = imageHeight * scale;

					var sampleWidth = destWidth / coverWidth * imageWidth;
					var sampleHeight = destHeight / coverHeight * imageHeight;
					var sampleX = (imageWidth - sampleWidth) / 2f;
					var sampleY = (imageHeight - sampleHeight) / 2f;

					srcRect = new SKRect( sampleX, sampleY, sampleX + sampleWidth, sampleY + sampleHeight );
					return (srcRect, destRect);
				}

			default:
				return (srcRect, destRect);
		}
	}

	private static SKRect CenterRect( SKRect bounds, float width, float height )
	{
		var offsetX = bounds.Left + (bounds.Width - width) / 2f;
		var offsetY = bounds.Top + (bounds.Height - height) / 2f;

		return new SKRect( offsetX, offsetY, offsetX + width, offsetY + height );
	}

	public override void Draw( SKCanvas canvas )
	{
		if ( Display is YogaDisplay.None ) return;

		DrawBackground( canvas );
		DrawImage( canvas );
		// DrawText( canvas );

		ClipOverflow( canvas );
		DrawChildren( canvas );

		IsDirty = false;
	}
}
