namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Cuts the currently selected text and places it in the Windows clipboard.
  /// </summary>
  public class Cut : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Document.ReadOnly)
        return;

      textArea.ClipboardHandler.Cut();
    }
  }


  /// <summary>
  /// Copies the currently selected text and places it in the Windows clipboard.
  /// </summary>
  public class Copy : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.AutoClearSelection = false;
      textArea.ClipboardHandler.Copy();
    }
  }


  /// <summary>
  /// Pastes the content of the Windows clipboard into the <see cref="TextArea"/>
  /// at the current position.
  /// </summary>
  public class Paste : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Document.ReadOnly)
        return;

      textArea.ClipboardHandler.Paste();
    }
  }
}
