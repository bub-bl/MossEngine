using MossEngine.Utility;

namespace MossEngine;

public class StatusBar
{
	private readonly Lock _syncRoot = new();
	private readonly List<StatusBarItem> _items = [];
	private readonly Dictionary<string, StatusBarItem> _itemsByKey = new( StringComparer.OrdinalIgnoreCase );

	public event Action? ItemsChanged;

	public StatusBar()
	{
		SetStatusMessage( "Ready" );

		RegisterDynamic( "status.fps", () =>
		{
			var fps = Time.Delta <= 0 ? 0 : 1f / Time.Delta;
			var ms = Time.Delta * 1000f;

			return $"{fps:F0} FPS ({ms:F0} ms)";
		}, StatusBarSection.Right, "Current frame time" );
	}

	public StatusBarItem Register( string key, string text, StatusBarSection section = StatusBarSection.Left,
		string? tooltip = null )
	{
		lock ( _syncRoot )
		{
			if ( _itemsByKey.TryGetValue( key, out var existing ) )
			{
				existing.SetText( text );
				existing.Tooltip = tooltip;
				existing.Section = section;

				OnItemsChanged();
				return existing;
			}

			var item = new StatusBarItem( key, text, section, tooltip, null );

			_items.Add( item );
			_itemsByKey[key] = item;

			OnItemsChanged();
			return item;
		}
	}

	public StatusBarItem RegisterDynamic( string key, Func<string> valueProvider,
		StatusBarSection section = StatusBarSection.Left, string? tooltip = null )
	{
		var initial = valueProvider();

		lock ( _syncRoot )
		{
			if ( _itemsByKey.TryGetValue( key, out var existing ) )
			{
				existing.SetText( initial );
				existing.Tooltip = tooltip;
				existing.Section = section;
				existing.ValueProvider = valueProvider;

				OnItemsChanged();
				return existing;
			}

			var item = new StatusBarItem( key, initial, section, tooltip, valueProvider );

			_items.Add( item );
			_itemsByKey[key] = item;

			OnItemsChanged();
			return item;
		}
	}

	public void Remove( string key )
	{
		lock ( _syncRoot )
		{
			if ( !_itemsByKey.TryGetValue( key, out var item ) )
				return;

			_items.Remove( item );
			_itemsByKey.Remove( key );
		}

		OnItemsChanged();
	}

	public void SetStatusMessage( string message, string? tooltip = null )
	{
		Register( "status.message", message, StatusBarSection.Left, tooltip );
	}

	public void SetCaretPosition( int line, int column )
	{
		Register( "status.caret", $"Ln {line}, Col {column}", StatusBarSection.Right, "Caret position" );
	}

	public void SetSelectionInfo( int characters, int lines )
	{
		Register( "status.selection", lines > 1 ? $"Sel {lines} lines" : $"Sel {characters} chars",
			StatusBarSection.Right );
	}

	public void SetProgress( string key, float progress, string? label = null )
	{
		progress = Math.Clamp( progress, 0f, 1f );

		var text = label is null ? $"{progress:P0}" : $"{label} {progress:P0}";
		Register( key, text, StatusBarSection.Left );
	}

	public IReadOnlyList<StatusBarItem> GetItems()
	{
		lock ( _syncRoot )
		{
			return _items.ToList();
		}
	}

	public bool RefreshDynamicItems()
	{
		var updated = false;

		lock ( _syncRoot )
		{
			foreach ( var item in _items )
				updated |= item.TryRefresh();
		}

		return updated;
	}

	internal bool TryGetItem( Guid id, out StatusBarItem? item )
	{
		lock ( _syncRoot )
		{
			item = _items.FirstOrDefault( x => x.Id == id );
			return item is not null;
		}
	}

	private void OnItemsChanged()
	{
		ItemsChanged?.Invoke();
	}

	public enum StatusBarSection
	{
		Left,
		Right
	}

	public sealed class StatusBarItem
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Key { get; }
		public string Text { get; private set; }
		public string? Tooltip { get; internal set; }
		public StatusBarSection Section { get; internal set; }
		public Func<string>? ValueProvider { get; internal set; }

		internal StatusBarItem( string key, string text, StatusBarSection section, string? tooltip,
			Func<string>? valueProvider )
		{
			Key = key;
			Text = text;
			Section = section;
			Tooltip = tooltip;
			ValueProvider = valueProvider;
		}

		internal void SetText( string text )
		{
			Text = text;
		}

		internal bool TryRefresh()
		{
			if ( ValueProvider is null ) return false;

			var latest = ValueProvider();
			if ( latest == Text ) return false;

			Text = latest;
			return true;
		}
	}
}
