using System;
using System.ComponentModel;
using DigitalRune.Windows.TextEditor.Completion;


namespace DigitalRune.Windows.TextEditor
{
  public partial class TextEditorControl
  {
    private CompletionWindow completionWindow;
    private bool _completionWindowRequested;
    private object _completionUserData;


    /// <summary>
    /// Gets a value indicating whether the completion window is shown.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if the completion window is shown; otherwise, <see langword="false"/>.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool CompletionWindowVisible
    {
      get { return completionWindow != null && !completionWindow.IsDisposed; }
    }


    /// <summary>
    /// Event raised when completion window should be shown.
    /// </summary>
    [Category("Misc")]
    [Description("Event raised when a completion window should be shown.")]
    public event EventHandler<CompletionEventArgs> CompletionRequest;


    /// <summary>
    /// Request a completion window.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method request a completion window. This method does not create the code completion 
    /// window itself, instead it raises the <see cref="CompletionRequest"/> event.
    /// </para>
    /// <para>
    /// The event <see cref="CompletionRequest"/> is delayed until the <see cref="TextEditorControl"/>
    /// is in a suitable state (e.g. after all is update and redrawn).
    /// </para>
    /// </remarks>
    public void RequestCompletionWindow()
    {
      RequestCompletionWindow(null);
    }


    /// <summary>
    /// Request a completion window.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This method requests a completion window. This method does not create the code completion 
    /// window itself, instead it raises the <see cref="CompletionRequest"/> event.
    /// </para>
    /// <para>
    /// The event <see cref="CompletionRequest"/> is delayed until the <see cref="TextEditorControl"/>
    /// is in a suitable state (e.g. after all is update and redrawn).
    /// </para>
    /// </remarks>
    public void RequestCompletionWindow(object userData)
    {
      if (IsInUpdate)
      {
        // Show completion window after update is down.
        _completionWindowRequested = true;
        _completionUserData = userData;
      }
      else
      {
        // Show completion window now.
        OnCompletionRequest(new CompletionEventArgs(userData));
      }      
    }


    private void HandleDelayedCompletionWindowRequest()
    {
      if (_completionWindowRequested)
      {
        _completionWindowRequested = false;
        OnCompletionRequest(new CompletionEventArgs(_completionUserData));
      }
    }


    /// <summary>
    /// Raises the <see cref="CompletionRequest" /> event.
    /// </summary>
    /// <param name="e"><see cref="CompletionEventArgs" /> object that provides the arguments for the event.</param>
    protected virtual void OnCompletionRequest(CompletionEventArgs e)
    {
      EventHandler<CompletionEventArgs> handler = CompletionRequest;

      if (handler != null)
        handler(this, e);
    }


    /// <summary>
    /// Shows the completion window.
    /// </summary>
    /// <param name="completionDataProvider">The completion data provider.</param>
    /// <param name="ch">The character that was typed - or <c>'\0'</c> if no character was typed.</param>
    /// <param name="closeAutomatically"><see langword="true"/> to close the completion wnidow automatically.</param>
    public void ShowCompletionWindow(ICompletionDataProvider completionDataProvider, char ch, bool closeAutomatically)
    {
      completionDataProvider.PreSelection = String.Empty;

      // Make default pre-selection
      string previousWord = TextHelper.GetWordBeforeCaret(ActiveTextAreaControl.TextArea);
      if (!String.IsNullOrEmpty(previousWord))
      {
        char lastChar = previousWord[previousWord.Length - 1];
        if (TextHelper.IsLetterDigitOrUnderscore(lastChar))
          completionDataProvider.PreSelection = previousWord;
      }

      completionWindow = CompletionWindow.ShowCompletionWindow(ParentForm, this, "", completionDataProvider, ch, true, false, closeAutomatically);

      if (completionWindow != null)
        completionWindow.Closed += OnCompletionWindowClosed;
    }


    /// <summary>
    /// Closes the completion window.
    /// </summary>
    public void CloseCompletionWindow()
    {
      if (completionWindow != null)
        completionWindow.Close();
    }


    void OnCompletionWindowClosed(object sender, EventArgs e)
    {
      if (completionWindow != null)
      {
        completionWindow.Closed -= OnCompletionWindowClosed;
        completionWindow.Dispose();
        completionWindow = null;
      }
    }
  }
}
