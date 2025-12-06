using System.Runtime.InteropServices;

namespace MossEngine.Utility;

/// <summary>
/// A convenience struct to easily measure time since an event last happened, based on <see cref="Time.Now"/>.<br/>
/// <br/>
/// Typical usage would see you assigning 0 to a variable of this type to reset the timer.
/// Then the struct would return time since the last reset. i.e.:
/// <code>
/// TimeSince lastUsed = 0;
/// if ( lastUsed > 10 ) { /*Do something*/ }
/// </code>
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct TimeSince : IEquatable<TimeSince>
{
	private float _time;

	public static implicit operator float( TimeSince ts ) => Time.Now - ts._time;
	public static implicit operator TimeSince( float ts ) => new() { _time = Time.Now - ts };
	public static bool operator <( in TimeSince ts, float f ) => ts.Relative < f;
	public static bool operator >( in TimeSince ts, float f ) => ts.Relative > f;
	public static bool operator <=( in TimeSince ts, float f ) => ts.Relative <= f;
	public static bool operator >=( in TimeSince ts, float f ) => ts.Relative >= f;
	public static bool operator <( in TimeSince ts, int f ) => ts.Relative < f;
	public static bool operator >( in TimeSince ts, int f ) => ts.Relative > f;
	public static bool operator <=( in TimeSince ts, int f ) => ts.Relative <= f;
	public static bool operator >=( in TimeSince ts, int f ) => ts.Relative >= f;

	/// <summary>
	/// Time at which the timer reset happened, based on <see cref="Time.Now"/>.
	/// </summary>
	public float Absolute => _time;

	/// <summary>
	/// Time passed since last reset, in seconds.
	/// </summary>
	public float Relative => this;

	public override string ToString() => $"{Relative}";

	#region equality

	public static bool operator ==( TimeSince left, TimeSince right ) => left.Equals( right );
	public static bool operator !=( TimeSince left, TimeSince right ) => !(left == right);
	public override bool Equals( object? obj ) => obj is TimeSince o && Equals( o );
	public readonly bool Equals( TimeSince o ) => Math.Abs( _time - o._time ) < 0.0001f;
	public readonly override int GetHashCode() => _time.GetHashCode();

	#endregion
}
