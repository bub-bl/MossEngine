using System.Runtime.CompilerServices;

namespace MossEngine.Core.Extensions;

public static class NumberExtensions
{
	extension<T>( T input ) where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
	{
		/// <summary>
		/// Given a number, will format as a memory value, ie 10gb, 4mb
		/// </summary>
		public string FormatBytes( bool shortFormat = false )
		{
			var i = input switch
			{
				float f => (ulong)global::System.Math.Max( 0, f ),
				int ii => (ulong)global::System.Math.Max( 0, ii ),
				long l => (ulong)global::System.Math.Max( 0, l ),
				double d => (ulong)global::System.Math.Max( 0, d ),
				_ => (ulong)Convert.ChangeType( input, typeof( ulong ) )
			};

			double readable;
			string suffix;

			switch ( i )
			{
				// Exabyte
				case >= 0x1000000000000000:
					suffix = "eb";
					readable = (i >> 50);
					break;
				// Petabyte
				case >= 0x4000000000000:
					suffix = "pb";
					readable = (i >> 40);
					break;
				// Terabyte
				case >= 0x10000000000:
					suffix = "tb";
					readable = (i >> 30);
					break;
				// Gigabyte
				case >= 0x40000000:
					suffix = "gb";
					readable = (i >> 20);
					break;
				// Megabyte
				case >= 0x100000:
					suffix = "mb";
					readable = (i >> 10);
					break;
				// Kilobyte
				case >= 0x400:
					suffix = "kb";
					readable = i;
					break;
				default:
					return i.ToString( "0b" ); // Byte
			}

			readable /= 1024;
			return readable.ToString( shortFormat ? "0" : "0.00" ) + suffix;
		}

		/// <summary>
		/// Clamp a number between two values.
		/// </summary>
		public T Clamp( T min, T max )
		{
			if ( input.CompareTo( min ) < 0 ) return min;
			if ( input.CompareTo( max ) > 0 ) return max;

			return input;
		}
	}

	/// <summary>
	/// Formats the given value in format "1w2d3h4m5s". Will not display 0 values.
	/// </summary>
	/// <param name="secs">Time to format, in seconds.</param>
	public static string FormatSeconds( this long secs )
	{
		var m = global::System.Math.Floor( secs / 60.0f );
		var h = global::System.Math.Floor( (float)m / 60.0f );
		var d = global::System.Math.Floor( (float)h / 24.0f );
		var w = global::System.Math.Floor( (float)d / 7.0f );

		if ( secs < 60 ) return $"{secs}s"; // 1s
		if ( m < 60 ) return string.Format( "{1}m{0}s", secs % 60, m ); // 5m3s
		if ( h < 48 ) return string.Format( "{2}h{1}m{0}s", secs % 60, m % 60, h ); // 6h40m34h
		if ( d < 7 ) return string.Format( "{3}d{2}h{1}m{0}s", secs % 60, m % 60, h % 24, d ); // 5d15h15m10s

		return string.Format( "{4}w{3}d{2}h{1}m{0}s", secs % 60, m % 60, h % 24, d % 7, w );
	}

	/// <inheritdoc cref=" FormatSeconds(long)"/>
	public static string FormatSeconds( this ulong secs ) { return FormatSeconds( (long)secs ); }

	/// <summary>
	/// Formats the given value in format "4 weeks, 3 days, 2 hours and 1 minutes".
	/// Will not display 0 values. Will not display seconds if value is more than 1 hour.
	/// </summary>
	/// <param name="secs">Time to format, in seconds.</param>
	public static string FormatSecondsLong( this long secs )
	{
		var m = global::System.Math.Floor( (float)secs / 60.0f );
		var h = global::System.Math.Floor( (float)m / 60.0f );
		var d = global::System.Math.Floor( (float)h / 24.0f );
		var w = global::System.Math.Floor( (float)d / 7.0f );

		if ( secs < 60 ) return $"{secs} seconds";
		if ( m < 60 ) return string.Format( "{1} minutes, {0} seconds", secs % 60, m );
		if ( h < 48 ) return string.Format( "{1} hours and {0} minutes", m % 60, h );
		if ( d < 7 ) return string.Format( "{2} days, {1} hours and {0} minutes", m % 60, h % 24, d );

		return string.Format( "{3} weeks, {2} days, {1} hours and {0} minutes", m % 60, h % 24, d % 7, w );
	}

	/// <inheritdoc cref=" FormatSecondsLong(long)"/>
	public static string FormatSecondsLong( this ulong secs ) { return FormatSecondsLong( (long)secs ); }

