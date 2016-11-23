using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Describes the action that shall be run with a key-press.
  /// </summary>
  public enum CompletionDataProviderKeyResult
  {
    /// <summary>
    /// Normal key, used to choose an entry from the completion list
    /// </summary>
    NormalKey,
    /// <summary>
    /// This key triggers insertion of the completed expression
    /// </summary>
    InsertionKey,
    /// <summary>
    /// Increment both start and end offset of completion region when inserting this
    /// key. Can be used to insert whitespace (or other characters) in front of the expression
    /// while the completion window is open.
    /// </summary>
    BeforeStartKey
  }


  /// <summary>
  /// Generates the completion data for a <see cref="CompletionWindow"/> and
  /// checks key presses to trigger an insertion.
  /// </summary>
  public interface ICompletionDataProvider
  {
    /// <summary>
    /// Gets the image list that holds the image for completion data.
    /// </summary>
    /// <value>The image list that holds the image for completion data.</value>
    ImageList ImageList { get; }


    /// <summary>
    /// Gets or sets the pre-selection.
    /// </summary>
    /// <value>The pre-selection.</value>
    /// <remarks>
    /// <para>
    /// The pre-selection is string that is used to select the entry in the 
    /// completion window, when creating the window. When no pre-selection is
    /// set, then no entry in the completion window will be selected. 
    /// </para>
    /// <example>
    /// Here is an example how to use the pre-selection: Imagine you have method
    /// <c>MyClass.MethodXyz()</c>. When the user types <c>myClass.Meth</c> and 
    /// then calls the completion window, you should use the string <c>Meth</c>
    /// as pre-selection. The completion window can automatically select the 
    /// first entry that starts with <c>Meth</c>.
    /// </example>
    /// </remarks>
    string PreSelection
    {
      get;
      set;
    }


    /// <summary>
    /// Gets the index of the element in the list that is chosen by default.
    /// </summary>
    int DefaultIndex { get; }


    /// <summary>
    /// Generates the completion data. This method is called by the text editor control.
    /// </summary>
    /// <param name="fileName">the name of the file.</param>
    /// <param name="textArea">The text area.</param>
    /// <param name="charTyped">The character typed.</param>
    /// <returns>The completion data.</returns>
    ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);


    /// <summary>
    /// Processes a key press and returns the action to be run with the key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>The action that shall be performed.</returns>
    CompletionDataProviderKeyResult ProcessKey(char key);


    /// <summary>
    /// Executes the insertion.
    /// </summary>
    /// <param name="data">The completion data.</param>
    /// <param name="textArea">The text area.</param>
    /// <param name="insertionOffset">The insertion offset.</param>
    /// <param name="key">The key.</param>
    /// <returns>
    /// Returns <see langword="true"/> when the insert action has processed the character
    /// <paramref name="key"/>; <see langword="false"/> when the character was not processed.
    /// </returns>
    /// <remarks>
    /// The provider should set the caret position and then call <see cref="ICompletionData.InsertAction"/>
    /// on <paramref name="data"/>
    /// </remarks>
    bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key);
  }
}
