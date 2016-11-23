using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Highlighting;


namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// Describes a line of a document.
  /// </summary>
  public sealed class LineSegment : ISegment
  {
    internal LineSegmentTree.Enumerator _treeEntry;
    private int _totalLength;
    private int _delimiterLength;
    private List<TextWord> _words;
    private SpanStack _highlightSpanStack;


    /// <summary>
    /// Gets the word at a certain column.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>The word at the specified column.</returns>
    public TextWord GetWord(int column)
    {
      int curColumn = 0;
      foreach (TextWord word in _words)
      {
        if (column < curColumn + word.Length)
          return word;

        curColumn += word.Length;
      }
      return null;
    }


    /// <summary>
    /// Gets a value indicating whether this segment has been deleted.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is deleted; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsDeleted
    {
      get { return !_treeEntry.IsValid; }
    }


    /// <summary>
    /// Gets the line number.
    /// </summary>
    /// <value>The line number.</value>
    public int LineNumber
    {
      get { return _treeEntry.CurrentIndex; }
    }


    /// <summary>
    /// Gets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    public int Offset
    {
      get { return _treeEntry.CurrentOffset; }
    }


    /// <summary>
    /// Gets the length of the line (without newline delimiter).
    /// </summary>
    /// <value>The length of the line (without the newline delimiter).</value>
    public int Length
    {
      get { return _totalLength - _delimiterLength; }
    }


    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    int ISegment.Offset
    {
      get { return Offset; }
      set { throw new NotSupportedException(); }
    }


    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length of the span.</value>
    int ISegment.Length
    {
      get { return Length; }
      set { throw new NotSupportedException(); }
    }


    /// <summary>
    /// Gets (or sets) the length of the line (including newline delimiter).
    /// </summary>
    /// <value>The length of the line (include newline delimiter).</value>
    public int TotalLength
    {
      get { return _totalLength; }
      internal set { _totalLength = value; }
    }


    /// <summary>
    /// Gets (or sets) the length of the newline delimiter.
    /// </summary>
    /// <value>The length of the newline delimiter.</value>
    public int DelimiterLength
    {
      get { return _delimiterLength; }
      internal set { _delimiterLength = value; }
    }


    /// <summary>
    /// Gets or sets the words in the line.
    /// </summary>
    /// <value>The words in this line.</value>
    public List<TextWord> Words
    {
      get { return _words; }
      set { _words = value; }
    }


    /// <summary>
    /// Gets the highlight color for a position.
    /// </summary>
    /// <param name="x">The position (column).</param>
    /// <returns></returns>
    public HighlightColor GetColorForPosition(int x)
    {
      if (Words != null)
      {
        int xPos = 0;
        foreach (TextWord word in Words)
        {
          if (x < xPos + word.Length)
            return word.SyntaxColor;
          
          xPos += word.Length;
        }
      }
      return new HighlightColor(Color.Black, false, false);
    }


    /// <summary>
    /// Gets or sets the spans in the line with the same highlighting.
    /// </summary>
    /// <value>The spans with same highlighting.</value>
    public SpanStack HighlightSpanStack
    {
      get { return _highlightSpanStack; }
      set { _highlightSpanStack = value; }
    }


    /// <summary>
    /// Converts a <see cref="LineSegment"/> instance to string (for debug purposes)
    /// </summary>
    public override string ToString()
    {
      if (IsDeleted)
        return "[LineSegment: (deleted) Length = " + Length + ", TotalLength = " + TotalLength + ", DelimiterLength = " + _delimiterLength + "]";
      else
        return "[LineSegment: LineNumber=" + LineNumber + ", Offset = " + Offset + ", Length = " + Length + ", TotalLength = " + TotalLength + ", DelimiterLength = " + _delimiterLength + "]";
    }


    #region ----- Anchor management -----
    private Utilities.WeakCollection<TextAnchor> _anchors;


    /// <summary>
    /// Creates a new anchor in the current line.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <returns>The <see cref="TextAnchor"/>.</returns>
    public TextAnchor CreateAnchor(int column)
    {
      if (column < 0 || column > Length)
        throw new ArgumentOutOfRangeException("column");

      TextAnchor anchor = new TextAnchor(this, column);
      AddAnchor(anchor);
      return anchor;
    }


    void AddAnchor(TextAnchor anchor)
    {
      Debug.Assert(anchor.Line == this);

      if (_anchors == null)
        _anchors = new Utilities.WeakCollection<TextAnchor>();

      _anchors.Add(anchor);
    }


    /// <summary>
    /// Is called when the <see cref="LineSegment"/> is deleted.
    /// </summary>
    /// <param name="deferredEventList">The deferred event list.</param>
    internal void Deleted(ref DeferredEventList deferredEventList)
    {
      //Console.WriteLine("Deleted");
      _treeEntry = LineSegmentTree.Enumerator.Invalid;
      if (_anchors != null)
      {
        foreach (TextAnchor anchor in _anchors)
        {
          anchor.Delete(ref deferredEventList);
        }
        _anchors = null;
      }
    }


    /// <summary>
    /// Is called when a part of the line is removed.
    /// </summary>
    internal void RemovedLinePart(ref DeferredEventList deferredEventList, int startColumn, int length)
    {
      if (length == 0)
        return;
      Debug.Assert(length > 0);

      if (_anchors != null)
      {
        List<TextAnchor> deletedAnchors = null;
        foreach (TextAnchor anchor in _anchors)
        {
          if (anchor.ColumnNumber > startColumn)
          {
            if (anchor.ColumnNumber >= startColumn + length)
            {
              anchor.ColumnNumber -= length;
            }
            else
            {
              if (deletedAnchors == null)
                deletedAnchors = new List<TextAnchor>();
              anchor.Delete(ref deferredEventList);
              deletedAnchors.Add(anchor);
            }
          }
        }
        if (deletedAnchors != null)
        {
          foreach (TextAnchor anchor in deletedAnchors)
          {
            _anchors.Remove(anchor);
          }
        }
      }
    }


    /// <summary>
    /// Is called when a part of the line is inserted.
    /// </summary>
    internal void InsertedLinePart(int startColumn, int length)
    {
      if (length == 0)
        return;
      Debug.Assert(length > 0);

      //Console.WriteLine("InsertedLinePart " + startColumn + ", " + length);
      if (_anchors != null)
      {
        foreach (TextAnchor anchor in _anchors)
        {
          if (anchor.MovementType == AnchorMovementType.BeforeInsertion
              ? anchor.ColumnNumber > startColumn
              : anchor.ColumnNumber >= startColumn)
          {
            anchor.ColumnNumber += length;
          }
        }
      }
    }


    /// <summary>
    /// Merges the <see cref="TextAnchor"/>s of two lines, if a newline is deleted.
    /// </summary>
    /// <param name="deletedLine">The deleted line.</param>
    /// <param name="firstLineLength">The length of the first line.</param>
    internal void MergedWith(LineSegment deletedLine, int firstLineLength)
    {
      // Is called after another line's content is appended to this line because the newline in between
      // was deleted.
      // The DefaultLineManager will call Deleted() on the deletedLine after the MergedWith call.
      // firstLineLength: the length of the line before the merge.
      
      if (deletedLine._anchors != null)
      {
        foreach (TextAnchor anchor in deletedLine._anchors)
        {
          anchor.Line = this;
          AddAnchor(anchor);
          anchor.ColumnNumber += firstLineLength;
        }
        deletedLine._anchors = null;
      }
    }


    /// <summary>
    /// Splits the <see cref="TextAnchor"/>s of a line if a newline is inserted.
    /// </summary>
    /// <param name="followingLine">The following line.</param>
    internal void SplitTo(LineSegment followingLine)
    {
      // Is called after a newline was inserted into this line, splitting it into this and followingLine.

      if (_anchors != null)
      {
        List<TextAnchor> movedAnchors = null;
        foreach (TextAnchor anchor in _anchors)
        {
          if (anchor.MovementType == AnchorMovementType.BeforeInsertion
              ? anchor.ColumnNumber > Length
              : anchor.ColumnNumber >= Length)
          {
            anchor.Line = followingLine;
            followingLine.AddAnchor(anchor);
            anchor.ColumnNumber -= Length;

            if (movedAnchors == null)
              movedAnchors = new List<TextAnchor>();
            movedAnchors.Add(anchor);
          }
        }

        if (movedAnchors != null)
          foreach (TextAnchor a in movedAnchors)
            _anchors.Remove(a);
      }
    }
    #endregion
  }
}
