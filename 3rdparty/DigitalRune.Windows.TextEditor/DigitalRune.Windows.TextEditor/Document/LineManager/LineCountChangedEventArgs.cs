using System;


namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// Event arguments for events raised by an <see cref="LineManager"/>.
  /// </summary>
  public class LineCountChangedEventArgs : EventArgs
  {
    private readonly IDocument _document;
    private readonly int _startLine;
    private readonly int _difference;


    /// <summary>
    /// Always a valid Document which is related to the event.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return _document; }
    }


    /// <summary>
    /// Gets the start line (-1 if no offset was specified for this event).
    /// </summary>
    /// <value>The start line (-1 if no offset was specified for this event).</value>
    public int StartLine
    {
      get { return _startLine; }
    }


    /// <summary>
    /// Gets the lines difference (-1 if no length was specified for this event).
    /// </summary>
    /// <value>The lines difference (-1 if no length was specified for this event).</value>
    /// <remarks>
    /// The line difference is calculated as <i>number of lines after - number of lines before</i>.
    /// </remarks>
    public int LineDifference
    {
      get { return _difference; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="LineCountChangedEventArgs"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineStart">The line start.</param>
    /// <param name="linesMoved">The lines moved.</param>
    public LineCountChangedEventArgs(IDocument document, int lineStart, int linesMoved)
    {
      _document = document;
      _startLine = lineStart;
      _difference = linesMoved;
    }
  }
}