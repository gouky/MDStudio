namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// An entry for a code completion window.
  /// </summary>
  public interface ICompletionData
  {
    /// <summary>
    /// Gets the index of the image.
    /// </summary>
    /// <value>The index of the image.</value>
    int ImageIndex { get; }


    /// <summary>
    /// Gets or sets the completion text.
    /// </summary>
    /// <value>The completion text.</value>
    string Text { get; }


    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    string Description { get; }


    /// <summary>
    /// Gets a priority value for the completion data item.
    /// </summary>
    /// <remarks>
    /// When selecting items by their start characters, the item with the highest
    /// priority is selected first.
    /// </remarks>
    double Priority { get; }


    /// <summary>
    /// Insert the element represented by the completion data into the text
    /// editor.
    /// </summary>
    /// <param name="textArea">
    /// TextArea to insert the completion data in.
    /// </param>
    /// <param name="ch">
    /// Character that should be inserted after the completion data. <c>'\0'</c>
    /// when no character should be inserted.
    /// </param>
    /// <returns>
    /// Returns true when the insert action has processed the character
    /// <paramref name="ch"/>; false when the character was not processed.
    /// </returns>
    bool InsertAction(TextArea textArea, char ch);
  }
}
