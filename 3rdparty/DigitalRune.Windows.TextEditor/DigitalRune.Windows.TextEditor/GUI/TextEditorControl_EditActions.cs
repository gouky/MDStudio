using System.Collections.Generic;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Actions;


namespace DigitalRune.Windows.TextEditor
{
  partial class TextEditorControl
  {
    /// <summary>
    /// This dictionary contains all editor keys, where the key is the key combination and the 
    /// value the action.
    /// </summary>
    protected Dictionary<Keys, IEditAction> EditActions = new Dictionary<Keys, IEditAction>();


    /// <summary>
    /// Determines whether a certain key (or key combination) is associated with an
    /// edit action.
    /// </summary>
    /// <param name="keyData">The key data.</param>
    /// <returns>
    /// 	<see langword="true"/> if the key is associated with an edit action; otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsEditAction(Keys keyData)
    {
      return EditActions.ContainsKey(keyData);
    }


    /// <summary>
    /// Gets the edit action of a given key.
    /// </summary>
    /// <param name="keyData">The key data.</param>
    /// <returns>The edit action. (<see langword="null"/> if no action is bound to <paramref name="keyData"/>.)</returns>
    internal IEditAction GetEditAction(Keys keyData)
    {
      if (!IsEditAction(keyData))
        return null;

      return EditActions[keyData];
    }


    void GenerateDefaultActions()
    {
      EditActions[Keys.Left] = new CaretLeft();
      EditActions[Keys.Left | Keys.Shift] = new ShiftCaretLeft();
      EditActions[Keys.Left | Keys.Control] = new WordLeft();
      EditActions[Keys.Left | Keys.Control | Keys.Shift] = new ShiftWordLeft();
      EditActions[Keys.Right] = new CaretRight();
      EditActions[Keys.Right | Keys.Shift] = new ShiftCaretRight();
      EditActions[Keys.Right | Keys.Control] = new WordRight();
      EditActions[Keys.Right | Keys.Control | Keys.Shift] = new ShiftWordRight();
      EditActions[Keys.Up] = new CaretUp();
      EditActions[Keys.Up | Keys.Shift] = new ShiftCaretUp();
      EditActions[Keys.Up | Keys.Control] = new ScrollLineUp();
      EditActions[Keys.Down] = new CaretDown();
      EditActions[Keys.Down | Keys.Shift] = new ShiftCaretDown();
      EditActions[Keys.Down | Keys.Control] = new ScrollLineDown();

      EditActions[Keys.Insert] = new ToggleEditMode();
      EditActions[Keys.Insert | Keys.Control] = new Copy();
      EditActions[Keys.Insert | Keys.Shift] = new Paste();
      EditActions[Keys.Delete] = new Delete();
      EditActions[Keys.Delete | Keys.Shift] = new Cut();
      EditActions[Keys.Home] = new Home();
      EditActions[Keys.Home | Keys.Shift] = new ShiftHome();
      EditActions[Keys.Home | Keys.Control] = new MoveToStart();
      EditActions[Keys.Home | Keys.Control | Keys.Shift] = new ShiftMoveToStart();
      EditActions[Keys.End] = new End();
      EditActions[Keys.End | Keys.Shift] = new ShiftEnd();
      EditActions[Keys.End | Keys.Control] = new MoveToEnd();
      EditActions[Keys.End | Keys.Control | Keys.Shift] = new ShiftMoveToEnd();
      EditActions[Keys.PageUp] = new MovePageUp();
      EditActions[Keys.PageUp | Keys.Shift] = new ShiftMovePageUp();
      EditActions[Keys.PageDown] = new MovePageDown();
      EditActions[Keys.PageDown | Keys.Shift] = new ShiftMovePageDown();

      EditActions[Keys.Return] = new Return();
      EditActions[Keys.Tab] = new Tab();
      EditActions[Keys.Tab | Keys.Shift] = new ShiftTab();
      EditActions[Keys.Back] = new Backspace();
      EditActions[Keys.Back | Keys.Shift] = new Backspace();

      EditActions[Keys.X | Keys.Control] = new Cut();
      EditActions[Keys.C | Keys.Control] = new Copy();
      EditActions[Keys.V | Keys.Control] = new Paste();

      EditActions[Keys.A | Keys.Control] = new SelectWholeDocument();
      EditActions[Keys.Escape] = new ClearAllSelections();

      EditActions[Keys.Divide | Keys.Control] = new ToggleComment();
      EditActions[Keys.OemQuestion | Keys.Control] = new ToggleComment();

      EditActions[Keys.Back | Keys.Alt] = new Actions.Undo();
      EditActions[Keys.Z | Keys.Control] = new Actions.Undo();
      EditActions[Keys.Y | Keys.Control] = new Redo();

      EditActions[Keys.Delete | Keys.Control] = new DeleteWord();
      EditActions[Keys.Back | Keys.Control] = new WordBackspace();
      EditActions[Keys.D | Keys.Control] = new DeleteLine();
      EditActions[Keys.D | Keys.Shift | Keys.Control] = new DeleteToLineEnd();

      EditActions[Keys.B | Keys.Control] = new GotoMatchingBrace();

      EditActions[Keys.Control | Keys.Space] = new CodeCompletion();
      EditActions[Keys.Alt | Keys.Right] = new CodeCompletion();
    }
  }
}
