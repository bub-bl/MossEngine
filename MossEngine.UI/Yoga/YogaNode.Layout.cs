using System.Drawing;
using System.Runtime.InteropServices;
using Yoga;

namespace MossEngine.UI.Yoga;

public unsafe partial class YogaNode
{
	private MeasureFuncUnmanaged _measureFuncUnmanaged;
	private BaselineFuncUnmanaged? _baselineFunctionUnmanaged;
	private DirtiedFunctionUnmanaged _dirtiedFunctionUnmanaged;

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
		get;
		set
		{
			field = value;

			if ( field is null )
			{
				YG.NodeSetMeasureFunc( this, null );
				return;
			}

			_measureFuncUnmanaged = ( node, width, mode, height, heightMode ) =>
			{
				var result = field.Invoke( this, width, (YogaMeasureMode)mode, height,
					(YogaMeasureMode)heightMode );

				return new YGSize { width = result.Width, height = result.Height };
			};

			YG.NodeSetMeasureFunc( this,
				(delegate* unmanaged[Cdecl]< void*, float, YGMeasureMode, float, YGMeasureMode, YGSize >)
				Marshal.GetFunctionPointerForDelegate( _measureFuncUnmanaged ) );
		}
	}

	public DirtiedFunctionUnmanaged DirtiedFunc
	{
		get => Marshal.GetDelegateForFunctionPointer<DirtiedFunctionUnmanaged>(
			(IntPtr)YG.NodeGetDirtiedFunc( this ) );
		set => YG.NodeSetDirtiedFunc( this,
			(delegate* unmanaged[Cdecl]< void*, void >)Marshal.GetFunctionPointerForDelegate( value ) );
	}

	public BaselineFunc? BaselineFunction
	{
		get;
		set
		{
			field = value;

			if ( field is null )
			{
				YG.NodeSetBaselineFunc( this, null );
				return;
			}

			_baselineFunctionUnmanaged = ( node, width, height ) => field( this, width, height );

			YG.NodeSetBaselineFunc( this,
				(delegate* unmanaged[Cdecl]< void*, float, float, float >)Marshal.GetFunctionPointerForDelegate(
					_baselineFunctionUnmanaged ) );
		}
	}

	public float LayoutWidth => YG.NodeLayoutGetWidth( this );
	public float LayoutHeight => YG.NodeLayoutGetHeight( this );
	public Length LayoutLeft => YG.NodeLayoutGetLeft( this );
	public Length LayoutTop => YG.NodeLayoutGetTop( this );
	public Length LayoutRight => YG.NodeLayoutGetRight( this );
	public Length LayoutBottom => YG.NodeLayoutGetBottom( this );

	public bool AlwaysFormsContainingBlock
	{
		set => YG.NodeSetAlwaysFormsContainingBlock( this, (byte)(value ? 1 : 0) );
	}

	public bool HasNewLayout
	{
		get => YG.NodeGetHasNewLayout( this ) is not 0;
		set => YG.NodeSetHasNewLayout( this, (byte)(value ? 1 : 0) );
	}

	public void SetDirtiedEvent()
	{
		_dirtiedFunctionUnmanaged = ( void* node ) =>
		{
			HaveDirtied?.Invoke( this );
		};

		DirtiedFunc = _dirtiedFunctionUnmanaged;
	}

	public void CalculateLayout()
	{
		YG.NodeCalculateLayout( this, YG.YGUndefined, YG.YGUndefined, (YGDirection)Direction );
	}
}
