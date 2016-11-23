using System;
using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Provides base functionality of an <see cref="ICompletionDataProvider"/>.
  /// </summary>
  public abstract class AbstractCompletionDataProvider : ICompletionDataProvider
  {
    private int _defaultIndex = -1;
    private string _preSelection;
    private bool _insertSpace;
    private char[] _commitKeys = { '{', '}', '[', ']', '(', ')', '.', ',', ':', ';', 
                                   '+', '-', '*', '/', '%', '&', '|', '^', '!', '~', 
                                   '=', '<', '>', '?', '@', '#', '\'', '"', '\\' };

    
    /// <summary>
    /// Gets the image list that holds the images for completion data.
    /// </summary>
    /// <value>The image list that holds the images for completion data.</value>
    public abstract ImageList ImageList
    {
      get;
    }


    /// <summary>
    /// Gets the index of the element in the list that is chosen by default.
    /// </summary>
    public int DefaultIndex
    {
      get { return _defaultIndex; }
      set { _defaultIndex = value; }
    }


    /// <summary>
    /// Gets or sets the pre-selection.
    /// </summary>
    /// <value>The pre-selection.</value>
    /// <remarks>
    /// 	<para>
    /// The pre-selection is string that is used to select the entry in the
    /// completion window, when creating the window. When no pre-selection is
    /// set, then no entry in the completion window will be selected.
    /// </para>
    /// 	<example>
    /// Here is an example how to use the pre-selection: Imagine you have method
    /// <c>MyClass.MethodXyz()</c>. When the user types <c>myClass.Meth</c> and
    /// then calls the completion window, you should use the string <c>Meth</c>
    /// as pre-selection. The completion window can automatically select the
    /// first entry that starts with <c>Meth</c>.
    /// </example>
    /// </remarks>
    public string PreSelection
    {
      get { return _preSelection ?? String.Empty; }
      set { _preSelection = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether a space should be inserted in front of the 
    /// completed expression.
    /// </summary>
    public bool InsertSpace
    {
      get { return _insertSpace; }
      set { _insertSpace = value; }
    }


    /// <summary>
    /// Gets or sets the characters that commit the completion and trigger the
    /// text insertion.
    /// </summary>
    /// <value>The commit triggers.</value>
    public char[] CommitKeys
    {
      get { return _commitKeys; }
      set { _commitKeys = value; }
    }


    /// <summary>
    /// Generates the completion data. This method is called by the text editor control.
    /// </summary>
    /// <param name="fileName">The name of the file.</param>
    /// <param name="textArea">The text area.</param>
    /// <param name="charTyped">The character typed.</param>
    /// <returns>The completion data.</returns>
    public abstract ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped);


    /// <summary>
    /// Gets if pressing 'key' should trigger the insertion of the currently selected element.
    /// </summary>
    public virtual CompletionDataProviderKeyResult ProcessKey(char key)
    {
      CompletionDataProviderKeyResult res;
      if (key == ' ' && _insertSpace)
      {
        _insertSpace = false; // insert space only once
        res = CompletionDataProviderKeyResult.BeforeStartKey;
      }
      else if (key == '\t' || key == ' ' || IsCommitKey(key))
      {
        // Do not reset insertSpace when doing an insertion.
        res = CompletionDataProviderKeyResult.InsertionKey;
      }
      else
      {
        // Don't insert space if user types normally
        _insertSpace = false; 
        res = CompletionDataProviderKeyResult.NormalKey;
      }
      return res;
    }


    /// <summary>
    /// Determines whether a key shall commit the completion.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>
    /// 	<see langword="true"/> if <paramref name="key"/> shall commit the completion; otherwise, <see langword="false"/>.
    /// </returns>
    private bool IsCommitKey(char key)
    {
      if (_commitKeys != null)
        foreach (char commitKey in _commitKeys)
          if (key == commitKey)
            return true;

      return false;
    }


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
    public virtual bool InsertAction(ICompletionData data, TextArea textArea, int insertionOffset, char key)
    {
      if (InsertSpace)
        textArea.Document.Insert(insertionOffset++, " ");
      
      textArea.Caret.Position = textArea.Document.OffsetToPosition(insertionOffset);
      return data.InsertAction(textArea, key);
    }
  }
}
