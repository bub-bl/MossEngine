using MossEngine.UI.Logging;

namespace MossEngine.UI
{
	public static class GlobalSystemNamespace
	{
		public static Logger Log { get; } = new( "Generic" );

		// Avoiding the temptation to swamp this will global properties
		// like IsServer etc - at least for now.
	}
}
