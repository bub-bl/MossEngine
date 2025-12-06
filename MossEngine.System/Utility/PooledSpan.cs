using System.Buffers;
using System.Runtime.CompilerServices;

namespace MossEngine.System.Utility;

/// <summary>
/// A stack-only type with the ability to rent a buffer of a specified length and getting a <see cref="Span{T}"/> from it.
/// It should be used like so:
/// <code>
/// using (PooledSpan&lt;byte> buffer = PooledSpan&lt;byte>(1024))
/// {
///     // Use the buffer here...
/// }
/// </code>
/// As soon as the code leaves the scope of that <see langword="using"/> block, the underlying buffer will automatically
/// be disposed.
/// </summary>
internal readonly ref struct PooledSpan<T> : IDisposable
{
	/// <summary>
	/// The usable length within <see cref="_array"/>.
	/// </summary>
	private readonly int _length;

	/// <summary>
	/// The underlying <typeparamref name="T"/> array.
	/// </summary>
	private readonly T[] _array;

	/// <summary>
	/// Initializes a new instance of the <see cref="PooledSpan{T}"/> struct with the specified parameters.
	/// </summary>
	/// <param name="length">The length of the new memory buffer to use.</param>
	public PooledSpan( int length )
	{
		_length = length;
		_array = ArrayPool<T>.Shared.Rent( length );
	}

	/// <summary>
	/// Gets a <see cref="Span{T}"/> wrapping the memory belonging to the current instance.
	/// </summary>
	public Span<T> Span => _array.AsSpan( 0, _length );

	/// <summary>
	/// Implements the duck-typed <see cref="IDisposable.Dispose"/> method.
	/// </summary>
	[MethodImpl( MethodImplOptions.AggressiveInlining )]
	public void Dispose()
	{
		ArrayPool<T>.Shared.Return( _array );
	}
}
