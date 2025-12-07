namespace MossEngine.Core.Attributes;

[AttributeUsage( AttributeTargets.All )]
public class CategoryAttribute( string value ) : Attribute
{
	public string Value { get; set; } = value;
}
