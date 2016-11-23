using System;


namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// Event arguments for events raised by an <see cref="LineManager"/>.
  /// </summary>
  public class LineEventArgs : EventArgs
  {
    private readonly IDocument _document;
    private readonly LineSegment _lineSegment;


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return _document; }
    }


    /// <summary>
    /// Gets the line segment.
    /// </summary>
    /// <value>The line segment.</value>
    public LineSegment LineSegment
    {
      get { return _lineSegment; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="LineEventArgs"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineSegment">The line segment.</param>
    public LineEventArgs(IDocument document, LineSegment lineSegment)
    {
      _document = document;
      _lineSegment = lineSegment;
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return string.Format("[LineEventArgs Document={0} LineSegment={1}]", _document, _lineSegment);
    }
  }
}