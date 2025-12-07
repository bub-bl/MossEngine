using System.Text;

namespace MossEngine.Editor;

public sealed class ConsoleInterceptor( TextWriter original ) : TextWriter
{
	public static event Action<string>? OnWrite;

	public override Encoding Encoding => original.Encoding;

	public override void Write( string? value )
	{
		OnWrite?.Invoke( value ?? "" );
		original.Write( value );
	}

	public override void WriteLine( string? value )
	{
		OnWrite?.Invoke( value ?? "" );
		original.WriteLine( value );
	}
}
