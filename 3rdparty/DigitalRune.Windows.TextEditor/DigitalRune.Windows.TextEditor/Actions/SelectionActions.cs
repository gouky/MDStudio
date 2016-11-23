using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Moves the caret to the right and extend the selection. 
  /// </summary>
  public class ShiftCaretRight : CaretRight
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret to the left and extends the selection.
  /// </summary>
  public class ShiftCaretLeft : CaretLeft
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret up and extends the selection.
  /// </summary>
  public class ShiftCaretUp : CaretUp
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret down and extends the selection.
  /// </summary>
  public class ShiftCaretDown : CaretDown
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret to the right by one word and extends the selection.
  /// </summary>
  public class ShiftWordRight : WordRight
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret to the left by one word and extends the selection.
  /// </summary>
  public class ShiftWordLeft : WordLeft
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret to the first position in the line and extends the selection
  /// to this position.
  /// </summary>
  public class ShiftHome : Home
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret to the end of the line and extends the selection to this 
  /// position.
  /// </summary>
  public class ShiftEnd : End
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret to the start and extends the selection.
  /// </summary>
  public class ShiftMoveToStart : MoveToStart
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret to the end and extends the selection.
  /// </summary>
  public class ShiftMoveToEnd : MoveToEnd
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret up by one page and extends the selection.
  /// </summary>
  public class ShiftMovePageUp : MovePageUp
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    /// <remarks>
    /// Executes this edit action
    /// </remarks>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Moves the caret down by one page and extends the selection.
  /// </summary>
  public class ShiftMovePageDown : MovePageDown
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldCaretPos = textArea.Caret.Position;
      base.Execute(textArea);
      textArea.AutoClearSelection = false;
      textArea.SelectionManager.ExtendSelection(oldCaretPos, textArea.Caret.Position);
    }
  }


  /// <summary>
  /// Selects the whole document.
  /// </summary>
  public class SelectWholeDocument : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.AutoClearSelection = false;
      TextLocation startPoint = new TextLocation(0, 0);
      TextLocation endPoint = textArea.Document.OffsetToPosition(textArea.Document.TextLength);
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        if (textArea.SelectionManager.Selections[0].StartPosition == startPoint &&
            textArea.SelectionManager.Selections[0].EndPosition == endPoint)
        {
          return;
        }
      }
      textArea.Caret.Position = textArea.SelectionManager.NextValidPosition(endPoint.Y);
      textArea.SelectionManager.ExtendSelection(startPoint, endPoint);
      // after a SelectWholeDocument selection, the caret is placed correctly,
      // but it is not positioned internally.  The effect is when the cursor
      // is moved up or down a line, the caret will take on the column that
      // it was in before the SelectWholeDocument
      textArea.SetDesiredColumn();
    }
  }


  /// <summary>
  /// Clears all selections.
  /// </summary>
  public class ClearAllSelections : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.SelectionManager.ClearSelection();
    }
  }
}
