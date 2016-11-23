using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Selection
{
  /// <summary>
  /// Representing a single selection.
  /// </summary>
  public interface ISelection
  {
    /// <summary>
    /// Gets or sets the start position of the selection.
    /// </summary>
    /// <value>The start position of the selection.</value>
    TextLocation StartPosition
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the end position of the selection.
    /// </summary>
    /// <value>The end position of the selection.</value>
    TextLocation EndPosition
    {
      get;
      set;
    }

    /// <summary>
    /// Gets the offset of the selection.
    /// </summary>
    /// <value>The offset of the selection.</value>
    int Offset { get; }

    /// <summary>
    /// Gets the end offset of the selection.
    /// </summary>
    /// <value>The end offset of the selection.</value>
    int EndOffset { get; }

    /// <summary>
    /// Gets the length of the selection.
    /// </summary>
    /// <value>The length of the selection.</value>
    int Length { get; }

    /// <summary>
    /// Gets a value indicating whether this is a rectangular selection.
    /// </summary>
    /// <value>Returns true, if the selection is rectangular</value>
    bool IsRectangularSelection { get; }

    /// <summary>
    /// Gets a value indicating whether this selection is empty.
    /// </summary>
    /// <value>Returns <see langword="true"/>, if the selection is empty.</value>
    bool IsEmpty { get; }

    /// <summary>
    /// Gets the selected text.
    /// </summary>
    /// <value>The text which is selected by this selection.</value>
    string SelectedText { get; }

    /// <summary>
    /// Determines whether this selection contains the specified offset.
    /// </summary>
    /// <param name="offset">The specified offset.</param>
    /// <returns>
    /// <see langword="true"/> if this selection contains the specified offset; otherwise, 
    /// <see langword="false"/>.
    /// </returns>
    bool ContainsOffset(int offset);

    /// <summary>
    /// Determines whether this selection contains the specified position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>
    /// <see langword="true"/> if this selection contains the specified position; otherwise, <see langword="false"/>.
    /// </returns>
    bool ContainsPosition(TextLocation position);
  }
}
