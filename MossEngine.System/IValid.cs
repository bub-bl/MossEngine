namespace MossEngine.System;

/// <summary>
/// Interface for objects that can become invalid over time,
/// such as references to deleted game objects or disposed resources.
/// </summary>
public interface IValid
{
	/// <summary>
	/// Returns true if this object is still valid and can be safely accessed.
	/// When false, accessing the object's properties or methods may throw exceptions.
	/// </summary>
	bool IsValid { get; }
}
