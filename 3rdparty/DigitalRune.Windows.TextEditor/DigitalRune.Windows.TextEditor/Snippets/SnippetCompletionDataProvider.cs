using System.Collections.Generic;
using System.Windows.Forms;

namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Provides the completion data for <see cref="Snippet"/>s (text templates).
  /// </summary>
  internal class SnippetCompletionDataProvider : AbstractCompletionDataProvider
  {
    private readonly IEnumerable<Snippet> _snippets;
    private readonly ImageList _imageList;
    private readonly int _imageIndex;


    /// <summary>
    /// Gets the image list that holds the image for completion data.
    /// </summary>
    /// <value>The image list that holds the image for completion data.</value>
    public override ImageList ImageList
    {
      get { return _imageList; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="SnippetCompletionDataProvider"/> class.
    /// </summary>
    /// <param name="snippets">The snippets (text template).</param>
    /// <param name="imageList">The image list.</param>
    /// <param name="imageIndex">Index of the image in the <paramref name="imageList"/>.</param>
    public SnippetCompletionDataProvider(IEnumerable<Snippet> snippets, ImageList imageList, int imageIndex)
    {
      _snippets = snippets;
      _imageList = imageList;
      _imageIndex = imageIndex;
    }


    /// <summary>
    /// Generates the completion data. This method is called by the text editor control.
    /// </summary>
    /// <param name="fileName">the name of the file.</param>
    /// <param name="textArea">The text area.</param>
    /// <param name="charTyped">The character typed.</param>
    /// <returns>The completion data.</returns>
    public override ICompletionData[] GenerateCompletionData(string fileName, TextArea textArea, char charTyped)
    {
      List<SnippetCompletionData> completionData = new List<SnippetCompletionData>();
      foreach (Snippet snippet in _snippets)
        completionData.Add(new SnippetCompletionData(snippet, _imageIndex));

      return completionData.ToArray();
    }
  }
}
