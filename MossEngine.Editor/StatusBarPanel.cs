using MossEngine.Windowing.UI;
using MossEngine.Windowing.UI.Components;
using MossEngine.Windowing.UI.Yoga;
using SkiaSharp;

namespace MossEngine.Editor;

public sealed class StatusBarPanel : Panel
{
	private readonly Panel _left;
	private readonly Panel _right;
	private readonly Dictionary<Guid, Text> _bindings = new();

	private static StatusBar StatusBar => Editor.StatusBar;

	public StatusBarPanel()
	{
		Width = Length.Percent( 100 );
		Height = Length.Point( 32 );
		// Flex = 1;
		// FlexShrink = 0;
		Padding = new Padding { Horizontal = Length.Point( 8 ) };
		AlignItems = YogaAlign.Center;
		// GapColumn = 8;
		FlexDirection = YogaFlexDirection.Row;

		_left = new Panel
		{
			Flex = 1,
			FlexDirection = YogaFlexDirection.Row,
			GapColumn = 8,
			AlignItems = YogaAlign.Center
		};
		_right = new Panel
		{
			Flex = 1,
			FlexDirection = YogaFlexDirection.Row,
			GapColumn = 8,
			AlignItems = YogaAlign.Center,
			JustifyContent = YogaJustify.FlexEnd
		};

		AddChild( _left );
		AddChild( _right );

		StatusBar.ItemsChanged += Rebuild;
		Rebuild();
	}

	~StatusBarPanel()
	{
		StatusBar.ItemsChanged -= Rebuild;
	}

	protected override void OnUpdate()
	{
		base.OnUpdate();

		StatusBar.RefreshDynamicItems();

		foreach ( var (id, text) in _bindings )
		{
			if ( !StatusBar.TryGetItem( id, out var item ) || item is null )
				continue;

			if ( text.Value != item.Text )
			{
				text.Value = item.Text;
			}
		}
	}

	private void Rebuild()
	{
		_left.ClearChildren();
		_right.ClearChildren();
		_bindings.Clear();

		foreach ( var item in StatusBar.GetItems() )
		{
			var text = CreateText( item );
			_bindings[item.Id] = text;

			if ( item.Section is StatusBar.StatusBarSection.Left )
			{
				_left.AddChild( text );
			}
			else
			{
				_right.AddChild( text );
			}
		}
	}

	private Text CreateText( StatusBar.StatusBarItem item )
	{
		return new Text
		{
			Value = item.Text,
			FontSize = 12,
			Foreground = SKColors.White,
			Position = YogaPositionType.Relative
		};
	}
}
