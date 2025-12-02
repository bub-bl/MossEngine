using System.Reflection;

namespace MossEngine.UI.Extend;

public static partial class SandboxSystemExtensions
{
	public static bool IsPackage( this Assembly assembly )
	{
		return assembly.GetName().Name?.StartsWith( "package." ) ?? false;
	}
}