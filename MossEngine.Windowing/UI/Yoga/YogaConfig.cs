using System.Runtime.InteropServices;
using Yoga;

namespace MossEngine.Windowing.UI.Yoga;

public unsafe class YogaConfig : YogaObject<YogaConfig>
{
	public static YogaConfig Default { get; internal set; }

	public delegate int YogaLoggingFunction( YogaConfig config, IntPtr node, YogaLogLevel logLevel,
		string format, string[] args );

	[UnmanagedFunctionPointer( CallingConvention.Cdecl )]
	private delegate int YGLoggingFunction( void* config, void* node, YGLogLevel logLevel, sbyte* format,
		sbyte* args );

	public YogaLoggingFunction? MeasureFunction
	{
		get;
		set
		{
			field = value;

			if ( field is null )
			{
				YG.ConfigSetLogger( this, null );
				return;
			}

			YGLoggingFunction unmanaged = ( config, node, logLevel, format, args ) =>
			{
				var result = field( this, new IntPtr( node ), (YogaLogLevel)logLevel, format->ToString(),
					[args->ToString()] );

				return result; // TODO - FIXME: how do we handle varargs?
			};

			YG.ConfigSetLogger( this,
#pragma warning disable CS8500
				(delegate* unmanaged[Cdecl]< void*, void*, YGLogLevel, sbyte*, sbyte*, int >)&unmanaged );
#pragma warning restore CS8500
		}
	}

	public bool UseWebDefaults
	{
		get
		{
			var byteResult = YG.ConfigGetUseWebDefaults( this );
			return byteResult is not 0;
		}
		set
		{
			YG.ConfigSetUseWebDefaults( this, (byte)(value ? 1 : 0) );
		}
	}

	public float PointScaleFactor
	{
		get
		{
			return YG.ConfigGetPointScaleFactor( this );
		}
		set
		{
			YG.ConfigSetPointScaleFactor( this, value );
		}
	}

	public YogaErrata Errata
	{
		get
		{
			return (YogaErrata)YG.ConfigGetErrata( this );
		}
		set
		{
			YG.ConfigSetErrata( this, (YGErrata)value );
		}
	}

	static YogaConfig()
	{
		Default = new YogaConfig();
	}

	public YogaConfig()
	{
		Pointer = YG.ConfigNew();
	}

	public YogaConfig( void* pointer ) : base( pointer )
	{
	}

	~YogaConfig()
	{
		Free();
	}

	private void Free()
	{
		YG.ConfigFree( this );
	}

	public void SetExperimentalFeatureEnabled( YogaExperimentalFeature feature, bool enabled )
	{
		YG.ConfigSetExperimentalFeatureEnabled( this, (YGExperimentalFeature)feature, (byte)(enabled ? 1 : 0) );
	}

	public bool IsExperimentalFeatureEnabled( YogaExperimentalFeature feature )
	{
		var byteResult = YG.ConfigIsExperimentalFeatureEnabled( this, (YGExperimentalFeature)feature );
		return byteResult is not 0;
	}

	public static implicit operator YGConfig*( YogaConfig o ) => (YGConfig*)o.Pointer;
	public static implicit operator void*( YogaConfig o ) => o.Pointer;
	public static implicit operator nint( YogaConfig o ) => (nint)o.Pointer;
}
