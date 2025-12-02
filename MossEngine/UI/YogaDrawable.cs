using AngleSharp;
using AngleSharp.Dom;
using SkiaSharp;
using Yoga;
using Node = Yoga.Node;
using NodeType = Yoga.NodeType;

namespace MossEngine.UI;

public class YogaDrawable : Node
{
	public SKPaint Paint = new();
	public string Text = "";
}

public static class Extensions
{
	public static void Draw( this YogaDrawable node, SKCanvas canvas, float offsetX, float offsetY )
	{
		canvas.DrawRect( new SKRect( node.Left + offsetX, node.Top + offsetY, node.Width, node.Height ), node.Paint );

		foreach ( var node1 in node.Children )
		{
			var child = (YogaDrawable)node1;

			if ( child.Type is NodeType.Default )
				child.Draw( canvas, offsetX + node.Left, offsetY + node.Top );
			else
				canvas.DrawText( child.Text, offsetX + node.Left, offsetY + node.Top, child.Paint );
		}
	}
}

public sealed class HtmlRenderer
{
	private readonly IBrowsingContext _context;
	private YogaDrawable _yogaRoot = null!;

	public readonly IConfiguration Config = Configuration.Default;
	public IDocument Document = null!;

	public HtmlRenderer()
	{
		_context = BrowsingContext.New( Config );
	}

	public void LoadInlineHtml( string htmlString )
	{
		Document = _context.OpenAsync( req => req.Content( htmlString ) ).Result;

		// _yogaRoot = new YogaDrawable
		// {
		// 	Width = GraphicsReferences.ScreenSize.Width, Height = GraphicsReferences.ScreenSize.Height
		// };

		foreach ( var element in Document.Children )
		{
			ConstructYogaDom( element );
		}

		_yogaRoot.CalculateLayout();
		_yogaRoot.Print( PrintOptions.Style );
	}

	private void ConstructYogaDom( IElement element, YogaDrawable? parent = null )
	{
		var node = new YogaDrawable();
		Console.WriteLine( $"Processing element {element}" );

		parent ??= _yogaRoot;
		parent.Children.Add( node );

		var nodeStyle = element.ComputeCurrentStyle();
		Console.WriteLine( $"Node style {nodeStyle}" );

		if ( element.HasTextNodes() )
		{
			var textNode = new YogaDrawable { Type = NodeType.Text, Text = element.Text() };
			node.Children.Add( textNode );
		}

		foreach ( var elementChild in element.Children )
		{
			ConstructYogaDom( elementChild, node );
		}
	}

	public void SkiaDraw( SKCanvas canvas )
	{
		_yogaRoot.Draw( canvas, 0, 0 );
	}
}
