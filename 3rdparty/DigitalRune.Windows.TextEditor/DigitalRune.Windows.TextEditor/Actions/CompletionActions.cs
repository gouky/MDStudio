namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Shows the code completion window.
  /// </summary>
  public class CodeCompletion : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.MotherTextEditorControl.RequestCompletionWindow();
    }
  }


  /// <summary>
  /// Shows the method insight window.
  /// </summary>
  public class Insight : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.MotherTextEditorControl.RequestInsightWindow();
    }
  }
}
