using System;
using System.ComponentModel;


namespace DigitalRune.Windows.TextEditor
{
  public partial class TextEditorControl
  {
    /// <summary>
    /// Occurs when a tool tip is requested.
    /// </summary>
    [Category("Misc")]
    [Description("Occurs when a tool tip is requested.")]
    public event EventHandler<ToolTipRequestEventArgs> ToolTipRequest;


    void TextArea_ToolTipRequest(object sender, ToolTipRequestEventArgs e)
    {
      // Forward event
      OnToolTipRequest(e);
    }


    /// <summary>
    /// Raises the <see cref="ToolTipRequest" /> event.
    /// </summary>
    /// <param name="e"><see cref="ToolTipRequestEventArgs" /> object that provides the arguments for the event.</param>
    protected virtual void OnToolTipRequest(ToolTipRequestEventArgs e)
    {
      EventHandler<ToolTipRequestEventArgs> handler = ToolTipRequest;

      if (handler != null)
        handler(this, e);
    }
  }
}
