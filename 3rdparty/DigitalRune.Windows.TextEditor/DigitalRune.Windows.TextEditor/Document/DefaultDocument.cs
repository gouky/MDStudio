using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Undo;
using DigitalRune.Windows.TextEditor.Bookmarks;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Formatting;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Markers;
using DigitalRune.Windows.TextEditor.TextBuffer;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// The default <see cref="IDocument"/> implementation.
  /// </summary>
  internal class DefaultDocument : IDocument
  {
    private bool _readOnly;
    private LineManager _lineTrackingStrategy;
    private BookmarkManager _bookmarkManager;
    private ITextBufferStrategy _textBufferStrategy;
    private IFormattingStrategy _formattingStrategy;
    private FoldingManager _foldingManager;
    private readonly UndoStack _undoStack = new UndoStack();
    private ITextEditorProperties _textEditorProperties = new DefaultTextEditorProperties();
    private MarkerStrategy _markerStrategy;
    private readonly List<TextAreaUpdate> _updateQueue = new List<TextAreaUpdate>();


    /// <summary>
    /// Gets or sets the line manager.
    /// </summary>
    /// <value>The line manager.</value>
    public LineManager LineManager
    {
      get { return _lineTrackingStrategy; }
      set { _lineTrackingStrategy = value; }
    }


    /// <summary>
    /// Occurs when a document has been changed.
    /// </summary>
    public event EventHandler<DocumentEventArgs> DocumentChanged;


    /// <summary>
    /// Is fired when CommitUpdate is called
    /// </summary>
    public event EventHandler UpdateCommited;


    /// <summary>
    /// Occurs when the text content has changed.
    /// </summary>
    public event EventHandler TextContentChanged;


    /// <summary>
    /// Occurs when length of a line changes.
    /// </summary>
    public event EventHandler<LineLengthChangedEventArgs> LineLengthChanged
    {
      add { _lineTrackingStrategy.LineLengthChanged += value; }
      remove { _lineTrackingStrategy.LineLengthChanged -= value; }
    }


    /// <summary>
    /// Occurs when number of lines changes.
    /// </summary>
    public event EventHandler<LineCountChangedEventArgs> LineCountChanged
    {
      add { _lineTrackingStrategy.LineCountChanged += value; }
      remove { _lineTrackingStrategy.LineCountChanged -= value; }
    }


    /// <summary>
    /// Occurs when a line is deleted.
    /// </summary>
    public event EventHandler<LineEventArgs> LineDeleted
    {
      add { _lineTrackingStrategy.LineDeleted += value; }
      remove { _lineTrackingStrategy.LineDeleted -= value; }
    }


    /// <summary>
    /// Gets the marker strategy.
    /// </summary>
    /// <value>The marker strategy.</value>
    public MarkerStrategy MarkerStrategy
    {
      get { return _markerStrategy; }
      set { _markerStrategy = value; }
    }


    /// <summary>
    /// Gets or sets the text editor properties.
    /// </summary>
    /// <value>The text editor properties.</value>
    public ITextEditorProperties TextEditorProperties
    {
      get { return _textEditorProperties; }
      set { _textEditorProperties = value; }
    }


    /// <summary>
    /// Gets the undo stack.
    /// </summary>
    /// <value>The undo stack.</value>
    public UndoStack UndoStack
    {
      get { return _undoStack; }
    }


    /// <summary>
    /// Gets the line segment collection.
    /// </summary>
    /// <value>A collection of all line segments</value>
    /// <remarks>
    /// The collection should only be used if you're aware
    /// of the 'last line ends with a delimiter problem'. Otherwise
    /// the <see cref="GetLineSegment"/> method should be used.
    /// </remarks>
    public IList<LineSegment> LineSegmentCollection
    {
      get { return _lineTrackingStrategy.LineSegments; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the document is read-only.
    /// </summary>
    /// <value>If <see langword="true"/> the document can't be altered</value>
    public bool ReadOnly
    {
      get { return _readOnly; }
      set { _readOnly = value; }
    }


    /// <summary>
    /// The <see cref="ITextBufferStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The text buffer strategy.</value>
    public ITextBufferStrategy TextBufferStrategy
    {
      get { return _textBufferStrategy; }
      set { _textBufferStrategy = value; }
    }


    /// <summary>
    /// The <see cref="IFormattingStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The formatting strategy.</value>
    public IFormattingStrategy FormattingStrategy
    {
      get { return _formattingStrategy; }
      set { _formattingStrategy = value; }
    }


    /// <summary>
    /// The <see cref="FoldingManager"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The folding manager.</value>
    public FoldingManager FoldingManager
    {
      get { return _foldingManager; }
      set { _foldingManager = value; }
    }


    /// <summary>
    /// The <see cref="IHighlightingStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The highlighting strategy.</value>
    public IHighlightingStrategy HighlightingStrategy
    {
      get { return _lineTrackingStrategy.HighlightingStrategy; }
      set { _lineTrackingStrategy.HighlightingStrategy = value; }
    }


    /// <summary>
    /// Gets the length of the text.
    /// </summary>
    /// <value>
    /// The current length of the sequence of characters that can be edited.
    /// </value>
    public int TextLength
    {
      get { return _textBufferStrategy.Length; }
    }


    /// <summary>
    /// The <see cref="BookmarkManager"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The bookmark manager.</value>
    public BookmarkManager BookmarkManager
    {
      get { return _bookmarkManager; }
      set { _bookmarkManager = value; }
    }


    /// <summary>
    /// Gets or sets the whole text as string.
    /// </summary>
    /// <value>The whole text as string.</value>
    /// <remarks>
    /// When setting the text using the TextContent property, the undo stack is cleared.
    /// Set TextContent only for actions such as loading a file; if you want to change the current document
    /// use the Replace method instead.
    /// </remarks>
    public string TextContent
    {
      get { return GetText(0, _textBufferStrategy.Length); }
      set
      {
        Debug.Assert(_textBufferStrategy != null);
        Debug.Assert(_lineTrackingStrategy != null);

        OnDocumentAboutToBeChanged(new DocumentEventArgs(this, 0, 0, value));

        _textBufferStrategy.SetContent(value);
        _lineTrackingStrategy.SetContent(value);
        _undoStack.ClearAll();

        OnDocumentChanged(new DocumentEventArgs(this, 0, 0, value));
        OnTextContentChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Gets the total number of lines.
    /// </summary>
    /// <value>
    /// The total number of lines, this may be != <c>LineSegments.Count</c>
    /// if the last line ends with a delimiter.
    /// </value>
    public int TotalNumberOfLines
    {
      get { return _lineTrackingStrategy.TotalNumberOfLines; }
    }


    /// <summary>
    /// Gets the update queue.
    /// </summary>
    /// <value>A container where all TextAreaUpdate objects get stored</value>
    public List<TextAreaUpdate> UpdateQueue
    {
      get { return _updateQueue; }
    }


    /// <summary>
    /// Occurs when a document is about to be changed.
    /// </summary>
    public event EventHandler<DocumentEventArgs> DocumentAboutToBeChanged;


    /// <summary>
    /// Moves, Resizes, Removes a list of segments on insert/remove/replace events.
    /// </summary>
    /// <typeparam name="T">A type of segment.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    public void UpdateSegmentListOnDocumentChange<T>(List<T> list, DocumentEventArgs e) where T : ISegment
    {
      int removedCharacters = e.Length > 0 ? e.Length : 0;
      int insertedCharacters = e.Text != null ? e.Text.Length : 0;
      for (int i = 0; i < list.Count; ++i)
      {
        ISegment s = list[i];
        int segmentStart = s.Offset;
        int segmentEnd = s.Offset + s.Length;

        if (e.Offset <= segmentStart)
        {
          segmentStart -= removedCharacters;
          if (segmentStart < e.Offset)
            segmentStart = e.Offset;
        }
        if (e.Offset < segmentEnd)
        {
          segmentEnd -= removedCharacters;
          if (segmentEnd < e.Offset)
            segmentEnd = e.Offset;
        }

        Debug.Assert(segmentStart <= segmentEnd);

        if (segmentStart == segmentEnd)
        {
          list.RemoveAt(i);
          --i;
          continue;
        }

        if (e.Offset <= segmentStart)
          segmentStart += insertedCharacters;
        if (e.Offset < segmentEnd)
          segmentEnd += insertedCharacters;

        Debug.Assert(segmentStart < segmentEnd);

        s.Offset = segmentStart;
        s.Length = segmentEnd - segmentStart;
      }
    }


    /// <summary>
    /// Inserts a string of characters into the sequence.
    /// </summary>
    /// <param name="offset">Offset where to insert the string.</param>
    /// <param name="text">Text to be inserted.</param>
    public void Insert(int offset, string text)
    {
      if (_readOnly)
        return;

      OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, -1, text));
      _textBufferStrategy.Insert(offset, text);
      _lineTrackingStrategy.Insert(offset, text);
      _undoStack.Push(new UndoableInsert(this, offset, text));
      OnDocumentChanged(new DocumentEventArgs(this, offset, -1, text));
    }


    /// <summary>
    /// Removes some portion of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the remove.</param>
    /// <param name="length">Number of characters to remove.</param>
    public void Remove(int offset, int length)
    {
      if (_readOnly)
        return;

      OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, length));
      _undoStack.Push(new UndoableDelete(this, offset, GetText(offset, length)));

      _textBufferStrategy.Remove(offset, length);
      _lineTrackingStrategy.Remove(offset, length);

      OnDocumentChanged(new DocumentEventArgs(this, offset, length));
    }


    /// <summary>
    /// Replace some portion of the sequence.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The number of characters to replace.</param>
    /// <param name="text">The text to be replaced with.</param>
    public void Replace(int offset, int length, string text)
    {
      if (_readOnly)
        return;

      OnDocumentAboutToBeChanged(new DocumentEventArgs(this, offset, length, text));
      _undoStack.Push(new UndoableReplace(this, offset, GetText(offset, length), text));

      _textBufferStrategy.Replace(offset, length, text);
      _lineTrackingStrategy.Replace(offset, length, text);

      OnDocumentChanged(new DocumentEventArgs(this, offset, length, text));
    }


    /// <summary>
    /// Returns a specific char of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the char to get.</param>
    /// <returns>The character.</returns>
    public char GetCharAt(int offset)
    {
      return _textBufferStrategy.GetCharAt(offset);
    }


    /// <summary>
    /// Fetches a string of characters contained in the sequence.
    /// </summary>
    /// <param name="offset">Offset into the sequence to fetch</param>
    /// <param name="length">The number of characters to copy.</param>
    /// <returns>
    /// The text at the <paramref name="offset"/>.
    /// </returns>
    public string GetText(int offset, int length)
    {
#if DEBUG
      if (length < 0) throw new ArgumentOutOfRangeException("length", length, "length < 0");
#endif
      return _textBufferStrategy.GetText(offset, length);
    }


    /// <summary>
    /// Gets the text of a certain segment.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <returns>The text in the segment.</returns>
    public string GetText(ISegment segment)
    {
      return GetText(segment.Offset, segment.Length);
    }


    /// <summary>
    /// Gets the line number for the given offset.
    /// </summary>
    /// <param name="offset">A offset which points to a character in the line which line number is
    /// returned.</param>
    /// <returns>
    /// An <c>int</c> which value is the line number.
    /// </returns>
    /// <remarks>
    /// Returns a valid line number for the given offset.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    public int GetLineNumberForOffset(int offset)
    {
      return _lineTrackingStrategy.GetLineNumberForOffset(offset);
    }


    /// <summary>
    /// Gets the line segment for a given offset.
    /// </summary>
    /// <param name="offset">A offset which points to a character in the line which
    /// is returned.</param>
    /// <returns>A <see cref="LineSegment"/> object.</returns>
    /// <remarks>
    /// Returns a <see cref="LineSegment"/> for the given offset.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    public LineSegment GetLineSegmentForOffset(int offset)
    {
      return _lineTrackingStrategy.GetLineSegmentForOffset(offset);
    }


    /// <summary>
    /// Gets the line segment.
    /// </summary>
    /// <param name="logicalLine">The line number which is requested.</param>
    /// <returns>A <see cref="LineSegment"/> object.</returns>
    /// <remarks>
    /// Returns a <see cref="LineSegment"/> for the given line number.
    /// This function should be used to get a line instead of getting the
    /// line using the <see cref="LineSegmentCollection"/>.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    public LineSegment GetLineSegment(int logicalLine)
    {
      return _lineTrackingStrategy.GetLineSegment(logicalLine);
    }


    /// <summary>
    /// Gets the first logical line for a given physical (visible) line.
    /// </summary>
    /// <param name="physicalLine">The physical line number.</param>
    /// <returns>The logical line number.</returns>
    /// <remarks>
    /// 	<para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines,
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// 	<para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// 	<para>
    /// Example: <c>lineNumber == 100</c>, foldings are in the <see cref="FoldingManager"/>
    /// between 0..1 (2 folded, invisible lines). This method returns 102 as
    /// the 'logical' line number.
    /// </para>
    /// 	<para>
    /// A visible line can contain several logical lines when it contains a (folded)
    /// folding. This method returns the <b>first</b> logical line that belongs to the
    /// visible line.
    /// </para>
    /// </remarks>
    public int GetFirstLogicalLine(int physicalLine)
    {
      return _lineTrackingStrategy.GetFirstLogicalLine(physicalLine);
    }


    /// <summary>
    /// Get the last logical line for a given physical (visible) line.
    /// </summary>
    /// <param name="physicalLine">The physical line number.</param>
    /// <returns>The logical line number.</returns>
    /// <remarks>
    /// 	<para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines,
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// 	<para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// </remarks>
    public int GetLastLogicalLine(int physicalLine)
    {
      return _lineTrackingStrategy.GetLastLogicalLine(physicalLine);
    }


    /// <summary>
    /// Get the physical (visible) line for a given logical line.
    /// </summary>
    /// <param name="logicalLine">The logical line number.</param>
    /// <returns>The physical (visible) line number.</returns>
    /// <remarks>
    /// 	<para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines,
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// 	<para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// 	<para>
    /// Example : <c>lineNumber == 100</c>, foldings are in the <see cref="FoldingManager"/>
    /// between 0..1 (2 folded, invisible lines). This method returns 98 as
    /// the 'visible' line number.
    /// </para>
    /// </remarks>
    public int GetVisibleLine(int logicalLine)
    {
      return _lineTrackingStrategy.GetVisibleLine(logicalLine);
    }


    /// <summary>
    /// Skips a certain number of visible lines forwards and returns the line
    /// number of the next visible line.
    /// </summary>
    /// <param name="logicalLine">The current logical line number.</param>
    /// <param name="visibleLineCount">The number of visible lines to skip.</param>
    /// <returns>
    /// The logical line number of the the next visible line.
    /// </returns>
    /// <remarks>
    /// 	<para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines,
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// 	<para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// </remarks>
    public int GetNextVisibleLineAfter(int logicalLine, int visibleLineCount)
    {
      return _lineTrackingStrategy.GetNextVisibleLineAfter(logicalLine, visibleLineCount);
    }


    /// <summary>
    /// Skips a certain number of visible lines backwards and returns the line
    /// number of the visible line before.
    /// </summary>
    /// <param name="logicalLine">The current logical line number.</param>
    /// <param name="visibleLineCount">The number of visible lines to skip.</param>
    /// <returns>
    /// The next visible line before the skipped block of lines.
    /// </returns>
    /// <remarks>
    /// 	<para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines,
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// 	<para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// </remarks>
    public int GetNextVisibleLineBefore(int logicalLine, int visibleLineCount)
    {
      return _lineTrackingStrategy.GetNextVisibleLineBefore(logicalLine, visibleLineCount);
    }


    /// <summary>
    /// Returns the logical line/column position from an offset
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>The position.</returns>
    public TextLocation OffsetToPosition(int offset)
    {
      int lineNr = GetLineNumberForOffset(offset);
      LineSegment line = GetLineSegment(lineNr);
      return new TextLocation(offset - line.Offset, lineNr);
    }


    /// <summary>
    /// Returns the offset from a logical line/column position
    /// </summary>
    /// <param name="p">The position.</param>
    /// <returns>The offset.</returns>
    public int PositionToOffset(TextLocation p)
    {
      if (p.Y >= TotalNumberOfLines)
      {
        return 0;
      }
      LineSegment line = GetLineSegment(p.Y);
      return Math.Min(TextLength, line.Offset + Math.Min(line.Length, p.X));
    }


    /// <summary>
    /// Raises the <see cref="DocumentAboutToBeChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    protected void OnDocumentAboutToBeChanged(DocumentEventArgs e)
    {
      if (DocumentAboutToBeChanged != null)
        DocumentAboutToBeChanged(this, e);
    }


    /// <summary>
    /// Raises the <see cref="DocumentChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    protected void OnDocumentChanged(DocumentEventArgs e)
    {
      if (DocumentChanged != null)
        DocumentChanged(this, e);
    }


    /// <summary>
    /// Requests an update.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <remarks>
    /// Requests an update of the text area.
    /// </remarks>
    public void RequestUpdate(TextAreaUpdate update)
    {
      if (_updateQueue.Count == 1 && _updateQueue[0].TextAreaUpdateType == TextAreaUpdateType.WholeTextArea)
      {
        // if we're going to update the whole text area, we don't need to store detail updates
        return;
      }
      if (update.TextAreaUpdateType == TextAreaUpdateType.WholeTextArea)
      {
        // if we're going to update the whole text area, we don't need to store detail updates
        _updateQueue.Clear();
      }
      _updateQueue.Add(update);
    }


    /// <summary>
    /// Commits the update.
    /// </summary>
    /// <remarks>
    /// Commits all updates in the queue to the text area (the
    /// text area will be painted)
    /// </remarks>
    public void CommitUpdate()
    {
      if (UpdateCommited != null)
        UpdateCommited(this, EventArgs.Empty);
    }


    /// <summary>
    /// Raises the <see cref="TextContentChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    protected virtual void OnTextContentChanged(EventArgs e)
    {
      if (TextContentChanged != null)
        TextContentChanged(this, e);
    }


    [Conditional("DEBUG")]
    internal static void ValidatePosition(IDocument document, TextLocation position)
    {
      document.GetLineSegment(position.Line);
    }
  }
}
