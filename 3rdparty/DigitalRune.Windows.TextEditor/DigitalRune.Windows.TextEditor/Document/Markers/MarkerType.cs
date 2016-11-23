namespace DigitalRune.Windows.TextEditor.Markers
{
  /// <summary>
  /// Defines the type of a <see cref="Marker"/>.
  /// </summary>
  public enum MarkerType
  {
    /// <summary>
    /// An invisible marker.
    /// </summary>
    Invisible,
    /// <summary>
    /// A solid colored block in the background of the text.
    /// </summary>
    SolidBlock,
    /// <summary>
    /// An underline.
    /// </summary>
    Underlined,
    /// <summary>
    /// A zigzag line below the text.
    /// </summary>
    WaveLine
  }
}