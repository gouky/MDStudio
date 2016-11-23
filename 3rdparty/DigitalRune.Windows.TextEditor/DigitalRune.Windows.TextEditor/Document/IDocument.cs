using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor;
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
  /// Represents a text document that can be shown and edited in a <see cref="TextEditorControl"/>.
  /// </summary>
  public interface IDocument
  {
    /// <summary>
    /// Gets or sets the text editor properties.
    /// </summary>
    /// <value>The text editor properties.</value>
    ITextEditorProperties TextEditorProperties
    {
      get;
      set;
    }


    /// <summary>
    /// Gets the undo stack.
    /// </summary>
    /// <value>The undo stack.</value>
    UndoStack UndoStack
    {
      get;
    }


    /// <summary>
    /// Gets or sets a value indicating whether the document is read-only.
    /// </summary>
    /// <value>If <see langword="true"/> the document can't be altered</value>
    bool ReadOnly
    {
      get;
      set;
    }


    /// <summary>
    /// The <see cref="IFormattingStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The formatting strategy.</value>
    IFormattingStrategy FormattingStrategy
    {
      get;
      set;
    }


    /// <summary>
    /// The <see cref="ITextBufferStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The text buffer strategy.</value>
    ITextBufferStrategy TextBufferStrategy
    {
      get;
    }


    /// <summary>
    /// The <see cref="FoldingManager"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The folding manager.</value>
    FoldingManager FoldingManager
    {
      get;
    }


    /// <summary>
    /// The <see cref="IHighlightingStrategy"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The highlighting strategy.</value>
    IHighlightingStrategy HighlightingStrategy
    {
      get;
      set;
    }


    /// <summary>
    /// The <see cref="BookmarkManager"/> attached to the <see cref="IDocument"/> instance
    /// </summary>
    /// <value>The bookmark manager.</value>
    BookmarkManager BookmarkManager
    {
      get;
    }


    /// <summary>
    /// Gets the marker strategy.
    /// </summary>
    /// <value>The marker strategy.</value>
    MarkerStrategy MarkerStrategy
    {
      get;
    }


    /// <summary>
    /// Moves, Resizes, Removes a list of segments on insert/remove/replace events.
    /// </summary>
    /// <typeparam name="T">A type of segment.</typeparam>
    /// <param name="list">The list.</param>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    void UpdateSegmentListOnDocumentChange<T>(List<T> list, DocumentEventArgs e) where T : ISegment;


    /// <summary>
    /// Is fired when CommitUpdate is called
    /// </summary>
    event EventHandler UpdateCommited;


    /// <summary>
    /// Occurs when a document is about to be changed.
    /// </summary>
    event EventHandler<DocumentEventArgs> DocumentAboutToBeChanged;


    /// <summary>
    /// Occurs when a document has been changed.
    /// </summary>
    event EventHandler<DocumentEventArgs> DocumentChanged;


    /// <summary>
    /// Occurs when the text content has changed.
    /// </summary>
    event EventHandler TextContentChanged;


    #region ----- ILineManager interface -----
    /// <summary>
    /// Gets the line segment collection.
    /// </summary>
    /// <value>A collection of all line segments</value>
    /// <remarks>
    /// The collection should only be used if you're aware
    /// of the 'last line ends with a delimiter problem'. Otherwise
    /// the <see cref="GetLineSegment"/> method should be used.
    /// </remarks>
    IList<LineSegment> LineSegmentCollection
    {
      get;
    }


    /// <summary>
    /// Gets the total number of lines.
    /// </summary>
    /// <value>
    /// The total number of lines in the document.
    /// </value>
    int TotalNumberOfLines
    {
      get;
    }


    /// <summary>
    /// Occurs when length of a line changes..
    /// </summary>
    event EventHandler<LineLengthChangedEventArgs> LineLengthChanged;


    /// <summary>
    /// Occurs when number of lines changes.
    /// </summary>
    event EventHandler<LineCountChangedEventArgs> LineCountChanged;


    /// <summary>
    /// Occurs when a line is deleted.
    /// </summary>
    event EventHandler<LineEventArgs> LineDeleted;


    /// <summary>
    /// Gets the line number for the given offset.
    /// </summary>
    /// <param name="offset">
    /// A offset which points to a character in the line which line number is 
    /// returned.
    /// </param>
    /// <returns>An <c>int</c> which value is the line number.</returns>
    /// <remarks>
    /// Returns a valid line number for the given offset.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    int GetLineNumberForOffset(int offset);


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
    LineSegment GetLineSegmentForOffset(int offset);


    /// <summary>
    /// Gets the line segment.
    /// </summary>
    /// <param name="logicalLine">The line number which is requested.</param>
    /// <returns>A <see cref="LineSegment"/> object.</returns>
    /// <remarks>
    /// Returns a <see cref="LineSegment"/> for the given line number.
    /// </remarks>
    /// <exception cref="System.ArgumentException">If offset points not to a valid position</exception>
    LineSegment GetLineSegment(int logicalLine);


    /// <summary>
    /// Gets the first logical line for a given physical (visible) line.
    /// </summary>
    /// <param name="physicalLine">The physical line number.</param>
    /// <returns>The logical line number.</returns>
    /// <remarks>
    /// <para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines, 
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// <para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// <para>
    /// Example: <c>lineNumber == 100</c>, foldings are in the <see cref="FoldingManager"/>
    /// between 0..1 (2 folded, invisible lines). This method returns 102 as
    /// the 'logical' line number.
    /// </para>
    /// <para>
    /// A visible line can contain several logical lines when it contains a (folded)
    /// folding. This method returns the <b>first</b> logical line that belongs to the 
    /// visible line.
    /// </para>
    /// </remarks>
    int GetFirstLogicalLine(int physicalLine);


    /// <summary>
    /// Get the last logical line for a given physical (visible) line.
    /// </summary>
    /// <param name="physicalLine">The physical line number.</param>
    /// <returns>The logical line number.</returns>
    /// <remarks>
    /// <para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines, 
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// <para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// </remarks>
    int GetLastLogicalLine(int physicalLine);


    /// <summary>
    /// Get the physical (visible) line for a given logical line.
    /// </summary>
    /// <param name="logicalLine">The logical line number.</param>
    /// <returns>The physical (visible) line number.</returns>
    /// <remarks>
    /// <para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines, 
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// <para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// <para>
    /// Example : <c>lineNumber == 100</c>, foldings are in the <see cref="FoldingManager"/>
    /// between 0..1 (2 folded, invisible lines). This method returns 98 as
    /// the 'visible' line number.
    /// </para>
    /// </remarks>
    int GetVisibleLine(int logicalLine);


    /// <summary>
    /// Skips a certain number of visible lines forwards and returns the line 
    /// number of the next visible line.
    /// </summary>
    /// <param name="logicalLine">The current logical line number.</param>
    /// <param name="visibleLineCount">The number of visible lines to skip.</param>
    /// <returns>The logical line number of the the next visible line.</returns>
    /// <remarks>
    /// <para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines, 
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// <para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// </remarks>
    int GetNextVisibleLineAfter(int logicalLine, int visibleLineCount);


    /// <summary>
    /// Skips a certain number of visible lines backwards and returns the line
    /// number of the visible line before.
    /// </summary>
    /// <param name="logicalLine">The current logical line number.</param>
    /// <param name="visibleLineCount">The number of visible lines to skip.</param>
    /// <returns>The next visible line before the skipped block of lines.</returns>
    /// <remarks>
    /// <para>
    /// A logical line is a line in the document, whereas a physical line is line that
    /// is rendered in the text editor. A physical line can contain multiple logical lines, 
    /// for example when they are collapsed (see "folding").
    /// </para>
    /// <para>
    /// The logical line numbers correspond with the line numbers drawn in the line-number margin.
    /// (Except that logical lines are 0-based and line-numbers drawn are 1-based.)
    /// </para>
    /// </remarks>
    int GetNextVisibleLineBefore(int logicalLine, int visibleLineCount);

    #endregion


    #region ----- ITextBufferStrategy interface -----
    /// <summary>
    /// Gets or sets the whole text as string.
    /// </summary>
    /// <value>
    /// The whole text as string.
    /// </value>
    /// <remarks>
    /// When setting the text using the TextContent property, the undo stack is cleared.
    /// Set TextContent only for actions such as loading a file; if you want to change the current document
    /// use the Replace method instead.
    /// </remarks>
    string TextContent
    {
      get;
      set;
    }


    /// <summary>
    /// Gets the length of the text.
    /// </summary>
    /// <value>
    /// The current length of the sequence of characters that can be edited.
    /// </value>
    int TextLength
    {
      get;
    }


    /// <summary>
    /// Inserts a string of characters into the sequence.
    /// </summary>
    /// <param name="offset">Offset where to insert the string.</param>
    /// <param name="text">Text to be inserted.</param>
    void Insert(int offset, string text);


    /// <summary>
    /// Removes some portion of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the remove.</param>
    /// <param name="length">Number of characters to remove.</param>
    void Remove(int offset, int length);


    /// <summary>
    /// Replace some portion of the sequence.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The number of characters to replace.</param>
    /// <param name="text">The text to be replaced with.</param>
    void Replace(int offset, int length, string text);


    /// <summary>
    /// Returns a specific char of the sequence.
    /// </summary>
    /// <param name="offset">Offset of the char to get.</param>
    /// <returns>The character.</returns>
    char GetCharAt(int offset);


    /// <summary>
    /// Fetches a string of characters contained in the sequence.
    /// </summary>
    /// <param name="offset">Offset into the sequence to fetch</param>
    /// <param name="length">The number of characters to copy.</param>
    /// <returns>The text at the <paramref name="offset"/>.</returns>
    string GetText(int offset, int length);
    #endregion


    /// <summary>
    /// Gets the text of a certain segment.
    /// </summary>
    /// <param name="segment">The segment.</param>
    /// <returns>The text in the segment.</returns>
    string GetText(ISegment segment);


    #region ----- ITextModel interface -----
    /// <summary>
    /// Returns the logical line/column position from an offset
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>The position.</returns>
    TextLocation OffsetToPosition(int offset);


    /// <summary>
    /// Returns the offset from a logical line/column position
    /// </summary>
    /// <param name="p">The position.</param>
    /// <returns>The offset.</returns>
    int PositionToOffset(TextLocation p);
    #endregion


    /// <summary>
    /// Gets the update queue.
    /// </summary>
    /// <value>A container where all TextAreaUpdate objects get stored</value>
    List<TextAreaUpdate> UpdateQueue { get; }


    /// <summary>
    /// Requests an update.
    /// </summary>
    /// <param name="update">The update.</param>
    /// <remarks>
    /// Requests an update of the text area.
    /// </remarks>
    void RequestUpdate(TextAreaUpdate update);


    /// <summary>
    /// Commits the update.
    /// </summary>
    /// <remarks>
    /// Commits all updates in the queue to the text area (the
    /// text area will be painted)
    /// </remarks>
    void CommitUpdate();
  }
}
