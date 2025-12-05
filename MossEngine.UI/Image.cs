using System;
using System.IO;
using MossEngine.UI.Yoga;
using SkiaSharp;

namespace MossEngine.UI;

public class Image : Panel
{
	private SKImage? _image;
	private string? _loadedSrc;

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

		if ( _image is not null )
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

		if ( _image is null )
		{
			Console.WriteLine( nameof(DrawImage) + ": Image is null" );
			return;
		}

		var position = GetFinalPosition();
		var width = LayoutWidth > 0 ? LayoutWidth : _image.Width;
		var height = LayoutHeight > 0 ? LayoutHeight : _image.Height;
		var destRect = new SKRect( position.X, position.Y, position.X + width, position.Y + height );

		using var paint = new SKPaint();
		paint.FilterQuality = SKFilterQuality.High;
		paint.IsAntialias = true;

		var hasCornerRadius = BorderRadius.LengthSquared() > 0.001f;
		
		if ( hasCornerRadius )
		{
			var roundRect = new SKRoundRect( destRect, BorderRadius.X, BorderRadius.Y );
			canvas.Save();
			canvas.ClipRoundRect( roundRect, SKClipOperation.Intersect, antialias: true );
			canvas.DrawImage( _image, destRect, paint );
			canvas.Restore();
		}
		else
		{
			canvas.DrawImage( _image, destRect, paint );
		}
	}

	public override void Draw( SKCanvas canvas )
	{
		DrawBackground( canvas );
		DrawImage( canvas );
		// DrawText( canvas );

		foreach ( var c in Children )
		{
			c.Draw( canvas );
		}

		IsDirty = false;
	}
}
