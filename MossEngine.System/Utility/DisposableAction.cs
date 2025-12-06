namespace MossEngine.System.Utility;

/// <summary>
/// A simple IDisposable that invokes an action when disposed.
/// Useful for creating using-blocks with cleanup logic.
/// </summary>
/// <example>
/// using ( DisposeAction.Create( () => Console.WriteLine( "Cleanup!" ) ) )
/// {
///     // Do work
/// } // "Cleanup!" is printed here
/// </example>
public readonly struct DisposeAction : IDisposable
{
	private readonly Action _action;

	/// <summary>
	/// Creates a new DisposeAction that will invoke the specified action on disposal.
	/// </summary>
	/// <param name="action">The action to invoke when disposed</param>
	public DisposeAction( Action action )
	{
		_action = action;
	}

	/// <summary>
	/// Invokes the action specified in the constructor.
	/// </summary>
	public void Dispose()
	{
		_action?.Invoke();
	}

	/// <summary>
	/// Factory method to create a DisposeAction as an IDisposable.
	/// </summary>
	/// <param name="action">The action to invoke when disposed</param>
	/// <returns>A disposable object that will invoke the action</returns>
	public static IDisposable Create( Action action )
	{
		return new DisposeAction( action );
	}
}
