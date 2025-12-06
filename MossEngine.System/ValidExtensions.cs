using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace MossEngine.System;

public static class ValidExtensions
{
	/// <summary>
	/// Returns false if <see cref="IValid"/> object is null or if <see cref="IValid.IsValid"/> returns false.
	/// </summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public static bool IsValid( [NotNullWhen( true )] this IValid? obj ) => obj is { IsValid: true };
}
