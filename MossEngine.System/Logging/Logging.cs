using System.Threading.Channels;
using MossEngine.System.Utility;
using NLog;

namespace MossEngine.System.Logging;

internal static partial class Logging
{
	private static readonly Dictionary<string, LogLevel> Rules = new();
	private static readonly Dictionary<int, bool> RuleCache = new();

	private static bool _initialized;
	
	internal static void InitializeConfig()
	{
		if ( _initialized ) return;
		_initialized = true;

		var config = new NLog.Config.LoggingConfiguration();
		var gameTarget = new GameLog();

		LogManager.Setup().SetupExtensions( s =>
		{
			s.RegisterLayoutRenderer( "nicestack", ( logEvent ) =>
			{
				var frames = logEvent.StackTrace.GetFrames().Skip( 1 ).Take( 10 ).Where( x => x.GetMethod()?.DeclaringType?.Name != "Logger" );
				var stack = string.Join( "\n", frames.Select( x => $"\t\t{x.GetMethod()?.DeclaringType?.Name}.{x.GetMethod()?.Name} - {x.GetFileName()}:{x.GetFileLineNumber()}" ) );
				
				return stack.StartsWith( "\t\tEngineLoop.Print - " ) ? "" : $"\n{stack}\n";
			} );
		} );

		var appName = Process.GetCurrentProcess().ProcessName.Split( '.' )[0];

		var gamePath = Environment.GetEnvironmentVariable( "FACEPUNCH_ENGINE", EnvironmentVariableTarget.User );
		gamePath ??= AppContext.BaseDirectory;

		var fileTarget = new NLog.Targets.FileTarget
		{
			FileName = Path.Combine( gamePath, $"logs/{appName}.log" ),
			ArchiveOldFileOnStartup = true,
			OpenFileCacheSize = 10,
			MaxArchiveFiles = 10,
			KeepFileOpen = true,

			Layout = "${date:format=yyyy/MM/dd HH\\:mm\\:ss.ffff}\t[${logger}] ${message}\t${exception:format=ToString}"
		};

		config.AddTarget( "file", fileTarget );
		config.AddTarget( "console", gameTarget );

		config.LoggingRules.Clear();

		// Create a logging rule that captures everything
		{
			var rule = new NLog.Config.LoggingRule( "global" ) { LoggerNamePattern = "*" };
			rule.EnableLoggingForLevels( NLog.LogLevel.Trace, NLog.LogLevel.Fatal );
			rule.Targets.Add( fileTarget );
			rule.Targets.Add( gameTarget );

			config.LoggingRules.Add( rule );
		}

		LogManager.Configuration = config;

		// When we quit, shut nlog down
		AppDomain.CurrentDomain.ProcessExit += ( x, y ) =>
		{
			LogManager.Shutdown();
		};

		SetRule( "*", LogLevel.Info );
	}

	// 
	// Garry: I imagine at some point we'll expose rules in a way where we can choose which systems
	// are which levels. Right now that seems like overkill - so I'm just exposing the ability to change
	// verbosity globally. This is complicated by the fact that I want to pull in all the engine logging
	// system too - hence the OnRulesChanged callback, so we can call shit in the sandbox.engine.dll on change.
	// 

	public static LogLevel GetDefaultLevel()
	{
		return Rules.GetValueOrDefault("*", LogLevel.Info);
	}

	public static void SetRule( string wildcard, LogLevel minimumLevel )
	{
		Rules[wildcard] = minimumLevel;
		RuleCache.Clear();
	}

	/// <summary>
	/// Return true if we should print this log entry. Use a cache to avoid craziness.
	/// </summary>
	public static bool ShouldLog( string loggerName, LogLevel level )
	{
		lock ( RuleCache )
		{
			var hash = HashCode.Combine( loggerName, level );
			
			if ( RuleCache.TryGetValue( hash, out var should ) )
				return should;

			should = WorkOutShouldLog( loggerName, level );
			RuleCache[hash] = should;

			return should;
		}
	}

	private static bool WorkOutShouldLog( string loggerName, LogLevel level )
	{
		foreach ( var v in Rules.OrderByDescending( x => x.Key.Length ) )
		{
			if ( loggerName.StartsWith( v.Key ) && level >= v.Value )
				return true;
		}

		return false;
	}

	public static bool Enabled { get; set; } = true;
	internal static bool PrintToConsole { get; set; } = true;

	internal static event Action<LogEvent>? OnMessage;
	internal static Action<Exception>? OnException;

	private static readonly Channel<LogEvent> QueuedMessages = Channel.CreateUnbounded<LogEvent>();

	private static int _callDepth;

	internal static void Write( in LogEvent e )
	{
		if ( ThreadSafe.IsMainThread && _callDepth < 3 )
		{
			try
			{
				_callDepth++;
				OnMessage?.Invoke( e );
			}
			finally
			{
				_callDepth--;
			}
		}
		else
		{
			QueuedMessages.Writer.TryWrite( e );
		}
	}

	internal static void PushQueuedMessages()
	{
		ThreadSafe.AssertIsMainThread();

		while ( QueuedMessages.Reader.TryRead( out var msg ) )
		{
			try
			{
				OnMessage?.Invoke( msg );
			}
			catch ( Exception e )
			{
				Log.Error( e );
			}
		}
	}

	public static Logger GetLogger( string? name = null )
	{
		if ( name is not null )
			return new Logger( name );

		var frame = new StackFrame( 1, false );
		var method = frame.GetMethod();
			
		name = method?.DeclaringType?.Name;
		return new Logger( name );
	}

	/// <summary>
	/// Keep a list of loggers
	/// </summary>
	public static readonly HashSet<string> Loggers = new( StringComparer.OrdinalIgnoreCase );

	internal static void RegisterEngineLogger( int id, string v )
	{
		v = $"engine/{v}";
		Loggers.Add( v );
	}

	internal static void RegisterLogger( string name )
	{
		Loggers.Add( name );
	}
}
