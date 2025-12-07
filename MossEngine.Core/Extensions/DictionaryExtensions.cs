using System.Collections.Immutable;

namespace MossEngine.Core.Extensions;

/// <summary>
/// Extension methods for working with immutable dictionaries.
/// Provides functional-style operations for creating modified copies.
/// </summary>
internal static class DictionaryExtensions
{
	extension<TKey, TValue>( IReadOnlyDictionary<TKey, TValue> dict ) where TKey : IEquatable<TKey>
	{
		/// <summary>
		/// Returns a new dictionary with the specified key-value pair added or updated.
		/// If the value already exists and is equal, returns the original dictionary.
		/// </summary>
		public IReadOnlyDictionary<TKey, TValue> With( TKey key, TValue value )
		{
			if ( dict.TryGetValue( key, out var existing ) && Equals( existing, value ) )
			{
				return dict;
			}

			var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();

			builder.AddRange( dict );
			builder[key] = value;

			return builder.ToImmutable();
		}

		/// <summary>
		/// Returns a new dictionary with the specified key removed.
		/// If the key doesn't exist, returns the original dictionary.
		/// </summary>
		public IReadOnlyDictionary<TKey, TValue> Without( TKey key )
		{
			if ( !dict.ContainsKey( key ) )
				return dict;

			var builder = ImmutableDictionary.CreateBuilder<TKey, TValue>();
			builder.AddRange( dict.Where( x => !x.Key.Equals( key ) ) );

			return builder.ToImmutable();
		}
	}

	/// <summary>
	/// If the key doesn't exist it is created and returned
	/// </summary>
	public static TValue GetOrCreate<TKey, TValue>( this IDictionary<TKey, TValue> dict, TKey key ) where TValue : new()
	{
		if ( dict.TryGetValue( key, out var val ) )
			return val;

		val = new TValue();
		dict.Add( key, val );

		return val;
	}

	/// <summary>
	/// Clones the dictionary. Doesn't clone the values.
	/// </summary>
	public static Dictionary<TKey, TValue> Clone<TKey, TValue>( this Dictionary<TKey, TValue> dict ) where TKey : notnull
	{
		var ret = new Dictionary<TKey, TValue>( dict.Count, dict.Comparer );

		foreach ( var entry in dict )
			ret.Add( entry.Key, entry.Value );

		return ret;
	}
}
