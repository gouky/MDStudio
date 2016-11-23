using System;
using System.Collections.Generic;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Undo
{
  /// <summary>
  /// Implements an undo stack.
  /// </summary>
  public class UndoStack
  {
    private readonly Stack<IUndoableOperation> undostack = new Stack<IUndoableOperation>();
    private readonly Stack<IUndoableOperation> redostack = new Stack<IUndoableOperation>();


    /// <summary>
    /// The <see cref="TextEditorControl"/> that owns this <see cref="UndoStack"/>.
    /// </summary>
    public TextEditorControl TextEditorControl;


    /// <summary>
    /// Occurs after action is undone.
    /// </summary>
    public event EventHandler ActionUndone;


    /// <summary>
    /// Occurs after action is redone.
    /// </summary>
    public event EventHandler ActionRedone;


    /// <summary>
    /// Occurs when an operation is pushed onto the undo stack.
    /// </summary>
    public event OperationEventHandler OperationPushed;
		

    /// <summary>
    /// Gets or sets if changes to the document are protocoled by the undo stack.
    /// Used internally to disable the undo stack temporarily while undoing an action.
    /// </summary>
    internal bool AcceptChanges = true;


    /// <summary>
    /// Returns whether there are actions on the undo stack.
    /// </summary>
    public bool CanUndo
    {
      get { return undostack.Count > 0; }
    }


    /// <summary>
    /// Returns whether there are actions on the redo stack.
    /// </summary>
    public bool CanRedo
    {
      get { return redostack.Count > 0; }
    }


    /// <summary>
    /// Gets the number of actions on the undo stack.
    /// </summary>
    public int UndoItemCount
    {
      get { return undostack.Count; }
    }


    /// <summary>
    /// Gets the number of actions on the redo stack.
    /// </summary>
    public int RedoItemCount
    {
      get { return redostack.Count; }
    }


    int undoGroupDepth;
    int actionCountInUndoGroup;

    /// <summary>
    /// Starts a new undo group.
    /// </summary>
    /// <remarks>
    /// An undo group is a group of commandos that is combined into a single
    /// undo operation.
    /// </remarks>
    public void StartUndoGroup()
    {
      if (undoGroupDepth == 0)
      {
        actionCountInUndoGroup = 0;
      }
      undoGroupDepth++;
    }


    /// <summary>
    /// Ends an undo group and pushes the group operations onto the <see cref="UndoStack"/>.
    /// </summary>
    /// <remarks>
    /// An undo group is a group of commandos that is combined into a single
    /// undo operation.
    /// </remarks>
    public void EndUndoGroup()
    {
      if (undoGroupDepth == 0)
        throw new InvalidOperationException("There are no open undo groups");
      undoGroupDepth--;
      if (undoGroupDepth == 0 && actionCountInUndoGroup > 1)
      {
        UndoQueue op = new UndoQueue(undostack, actionCountInUndoGroup);
        undostack.Push(op);
        if (OperationPushed != null)
        {
          OperationPushed(this, new OperationEventArgs(op));
        }
      }
    }


    /// <summary>
    /// Asserts that no undo groups are open.
    /// </summary>
    [Conditional("DEBUG")]
    void AssertNoUndoGroupOpen()
    {
      if (undoGroupDepth != 0)
      {
        undoGroupDepth = 0;
        throw new InvalidOperationException("No undo group should be open at this point");
      }
    }



    /// <summary>
    /// Undoes the last operation.
    /// </summary>
    public void Undo()
    {
      AssertNoUndoGroupOpen();
      if (undostack.Count > 0)
      {
        IUndoableOperation uedit = undostack.Pop();
        redostack.Push(uedit);
        uedit.Undo();
        OnActionUndone();
      }
    }


    /// <summary>
    /// Redoes the last undone operation.
    /// </summary>
    public void Redo()
    {
      AssertNoUndoGroupOpen();
      if (redostack.Count > 0)
      {
        IUndoableOperation uedit = redostack.Pop();
        undostack.Push(uedit);
        uedit.Redo();
        OnActionRedone();
      }
    }


    /// <summary>
    /// Call this method to push an UndoableOperation on the undo-stack; the redo-stack
    /// will be cleared.
    /// </summary>
    public void Push(IUndoableOperation operation)
    {
      if (operation == null)
      {
        throw new ArgumentNullException("operation");
      }

      if (AcceptChanges)
      {
        StartUndoGroup();
        undostack.Push(operation);
        actionCountInUndoGroup++;
        if (TextEditorControl != null)
        {
          undostack.Push(new UndoableSetCaretPosition(this, TextEditorControl.ActiveTextAreaControl.Caret.Position));
          actionCountInUndoGroup++;
        }
        EndUndoGroup();
        ClearRedoStack();
      }
    }


    /// <summary>
    /// Clears the redo stack.
    /// </summary>
    public void ClearRedoStack()
    {
      redostack.Clear();
    }


    /// <summary>
    /// Clears both the undo and redo stack.
    /// </summary>
    public void ClearAll()
    {
      AssertNoUndoGroupOpen();
      undostack.Clear();
      redostack.Clear();
      actionCountInUndoGroup = 0;
    }


    /// <summary>
    /// Raises the <see cref="ActionUndone"/> event.
    /// </summary>
    protected void OnActionUndone()
    {
      if (ActionUndone != null)
        ActionUndone(null, null);
    }


    /// <summary>
    /// Raises the <see cref="ActionRedone"/> event.
    /// </summary>
    protected void OnActionRedone()
    {
      if (ActionRedone != null)
        ActionRedone(null, null);
    }


    private class UndoableSetCaretPosition : IUndoableOperation
    {
      private readonly UndoStack stack;
      private readonly TextLocation pos;
      private TextLocation redoPos;

      public UndoableSetCaretPosition(UndoStack stack, TextLocation pos)
      {
        this.stack = stack;
        this.pos = pos;
      }

      public void Undo()
      {
        redoPos = stack.TextEditorControl.ActiveTextAreaControl.Caret.Position;
        stack.TextEditorControl.ActiveTextAreaControl.Caret.Position = pos;
        stack.TextEditorControl.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
      }

      public void Redo()
      {
        stack.TextEditorControl.ActiveTextAreaControl.Caret.Position = redoPos;
        stack.TextEditorControl.ActiveTextAreaControl.TextArea.SelectionManager.ClearSelection();
      }
    }
  }



  /// <summary>
  /// Provides data for the <see cref="UndoStack.OperationPushed"/> events.
  /// </summary>
  public class OperationEventArgs : EventArgs
  {
    private readonly IUndoableOperation _op;


    /// <summary>
    /// Initializes a new instance of the <see cref="OperationEventArgs"/> class.
    /// </summary>
    /// <param name="op">The operation.</param>
    public OperationEventArgs(IUndoableOperation op)
    {
      _op = op;
    }


    /// <summary>
    /// Gets the operation.
    /// </summary>
    /// <value>The operation.</value>
    public IUndoableOperation Operation
    {
      get { return _op; }
    }
  }


  /// <summary>
  /// Represents the method that will handle the <see cref="UndoStack.OperationPushed"/> event.
  /// </summary>
  /// <param name="sender">The source of the event.</param>
  /// <param name="e">A <see cref="OperationEventArgs"/> that contains the event data.</param>
  public delegate void OperationEventHandler(object sender, OperationEventArgs e);
}
