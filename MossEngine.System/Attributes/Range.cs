namespace MossEngine.System.Attributes;

[AttributeUsage( AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field )]
public class RangeAttribute( float min, float max, bool clamped = true ) : Attribute
{
	public float Min { get; } = min;
	public float Max { get; } = max;
	public bool Clamped { get; } = clamped;
}
