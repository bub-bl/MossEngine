namespace MossEngine.UI.Extensions;

public static class HashCodeExtensions
{
	extension( HashCode )
	{
		public static HashCode Combine( params object[] values )
		{
			var hash = new HashCode();

			foreach ( var value in values )
				hash.Add( value );

			return hash;
		}
	}
}
