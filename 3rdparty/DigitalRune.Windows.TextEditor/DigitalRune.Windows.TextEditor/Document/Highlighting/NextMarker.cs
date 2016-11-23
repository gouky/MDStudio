using System;
using System.Xml;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Used to mark next token.
  /// </summary>
  public class NextMarker
  {
    private readonly string _what;
    private readonly HighlightColor _color;
    private readonly bool _markMarker;


    /// <summary>
    /// Gets the string that indicates that the next token should be marked.
    /// </summary>
    /// <value>The string that indicates that the next token should be marked.</value>
    public string What
    {
      get { return _what; }
    }


    /// <summary>
    /// Gets the highlighting color for the next token.
    /// </summary>
    /// <value>Color for marking next token.</value>
    public HighlightColor Color
    {
      get { return _color; }
    }


    /// <summary>
    /// Gets a value indicating whether indication text (<see cref="What"/>) should 
    /// marked with the same color.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the indication text (<see cref="What"/>) should 
    /// marked with the same color.
    /// </value>
    public bool MarkMarker
    {
      get { return _markMarker; }
    }


    /// <summary>
    /// Creates a new instance of <see cref="NextMarker"/>
    /// </summary>
    /// <param name="mark">The XML element that defines this <see cref="NextMarker"/>.</param>
    public NextMarker(XmlElement mark)
    {
      _color = new HighlightColor(mark);
      _what = mark.InnerText;
      if (mark.Attributes["markmarker"] != null)
        _markMarker = Boolean.Parse(mark.Attributes["markmarker"].InnerText);
    }
  }

}
