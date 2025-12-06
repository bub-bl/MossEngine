namespace MossEngine.System.Attributes;

[AttributeUsage( AttributeTargets.All, AllowMultiple = true )]
public class DescriptionAttribute( string value ) : Attribute
{
	public string Value { get; set; } = value;
}
