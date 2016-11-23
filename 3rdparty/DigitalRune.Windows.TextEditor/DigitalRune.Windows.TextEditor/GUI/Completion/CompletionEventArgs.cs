using System;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Provides arguments for an code completion event.
  /// </summary>
  [Serializable]
  public class CompletionEventArgs : EventArgs
  {
    /// <summary>
    /// Default <see cref="CompletionEventArgs"/>.
    /// </summary>
    public new static readonly CompletionEventArgs Empty = new CompletionEventArgs();


    private readonly char _ch = '\0';
    private readonly object _userData;


    /// <summary>
    /// Gets the character that was inserted and raised this event.
    /// </summary>
    /// <value>The character. <c>'\0'</c> if no character was inserted.</value>
    public char Key
    {
      get { return _ch; }
    }


    /// <summary>
    /// Gets custom data specified by the caller of the completion window.
    /// </summary>
    /// <value>The user data.</value>
    public object UserData
    {
      get { return _userData; }  
    }


    /// <summary>
    /// Constructs a new instance of the <see cref="CompletionEventArgs" /> class.
    /// </summary>
    public CompletionEventArgs()
    {
    }

    
    /// <summary>
    /// Constructs a new instance of the <see cref="CompletionEventArgs"/> class.
    /// </summary>
    /// <param name="ch">The character that is going to be inserted.</param>
    public CompletionEventArgs(char ch)
    {
      _ch = ch;
    }


    /// <summary>
    /// Constructs a new instance of the <see cref="CompletionEventArgs"/> class.
    /// </summary>
    /// <param name="userData">The user data.</param>
    public CompletionEventArgs(object userData)
    {
      _userData = userData;
    }


    /// <summary>
    /// Constructs a new instance of the <see cref="CompletionEventArgs"/> class.
    /// </summary>
    /// <param name="ch">The character that is going to be inserted.</param>
    /// <param name="userData">The user data.</param>
    public CompletionEventArgs(char ch, object userData)
    {
      _ch = ch;
      _userData = userData;
    }
  }
}
