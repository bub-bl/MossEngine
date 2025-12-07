namespace MossEngine.Core.Utility;

/// <summary>
/// A convenience struct to easily manage a time countdown, based on <see cref="RealTime.GlobalNow"/>.<br/>
/// <br/>
/// Typical usage would see you assigning to a variable of this type a necessary amount of seconds.
/// Then the struct would return the time countdown, or can be used as a bool i.e.:
/// <code>
/// RealTimeUntil nextAttack = 10;
/// if ( nextAttack ) { /*Do something*/ }
/// </code>
/// </summary>
public struct RealTimeUntil : IEquatable<RealTimeUntil>
{
	private double _startTime;

	public static implicit operator bool( RealTimeUntil ts ) => RealTime.GlobalNow >= ts.Absolute;
	public static implicit operator float( RealTimeUntil ts ) => (float)(ts.Absolute - RealTime.GlobalNow);

	public static implicit operator RealTimeUntil( float ts ) =>
		new() { Absolute = RealTime.GlobalNow + ts, _startTime = RealTime.GlobalNow };

	public static bool operator <( in RealTimeUntil ts, float f ) => ts.Relative < f;
	public static bool operator >( in RealTimeUntil ts, float f ) => ts.Relative > f;
	public static bool operator <=( in RealTimeUntil ts, float f ) => ts.Relative <= f;
	public static bool operator >=( in RealTimeUntil ts, float f ) => ts.Relative >= f;
	public static bool operator <( in RealTimeUntil ts, int f ) => ts.Relative < f;
	public static bool operator >( in RealTimeUntil ts, int f ) => ts.Relative > f;
	public static bool operator <=( in RealTimeUntil ts, int f ) => ts.Relative <= f;
	public static bool operator >=( in RealTimeUntil ts, int f ) => ts.Relative >= f;

	/// <summary>
	/// Time to which we are counting down to, based on <see cref="RealTime.GlobalNow"/>.
	/// </summary>
	public double Absolute { get; private set; }

	/// <summary>
	/// The actual countdown, in seconds.
	/// </summary>
	public double Relative => this;

	/// <summary>
	/// Amount of seconds passed since the countdown started.
	/// </summary>
	public double Passed => (RealTime.GlobalNow - _startTime);

	/// <summary>
	/// The countdown, but as a fraction, i.e. a value from 0 (start of countdown) to 1 (end of countdown)
	/// </summary>
	public double Fraction =>
		global::System.Math.Clamp( (RealTime.GlobalNow - _startTime) / (Absolute - _startTime), 0.0f, 1.0f );

	public override string ToString() => $"{Relative}";

	#region equality

	public static bool operator ==( RealTimeUntil left, RealTimeUntil right ) => left.Equals( right );
	public static bool operator !=( RealTimeUntil left, RealTimeUntil right ) => !(left == right);
	public readonly override bool Equals( object? obj ) => obj is RealTimeUntil o && Equals( o );
	public readonly bool Equals( RealTimeUntil o ) => global::System.Math.Abs( Absolute - o.Absolute ) < 0.0001f;
	public readonly override int GetHashCode() => HashCode.Combine( Absolute );

	#endregion
}
