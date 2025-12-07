namespace MossEngine.System;

/// <summary>
/// Hidden random class. This is secretly used by Game.Random, but being here 
/// allows all of our system functions to use the same Random instance.
/// </summary>
internal static class Rand
{
	[field: ThreadStatic]
	public static Random Random
	{
		get
		{
			field ??= new Random();
			return field;
		}
		private set;
	}

	/// <summary>
	/// Sets the seed for these static classes
	/// </summary>
	public static void SetRandomSeed( int seed )
	{
		Random = new Random( seed );
	}
}
