using Yoga;

namespace MossEngine.UI.Yoga;

public readonly struct Length
{
	public readonly float Value;
	public readonly YogaUnit Unit = YogaUnit.Point;

	public static Length Auto => new( 0, YogaUnit.Auto );
	public static Length FitContent => new( 0, YogaUnit.FitContent );
	public static Length MaxContent => new( 0, YogaUnit.MaxContent );
	public static Length Stretch => new( 0, YogaUnit.Stretch );
	public static Length Undefined => new( 0, YogaUnit.Undefined );
	public static Length Zero => Point( 0 );

	public bool IsUndefined => Unit == YogaUnit.Undefined;

	private Length( float value, YogaUnit unit )
	{
		Value = value;
		Unit = unit;
	}

	public static Length Point( float value ) => new( value, YogaUnit.Point );
	public static Length Percent( float value ) => new( value, YogaUnit.Percent );

	public static implicit operator float( Length length ) => length.Value;
	public static implicit operator Length( float length ) => new( length, YogaUnit.Point );

	public static implicit operator Length( YGValue edge ) => new( edge.unit is YGUnit.YGUnitUndefined ? 0 : edge.value,
		edge.unit is YGUnit.YGUnitUndefined ? YogaUnit.Point : (YogaUnit)edge.unit );

	public override string ToString()
	{
		return $"{Unit}({Value})";
	}
}
