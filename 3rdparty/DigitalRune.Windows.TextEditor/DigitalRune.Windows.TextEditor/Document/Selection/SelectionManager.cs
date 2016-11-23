using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Markers;


namespace DigitalRune.Windows.TextEditor.Selection
{
  /// <summary>
  /// Manages the selections in a document.
  /// </summary>
  public class SelectionManager
  {
    private readonly IDocument _document;
    private readonly TextArea _textArea;
    private TextLocation _selectionStart;
    private readonly SelectFrom _selectFrom = new SelectFrom();
    private readonly List<ISelection> _selections = new List<ISelection>();


    internal TextLocation SelectionStart
    {
      get { return _selectionStart; }
      set
      {
        DefaultDocument.ValidatePosition(_document, value);
        _selectionStart = value;
      }
    }


    internal SelectFrom SelectFrom
    {
      get { return _selectFrom; }
    }


    /// <summary>
    /// Gets the collection containing all selection.
    /// </summary>
    /// <value>A collection containing all selections.</value>
    public List<ISelection> Selections
    {
      get { return _selections; }
    }


    /// <summary>
    /// Gets a value indicating whether this instance has something selected.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the <see cref="Selections"/> is not empty, 
    /// <see langword="false"/> otherwise.
    /// </value>
    public bool HasSomethingSelected
    {
      get { return _selections.Count > 0; }
    }


    /// <summary>
    /// Gets a value indicating whether the selections are read-only.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the selections are read-only; otherwise, <see langword="false"/>.
    /// </value>
    public bool SelectionIsReadonly
    {
      get
      {
        if (_document.ReadOnly)
          return true;
        foreach (ISelection selection in _selections)
        {
          if (SelectionIsReadOnly(_document, selection))
            return true;
        }
        return false;
      }
    }


		internal static bool SelectionIsReadOnly(IDocument document, ISelection sel)
		{
			if (document.TextEditorProperties.SupportsReadOnlySegments)
				return document.MarkerStrategy.GetMarkers(sel.Offset, sel.Length).Exists(Marker.IsReadOnlyPredicate);
			else
				return false;
		}


    /// <summary>
    /// Gets the selected text.
    /// </summary>
    /// <value>The text that is currently selected.</value>
    /// <remarks>
    /// If multiple non-consecutive texts are selected, then these texts are 
    /// returned as a single string.
    /// </remarks>
    public string SelectedText
    {
      get
      {
        StringBuilder builder = new StringBuilder();

        foreach (ISelection s in _selections)
          builder.Append(s.SelectedText);

        return builder.ToString();
      }
    }


    /// <summary>
    /// Occurs when the selection is changed.
    /// </summary>
    public event EventHandler SelectionChanged;


    /// <summary>
    /// Creates a new instance of <see cref="SelectionManager"/>
    /// </summary>
    /// <param name="document">The document.</param>
    public SelectionManager(IDocument document)
    {
      _document = document;
    }


    /// <summary>
    /// Creates a new instance of <see cref="SelectionManager"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="textArea">The text area.</param>
    public SelectionManager(IDocument document, TextArea textArea)
    {
      _document = document;
      _textArea = textArea;
    }


