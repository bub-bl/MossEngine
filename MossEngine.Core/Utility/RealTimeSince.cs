namespace MossEngine.Core.Utility;

/// <summary>
/// A convenience struct to easily measure time since an event last happened, based on <see cref="RealTime.GlobalNow"/>.<br/>
/// <br/>
/// Typical usage would see you assigning 0 to a variable of this type to reset the timer.
/// Then the struct would return time since the last reset. i.e.:
/// <code>
/// RealTimeSince lastUsed = 0;
/// if ( lastUsed > 10 ) { /*Do something*/ }
/// </code>
/// </summary>
public struct RealTimeSince : IEquatable<RealTimeSince>
{
	public static implicit operator float( RealTimeSince ts ) => (float)(RealTime.GlobalNow - ts.Absolute);
	public static implicit operator RealTimeSince( float ts ) => new() { Absolute = RealTime.GlobalNow - ts };
	public static bool operator <( in RealTimeSince ts, float f ) => ts.Relative < f;
	public static bool operator >( in RealTimeSince ts, float f ) => ts.Relative > f;
	public static bool operator <=( in RealTimeSince ts, float f ) => ts.Relative <= f;
	public static bool operator >=( in RealTimeSince ts, float f ) => ts.Relative >= f;
	public static bool operator <( in RealTimeSince ts, int f ) => ts.Relative < f;
	public static bool operator >( in RealTimeSince ts, int f ) => ts.Relative > f;
	public static bool operator <=( in RealTimeSince ts, int f ) => ts.Relative <= f;
	public static bool operator >=( in RealTimeSince ts, int f ) => ts.Relative >= f;

	/// <summary>
	/// Time at which the timer reset happened, based on <see cref="RealTime.GlobalNow"/>.
	/// </summary>
	public double Absolute { get; private set; }

	/// <summary>
	/// Time passed since last reset, in seconds.
	/// </summary>
	public float Relative => this;

	public override string ToString() => $"{Relative}";

	#region equality

	public static bool operator ==( RealTimeSince left, RealTimeSince right ) => left.Equals( right );
	public static bool operator !=( RealTimeSince left, RealTimeSince right ) => !(left == right);
	public override bool Equals( object? obj ) => obj is RealTimeSince o && Equals( o );
	public bool Equals( RealTimeSince o ) => global::System.Math.Abs( Absolute - o.Absolute ) < 0.0001f;
	public readonly override int GetHashCode() => HashCode.Combine( Absolute );

	#endregion
}
