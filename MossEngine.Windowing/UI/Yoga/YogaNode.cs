using Yoga;

namespace MossEngine.Windowing.UI.Yoga;

public unsafe partial class YogaNode : YogaObject<YogaNode>, IDisposable
{
	private YogaConfig? _config;
	private YogaNode? _owner;
	private bool _disposed;

	public bool HasParent => Parent is not null;

	public YogaNode? Parent
	{
		get => _owner;
		set
		{
			if ( value is not null )
				value.Children.Add( this );
			else
				_owner?.Children.Remove( this );
		}
	}

	public bool IsDirty
	{
		get => YG.NodeIsDirty( this ) is not 0;
		set
		{
			if ( value )
				YG.NodeMarkDirty( this );
		}
	}

	public bool IsReferenceBaseline
	{
		get => YG.NodeIsReferenceBaseline( this ) is not 0;
		set => YG.NodeSetIsReferenceBaseline( this, (byte)(value ? 1 : 0) );
	}

	public YogaNodeList Children { get; }

	public YogaConfig? Config
	{
		get => _config;
		set
		{
			_config = value;

			//FIXME: if we null config what should we do?
			if ( value is not null )
				YG.NodeSetConfig( this, value );
		}
	}

	public YogaNodeType Type
	{
		get
		{
			return (YogaNodeType)YG.NodeGetNodeType( this );
		}
		set
		{
			YG.NodeSetNodeType( this, (YGNodeType)value );
		}
	}

	protected YogaNode( void* pointer ) : base( pointer )
	{
		Children = new YogaNodeList( this );
	}

	public YogaNode()
	{
		Pointer = YG.NodeNew();
		Children = new YogaNodeList( this );
		ObjectCache.Add( this, new WeakReference<YogaNode>( this ) );

		SetDirtiedEvent();
	}

	public YogaNode( YogaConfig config )
	{
		Pointer = YG.NodeNewWithConfig( config );

		Children = new YogaNodeList( this );
		ObjectCache.Add( this, new WeakReference<YogaNode>( this ) );

		SetDirtiedEvent();
	}

	~YogaNode()
	{
		Dispose( false );
	}

	public void InsertChildAt( YogaNode child, int index )
	{
		Children.Insert( index, child );
	}

	public void RemoveChildAt( int index )
	{
		Children.Remove( index );
	}

	public void RemoveChild( YogaNode child )
	{
		Children.Remove( child );
	}

	private static YogaNode GetOrCreate( nint ptr )
	{
		lock ( ObjectCache )
		{
			if ( ObjectCache.TryGetValue( ptr, out var weakRef ) )
			{
				if ( weakRef.TryGetTarget( out var existing ) )
					return existing;

				ObjectCache.Remove( ptr );
			}

			var obj = new YogaNode();
			ObjectCache[ptr] = new WeakReference<YogaNode>( obj );

			return obj;
		}
	}

	internal void SetParent( YogaNode? parent = null )
	{
		_owner = parent;
	}

	public YogaNode Clone()
	{
		return new YogaNode( YG.NodeClone( this ) ) { _config = _config };
	}

	public void Reset()
	{
		YG.NodeReset( this );
	}

	private void Free()
	{
		YG.NodeFree( this );
	}

	internal void FreeRecursive()
	{
		foreach ( var child in Children )
		{
			child.FreeRecursive();
		}

		Free();
		//YG.NodeFreeRecursive(RawPointer);
	}

	internal void NodeFinalize()
	{
		YG.NodeFinalize( this );
	}

	public static implicit operator YGNode*( YogaNode node ) => (YGNode*)node.Pointer;
	public static implicit operator YogaNode( void* o ) => GetOrCreate( (nint)o );
	public static implicit operator YogaNode( nint o ) => GetOrCreate( o );

	public void Dispose()
	{
		Dispose( true );
		GC.SuppressFinalize( this );
	}

	protected virtual void Dispose( bool disposing )
	{
		if ( _disposed ) return;

		if ( disposing )
		{
			ContextCache.Remove( this );
		}

		Free();
		_disposed = true;
	}
}
