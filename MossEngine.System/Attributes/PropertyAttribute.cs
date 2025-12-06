namespace MossEngine.System.Attributes;

[AttributeUsage( AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Method )]
public class PropertyAttribute( string name, string title ) : Attribute
{
	public string Name { get; set; } = name;
	public string Title { get; set; } = title;
}
