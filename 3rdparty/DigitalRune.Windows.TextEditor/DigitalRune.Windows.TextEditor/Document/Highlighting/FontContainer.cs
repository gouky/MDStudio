using System;
using System.Drawing;

namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// This class is used to generate bold, italic and bold/italic fonts out
  /// of a base font.
  /// </summary>
  public class FontContainer
  {
    private static float _twipsPerPixelY;
    private Font _defaultFont;
    private Font _regularFont;
    private Font _boldFont;
    private Font _italicFont;
    private Font _boldItalicFont;


    /// <summary>
    /// Gets the regular font.
    /// </summary>
    /// <value>The regular version of the base font.</value>
    public Font RegularFont
    {
      get { return _regularFont; }
    }


    /// <summary>
    /// Gets the bold font.
    /// </summary>
    /// <value>The scaled, bold version of the base font</value>
    public Font BoldFont
    {
      get { return _boldFont; }
    }


    /// <summary>
    /// Gets the italic font.
    /// </summary>
    /// <value>The scaled, italic version of the base font</value>
    public Font ItalicFont
    {
      get { return _italicFont; }
    }


    /// <summary>
    /// Gets the bold/italic font.
    /// </summary>
    /// <value>The scaled, bold/italic version of the base font</value>
    public Font BoldItalicFont
    {
      get { return _boldItalicFont; }
    }


    /// <summary>
    /// Gets the twips per pixel in y direction.
    /// </summary>
    /// <value>The twips per pixel in y direction.</value>
    public static float TwipsPerPixelY
    {
      get
      {
        if (_twipsPerPixelY == 0)
          using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
              _twipsPerPixelY = 1440 / g.DpiY;

        return _twipsPerPixelY;
      }
    }


    /// <summary>
    /// Gets or sets the default font.
    /// </summary>
    /// <value>The base font.</value>
    public Font DefaultFont
    {
      get { return _defaultFont; }
      set
      {
        // 1440 twips is one inch
        float pixelSize = (float) Math.Round(value.SizeInPoints * 20 / TwipsPerPixelY);

        _defaultFont = value;
        _regularFont = new Font(value.FontFamily, pixelSize * TwipsPerPixelY / 20f, FontStyle.Regular);
        _boldFont = new Font(_regularFont, FontStyle.Bold);
        _italicFont = new Font(_regularFont, FontStyle.Italic);
        _boldItalicFont = new Font(_regularFont, FontStyle.Bold | FontStyle.Italic);
      }
    }


    /// <summary>
    /// Converts a string to <see cref="Font"/> object.
    /// </summary>
    /// <param name="font">The font.</param>
    /// <returns>The font.</returns>
    public static Font ParseFont(string font)
    {
      string[] descr = font.Split(new char[] { ',', '=' });
      return new Font(descr[1], Single.Parse(descr[3]));
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FontContainer"/> class.
    /// </summary>
    /// <param name="defaultFont">The default font.</param>
    public FontContainer(Font defaultFont)
    {
      DefaultFont = defaultFont;
    }
  }
}
