namespace MossEngine.UI.Utility.BytePack.Packers;

internal partial class BytePack
{
	/// <summary>
	/// Writes a type header and then the value
	/// </summary>
	public class ObjectPacker : Utility.BytePack.BytePack.Packer
	{
		public override Type TargetType => typeof( object );
		internal override Utility.BytePack.BytePack.Identifier Header => Utility.BytePack.BytePack.Identifier.Object;

		internal ObjectPacker()
		{

		}

		public override void Write( ref ByteStream.ByteStream bs, object obj )
		{
			Serialize( ref bs, obj );
		}

		public override object Read( ref ByteStream.ByteStream data )
		{
			return Deserialize( ref data );
		}
	}
}
