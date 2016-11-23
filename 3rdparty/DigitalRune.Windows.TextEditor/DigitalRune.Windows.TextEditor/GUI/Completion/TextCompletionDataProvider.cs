using System;
using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Provides completion data for simple texts.
  /// </summary>
  /// <remarks>
  /// This <see cref="ICompletionDataProvider"/> provides completion data for 
  /// one type: Simple texts (<see cref="String"/>s). The text in the completion 
  /// window is copied directly into the document.
  /// </remarks>
  public class TextCompletionDataProvider : AbstractCompletionDataProvider
  {
    private readonly string[] _texts;
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
    /// Initializes a new instance of the <see cref="TextCompletionDataProvider"/> class.
    /// </summary>
    /// <param name="texts">The completion texts.</param>
    /// <param name="imageList">The image list.</param>
    /// <param name="imageIndex">Index of the image in the <paramref name="imageList"/>.</param>
    public TextCompletionDataProvider(string[] texts, ImageList imageList, int imageIndex)
    {
      if (texts == null)
        throw new ArgumentNullException("texts");

      _texts = texts;
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
      ICompletionData[] completionData = new ICompletionData[_texts.Length];
      for (int i = 0; i < completionData.Length; i++)
        completionData[i] = new DefaultCompletionData(_texts[i], null, _imageIndex);
      return completionData;
    }
  }
}
