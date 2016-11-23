using System;


namespace DigitalRune.Windows.TextEditor.Insight
{
  /// <summary>
  /// Provides arguments for an method insight event.
  /// </summary>
  [Serializable]
  public class InsightEventArgs : EventArgs
  {
    /// <summary>
    /// The default <see cref="InsightEventArgs"/>.
    /// </summary>
    public new static readonly InsightEventArgs Empty = new InsightEventArgs();


    private readonly char _ch = '\0';

    /// <summary>
    /// Gets the character that was inserted and raised this event.
    /// </summary>
    /// <value>The character. <c>'\0'</c> if no character was inserted.</value>
    public char Key
    {
      get { return _ch; }
    }

    
    /// <summary>
    /// Constructs a new instance of the <see cref="InsightEventArgs" /> class.
    /// </summary>
    public InsightEventArgs()
    {
    }

  
    /// <summary>
    /// Constructs a new instance of the <see cref="InsightEventArgs"/> class.
    /// </summary>
    /// <param name="ch">The character that is going to be inserted.</param>
    public InsightEventArgs(char ch)
    {
      _ch = ch;
    }
  }
}
