using System;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Undo
{
  /// <summary>
  /// Implements an undoable replace operation for the <see cref="IDocument"/>.
  /// </summary>
  internal class UndoableReplace : IUndoableOperation
  {
    private readonly IDocument document;
    private readonly int offset;
    private readonly string text;
    private readonly string origText;


    /// <summary>
    /// Creates a new instance of <see cref="UndoableReplace"/>
    /// </summary>	
    public UndoableReplace(IDocument document, int offset, string origText, string text)
    {
      if (document == null)
        throw new ArgumentNullException("document");

      if (offset < 0 || offset > document.TextLength)
        throw new ArgumentOutOfRangeException("offset");

      Debug.Assert(text != null, "text can't be null");
      this.document = document;
      this.offset = offset;
      this.text = text;
      this.origText = origText;
    }


    /// <summary>
    /// Undoes the last operation
    /// </summary>
    public void Undo()
    {
      // we clear all selection direct, because the redraw
      // is done per refresh at the end of the action
      //			document.Selections.Clear();

      document.UndoStack.AcceptChanges = false;
      document.Replace(offset, text.Length, origText);
      document.UndoStack.AcceptChanges = true;
    }


    /// <summary>
    /// Redoes the last operation
    /// </summary>
    public void Redo()
    {
      // we clear all selection direct, because the redraw
      // is done per refresh at the end of the action
      //			document.SelectionCollection.Clear();

      document.UndoStack.AcceptChanges = false;
      document.Replace(offset, origText.Length, text);
      document.UndoStack.AcceptChanges = true;
    }
  }
}
