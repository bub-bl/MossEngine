using MossEngine.System.Logging;

namespace MossEngine.System;

public static class GlobalSystemNamespace
{
	public static Logger Log { get; } = new( "Generic" );
}
