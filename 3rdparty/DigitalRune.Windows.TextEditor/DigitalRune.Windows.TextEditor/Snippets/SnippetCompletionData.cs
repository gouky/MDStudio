using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Completion data for a <see cref="Snippet"/> (text template).
  /// </summary>
  public class SnippetCompletionData : ICompletionData
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    
    private readonly Snippet _snippet;
    private readonly int _imageIndex;
    #endregion


    //--------------------------------------------------------------
    #region Properties
    //--------------------------------------------------------------
    
    /// <summary>
    /// Gets the index of the image in the <see cref="ImageList"/> of the 
    /// <see cref="CompletionWindow"/>.
    /// </summary>
    /// <value>The index of the image.</value>
    public int ImageIndex
    {
      get { return _imageIndex; }
    }


    /// <summary>
    /// Gets or sets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get { return _snippet.Shortcut; }
    }


    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description
    {
      get { return _snippet.Description + '\n' + _snippet.Text; }
    }


    /// <summary>
    /// Gets a priority value for the completion data item.
    /// When selecting items by their start characters, the item with the highest
    /// priority is selected first.
    /// </summary>
    /// <value></value>
    public double Priority
    {
      get { return 0; }
    }
    #endregion


    //--------------------------------------------------------------
    #region Creation and Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="SnippetCompletionData"/> class.
    /// </summary>
    /// <param name="snippet">The snippet.</param>
    /// <param name="imageIndex">Index of the image in the <see cref="ImageList"/> of the 
    /// <see cref="CompletionWindow"/>.</param>
    public SnippetCompletionData(Snippet snippet, int imageIndex)
    {
      _snippet = snippet;
      _imageIndex = imageIndex;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Insert the element represented by the completion data into the text
    /// editor.
    /// </summary>
    /// <param name="textArea">TextArea to insert the completion data in.</param>
    /// <param name="ch">Character that should be inserted after the completion data. \0 when no
    /// character should be inserted.</param>
    /// <returns>
    /// Returns true when the insert action has processed the character
    /// <paramref name="ch"/>; false when the character was not processed.
    /// </returns>
    public bool InsertAction(TextArea textArea, char ch)
    {
      _snippet.Insert(textArea);
      return false;
    }
    #endregion
  }
}