using System.Numerics;
using MossEngine.Windowing.UI.Yoga;
using SkiaSharp;

namespace MossEngine.Windowing.UI.Components;

public class TextField : Panel
{
	private readonly BaseInputText _inputText = new();
	
	public string Text
	{
		get => _inputText.Text;
		set => _inputText.Text = value;
	}
	
	public SKColor Foreground
	{
		get => _inputText.Foreground;
		set => _inputText.Foreground = value;
	}
	
	public SKColor PlaceholderColor
	{
		get => _inputText.PlaceholderColor;
		set => _inputText.PlaceholderColor = value;
	}

	public SKColor SelectionColor
	{
		get => _inputText.SelectionColor;
		set => _inputText.SelectionColor = value;
	}
	
	public SKColor CursorColor
	{
		get => _inputText.CursorColor;
		set => _inputText.CursorColor = value;
	}
	
	public string Placeholder
	{
		get => _inputText.Placeholder;
		set => _inputText.Placeholder = value;
	}
	
	public float FontSize
	{
		get => _inputText.FontSize;
		set => _inputText.FontSize = value;
	}
	
	public string FontFamily
	{
		get => _inputText.FontFamily;
		set => _inputText.FontFamily = value;
	}
	
	public int MaxLength
	{
		get => _inputText.MaxLength;
		set => _inputText.MaxLength = value;
	}
	
	public bool IsPassword
	{
		get => _inputText.IsPassword;
		set => _inputText.IsPassword = value;
	}
	
	public bool IsReadOnly
	{
		get => _inputText.IsReadOnly;
		set => _inputText.IsReadOnly = value;
	}

	public TextField()
	{
		Width = Length.Percent( 100 );
		Height = Length.Point( 32 );
		Background = SKColors.White;
		BorderRadius = new Vector2( 4 );
		StrokeWidth = 1;
		Padding = new Padding( 4 );
		
		_inputText.Width = Length.Percent( 100 );
		_inputText.Height = Length.Percent( 100 );
		_inputText.Foreground = SKColors.Black;
		_inputText.CursorColor = SKColors.Black;
		
		AddChild( _inputText );
	}
}
