using System;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Undo
{
  /// <summary>
  /// Implements an undoable delete operation for the <see cref="IDocument"/>.
  /// </summary>
  internal class UndoableDelete : IUndoableOperation
  {
    private readonly IDocument document;
    private readonly int offset;
    private readonly string text;


    /// <summary>
    /// Creates a new instance of <see cref="UndoableDelete"/>
    /// </summary>	
    public UndoableDelete(IDocument document, int offset, string text)
    {
      if (document == null)
        throw new ArgumentNullException("document");

      if (offset < 0 || offset > document.TextLength)
        throw new ArgumentOutOfRangeException("offset");

      Debug.Assert(text != null, "text can't be null");
      this.document = document;
      this.offset = offset;
      this.text = text;
    }


    /// <summary>
    /// Undoes the last operation
    /// </summary>
    public void Undo()
    {
      // we clear all selection direct, because the redraw
      // is done per refresh at the end of the action
      //			textArea.SelectionManager.Selections.Clear();
      document.UndoStack.AcceptChanges = false;
      document.Insert(offset, text);
      document.UndoStack.AcceptChanges = true;
    }


    /// <summary>
    /// Redoes the last operation
    /// </summary>
    public void Redo()
    {
      // we clear all selection direct, because the redraw
      // is done per refresh at the end of the action
      //			textArea.SelectionManager.SelectionCollection.Clear();

      document.UndoStack.AcceptChanges = false;
      document.Remove(offset, text.Length);
      document.UndoStack.AcceptChanges = true;
    }
  }
}
