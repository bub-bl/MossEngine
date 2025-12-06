namespace MossEngine.System.Extensions;

public static class DateTimeExtensions
{
	private static readonly DateTime Epoch = new( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );

	/// <summary>
	/// Returns the UNIX time stamp - number of seconds since 1st of January, 1970.
	/// </summary>
	/// <param name="d"></param>
	/// <returns></returns>
	public static int GetEpoch( this DateTime d )
	{
		var t = d - Epoch;
		return (int)(t.TotalSeconds);
	}

	/// <summary>
	/// Converts UNIX time stamp to a DateTime object.
	/// </summary>
	/// <param name="seconds">UNIX time stamp in seconds.</param>
	public static DateTime ToDateTime( this int seconds )
	{
		return Epoch.AddSeconds( seconds );
	}

	/// <inheritdoc cref="ToDateTime(int)"/>
	public static DateTime ToDateTime( this long seconds )
	{
		return Epoch.AddSeconds( seconds );
	}

	extension(DateTime dateTime)
	{
		public string ToRelativeTimeString()
		{
			var span = DateTime.Now - dateTime;
			return span.ToRelativeTimeString();
		}

		/// <summary>
		/// Convert date into a human readable relative time string.
		/// </summary>
		public string Humanize()
		{
			return Humanizer.DateHumanizeExtensions.Humanize( dateTime );
		}
	}

	/// <summary>
	/// Convert date into a human readable relative time string.
	/// </summary>
	public static string Humanize( this DateTimeOffset dateTime )
	{
		return Humanizer.DateHumanizeExtensions.Humanize( dateTime );
	}

	/// <summary>
	/// Convert date into a human readable relative time string.
	/// </summary>
	public static string Humanize( this TimeOnly dateTime )
	{
		return Humanizer.DateHumanizeExtensions.Humanize( dateTime );
	}

	/// <summary>
	/// Convert date into a human readable relative time string.
	/// </summary>
	public static string Humanize( this DateOnly dateTime )
	{
		return Humanizer.DateHumanizeExtensions.Humanize( dateTime );
	}

	/// <summary>
	/// Convert date into a human readable relative time string.
	/// </summary>
	public static string Humanize( this TimeSpan dateTime, int precision = 1 )
	{
		return Humanizer.TimeSpanHumanizeExtensions.Humanize( dateTime, precision, minUnit: Humanizer.Localisation.TimeUnit.Second );
	}

}
