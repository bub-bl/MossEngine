
namespace MossEngine.Core.Extensions;

public static class ActionExtensions
{
	/// <summary>
	/// Call an action, swallow any exceptions with a warning
	/// </summary>
	public static void InvokeWithWarning( this Action action )
	{
		try
		{
			action.Invoke();
		}
		catch ( Exception e )
		{
			Console.WriteLine( e.Message );
			// Log.Warning( e, $"{e.Message}" );
		}
	}

	/// <summary>
	/// Call an action, swallow any exceptions with a warning
	/// </summary>
	public static void InvokeWithWarning<T>( this Action<T> action, T arg0 )
	{
		try
		{
			action.Invoke( arg0 );
		}
		catch ( Exception e )
		{
			Console.WriteLine( e.Message );
			// Log.Warning( e, $"{e.Message}" );
		}
	}

	/// <summary>
	/// Call an action, swallow any exceptions with a warning
	/// </summary>
	public static void InvokeWithWarning<T1, T2>( this Action<T1, T2> action, T1 arg0, T2 arg1 )
	{
		try
		{
			action.Invoke( arg0, arg1 );
		}
		catch ( Exception e )
		{
			Console.WriteLine( e.Message );
			// Log.Warning( e, $"{e.Message}" );
		}
	}
}
