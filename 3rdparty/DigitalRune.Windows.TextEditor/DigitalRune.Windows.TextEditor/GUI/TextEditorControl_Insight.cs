using System;
using System.ComponentModel;
using DigitalRune.Windows.TextEditor.Insight;


namespace DigitalRune.Windows.TextEditor
{
  public partial class TextEditorControl
  {
    private InsightWindow _insightWindow;


    /// <summary>
    /// Gets a value indicating whether the insight window is shown at the moment.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if the insight window is visible; otherwise, <see langword="false"/>.
    /// </value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool InsightWindowVisible
    {
      get { return _insightWindow != null; }
    }


    /// <summary>
    /// Occurs when method insight is requested.
    /// </summary>
    [Category("Misc")]
    [Description("Occurs when method insight is requested.")]
    public event EventHandler<InsightEventArgs> InsightRequest;


    /// <summary>
    /// Shows the insight window.
    /// </summary>
    /// <remarks>
    /// The <see cref="TextEditorControl"/> does not know how to display the method
    /// insight window. This method does not create the method insight window
    /// itself, instead it raises the <see cref="InsightRequest"/> event.
    /// </remarks>
    public void RequestInsightWindow()
    {
      OnInsightRequest(InsightEventArgs.Empty);
    }


    /// <summary>
    /// Raises the <see cref="InsightRequest" /> event.
    /// </summary>
    /// <param name="e"><see cref="InsightEventArgs" /> object that provides the arguments for the event.</param>
    protected virtual void OnInsightRequest(InsightEventArgs e)
    {
      EventHandler<InsightEventArgs> handler = InsightRequest;

      if (handler != null)
        handler(this, e);
    }


    /// <summary>
    /// Shows the insight window.
    /// </summary>
    /// <param name="insightDataProvider">The insight data provider.</param>
    public void ShowInsightWindow(IInsightDataProvider insightDataProvider)
    {
      if (_insightWindow == null || _insightWindow.IsDisposed)
      {
        _insightWindow = new InsightWindow(ParentForm, this);
        _insightWindow.Closed += OnInsightWindowClosed;
      }
      _insightWindow.AddInsightDataProvider(insightDataProvider, FileName);
      _insightWindow.ShowInsightWindow();
    }


    void OnInsightWindowClosed(object sender, EventArgs e)
    {
      if (_insightWindow != null)
      {
        _insightWindow.Closed -= OnInsightWindowClosed;
        _insightWindow.Dispose();
        _insightWindow = null;
      }
    }
  }
}
