using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MossEngine;

public static partial class SystemExtensions
{
	/// <summary>
	/// Returns false if <see cref="IValid"/> object is null or if <see cref="IValid.IsValid"/> returns false.
	/// </summary>
#nullable enable
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool IsValid( [NotNullWhen( true )] this IValid? obj ) => obj is { IsValid: true };
#nullable restore
}
