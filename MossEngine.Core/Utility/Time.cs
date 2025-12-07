namespace MossEngine.Core.Utility;

public static class Time
{
	/// <summary>
	/// The time since game startup
	/// </summary>
	public static float Now { get; private set; }

	/// <summary>
	/// The delta between the last frame and the current (for all intents and purposes)
	/// </summary>
	public static float Delta { get; set; }

	public static void Update( double now, double delta )
	{
		Now = (float)now;
		Delta = (float)delta;
	}

	public static IDisposable Scope( double now, double delta )
	{
		var d = Delta;
		var n = Now;

		Update( now, delta );

		return DisposeAction.Create( () =>
		{
			Delta = d;
			Now = n;
		} );
	}
}
