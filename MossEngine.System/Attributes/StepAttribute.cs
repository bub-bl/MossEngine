namespace MossEngine.System.Attributes;

[AttributeUsage( AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.Field )]
public class StepAttribute( float step ) : Attribute
{
	public float Step { get; } = step;
}
