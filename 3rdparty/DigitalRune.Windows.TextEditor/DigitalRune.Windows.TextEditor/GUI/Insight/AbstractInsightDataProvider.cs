using System;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Insight
{
  /// <summary>
  /// Provides base functionality of an <see cref="IInsightDataProvider"/>.
  /// </summary>
  public abstract class AbstractInsightDataProvider : IInsightDataProvider
  {
    private TextArea _textArea;
    private int _defaultIndex = -1;


    /// <summary>
    /// Gets the index of the entry to initially select.
    /// </summary>
    /// <value>The index of the entry to initially select.</value>
    public int DefaultIndex
    {
      get { return _defaultIndex; }
      set { _defaultIndex = value; }
    }


    /// <summary>
    /// Gets the offset in the document where the arguments start.
    /// </summary>
    /// <value>The start offset of the method's arguments.</value>
    /// <remarks>
    /// <para>
    /// For example, if the document contains the text <c>"Foo(x, ...)"</c> 
    /// then <see cref="ArgumentStartOffset"/> would be <c>4</c>.
    /// </para>
    /// <para>
    /// <see cref="AbstractInsightDataProvider"/> requires this information to track
    /// the caret. When the caret moves before <see cref="ArgumentStartOffset"/>,
    /// then the insight window is closed automatically.
    /// </para>
    /// </remarks>
    protected abstract int ArgumentStartOffset { get; }


    /// <summary>
    /// Gets the <see cref="TextArea"/>.
    /// </summary>
    /// <value>The text area.</value>
    protected TextArea TextArea
    {
      get { return _textArea; }
    }


    /// <summary>
    /// Gets the number of available insight entries, e.g. the number of available
    /// overloads to call.
    /// </summary>
    /// <value>The number of available insight entries.</value>
    public abstract int InsightDataCount { get; }


    /// <summary>
    /// Gets the text to display in the insight window.
    /// </summary>
    /// <param name="number">The number of the active insight entry.
    /// Multiple insight entries might be multiple overloads of the same method.</param>
    /// <returns>The text to display, e.g. a multi-line string where
    /// the first line is the method definition, followed by a description.</returns>
    public abstract string GetInsightData(int number);


    /// <summary>
    /// Tells the insight provider to prepare its data.
    /// </summary>
    /// <param name="fileName">The name of the edited file.</param>
    /// <param name="textArea">The text area in which the file is being edited.</param>
    public void SetupDataProvider(string fileName, TextArea textArea)
    {
      _textArea = textArea;
      SetupDataProvider(fileName);
    }


    /// <summary>
    /// Tells the insight provider to prepare its data.
    /// </summary>
    /// <param name="fileName">The name of the edited file.</param>
    public abstract void SetupDataProvider(string fileName);


    /// <summary>
    /// Notifies the insight provider that the caret offset has changed.
    /// </summary>
    /// <returns>
    /// Return <see langword="true"/> to close the insight window (e.g. when the
    /// caret was moved outside the region where insight is displayed for).
    /// Return <see langword="false"/> to keep the window open.
    /// </returns>
    public virtual bool CaretOffsetChanged()
    {
      IDocument document = _textArea.Document;
      bool closeDataProvider = _textArea.Caret.Offset <= ArgumentStartOffset;
      int brackets = 0;
      int curlyBrackets = 0;
      if (!closeDataProvider)
      {
        bool insideChar = false;
        bool insideString = false;
        for (int offset = ArgumentStartOffset; offset < Math.Min(_textArea.Caret.Offset, document.TextLength); ++offset)
        {
          char ch = document.GetCharAt(offset);
          switch (ch)
          {
            case '\'':
              insideChar = !insideChar;
              break;
            case '(':
              if (!(insideChar || insideString))
              {
                ++brackets;
              }
              break;
            case ')':
              if (!(insideChar || insideString))
              {
                --brackets;
              }
              if (brackets <= 0)
              {
                return true;
              }
              break;
            case '"':
              insideString = !insideString;
              break;
            case '}':
              if (!(insideChar || insideString))
              {
                --curlyBrackets;
              }
              if (curlyBrackets < 0)
              {
                return true;
              }
              break;
            case '{':
              if (!(insideChar || insideString))
              {
                ++curlyBrackets;
              }
              break;
            case ';':
              if (!(insideChar || insideString))
              {
                return true;
              }
              break;
          }
        }
      }
      return closeDataProvider;
    }
  }
}
