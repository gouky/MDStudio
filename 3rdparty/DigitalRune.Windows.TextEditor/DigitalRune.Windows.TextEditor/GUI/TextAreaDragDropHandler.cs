using System;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Selection;

namespace DigitalRune.Windows.TextEditor
{
  internal class TextAreaDragDropHandler
  {
    private TextArea _textArea;


    private class SafeDragEventHandler
    {
      private readonly DragEventHandler _eventHandler;

      public SafeDragEventHandler(DragEventHandler eventHandler)
      {
        _eventHandler = eventHandler;
      }

      public void OnDragEvent(object sender, DragEventArgs eventArgs)
      {
        try
        {
          _eventHandler(sender, eventArgs);
        }
        catch (Exception exception)
        {
          MessageBox.Show(exception.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }          
      }
    }


    /// <summary>
    /// Attaches the specified text area.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public void Attach(TextArea textArea)
    {
      _textArea = textArea;
      textArea.AllowDrop = true;

      textArea.DragEnter += new SafeDragEventHandler(OnDragEnter).OnDragEvent;
      textArea.DragDrop += new SafeDragEventHandler(OnDragDrop).OnDragEvent;
      textArea.DragOver += new SafeDragEventHandler(OnDragOver).OnDragEvent;
    }


    static DragDropEffects GetDragDropEffect(DragEventArgs e)
    {
      if ((e.AllowedEffect & DragDropEffects.Move) > 0 &&
          (e.AllowedEffect & DragDropEffects.Copy) > 0)
      {
        return (e.KeyState & 8) > 0 ? DragDropEffects.Copy : DragDropEffects.Move;
      }
      else if ((e.AllowedEffect & DragDropEffects.Move) > 0)
      {
        return DragDropEffects.Move;
      }
      else if ((e.AllowedEffect & DragDropEffects.Copy) > 0)
      {
        return DragDropEffects.Copy;
      }
      return DragDropEffects.None;
    }


    protected static void OnDragEnter(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(typeof(string)))
      {
        e.Effect = GetDragDropEffect(e);
      }
    }


    void InsertString(int offset, string str)
    {
      _textArea.Document.Insert(offset, str);

      _textArea.SelectionManager.SetSelection(new DefaultSelection(_textArea.Document,
                                                                  _textArea.Document.OffsetToPosition(offset),
                                                                  _textArea.Document.OffsetToPosition(offset + str.Length)));
      _textArea.Caret.Position = _textArea.Document.OffsetToPosition(offset + str.Length);
      _textArea.Refresh();
    }


    protected void OnDragDrop(object sender, DragEventArgs e)
    {
      if (e.Data.GetDataPresent(typeof(string)))
      {
        _textArea.BeginUpdate();
        _textArea.Document.UndoStack.StartUndoGroup();
        try
        {
          int offset = _textArea.Caret.Offset;
					if (_textArea.IsReadOnly(offset))
          {
            // prevent dragging text into readonly section
            return;
          }
          if (e.Data.GetDataPresent(typeof(DefaultSelection)))
          {
            ISelection sel = (ISelection) e.Data.GetData(typeof(DefaultSelection));
            if (sel.ContainsPosition(_textArea.Caret.Position))
            {
              return;
            }
            if (GetDragDropEffect(e) == DragDropEffects.Move)
            {
              if (SelectionManager.SelectionIsReadOnly(_textArea.Document, sel))
              {
                // prevent dragging text out of readonly section
                return;
              }
              int len = sel.Length;
              _textArea.Document.Remove(sel.Offset, len);
              if (sel.Offset < offset)
              {
                offset -= len;
              }
            }
          }
          _textArea.SelectionManager.ClearSelection();
          InsertString(offset, (string) e.Data.GetData(typeof(string)));
          _textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        }
        finally
        {
          _textArea.Document.UndoStack.EndUndoGroup();
          _textArea.EndUpdate();
        }
      }
    }


    protected void OnDragOver(object sender, DragEventArgs e)
    {
      if (!_textArea.Focused)
      {
        _textArea.Focus();
      }

      Point p = _textArea.PointToClient(new Point(e.X, e.Y));

      if (_textArea.TextView.DrawingPosition.Contains(p.X, p.Y))
      {
        TextLocation realmousepos = _textArea.TextView.GetLogicalPosition(p.X - _textArea.TextView.DrawingPosition.X, p.Y - _textArea.TextView.DrawingPosition.Y);
        int lineNr = Math.Min(_textArea.Document.TotalNumberOfLines - 1, Math.Max(0, realmousepos.Y));
        _textArea.Caret.Position = new TextLocation(realmousepos.X, lineNr);
        _textArea.SetDesiredColumn();
        if (e.Data.GetDataPresent(typeof(string)) && !_textArea.IsReadOnly(_textArea.Caret.Offset))
          e.Effect = GetDragDropEffect(e);
        else
          e.Effect = DragDropEffects.None;
      }
      else
      {
        e.Effect = DragDropEffects.None;
      }
    }
  }
}
