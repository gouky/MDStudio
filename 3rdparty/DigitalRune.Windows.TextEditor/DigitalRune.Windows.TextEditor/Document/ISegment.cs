namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// This interface is used to describe a segment (span) inside a text sequence.
  /// </summary>
  public interface ISegment
  {
    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    int Offset
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length of the span</value>
    int Length
    {
      get;
      set;
    }
  }
}
