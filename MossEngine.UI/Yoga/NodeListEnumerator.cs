using System.Collections;
using Yoga;

namespace MossEngine.UI.Yoga;

internal class NodeListEnumerator( NodeList nodeList ) : IEnumerator<YogaNode>
{
	private int _cursor = -1;
	private NodeList _nodeList = nodeList;

	public YogaNode Current => _nodeList[_cursor];

	object IEnumerator.Current => Current;

	public bool MoveNext()
	{
		if ( _cursor < _nodeList.Count )
			_cursor++;

		return _cursor != _nodeList.Count;
	}

	public void Reset()
	{
		_cursor = -1;
	}

	public void Dispose()
	{
		Reset();
		_nodeList = null!;
	}
}
