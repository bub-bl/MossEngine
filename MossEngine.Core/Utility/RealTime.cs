namespace MossEngine.Core.Utility;

/// <summary>
/// Access to time.
/// </summary>
public static class RealTime
{
	static RealTime()
	{
		TimeMeasure = FastTimer.StartNew();

		var epoch = new DateTime( 2022, 1, 1, 1, 1, 1, DateTimeKind.Utc );
		var now = DateTime.UtcNow;

		NowOffset = (now - epoch).TotalSeconds;
	}

	private static readonly FastTimer TimeMeasure;
	private static readonly double NowOffset;

	/// <summary>
	/// The time since game startup, in seconds.
	/// </summary>
	public static float Now => (float)DoubleNow;

	/// <summary>
	/// The time since game startup, in seconds.
	/// </summary>
	internal static double DoubleNow => TimeMeasure.ElapsedSeconds;

	/// <summary>
	/// The number of a seconds since a set point in time. This value should match between servers and clients. If they have their timezone set correctly.
	/// </summary>
	public static double GlobalNow => (NowOffset + TimeMeasure.ElapsedSeconds);

	/// <summary>
	/// The time delta (in seconds) between the last frame and the current (for all intents and purposes)
	/// </summary>
	public static float Delta { get; internal set; }

	/// <summary>
	/// Like Delta but smoothed to avoid large disparities between deltas
	/// </summary>
	public static float SmoothDelta { get; internal set; }

	private static double _lastTick;

	internal static void Update( double now )
	{
		if ( _lastTick > 0 )
		{
			Delta = float.Clamp( (float)(now - _lastTick), 0.0f, 2.0f );
			SmoothDelta = MathX.Lerp( SmoothDelta, Delta, 0.1f );
		}

		_lastTick = now;
	}
}
