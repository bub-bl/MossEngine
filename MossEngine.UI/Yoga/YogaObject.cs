using Yoga;

namespace MossEngine.UI.Yoga;

public abstract unsafe class YogaObject<T> where T : YogaObject<T>, new()
{
	public const float Undefined = YG.YGUndefined;

	protected void* Pointer;

	internal static readonly Dictionary<nint, WeakReference<T>> ObjectCache = new();
	public static readonly Dictionary<nint, object?> ContextCache = new();

	protected YogaObject() { }

	public YogaObject( void* pointer )
	{
		Pointer = pointer;
	}

	public override bool Equals( object? obj )
	{
		if ( obj is not YogaObject<T> type )
			return false;

		return type.Pointer == Pointer;
	}
	
	public static implicit operator void*( YogaObject<T> o ) => o.Pointer;
	public static implicit operator nint( YogaObject<T> o ) => (nint)o.Pointer;

	public object? Context
	{
		get => ContextCache.GetValueOrDefault( this );
		set => ContextCache[this] = value;
	}
}