	/// <summary>
	/// "1500" becomes "1,500", "15 000" becomes "15K", "15 000 000" becomes "15KK", etc.
	/// </summary>
	public static string FormatNumberShort( this long num )
	{
		if ( num >= 100000 )
		{
			return FormatNumberShort( num / 1000 ) + "K";
		}

		if ( num >= 10000 )
		{
			return (num / 1000D).ToString( "0.#" ) + "K";
		}

		return num.ToString( "#,0" );
	}

	/// <inheritdoc cref=" FormatNumberShort(long)"/>
	public static string FormatNumberShort( this ulong num ) { return FormatNumberShort( (long)num ); }

	extension( int a )
	{
		/// <summary>
		/// Does what you expected to happen when you did "a % b", that is, handles negative <paramref name="a"/> values by returning a positive number from the end.
		/// </summary>
		public int UnsignedMod( int b )
		{
			// pasted from https://stackoverflow.com/questions/2691025/mathematical-modulus-in-c-sharp
			return (global::System.Math.Abs( a * b ) + a) % b;
		}

		/// <summary>
		/// Returns the number of bits set in an integer. This us usually used for flags to count
		/// the amount of flags set.
		/// </summary>
		public int BitsSet()
		{
			a -= ((a >> 1) & 0x55555555); // add pairs of bits
			a = (a & 0x33333333) + ((a >> 2) & 0x33333333); // quads
			a = (a + (a >> 4)) & 0x0F0F0F0F; // groups of 8

			return (a * 0x01010101) >> 24; // horizontal sum of bytes
		}

		/// <summary>
		/// Return single if 1 else plural
		/// </summary>
		public string Plural( string single, string plural )
		{
			return a is 1 or -1 ? single : plural;
		}

		/// <summary>
		/// Change 1 to 1st, 2 to 2nd etc
		/// </summary>
		public string FormatWithSuffix()
		{
			var number = a.ToString();

			if ( number.EndsWith( "11" ) ) return number + "th";
			if ( number.EndsWith( "12" ) ) return number + "th";
			if ( number.EndsWith( "13" ) ) return number + "th";
			if ( number.EndsWith( "1" ) ) return number + "st";
			if ( number.EndsWith( "2" ) ) return number + "nd";
			if ( number.EndsWith( "3" ) ) return number + "rd";

			return number + "th";
		}
	}

	private enum SizeUnit { B, Kb, Mb, Gb, Tb, Pb, Eb, Zb, Yb }

	public static string SizeFormat( this long bytes )
	{
		if ( bytes < 0 )
			return "-" + SizeFormat( -bytes );

		var unit = 0;
		double value = bytes;

		while ( unit < (int)SizeUnit.Yb && value >= 1024 )
		{
			value /= 1024;
			unit++;
		}

		return $"{value:F2} {(SizeUnit)unit}";
	}

	extension( int bytes )
	{
		public string SizeFormat() => SizeFormat( (long)bytes );

		/// <summary>
		/// Format a large number into "1045M", "56K"
		/// </summary>
		public string KiloFormat() => bytes switch
		{
			>= 10000000 => (bytes / 1000000).ToString( "#,0M" ),
			>= 1000000 => (bytes / 1000000).ToString( "0.#" ) + "M",
			>= 100000 => (bytes / 1000).ToString( "#,0K" ),
			>= 1000 => (bytes / 1000).ToString( "0.#" ) + "K",
			_ => bytes.ToString( "#,0" )
		};
	}

	/// <summary>
	/// Format a large number into "1045M", "56K"
	/// </summary>
	public static string KiloFormat( this long num ) => num switch
	{
		>= 10000000 => (num / 1000000).ToString( "#,0M" ),
		>= 1000000 => (num / 1000000).ToString( "0.#" ) + "M",
		>= 100000 => (num / 1000).ToString( "#,0K" ),
		>= 1000 => (num / 1000).ToString( "0.#" ) + "K",
		_ => num.ToString( "#,0" )
	};

