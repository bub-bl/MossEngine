namespace MossEngine.UI.Utility.BytePack.Packers;

internal partial class BytePack
{
	/// <summary>
	/// A value array is useful because we only have to write the header once, and it's a single
	/// block of memory to read. Useful.
	/// </summary>
	class ValueArrayPacker : Utility.BytePack.BytePack.Packer
	{
		internal override Utility.BytePack.BytePack.Identifier Header => Utility.BytePack.BytePack.Identifier.ArrayValue;

		public override void Write( ref ByteStream.ByteStream bs, object obj )
		{
			if ( obj is not Array array ) throw new NotSupportedException();

			bs.Write( array.Length );

			var et = array.GetType().GetElementType();
			var w = GetHandlerFor( et );


			w.WriteTypeIdentifier( ref bs, et );
			bs.WriteValueArray( array );
		}

		public override object Read( ref ByteStream.ByteStream bs )
		{
			var len = bs.Read<int>();

			var handler = GetHandlerFor( ref bs );

			var array = Array.CreateInstance( handler.TargetType, len );
			bs.ReadValueArray( array );

			return array;
		}
	}

}
