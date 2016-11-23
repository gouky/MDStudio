using System.Collections.Generic;
using System.Drawing;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// Contains brushes/pens for the text editor to speed up drawing. Re-Creation of brushes and pens
  /// seems too costly.
  /// </summary>
  /// <remarks>
  /// All brushes and pens are cached internally. When a new brush is requested
  /// the <see cref="BrushRegistry"/> looks up the internal cache and returns the
  /// appropriate brush object. If the brush is not found in the cache, then a
  /// new brush is created and automatically cached.
  /// </remarks>
  internal static class BrushRegistry
  {
    private static readonly Dictionary<Color, Brush> _brushes = new Dictionary<Color, Brush>();
    private static readonly Dictionary<Color, Pen> _pens = new Dictionary<Color, Pen>();
    private static readonly Dictionary<Color, Pen> _dotPens = new Dictionary<Color, Pen>();
    private static readonly float[] _dotPattern = { 1, 1, 1, 1 };


    /// <summary>
    /// Gets a brush with a certain color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The brush with the specified color.</returns>
    public static Brush GetBrush(Color color)
    {
      lock (_brushes)
      {
        Brush brush;
        if (!_brushes.TryGetValue(color, out brush))
        {
          brush = new SolidBrush(color);
          _brushes.Add(color, brush);
        }
        return brush;
      }
    }


    /// <summary>
    /// Gets a pen with a certain color.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The pen with the specified color.</returns>
    public static Pen GetPen(Color color)
    {
      lock (_pens)
      {
        Pen pen;
        if (!_pens.TryGetValue(color, out pen))
        {
          pen = new Pen(color);
          _pens.Add(color, pen);
        }
        return pen;
      }
    }


    /// <summary>
    /// Gets a pen for drawing dotted lines.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>A pen for drawing dotted lines.</returns>
    public static Pen GetDotPen(Color color)
    {
      lock (_dotPens)
      {
        Pen pen;
        if (!_dotPens.TryGetValue(color, out pen))
        {
          pen = new Pen(color);
          pen.DashPattern = _dotPattern;
          _dotPens.Add(color, pen);
        }
        return pen;
      }
    }
  }
}
