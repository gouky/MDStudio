using System;
using System.ComponentModel;
using System.Diagnostics;


namespace DigitalRune.Windows.TextEditor
{
  partial class TextEditorControl
  {
    private int _updateLevel;


    /// <summary>
    /// Gets a value indicating whether this instance is in currently in an update.
    /// </summary>
    /// <value>
    /// <see langword="true"/>, if the text area is updating it's status, while
    /// it updates it status no redraw operation occurs.
    /// </value>
    /// <seealso cref="BeginUpdate"/>
    /// <see cref="EndUpdate"/>
    [Browsable(false)]
    public bool IsInUpdate
    {
      get { return _updateLevel > 0; }
    }


    /// <summary>
    /// Begin of update.
    /// </summary>
    /// <remarks>
    /// Call this method before a long update operation. This
    /// 'locks' the text area so that no screen update occurs.
    /// </remarks>
    public void BeginUpdate()
    {
      ++_updateLevel;
    }


    /// <summary>
    /// End of update.
    /// </summary>
    /// <remarks>
    /// Call this method to 'unlock' the text area. After this call
    /// screen update can occur.
    /// </remarks>
    public void EndUpdate()
    {
      Debug.Assert(_updateLevel > 0);
      _updateLevel = Math.Max(0, _updateLevel - 1);
      Document.CommitUpdate();

      if (!IsInUpdate)
        ActiveTextAreaControl.Caret.OnEndUpdate();
    }


    void CommitUpdateRequested(object sender, EventArgs e)
    {
      if (IsInUpdate)
        return;

      foreach (TextAreaUpdate update in Document.UpdateQueue)
      {
        switch (update.TextAreaUpdateType)
        {
          case TextAreaUpdateType.PositionToEnd:
            _primaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
            if (_secondaryTextArea != null)
              _secondaryTextArea.TextArea.UpdateToEnd(update.Position.Y);
            break;
          case TextAreaUpdateType.PositionToLineEnd:
          case TextAreaUpdateType.SingleLine:
            _primaryTextArea.TextArea.UpdateLine(update.Position.Y);
            if (_secondaryTextArea != null)
              _secondaryTextArea.TextArea.UpdateLine(update.Position.Y);
            break;
          case TextAreaUpdateType.SinglePosition:
            _primaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
            if (_secondaryTextArea != null)
              _secondaryTextArea.TextArea.UpdateLine(update.Position.Y, update.Position.X, update.Position.X);
            break;
          case TextAreaUpdateType.LinesBetween:
            _primaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
            if (_secondaryTextArea != null)
              _secondaryTextArea.TextArea.UpdateLines(update.Position.X, update.Position.Y);
            break;
          case TextAreaUpdateType.WholeTextArea:
            _primaryTextArea.TextArea.Invalidate();
            if (_secondaryTextArea != null)
              _secondaryTextArea.TextArea.Invalidate();
            break;
        }
      }

      Document.UpdateQueue.Clear();

      // Requests to show the completion window are delayed if an update is in progress.
      // --> Show completion window now.
      HandleDelayedCompletionWindowRequest();
    }
  }
}
