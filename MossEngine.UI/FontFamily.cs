using SkiaSharp;

namespace MossEngine.UI;

public readonly record struct FontFamily
{
	public readonly string Name;
	public readonly SKTypeface Typeface;
	
	private FontFamily( string name, SKTypeface typeface )
	{
		Name = name;
		Typeface = typeface;
	}

	public static FontFamily Load( string name, string path )
	{
		var typeface = SKTypeface.FromFile( path );
		return new FontFamily( name, typeface );
	}
}
