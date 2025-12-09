namespace MossEngine.Windowing.UI.Yoga;

public struct Padding
{
	public Length Left { readonly get; set; }
	public Length Right { readonly get; set; }
	public Length Top { readonly get; set; }
	public Length Bottom { readonly get; set; }
	public Length Start { readonly get; set; }
	public Length End { readonly get; set; }
	public Length Horizontal { readonly get; set; }
	public Length Vertical { readonly get; set; }
	public Length All { readonly get; set; }

	public Padding( Length value )
	{
		All = value;
	}
}
