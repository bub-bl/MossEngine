using System.Collections;
using Yoga;

namespace MossEngine.Windowing.UI.Yoga;

public class YogaNodeList : ICollection
{
	private readonly YogaNode _node;
	private readonly List<YogaNode> _array = [];

	public int Count
	{
		get
		{
			unsafe
			{
				return (int)YG.NodeGetChildCount( _node );
			}
		}
	}

	public bool IsSynchronized => false;
	public object SyncRoot => this;

	internal YogaNodeList( YogaNode node )
	{
		_node = node;
	}

	public YogaNode this[int index]
	{
		get => Get( index );
		set => Swap( index, value );
	}

	internal YogaNode Get( int index )
	{
		return _array[index];
	}

	public void Add( YogaNode node )
	{
		Insert( Count, node );
	}

	public void Insert( int index, YogaNode node )
	{
		_node.SetParent( node );
		_array.Insert( index, node );

		unsafe
		{
			YG.NodeInsertChild( _node, node, (nuint)index );
		}
	}

	public void Remove( YogaNode node )
	{
		_node.SetParent();
		_array.Remove( node );

		unsafe
		{
			YG.NodeRemoveChild( _node, node );
		}
	}

	public void Clear()
	{
		foreach ( var node in _array )
		{
			_node.SetParent();
		}

		_array.Clear();

		unsafe
		{
			YG.NodeRemoveAllChildren( _node );
		}
	}

	public void Set( IEnumerable<YogaNode> nodes )
	{
		Clear();

		foreach ( var node in nodes )
		{
			Add( node );
		}
	}

	internal void Swap( int index, YogaNode node )
	{
		_array[index].SetParent();
		_node.SetParent( node );
		_array[index] = node;

		unsafe
		{
			YG.NodeSwapChild( _node, node, (nuint)index );
		}
	}

	public IEnumerator<YogaNode> GetEnumerator()
	{
		return new YogaNodeListEnumerator( this );
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public void CopyTo( Array array, int index = 0 )
	{
		foreach ( var child in this )
		{
			array.SetValue( child, index++ );
		}
	}
}
