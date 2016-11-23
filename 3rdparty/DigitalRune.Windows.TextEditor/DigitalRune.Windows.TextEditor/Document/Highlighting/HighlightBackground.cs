using System.Drawing;
using System.Xml;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Extends the highlighting color with a background image.
  /// </summary>
  public class HighlightBackground : HighlightColor
  {
    private readonly Image _backgroundImage;


    /// <summary>
    /// Gets the background image.
    /// </summary>
    /// <value>The image used as background.</value>
    public Image BackgroundImage
    {
      get { return _backgroundImage; }
    }


    /// <summary>
    /// Creates a new instance of <see cref="HighlightBackground"/>
    /// </summary>
    /// <param name="el">The XML element that describes the highlighting color.</param>
    public HighlightBackground(XmlElement el) : base(el)
    {
      if (el.Attributes["image"] != null)
        _backgroundImage = new Bitmap(el.Attributes["image"].InnerText);
    }


    /// <summary>
    /// Creates a new instance of <see cref="HighlightBackground"/>
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="backgroundcolor">The backgroundcolor.</param>
    /// <param name="bold">if set to <see langword="true"/> [bold].</param>
    /// <param name="italic">if set to <see langword="true"/> [italic].</param>
    public HighlightBackground(Color color, Color backgroundcolor, bool bold, bool italic)
      : base(color, backgroundcolor, bold, italic)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightBackground"/> class.
    /// </summary>
    /// <param name="systemColor">Color of the system.</param>
    /// <param name="systemBackgroundColor">Color of the system background.</param>
    /// <param name="bold">if set to <see langword="true"/> [bold].</param>
    /// <param name="italic">if set to <see langword="true"/> [italic].</param>
    public HighlightBackground(string systemColor, string systemBackgroundColor, bool bold, bool italic)
      : base(systemColor, systemBackgroundColor, bold, italic)
    {
    }
  }
}
