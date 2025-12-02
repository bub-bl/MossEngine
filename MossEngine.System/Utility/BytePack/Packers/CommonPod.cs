using MossEngine.UI.Graphics;
using MossEngine.UI.Math;

namespace MossEngine.UI.Utility.BytePack.Packers;

internal partial class BytePack
{
	/// <summary>
	/// Plain old data types. It's faster to have these as identifier types than
	/// have them each being a TypeLibrary lookup dynamic type, so anything quite
	/// common should be included here.
	/// </summary>
	public class PodPacker<T> : Utility.BytePack.BytePack.Packer where T : unmanaged
	{
		private Utility.BytePack.BytePack.Identifier header;

		public override Type TargetType => typeof( T );
		internal override Utility.BytePack.BytePack.Identifier Header => header;

		internal PodPacker( Utility.BytePack.BytePack.Identifier header )
		{
			this.header = header;
		}

		public override void Write( ref ByteStream.ByteStream bs, object obj )
		{
			bs.Write<T>( (T)obj );
		}

		public override object Read( ref ByteStream.ByteStream data )
		{
			return data.Read<T>();
		}
	}


	void InstallPodCommon()
	{
		// native types
		Add( new PodPacker<bool>( Utility.BytePack.BytePack.Identifier.Bool ) );
		Add( new PodPacker<byte>( Utility.BytePack.BytePack.Identifier.Byte ) );
		Add( new PodPacker<char>( Utility.BytePack.BytePack.Identifier.Char ) );
		Add( new PodPacker<sbyte>( Utility.BytePack.BytePack.Identifier.SByte ) );
		Add( new PodPacker<short>( Utility.BytePack.BytePack.Identifier.Short ) );
		Add( new PodPacker<ushort>( Utility.BytePack.BytePack.Identifier.UShort ) );
		Add( new PodPacker<int>( Utility.BytePack.BytePack.Identifier.Int ) );
		Add( new PodPacker<uint>( Utility.BytePack.BytePack.Identifier.UInt ) );
		Add( new PodPacker<float>( Utility.BytePack.BytePack.Identifier.Float ) );
		Add( new PodPacker<double>( Utility.BytePack.BytePack.Identifier.Double ) );
		Add( new PodPacker<decimal>( Utility.BytePack.BytePack.Identifier.Decimal ) );
		Add( new PodPacker<long>( Utility.BytePack.BytePack.Identifier.Long ) );
		Add( new PodPacker<ulong>( Utility.BytePack.BytePack.Identifier.ULong ) );
		Add( new PodPacker<Guid>( Utility.BytePack.BytePack.Identifier.Guid ) );
		Add( new PodPacker<TimeSpan>( Utility.BytePack.BytePack.Identifier.TimeSpan ) );
		Add( new PodPacker<DateTime>( Utility.BytePack.BytePack.Identifier.DateTime ) );
		Add( new PodPacker<DateTimeOffset>( Utility.BytePack.BytePack.Identifier.DateTimeOffset ) );

		Add( new PodPacker<Vector2>( Utility.BytePack.BytePack.Identifier.Vector2 ) );
		Add( new PodPacker<Vector2Int>( Utility.BytePack.BytePack.Identifier.Vector2Int ) );
		Add( new PodPacker<Vector3>( Utility.BytePack.BytePack.Identifier.Vector3 ) );
		Add( new PodPacker<Vector3Int>( Utility.BytePack.BytePack.Identifier.Vector3Int ) );
		Add( new PodPacker<Vector4>( Utility.BytePack.BytePack.Identifier.Vector4 ) );
		Add( new PodPacker<Rotation>( Utility.BytePack.BytePack.Identifier.Rotation ) );
		Add( new PodPacker<Angles>( Utility.BytePack.BytePack.Identifier.Angles ) );
		Add( new PodPacker<Transform>( Utility.BytePack.BytePack.Identifier.Transform ) );
		Add( new PodPacker<Color>( Utility.BytePack.BytePack.Identifier.Color ) );
		Add( new PodPacker<Color32>( Utility.BytePack.BytePack.Identifier.Color32 ) );
		Add( new PodPacker<BBox>( Utility.BytePack.BytePack.Identifier.BBox ) );
		Add( new PodPacker<Sphere>( Utility.BytePack.BytePack.Identifier.Sphere ) );
		Add( new PodPacker<Plane>( Utility.BytePack.BytePack.Identifier.Plane ) );
		Add( new PodPacker<Rect>( Utility.BytePack.BytePack.Identifier.Rect ) );
		Add( new PodPacker<Ray>( Utility.BytePack.BytePack.Identifier.Ray ) );
		Add( new PodPacker<Line>( Utility.BytePack.BytePack.Identifier.Line ) );
		Add( new PodPacker<Matrix>( Utility.BytePack.BytePack.Identifier.Matrix ) );
		Add( new PodPacker<SteamId>( Utility.BytePack.BytePack.Identifier.SteamId ) );
	}
}
