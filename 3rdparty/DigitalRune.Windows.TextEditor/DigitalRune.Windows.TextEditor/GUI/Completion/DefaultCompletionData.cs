using System;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// The default implementation for <see cref="ICompletionData"/>.
  /// </summary>
  public class DefaultCompletionData : ICompletionData
  {
    private readonly string _text;
    private readonly string _description;
    private readonly int _imageIndex;
    private double _priority;


    /// <summary>
    /// Gets the index of the image.
    /// </summary>
    /// <value>The index of the image.</value>
    public int ImageIndex
    {
      get { return _imageIndex; }
    }


    /// <summary>
    /// Gets or sets the completion text.
    /// </summary>
    /// <value>The completion text.</value>
    public string Text
    {
      get { return _text; }
    }


    /// <summary>
    /// Gets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description
    {
      get { return _description; }
    }


    /// <summary>
    /// Gets a priority value for the completion data item.
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// When selecting items by their start characters, the item with the highest
    /// priority is selected first.
    /// </remarks>
    public double Priority
    {
      get { return _priority; }
      set { _priority = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultCompletionData"/> class.
    /// </summary>
    /// <param name="text">The completion text.</param>
    /// <param name="description">The description.</param>
    /// <param name="imageIndex">Index of the image.</param>
    public DefaultCompletionData(string text, string description, int imageIndex)
    {
      _text = text;
      _description = description;
      _imageIndex = imageIndex;
    }


    /// <summary>
    /// Insert the element represented by the completion data into the text
    /// editor.
    /// </summary>
    /// <param name="textArea">TextArea to insert the completion data in.</param>
    /// <param name="ch">Character that should be inserted after the completion data. <c>'\0'</c>
    /// when no character should be inserted.</param>
    /// <returns>
    /// Returns true when the insert action has processed the character
    /// <paramref name="ch"/>; false when the character was not processed.
    /// </returns>
    public virtual bool InsertAction(TextArea textArea, char ch)
    {
      textArea.InsertString(_text);
      return false;
    }


    /// <summary>
    /// Compares two instances of <see cref="ICompletionData"/>.
    /// </summary>
    /// <param name="a">The first instance of <see cref="ICompletionData"/>.</param>
    /// <param name="b">The second instance of <see cref="ICompletionData"/>.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance is less than obj. Zero This instance is equal to obj. Greater than zero This instance is greater than obj.
    /// </returns>
    public static int Compare(ICompletionData a, ICompletionData b)
    {
      if (a == null)
        throw new ArgumentNullException("a");
      if (b == null)
        throw new ArgumentNullException("b");

      return string.Compare(a.Text, b.Text, StringComparison.InvariantCultureIgnoreCase);
    }
  }
}