namespace MossEngine.Core.Attributes;

[AttributeUsage( AttributeTargets.Method, AllowMultiple = true )]
public class EventAttribute( string eventName, int priority = 0 ) : Attribute
{
	public string EventName { get; set; } = eventName;
	public int Priority { get; set; } = priority;
}
