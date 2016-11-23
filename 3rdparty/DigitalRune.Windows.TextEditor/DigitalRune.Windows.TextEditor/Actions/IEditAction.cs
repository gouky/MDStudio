using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Executes a complex action on a <see cref="TextArea"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// <see cref="IEditAction"/>s are usually complex actions, which manipulate
  /// the <see cref="TextArea"/> (document, caret position, current selection, etc.).
  /// Examples are: Cut, Copy, Paste, Select All, Page-Down, etc.
  /// </para>
  /// <para>
  /// An actions action is usually automatically triggered by the <see cref="TextArea"/>
  /// when it detects a certain keyboard shortcut. But you can also call actions
  /// manually from your application. For example: You can invoke an <see cref="IEditAction"/>
  /// when the user selects 'Cut' from your application menu.
  /// </para>
  /// <para>
  /// Action do not require additional parameters. They depend only on the current
  /// state of the <see cref="TextArea"/> (document, caret position, current
  /// selection, etc.).
  /// </para>
  /// <para>
  /// To define a new key for the <see cref="TextArea"/>, you must write a class 
  /// which implements this interface.
  /// </para>
  /// </remarks>
  public interface IEditAction
  {
    /// <value>
    /// An array of keys on which this edit action occurs.
    /// </value>
    Keys[] Keys
    {
      get;
      set;
    }

    /// <remarks>
    /// When the key which is defined per XML is pressed, this method will be launched.
    /// </remarks>
    void Execute(TextArea textArea);
  }


  /// <summary>
  /// Defines the base functionality for an <see cref="IEditAction"/>.
  /// </summary>
  public abstract class AbstractEditAction : IEditAction
  {
    Keys[] keys;


    /// <value>
    /// An array of keys on which this edit action occurs.
    /// </value>
    public Keys[] Keys
    {
      get { return keys; }
      set { keys = value; }
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">
    /// The text area on which to execute the action.
    /// </param>
    public abstract void Execute(TextArea textArea);


    /// <summary>
    /// Executes the action on the active <see cref="TextArea"/> of a
    /// <see cref="TextEditorControl"/>.
    /// </summary>
    /// <param name="textEditor">
    /// The text editor control on which the to execute the action.
    /// </param>
    public void Execute(TextEditorControl textEditor)
    {
      Execute(textEditor.ActiveTextAreaControl.TextArea);
    }
  }
}
