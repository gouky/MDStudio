using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Markers;
using DigitalRune.Windows.TextEditor.Selection;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// This class handles all mouse stuff for a textArea.
  /// </summary>
  internal class TextAreaMouseHandler
  {
    private readonly TextArea _textArea;
    private bool _doubleClick;
    private bool _clickedOnSelectedText;
    private MouseButtons _button;
    private static readonly Point _nilPoint = new Point(-1, -1);
    private Point _mouseDownPosition = _nilPoint;
    private Point _lastMouseDownPosition = _nilPoint;
    private bool _gotMouseDown;
    private bool _doDragDrop;
    private TextLocation _minSelection = TextLocation.Empty;
    private TextLocation _maxSelection = TextLocation.Empty;


    /// <summary>
    /// Initializes a new instance of the <see cref="TextAreaMouseHandler"/> class.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public TextAreaMouseHandler(TextArea textArea)
    {
      _textArea = textArea;
    }


    /// <summary>
    /// Attaches this instance.
    /// </summary>
    public void Attach()
    {
      _textArea.Click += TextAreaClick;
      _textArea.MouseMove += TextAreaMouseMove;
      _textArea.MouseDown += OnMouseDown;
      _textArea.DoubleClick += OnDoubleClick;
      _textArea.MouseLeave += OnMouseLeave;
      _textArea.MouseUp += OnMouseUp;
      _textArea.LostFocus += TextAreaLostFocus;
      _textArea.ToolTipRequest += OnToolTipRequest;
    }


    void OnToolTipRequest(object sender, ToolTipRequestEventArgs e)
    {
      if (e.ToolTipShown)
        return;

      Point mousepos = e.MousePosition;
      Fold fold = _textArea.TextView.GetFoldMarkerFromPosition(mousepos.X - _textArea.TextView.DrawingPosition.X,
                                                               mousepos.Y - _textArea.TextView.DrawingPosition.Y);
      if (fold != null && fold.IsFolded)
      {
        StringBuilder sb = new StringBuilder(fold.InnerText);

        // Skip leading newlines
        int i = 0;
        while (sb[i] == '\r' || sb[i] == '\n')
          i++;
        if (i > 0)
          sb.Remove(0, i);
        
        // max 10 lines
        int endLines = 0;
        for (i = 0; i < sb.Length; ++i)
        {
          if (sb[i] == '\n')
          {
            ++endLines;
            if (endLines >= 10)
            {
              sb.Remove(i + 1, sb.Length - i - 1);
              sb.Append(Environment.NewLine);
              sb.Append("...");
              break;
            }
          }
        }
        sb.Replace("\t", "    ");
        e.ShowToolTip(sb.ToString());
        return;
      }

      List<Marker> markers = _textArea.Document.MarkerStrategy.GetMarkers(e.LogicalPosition);
      foreach (Marker marker in markers)
      {
        if (marker.ToolTip != null)
        {
          e.ShowToolTip(marker.ToolTip.Replace("\t", "    "));
          return;
        }
      }
    }


    void ShowHiddenCursorIfMovedOrLeft()
    {
      _textArea.ShowHiddenCursor(!_textArea.Focused || !_textArea.ClientRectangle.Contains(_textArea.PointToClient(Cursor.Position)));
    }


    void TextAreaLostFocus(object sender, EventArgs e)
    {
      // The call to ShowHiddenCursorIfMovedOrLeft is delayed
      // until pending messages have been processed
      // so that it can properly detect whether the TextArea
      // has really lost focus.
      // For example, the CodeCompletionWindow gets focus when it is shown,
      // but immediately gives back focus to the TextArea.
      _textArea.BeginInvoke(new MethodInvoker(ShowHiddenCursorIfMovedOrLeft));
    }


    void OnMouseLeave(object sender, EventArgs e)
    {
      ShowHiddenCursorIfMovedOrLeft();
      _gotMouseDown = false;
      _mouseDownPosition = _nilPoint;
    }


    void OnMouseUp(object sender, MouseEventArgs e)
    {
      _textArea.SelectionManager.SelectFrom.Where = WhereFrom.None;
      _gotMouseDown = false;
      _mouseDownPosition = _nilPoint;
    }


    void TextAreaClick(object sender, EventArgs e)
    {
      Point mousepos = _textArea.MousePositionInternal;

      if (_doDragDrop)
        return;

      if (_clickedOnSelectedText && _textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y))
      {
        _textArea.SelectionManager.ClearSelection();

        TextLocation clickPosition = _textArea.TextView.GetLogicalPosition(mousepos.X - _textArea.TextView.DrawingPosition.X, mousepos.Y - _textArea.TextView.DrawingPosition.Y);
        _textArea.Caret.Position = clickPosition;
        _textArea.SetDesiredColumn();
      }
    }


    void TextAreaMouseMove(object sender, MouseEventArgs e)
    {
      _textArea.MousePositionInternal = e.Location;

      // honor the starting selection strategy
      switch (_textArea.SelectionManager.SelectFrom.Where)
      {
        case WhereFrom.Gutter:
          ExtendSelectionToMouse();
          return;

        case WhereFrom.TextArea:
          break;
      }

      _textArea.ShowHiddenCursor(false);
      if (_doDragDrop)
      {
        _doDragDrop = false;
        return;
      }

      _doubleClick = false;
      _textArea.MousePositionInternal = new Point(e.X, e.Y);

      if (_clickedOnSelectedText)
      {
        if (Math.Abs(_mouseDownPosition.X - e.X) >= SystemInformation.DragSize.Width / 2 
            || Math.Abs(_mouseDownPosition.Y - e.Y) >= SystemInformation.DragSize.Height / 2)
        {
          _clickedOnSelectedText = false;
          ISelection selection = _textArea.SelectionManager.GetSelectionAt(_textArea.Caret.Offset);
          if (selection != null)
          {
            string text = selection.SelectedText;
            bool isReadOnly = SelectionManager.SelectionIsReadOnly(_textArea.Document, selection);
            if (!String.IsNullOrEmpty(text))
            {
              DataObject dataObject = new DataObject();
              dataObject.SetData(DataFormats.UnicodeText, true, text);
              dataObject.SetData(selection);
              _doDragDrop = true;
              _textArea.DoDragDrop(dataObject, isReadOnly ? DragDropEffects.All & ~DragDropEffects.Move : DragDropEffects.All);
            }
          }
        }
        return;
      }

      if (e.Button == MouseButtons.Left)
        if (_gotMouseDown && _textArea.SelectionManager.SelectFrom.Where == WhereFrom.TextArea)
          ExtendSelectionToMouse();
    }


    void ExtendSelectionToMouse()
    {
      Point mousepos = _textArea.MousePositionInternal;
      TextLocation realmousepos = _textArea.TextView.GetLogicalPosition(
        Math.Max(0, mousepos.X - _textArea.TextView.DrawingPosition.X),
                    mousepos.Y - _textArea.TextView.DrawingPosition.Y);
      realmousepos = _textArea.Caret.ValidatePosition(realmousepos);
      TextLocation oldPos = _textArea.Caret.Position;
      if (oldPos == realmousepos && _textArea.SelectionManager.SelectFrom.Where != WhereFrom.Gutter)
      {
        return;
      }

      // the selection is from the gutter
      if (_textArea.SelectionManager.SelectFrom.Where == WhereFrom.Gutter)
      {
        if (realmousepos.Y < _textArea.SelectionManager.SelectionStart.Y)
        {
          // the selection has moved above the startpoint
          _textArea.Caret.Position = new TextLocation(0, realmousepos.Y);
        }
        else
        {
          // the selection has moved below the startpoint
          _textArea.Caret.Position = _textArea.SelectionManager.NextValidPosition(realmousepos.Y);
        }
      }
      else
      {
        _textArea.Caret.Position = realmousepos;
      }

      // moves selection across whole words for double-click initiated selection
      if (!_minSelection.IsEmpty && _textArea.SelectionManager.Selections.Count > 0 && _textArea.SelectionManager.SelectFrom.Where == WhereFrom.TextArea)
      {
        // Extend selection when selection was started with double-click
        TextLocation min = SelectionManager.IsPositionGreaterOrEqual(_minSelection, _maxSelection) ? _maxSelection : _minSelection;
        TextLocation max = SelectionManager.IsPositionGreaterOrEqual(_minSelection, _maxSelection) ? _minSelection : _maxSelection;
        if (SelectionManager.IsPositionGreaterOrEqual(max, realmousepos) && SelectionManager.IsPositionGreaterOrEqual(realmousepos, min))
        {
          _textArea.SelectionManager.SetSelection(min, max);
        }
        else if (SelectionManager.IsPositionGreaterOrEqual(max, realmousepos))
        {
          int moff = _textArea.Document.PositionToOffset(realmousepos);
          min = _textArea.Document.OffsetToPosition(FindWordStart(_textArea.Document, moff));
          _textArea.SelectionManager.SetSelection(min, max);
        }
        else
        {
          int moff = _textArea.Document.PositionToOffset(realmousepos);
          max = _textArea.Document.OffsetToPosition(FindWordEnd(_textArea.Document, moff));
          _textArea.SelectionManager.SetSelection(min, max);
        }
      }
      else
      {
        _textArea.SelectionManager.ExtendSelection(oldPos, _textArea.Caret.Position);
      }
      _textArea.SetDesiredColumn();
    }


    /// <summary>
    /// Doubles the click selection extend.
    /// </summary>
    void DoubleClickSelectionExtend()
    {
      Point mousepos = _textArea.MousePositionInternal;

      _textArea.SelectionManager.ClearSelection();
      if (_textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y))
      {
        Fold marker = _textArea.TextView.GetFoldMarkerFromPosition(mousepos.X - _textArea.TextView.DrawingPosition.X,
                                                                   mousepos.Y - _textArea.TextView.DrawingPosition.Y);
        if (marker != null && marker.IsFolded)
        {
          marker.IsFolded = false;
          _textArea.MotherTextAreaControl.AdjustScrollBars();
        }
        if (_textArea.Caret.Offset < _textArea.Document.TextLength)
        {
          switch (_textArea.Document.GetCharAt(_textArea.Caret.Offset))
          {
            case '"':
              if (_textArea.Caret.Offset < _textArea.Document.TextLength)
              {
                int next = FindNext(_textArea.Document, _textArea.Caret.Offset + 1, '"');
                _minSelection = _textArea.Caret.Position;
                if (next > _textArea.Caret.Offset && next < _textArea.Document.TextLength)
                  next += 1;
                _maxSelection = _textArea.Document.OffsetToPosition(next);
              }
              break;
            default:
              _minSelection = _textArea.Document.OffsetToPosition(FindWordStart(_textArea.Document, _textArea.Caret.Offset));
              _maxSelection = _textArea.Document.OffsetToPosition(FindWordEnd(_textArea.Document, _textArea.Caret.Offset));
              break;

          }
          _textArea.Caret.Position = _maxSelection;
          _textArea.SelectionManager.ExtendSelection(_minSelection, _maxSelection);
        }

        if (_textArea.SelectionManager.Selections.Count > 0)
        {
          ISelection selection = _textArea.SelectionManager.Selections[0];

          selection.StartPosition = _minSelection;
          selection.EndPosition = _maxSelection;
          _textArea.SelectionManager.SelectionStart = _minSelection;
        }

        // after a double-click selection, the caret is placed correctly,
        // but it is not positioned internally.  The effect is when the cursor
        // is moved up or down a line, the caret will take on the column first
        // clicked on for the double-click
        _textArea.SetDesiredColumn();

        // HACK WARNING !!!
        // must refresh here, because when a error tooltip is showed and the underlined
        // code is double clicked the textArea don't update correctly, updateline doesn't
        // work ... but the refresh does.
        // Mike
        _textArea.Refresh();
      }
    }

    void OnMouseDown(object sender, MouseEventArgs e)
    {
      _textArea.MousePositionInternal = e.Location;
      Point mousepos = e.Location;

      if (_doDragDrop)
        return;

      if (_doubleClick)
      {
        _doubleClick = false;
        return;
      }

      if (_textArea.TextView.DrawingPosition.Contains(mousepos.X, mousepos.Y))
      {
        _gotMouseDown = true;
        _textArea.SelectionManager.SelectFrom.Where = WhereFrom.TextArea;
        _button = e.Button;

        // double-click
        if (_button == MouseButtons.Left && e.Clicks == 2)
        {
          int deltaX = Math.Abs(_lastMouseDownPosition.X - e.X);
          int deltaY = Math.Abs(_lastMouseDownPosition.Y - e.Y);
          if (deltaX <= SystemInformation.DoubleClickSize.Width &&
              deltaY <= SystemInformation.DoubleClickSize.Height)
          {
            DoubleClickSelectionExtend();
            _lastMouseDownPosition = new Point(e.X, e.Y);

            if (_textArea.SelectionManager.SelectFrom.Where == WhereFrom.Gutter)
            {
              if (!_minSelection.IsEmpty && !_maxSelection.IsEmpty && _textArea.SelectionManager.Selections.Count > 0)
              {
                _textArea.SelectionManager.Selections[0].StartPosition = _minSelection;
                _textArea.SelectionManager.Selections[0].EndPosition = _maxSelection;
                _textArea.SelectionManager.SelectionStart = _minSelection;

                _minSelection = TextLocation.Empty;
                _maxSelection = TextLocation.Empty;
              }
            }
            return;
          }
        }
        _minSelection = TextLocation.Empty;
        _maxSelection = TextLocation.Empty;

        _lastMouseDownPosition = _mouseDownPosition = new Point(e.X, e.Y);

        if (_button == MouseButtons.Left)
        {
          Fold marker = _textArea.TextView.GetFoldMarkerFromPosition(mousepos.X - _textArea.TextView.DrawingPosition.X,
                                                                     mousepos.Y - _textArea.TextView.DrawingPosition.Y);
          if (marker != null && marker.IsFolded)
          {
            if (_textArea.SelectionManager.HasSomethingSelected)
              _clickedOnSelectedText = true;

            TextLocation startLocation = new TextLocation(marker.StartColumn, marker.StartLine);
            TextLocation endLocation = new TextLocation(marker.EndColumn, marker.EndLine);
            _textArea.SelectionManager.SetSelection(new DefaultSelection(_textArea.TextView.Document, startLocation, endLocation));
            _textArea.Caret.Position = startLocation;
            _textArea.SetDesiredColumn();
            _textArea.Focus();
            return;
          }

          if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
          {
            ExtendSelectionToMouse();
          }
          else
          {
            TextLocation realmousepos = _textArea.TextView.GetLogicalPosition(mousepos.X - _textArea.TextView.DrawingPosition.X, mousepos.Y - _textArea.TextView.DrawingPosition.Y);
            _clickedOnSelectedText = false;

            int offset = _textArea.Document.PositionToOffset(realmousepos);

            if (_textArea.SelectionManager.HasSomethingSelected &&
                _textArea.SelectionManager.IsSelected(offset))
            {
              _clickedOnSelectedText = true;
            }
            else
            {
              _textArea.SelectionManager.ClearSelection();
              if (mousepos.Y > 0 && mousepos.Y < _textArea.TextView.DrawingPosition.Height)
              {
                TextLocation pos = new TextLocation
                {
                  X = realmousepos.X,
                  Y = Math.Min(_textArea.Document.TotalNumberOfLines - 1, realmousepos.Y),
                };
                _textArea.Caret.Position = pos;
                _textArea.SetDesiredColumn();
              }
            }
          }
        }
        else if (_button == MouseButtons.Right)
        {
          // Rightclick sets the cursor to the click position unless
          // the previous selection was clicked
          TextLocation realmousepos = _textArea.TextView.GetLogicalPosition(mousepos.X - _textArea.TextView.DrawingPosition.X, mousepos.Y - _textArea.TextView.DrawingPosition.Y);
          int offset = _textArea.Document.PositionToOffset(realmousepos);
          if (!_textArea.SelectionManager.HasSomethingSelected ||
              !_textArea.SelectionManager.IsSelected(offset))
          {
            _textArea.SelectionManager.ClearSelection();
            if (mousepos.Y > 0 && mousepos.Y < _textArea.TextView.DrawingPosition.Height)
            {
              TextLocation pos = new TextLocation
              {
                X = realmousepos.X,
                Y = Math.Min(_textArea.Document.TotalNumberOfLines - 1, realmousepos.Y),
              };
              _textArea.Caret.Position = pos;
              _textArea.SetDesiredColumn();
            }
          }
        }
      }
      _textArea.Focus();
    }


    static int FindNext(IDocument document, int offset, char ch)
    {
      LineSegment line = document.GetLineSegmentForOffset(offset);
      int endPos = line.Offset + line.Length;

      while (offset < endPos && document.GetCharAt(offset) != ch)
        ++offset;

      return offset;
    }


    static bool IsSelectableChar(char ch)
    {
      return Char.IsLetterOrDigit(ch) || ch == '_';
    }


    static int FindWordStart(IDocument document, int offset)
    {
      LineSegment line = document.GetLineSegmentForOffset(offset);

      if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && Char.IsWhiteSpace(document.GetCharAt(offset)))
      {
        while (offset > line.Offset && Char.IsWhiteSpace(document.GetCharAt(offset - 1)))
        {
          --offset;
        }
      }
      else if (IsSelectableChar(document.GetCharAt(offset)) || (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset)) && IsSelectableChar(document.GetCharAt(offset - 1))))
      {
        while (offset > line.Offset && IsSelectableChar(document.GetCharAt(offset - 1)))
        {
          --offset;
        }
      }
      else
      {
        if (offset > 0 && !Char.IsWhiteSpace(document.GetCharAt(offset - 1)) && !IsSelectableChar(document.GetCharAt(offset - 1)))
        {
          return Math.Max(0, offset - 1);
        }
      }
      return offset;
    }


    static int FindWordEnd(IDocument document, int offset)
    {
      LineSegment line = document.GetLineSegmentForOffset(offset);
      if (line.Length == 0)
        return offset;

      int endPos = line.Offset + line.Length;
      offset = Math.Min(offset, endPos - 1);

      if (IsSelectableChar(document.GetCharAt(offset)))
      {
        while (offset < endPos && IsSelectableChar(document.GetCharAt(offset)))
        {
          ++offset;
        }
      }
      else if (Char.IsWhiteSpace(document.GetCharAt(offset)))
      {
        if (offset > 0 && Char.IsWhiteSpace(document.GetCharAt(offset - 1)))
        {
          while (offset < endPos && Char.IsWhiteSpace(document.GetCharAt(offset)))
          {
            ++offset;
          }
        }
      }
      else
      {
        return Math.Max(0, offset + 1);
      }

      return offset;
    }


    void OnDoubleClick(object sender, EventArgs e)
    {
      if (_doDragDrop)
        return;

      _textArea.SelectionManager.SelectFrom.Where = WhereFrom.TextArea;
      _doubleClick = true;
    }
  }
}
