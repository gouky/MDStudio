using System;

namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// Event arguments for events raised when the length of lines changes.
  /// </summary>
	public class LineLengthChangedEventArgs : LineEventArgs
	{
    private readonly int _difference;


    /// <summary>
    /// Gets the length delta.
    /// </summary>
    /// <value>The length delta.</value>
		public int LengthDifference 
    {
			get { return _difference; }
		}
		

    /// <summary>
    /// Initializes a new instance of the <see cref="LineLengthChangedEventArgs"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineSegment">The line segment.</param>
    /// <param name="difference">The difference (new line length - old line length).</param>
    public LineLengthChangedEventArgs(IDocument document, LineSegment lineSegment, int difference)
			: base(document, lineSegment)
		{
      _difference = difference;
		}
		

    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
		public override string ToString()
		{
			return string.Format("[LineLengthEventArgs Document={0} LineSegment={1} LengthDifference={2}]", Document, LineSegment, _difference);
		}
  }
}
