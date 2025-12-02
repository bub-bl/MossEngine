using System.Collections;

namespace MossEngine.UI.Utility.BytePack.Packers;

internal partial class BytePack
{
	class DictionaryPacker : Utility.BytePack.BytePack.Packer
	{
		internal override Utility.BytePack.BytePack.Identifier Header => Utility.BytePack.BytePack.Identifier.Dictionary;

		public override void Write( ref ByteStream.ByteStream bs, object obj )
		{
			if ( obj is not IDictionary dict ) throw new NotSupportedException();

			var kt = obj.GetType().GenericTypeArguments[0];
			var vt = obj.GetType().GenericTypeArguments[1];

			var kh = GetHandlerFor( kt );
			var vh = GetHandlerFor( vt );

			bs.Write( dict.Count );
			kh.WriteTypeIdentifier( ref bs, kt );
			vh.WriteTypeIdentifier( ref bs, vt );

			foreach ( DictionaryEntry de in dict )
			{
				kh.Write( ref bs, de.Key );
				vh.Write( ref bs, de.Value );
			}
		}

		public override object Read( ref ByteStream.ByteStream bs )
		{
			var len = bs.Read<int>();
			var keyHandler = GetHandlerFor( ref bs );
			var valueHandler = GetHandlerFor( ref bs );

			var dict = (IDictionary)Activator.CreateInstance( typeof( Dictionary<,> ).MakeGenericType( keyHandler.TargetType, valueHandler.TargetType ) );

			for ( int i = 0; i < len; i++ )
			{
				var k = keyHandler.Read( ref bs );
				var v = valueHandler.Read( ref bs );
				dict.Add( k, v );
			}


			return dict;
		}
	}

}