    /// <remarks>
    /// Clears the selection and sets a new selection
    /// using the given <see cref="ISelection"/> object.
    /// </remarks>
    public void SetSelection(ISelection selection)
    {
      if (selection != null)
      {
        if (Selections.Count == 1 &&
            selection.StartPosition == Selections[0].StartPosition &&
            selection.EndPosition == Selections[0].EndPosition)
        {
          return;
        }
        ClearWithoutUpdate();
        _selections.Add(selection);
        _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, selection.StartPosition.Y, selection.EndPosition.Y));
        _document.CommitUpdate();
        OnSelectionChanged(EventArgs.Empty);
      }
      else
      {
        ClearSelection();
      }
    }


    /// <summary>
    /// Sets a selection.
    /// </summary>
    /// <param name="startPosition">The start position.</param>
    /// <param name="endPosition">The end position.</param>
    public void SetSelection(TextLocation startPosition, TextLocation endPosition)
    {
      SetSelection(new DefaultSelection(_document, startPosition, endPosition));
    }


    /// <summary>
    /// Determines whether a text position is greater than or equal to another 
    /// text position.
    /// </summary>
    /// <param name="position1">The first position.</param>
    /// <param name="position2">The second position.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="position1"/> is greater than or equal to 
    /// <paramref name="position2"/>; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsPositionGreaterOrEqual(TextLocation position1, TextLocation position2)
    {
      return position1.Y > position2.Y || position1.Y == position2.Y && position1.X >= position2.X;
    }


    /// <summary>
    /// Extends the selection.
    /// </summary>
    /// <param name="oldPosition">The old position.</param>
    /// <param name="newPosition">The new position.</param>
    public void ExtendSelection(TextLocation oldPosition, TextLocation newPosition)
    {
      // where oldposition is where the cursor was,
      // and newposition is where it has ended up from a click (both zero based)

      if (oldPosition == newPosition)
        return;

      TextLocation min;
      TextLocation max;
      int oldnewX = newPosition.X;
      bool oldIsGreater = IsPositionGreaterOrEqual(oldPosition, newPosition);
      if (oldIsGreater)
      {
        min = newPosition;
        max = oldPosition;
      }
      else
      {
        min = oldPosition;
        max = newPosition;
      }

      if (min == max)
      {
        return;
      }

      if (!HasSomethingSelected)
      {
        SetSelection(new DefaultSelection(_document, min, max));
        // initialize SelectFrom for a cursor selection
        if (_selectFrom.Where == WhereFrom.None)
          SelectionStart = oldPosition; //textArea.Caret.Position;
        return;
      }

      ISelection selection = _selections[0];

      if (min == max)
      {
        //selection.StartPosition = newPosition;
        return;
      }
      else
      {
        // changed selection via gutter
        if (_selectFrom.Where == WhereFrom.Gutter)
        {
          // selection new position is always at the left edge for gutter selections
          newPosition.X = 0;
        }

        if (IsPositionGreaterOrEqual(newPosition, SelectionStart)) // selecting forward
        {
          selection.StartPosition = SelectionStart;
          // this handles last line selection
          if (_selectFrom.Where == WhereFrom.Gutter) //&& newPosition.Y != oldPosition.Y)
            selection.EndPosition = new TextLocation(_textArea.Caret.Column, _textArea.Caret.Line);
          else
          {
            newPosition.X = oldnewX;
            selection.EndPosition = newPosition;
          }
        }
        else
        { // selecting back
          if (_selectFrom.Where == WhereFrom.Gutter && _selectFrom.First == WhereFrom.Gutter)
          { // gutter selection
            selection.EndPosition = NextValidPosition(SelectionStart.Y);
          }
          else
          { // internal text selection
            selection.EndPosition = SelectionStart; //selection.StartPosition;
          }
          selection.StartPosition = newPosition;
        }
      }

      _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, min.Y, max.Y));
      _document.CommitUpdate();
      OnSelectionChanged(EventArgs.Empty);
    }


    /// <summary>
    /// Returns the next valid position.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <returns>The next valid position after the given line.</returns>
    /// <remarks>
    /// This methods checks that there are more lines available after current one.
    /// If there are then the next line is returned. Otherwise, the last position 
    /// on the given line is returned.
    /// </remarks>
    public TextLocation NextValidPosition(int line)
    {
      if (line < _document.TotalNumberOfLines - 1)
        return new TextLocation(0, line + 1);
      else
        return new TextLocation(_document.GetLineSegment(_document.TotalNumberOfLines - 1).Length + 1, line);
    }


    /// <summary>
    /// Clears the without update.
    /// </summary>
    void ClearWithoutUpdate()
    {
      while (_selections.Count > 0)
      {
        ISelection selection = _selections[_selections.Count - 1];
        _selections.RemoveAt(_selections.Count - 1);
        _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, selection.StartPosition.Y, selection.EndPosition.Y));
        OnSelectionChanged(EventArgs.Empty);
      }
    }


    /// <remarks>
    /// Clears the selection.
    /// </remarks>
    public void ClearSelection()
    {
      Point mousepos = _textArea.MousePositionInternal;
      // this is the most logical place to reset selection starting
      // positions because it is always called before a new selection
      _selectFrom.First = _selectFrom.Where;
      TextLocation newSelectionStart = _textArea.TextView.GetLogicalPosition(mousepos.X - _textArea.TextView.DrawingPosition.X, mousepos.Y - _textArea.TextView.DrawingPosition.Y);
      if (_selectFrom.Where == WhereFrom.Gutter)
        newSelectionStart.X = 0;

      if (newSelectionStart.Line >= _document.TotalNumberOfLines)
      {
        newSelectionStart.Line = _document.TotalNumberOfLines - 1;
        newSelectionStart.Column = _document.GetLineSegment(_document.TotalNumberOfLines - 1).Length;
      }
      SelectionStart = newSelectionStart;

      ClearWithoutUpdate();
      _document.CommitUpdate();
    }


    /// <summary>
    /// Removes the selected text from the buffer and clears
    /// the selection.
    /// </summary>
    /// <remarks>
    /// The position of the caret is <strong>not</strong> updated automatically.
    /// </remarks>
    public void RemoveSelectedText()
    {
      if (SelectionIsReadonly)
      {
        ClearSelection();
        return;
      }

      List<int> lines = new List<int>();
      int offset = -1;
      bool oneLine = true;
      foreach (ISelection s in _selections)
      {
        if (oneLine)
        {
          int lineBegin = s.StartPosition.Y;
          if (lineBegin != s.EndPosition.Y)
          {
            oneLine = false;
          }
          else
          {
            lines.Add(lineBegin);
          }
        }
        offset = s.Offset;
        _document.Remove(s.Offset, s.Length);
      }

      ClearSelection();

      if (offset != -1)
      {
        if (oneLine)
        {
          foreach (int i in lines)
            _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, i));
        }
        else
        {
          _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        }
        _document.CommitUpdate();
      }
    }


    /// <summary>
    /// Determines whether the specified offset points to a section which is
    /// selected.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified offset is selected; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsSelected(int offset)
    {
      return GetSelectionAt(offset) != null;
    }


    /// <summary>
    /// Returns a <see cref="ISelection"/> object giving the selection in which
    /// the offset points to.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <returns>
    /// 	<see langword="null"/> if the offset doesn't point to a selection
    /// </returns>
    public ISelection GetSelectionAt(int offset)
    {
      foreach (ISelection s in _selections)
      {
        if (s.ContainsOffset(offset))
          return s;
      }
      return null;
    }


    /// <summary>
    /// Gets the selection at a given line.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>The range of selected columns.</returns>
    public ColumnRange GetSelectionAtLine(int lineNumber)
    {
      foreach (ISelection selection in _selections)
      {
        int startLine = selection.StartPosition.Y;
        int endLine = selection.EndPosition.Y;
        if (startLine < lineNumber && lineNumber < endLine)
        {
          return ColumnRange.WholeColumn;
        }

        if (startLine == lineNumber)
        {
          LineSegment line = _document.GetLineSegment(startLine);
          int startColumn = selection.StartPosition.X;
          int endColumn = endLine == lineNumber ? selection.EndPosition.X : line.Length + 1;
          return new ColumnRange(startColumn, endColumn);
        }

        if (endLine == lineNumber)
        {
          int endColumn = selection.EndPosition.X;
          return new ColumnRange(0, endColumn);
        }
      }

      return ColumnRange.NoColumn;
    }


    /// <summary>
    /// Fires the selection changed.
    /// </summary>
    public void FireSelectionChanged()
    {
      OnSelectionChanged(EventArgs.Empty);
    }


    /// <summary>
    /// Raises the <see cref="SelectionChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnSelectionChanged(EventArgs e)
    {
      if (SelectionChanged != null)
        SelectionChanged(this, e);
    }
  }


  // selection initiated from...
  internal class SelectFrom
  {
    public WhereFrom Where = WhereFrom.None; // last selection initiator
    public WhereFrom First = WhereFrom.None; // first selection initiator
  }


  // selection initiated from type...
  internal enum WhereFrom
  {
    None,
    Gutter,
    TextArea
  }
}
