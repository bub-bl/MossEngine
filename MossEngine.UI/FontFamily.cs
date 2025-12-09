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

	public static FontFamily FromFile( string name, string path )
	{
		var typeface = SKTypeface.FromFile( path );
		return new FontFamily( name, typeface );
	}
	
	public static FontFamily FromFontName( string name )
	{
		var typeface = SKTypeface.FromFamilyName( name );
		return new FontFamily( name, typeface );
	}
}
