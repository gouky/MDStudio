using System;


namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// Describes how a <see cref="TextAnchor"/> is moved when text is inserted.
  /// </summary>
  public enum AnchorMovementType
  {
    /// <summary>
    /// Behaves like a start marker - when text is inserted at the anchor position, the anchor will stay
    /// before the inserted text.
    /// </summary>
    BeforeInsertion,

    /// <summary>
    /// Behave like an end marker - when text is insered at the anchor position, the anchor will move
    /// after the inserted text.
    /// </summary>
    AfterInsertion
  }


  /// <summary>
  /// An anchor that can be put into a document and moves around when the document is changed.
  /// </summary>
  public sealed class TextAnchor
  {
    private LineSegment _lineSegment;
    private int _columnNumber;
    private AnchorMovementType _movementType;


    /// <summary>
    /// Controls how the anchor moves.
    /// </summary>
    public AnchorMovementType MovementType
    {
      get { return _movementType; }
      set { _movementType = value; }
    }


    /// <summary>
    /// Gets (or sets) the line.
    /// </summary>
    /// <value>The line.</value>
    public LineSegment Line
    {
      get
      {
        if (_lineSegment == null) 
          throw AnchorDeletedError();
        return _lineSegment;
      }
      internal set
      {
        _lineSegment = value;
      }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is deleted.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is deleted; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsDeleted
    {
      get { return _lineSegment == null; }
    }


    /// <summary>
    /// Gets the line number.
    /// </summary>
    /// <value>The line number.</value>
    public int LineNumber
    {
      get { return Line.LineNumber; }
    }


    /// <summary>
    /// Gets or sets the column number.
    /// </summary>
    /// <value>The column number.</value>
    public int ColumnNumber
    {
      get
      {
        if (_lineSegment == null) 
          throw AnchorDeletedError();
        return _columnNumber;
      }
      internal set
      {
        _columnNumber = value;
      }
    }


    /// <summary>
    /// Occurs when text containing the anchor is removed.
    /// </summary>
    public event EventHandler Deleted;


    /// <summary>
    /// Gets the location.
    /// </summary>
    /// <value>The location.</value>
    public TextLocation Location
    {
      get { return new TextLocation(ColumnNumber, LineNumber); }
    }


    /// <summary>
    /// Gets the offset.
    /// </summary>
    /// <value>The offset.</value>
    public int Offset
    {
      get { return Line.Offset + _columnNumber; }
    }


    internal TextAnchor(LineSegment lineSegment, int columnNumber)
    {
      _lineSegment = lineSegment;
      _columnNumber = columnNumber;
    }


    static Exception AnchorDeletedError()
    {
      return new InvalidOperationException("The text containing the anchor was deleted");
    }


    internal void Delete(ref DeferredEventList deferredEventList)
    {
      // we cannot fire an event here because this method is called while the LineManager adjusts the
      // lineCollection, so an event handler could see inconsistent state
      _lineSegment = null;
      deferredEventList.AddDeletedAnchor(this);
    }


    internal void RaiseDeleted()
    {
      if (Deleted != null)
        Deleted(this, EventArgs.Empty);
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="TextAnchor"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="TextAnchor"></see>.
    /// </returns>
    public override string ToString()
    {
      if (IsDeleted)
        return "[TextAnchor (deleted)]";
      else
        return "[TextAnchor " + Location + "]";
    }
  }
}
