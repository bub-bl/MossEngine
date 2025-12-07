namespace MossEngine.Core.Attributes;

[AttributeUsage( AttributeTargets.All )]
public class TitleAttribute( string value ) : Attribute
{
	public string Value { get; set; } = value;
}