	/// <summary>
	/// Humanize a timespan into "x hours", "x seconds"
	/// </summary>
	public static string Humanize( this TimeSpan timespan, bool shortVersion = false, bool minutes = true,
		bool hours = true, bool days = true )
	{
		if ( shortVersion )
		{
			if ( timespan.TotalSeconds < 1 )
			{
				return "0s";
			}

			if ( timespan.TotalSeconds < 60 || !minutes )
			{
				return $"{timespan.TotalSeconds:n0}s";
			}

			if ( timespan.TotalMinutes < 60 || !hours )
			{
				return $"{timespan.TotalMinutes:n0}m";
			}

			if ( timespan.TotalHours < 24 || !days )
			{
				return $"{timespan.TotalHours:n0}h";
			}

			return $"{timespan.TotalDays:n0}d";
		}

		if ( timespan.TotalSeconds < 1 )
		{
			return "0 seconds";
		}

		if ( timespan.TotalSeconds < 60 || !minutes )
		{
			return $"{timespan.TotalSeconds:n0} seconds";
		}

		if ( timespan.TotalMinutes < 60 || !hours )
		{
			return $"{timespan.TotalMinutes:n0} minutes";
		}

		if ( timespan.TotalHours < 24 || !days )
		{
			return $"{timespan.TotalHours:n0} hours";
		}

		return $"{timespan.TotalDays:n0} days";
	}

	extension<T>( T value ) where T : unmanaged, Enum
	{
		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public bool Contains( T flag )
		{
			if ( Unsafe.SizeOf<T>() == sizeof( int ) )
			{
				return (Unsafe.As<T, int>( ref value ) & Unsafe.As<T, int>( ref flag )) ==
					   Unsafe.As<T, int>( ref flag );
			}

			if ( Unsafe.SizeOf<T>() == sizeof( long ) )
			{
				return (Unsafe.As<T, long>( ref value ) & Unsafe.As<T, long>( ref flag )) ==
					   Unsafe.As<T, long>( ref flag );
			}

			throw new ArgumentException( "Unsupported enum type" );
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public unsafe T WithFlag( T flag, bool set )
		{
			switch ( sizeof( T ) )
			{
				case 1:
					{
						var valPtr = (byte*)&value;
						var flagPtr = (byte*)&flag;
						var result = set ? (byte)(*valPtr | *flagPtr) : (byte)(*valPtr & ~(*flagPtr));

						return *(T*)&result;
					}
				case 2:
					{
						var valPtr = (ushort*)&value;
						var flagPtr = (ushort*)&flag;
						var result = set ? (ushort)(*valPtr | *flagPtr) : (ushort)(*valPtr & ~(*flagPtr));

						return *(T*)&result;
					}
				case 4:
					{
						var valPtr = (uint*)&value;
						var flagPtr = (uint*)&flag;
						var result = set ? *valPtr | *flagPtr : *valPtr & ~(*flagPtr);

						return *(T*)&result;
					}
				case 8:
					{
						var valPtr = (ulong*)&value;
						var flagPtr = (ulong*)&flag;
						var result = set ? *valPtr | *flagPtr : *valPtr & ~(*flagPtr);

						return *(T*)&result;
					}
				default:
					throw new NotSupportedException( $"Unsupported enum underlying type size {sizeof( T )}" );
			}
		}

		[MethodImpl( MethodImplOptions.AggressiveInlining )]
		public int AsInt()
		{
			if ( Unsafe.SizeOf<T>() == sizeof( int ) ) return Unsafe.As<T, int>( ref value );
			if ( Unsafe.SizeOf<T>() == sizeof( byte ) ) return Unsafe.As<T, byte>( ref value );
			if ( Unsafe.SizeOf<T>() == sizeof( short ) ) return Unsafe.As<T, short>( ref value );
			if ( Unsafe.SizeOf<T>() == sizeof( long ) ) return (int)Unsafe.As<T, long>( ref value );

			return 0;
		}
	}

	extension( int input )
	{
		/// <summary>
		/// Convert 1100 to 1.1k
		/// </summary>
		public string ToMetric( int decimals = 2 )
		{
			return Humanizer.MetricNumeralExtensions.ToMetric( input, decimals: decimals );
		}

		/// <summary>
		/// Return true if the number is a power of two (2, 4, 8, 16, 32, 64, 128, 256, 512, 1024, etc)
		/// </summary>
		public bool IsPowerOfTwo()
		{
			return (input & (input - 1)) == 0;
		}
	}

	/// <summary>
	/// Convert 1100 to 1.1k
	/// </summary>
	public static string ToMetric( this long input, int decimals = 2 )
	{
		return Humanizer.MetricNumeralExtensions.ToMetric( (int)input, decimals: decimals );
	}

	/// <summary>
	/// Convert 1100 to 1.1k
	/// </summary>
	public static string ToMetric( this double input, int decimals = 2 )
	{
		return Humanizer.MetricNumeralExtensions.ToMetric( input, decimals: decimals );
	}

	/// <summary>
	/// Convert 1100 to 1.1k
	/// </summary>
	public static string ToMetric( this float input, int decimals = 2 )
	{
		return Humanizer.MetricNumeralExtensions.ToMetric( input, decimals: decimals );
	}
}
