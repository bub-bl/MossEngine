namespace MossEngine.Core.Attributes;

[AttributeUsage( AttributeTargets.Property )]
public sealed class DefaultValueAttribute( object value ) : Attribute
{
	public object Value { get; internal set; } = value;
}
