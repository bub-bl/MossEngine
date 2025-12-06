using System.Runtime.InteropServices;

namespace MossEngine.Utility;

/// <summary>
/// A convenience struct to easily manage a time countdown, based on <see cref="Time.Now"/>.<br/>
/// <br/>
/// Typical usage would see you assigning to a variable of this type a necessary amount of seconds.
/// Then the struct would return the time countdown, or can be used as a bool i.e.:
/// <code>
/// TimeUntil nextAttack = 10;
/// if ( nextAttack ) { /*Do something*/ }
/// </code>
/// </summary>
[StructLayout( LayoutKind.Sequential )]
public struct TimeUntil : IEquatable<TimeUntil>
{
	private float _time;
	private float _startTime;

	public static implicit operator bool( TimeUntil ts ) => Time.Now >= ts._time;
	public static implicit operator float( TimeUntil ts ) => ts._time - Time.Now;
	public static bool operator <( in TimeUntil ts, float f ) => ts.Relative < f;
	public static bool operator >( in TimeUntil ts, float f ) => ts.Relative > f;
	public static bool operator <=( in TimeUntil ts, float f ) => ts.Relative <= f;
	public static bool operator >=( in TimeUntil ts, float f ) => ts.Relative >= f;
	public static bool operator <( in TimeUntil ts, int f ) => ts.Relative < f;
	public static bool operator >( in TimeUntil ts, int f ) => ts.Relative > f;
	public static bool operator <=( in TimeUntil ts, int f ) => ts.Relative <= f;
	public static bool operator >=( in TimeUntil ts, int f ) => ts.Relative >= f;
	public static implicit operator TimeUntil( float ts ) => new() { _time = Time.Now + ts, _startTime = Time.Now };

	/// <summary>
	/// Time to which we are counting down to, based on <see cref="Time.Now"/>.
	/// </summary>
	public float Absolute => _time;

	/// <summary>
	/// The actual countdown, in seconds.
	/// </summary>
	public float Relative => this;

	/// <summary>
	/// Amount of seconds passed since the countdown started.
	/// </summary>
	public float Passed => Time.Now - _startTime;

	/// <summary>
	/// The countdown, but as a fraction, i.e. a value from 0 (start of countdown) to 1 (end of countdown)
	/// </summary>
	public float Fraction => Math.Clamp( (Time.Now - _startTime) / (_time - _startTime), 0.0f, 1.0f );

	public override string ToString() => $"{Relative}";

	#region equality

	public static bool operator ==( TimeUntil left, TimeUntil right ) => left.Equals( right );
	public static bool operator !=( TimeUntil left, TimeUntil right ) => !(left == right);
	public readonly override bool Equals( object? obj ) => obj is TimeUntil o && Equals( o );
	public readonly bool Equals( TimeUntil o ) => Math.Abs( _time - o._time ) < 0.0001f;
	public readonly override int GetHashCode() => HashCode.Combine( _time );

	#endregion
}
