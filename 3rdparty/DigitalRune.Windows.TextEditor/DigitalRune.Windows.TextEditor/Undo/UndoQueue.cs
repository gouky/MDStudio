using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DigitalRune.Windows.TextEditor.Undo
{
  /// <summary>
  /// This class stacks the last x operations from the <see cref="UndoStack"/> and makes
  /// one undo/redo operation from it.
  /// </summary>
  internal sealed class UndoQueue : IUndoableOperation
  {
    private readonly List<IUndoableOperation> undolist = new List<IUndoableOperation>();


    /// <summary>
    /// Initializes a new instance of the <see cref="UndoQueue"/> class.
    /// </summary>
    /// <param name="stack">The undo stack.</param>
    /// <param name="numops">The number of operations.</param>
    public UndoQueue(Stack<IUndoableOperation> stack, int numops)
    {
      if (stack == null)
        throw new ArgumentNullException("stack");

      Debug.Assert(numops > 0, "DigitalRune.Windows.TextEditor.Undo.UndoQueue : numops should be > 0");
      if (numops > stack.Count)
        numops = stack.Count;

      for (int i = 0; i < numops; ++i)
        undolist.Add(stack.Pop());
    }


    /// <summary>
    /// Undoes the last operation
    /// </summary>
    public void Undo()
    {
      for (int i = 0; i < undolist.Count; ++i)
        undolist[i].Undo();
    }


    /// <summary>
    /// Redoes the last operation
    /// </summary>
    public void Redo()
    {
      for (int i = undolist.Count - 1; i >= 0; --i)
        undolist[i].Redo();
    }
  }
}
