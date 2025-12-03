using System.Drawing;
using System.Runtime.InteropServices;
using Yoga;

namespace MossEngine.UI.Yoga;

public unsafe partial class YogaNode : YogaObject<YogaNode>, IDisposable
{
	private YogaConfig? _config;
	private YogaNode? _owner;

	private bool _disposed;

	private MeasureFuncUnmanaged _measureFuncUnmanaged;
	private BaselineFuncUnmanaged? _baselineFunctionUnmanaged;
	private DirtiedFunctionUnmanaged _dirtiedFunctionUnmanaged;

	private MeasureFunc? _measureFunction;
	private BaselineFunc? _baselineFunction;

	public delegate float BaselineFunc( YogaNode node, float width, float height );

	[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
	private delegate float BaselineFuncUnmanaged( void* node, float width, float height );

	public delegate SizeF MeasureFunc( YogaNode node, float width, YogaMeasureMode widthMode,
		float height, YogaMeasureMode heightMode );

	[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
	private delegate YGSize MeasureFuncUnmanaged( void* node, float width, YGMeasureMode widthMode, float height,
		YGMeasureMode heightMode );

	public delegate void DirtiedFunction( YogaNode node );

	[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
	public delegate void DirtiedFunctionUnmanaged( void* node );

	public event DirtiedFunction? HaveDirtied;

	public MeasureFunc? MeasureFunction
	{
		get => _measureFunction;
		set
		{
			_measureFunction = value;

			if ( _measureFunction is null )
			{
				YG.NodeSetMeasureFunc( this, null );
				return;
			}

			_measureFuncUnmanaged = ( node, width, mode, height, heightMode ) =>
			{
				var result = _measureFunction.Invoke( this, width, (YogaMeasureMode)mode, height,
					(YogaMeasureMode)heightMode );

				return new YGSize { width = result.Width, height = result.Height };
			};

			YG.NodeSetMeasureFunc( this,
				(delegate* unmanaged[Cdecl]< void*, float, YGMeasureMode, float, YGMeasureMode, YGSize >)
				Marshal.GetFunctionPointerForDelegate( _measureFuncUnmanaged ) );
		}
	}

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
			if ( value ) YG.NodeMarkDirty( this );
		}
	}

	public bool HasNewLayout
	{
		get => YG.NodeGetHasNewLayout( this ) is not 0;
		set => YG.NodeSetHasNewLayout( this, (byte)(value ? 1 : 0) );
	}

	public bool IsReferenceBaseline
	{
		get => YG.NodeIsReferenceBaseline( this ) is not 0;
		set => YG.NodeSetIsReferenceBaseline( this, (byte)(value ? 1 : 0) );
	}

	public NodeList Children { get; }

	public DirtiedFunctionUnmanaged DirtiedFunc
	{
		get => Marshal.GetDelegateForFunctionPointer<DirtiedFunctionUnmanaged>(
			(IntPtr)YG.NodeGetDirtiedFunc( this ) );
		set => YG.NodeSetDirtiedFunc( this,
			(delegate* unmanaged[Cdecl]< void*, void >)Marshal.GetFunctionPointerForDelegate( value ) );
	}

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

	public BaselineFunc? BaselineFunction
	{
		get => _baselineFunction;
		set
		{
			_baselineFunction = value;

			if ( _baselineFunction is null )
			{
				YG.NodeSetBaselineFunc( this, null );
				return;
			}

			_baselineFunctionUnmanaged = ( node, width, height ) => _baselineFunction( this, width, height );

			YG.NodeSetBaselineFunc( this,
				(delegate* unmanaged[Cdecl]< void*, float, float, float >)Marshal.GetFunctionPointerForDelegate(
					_baselineFunctionUnmanaged ) );
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

	public bool AlwaysFormsContainingBlock
	{
		set
		{
			YG.NodeSetAlwaysFormsContainingBlock( this, (byte)(value ? 1 : 0) );
		}
	}

	protected YogaNode( void* pointer ) : base( pointer )
	{
		Children = new NodeList( this );
	}

	public YogaNode()
	{
		Pointer = YG.NodeNew();
		Children = new NodeList( this );
		ObjectCache.Add( this, new WeakReference<YogaNode>( this ) );

		SetDirtiedEvent();
	}

	public YogaNode( YogaConfig config )
	{
		Pointer = YG.NodeNewWithConfig( config );

		Children = new NodeList( this );
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

	public void SetDirtiedEvent()
	{
		_dirtiedFunctionUnmanaged = ( void* node ) =>
		{
			HaveDirtied?.Invoke( this );
		};

		DirtiedFunc = _dirtiedFunctionUnmanaged;
	}

	internal void SetParent( YogaNode? parent = null )
	{
		_owner = parent;
	}

	public YogaNode Clone()
	{
		var cloned = new YogaNode( YG.NodeClone( this ) ) { _config = _config };
		return cloned;
	}

// #if DEBUG
// 	// YGNodePrint only exist in debug builds of yoga
// 	public void Print( YogaPrintOptions printOptions )
// 	{
// 		YG.NodePrint( this, (YGPrintOptions)printOptions );
// 	}
// #endif

	public void Reset()
	{
		YG.NodeReset( this );
	}

	public void CalculateLayout( float width = Undefined, float height = Undefined,
		YogaDirection direction = YogaDirection.Inherit )
	{
		YG.NodeCalculateLayout( this, width, height, (YGDirection)direction );
	}

	internal void Free()
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
