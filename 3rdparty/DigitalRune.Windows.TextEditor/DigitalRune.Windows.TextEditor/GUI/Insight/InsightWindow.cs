using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Completion;
using DigitalRune.Windows.TextEditor.Utilities;


namespace DigitalRune.Windows.TextEditor.Insight
{
  /// <summary>
  /// Shows insight information for methods (and its overloads).
  /// </summary>
  public class InsightWindow : AbstractCompletionWindow
  {
    private readonly MouseWheelHandler _mouseWheelHandler = new MouseWheelHandler();


    /// <summary>
    /// Initializes a new instance of the <see cref="InsightWindow"/> class.
    /// </summary>
    /// <param name="parentForm">The parent form.</param>
    /// <param name="control">The text editor.</param>
    public InsightWindow(Form parentForm, TextEditorControl control)
      : base(parentForm, control)
    {
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
    }


    /// <summary>
    /// Shows the insight window.
    /// </summary>
    public void ShowInsightWindow()
    {
      if (!Visible)
      {
        if (insightDataProviderStack.Count > 0)
          ShowCompletionWindow();
      }
      else
      {
        Refresh();
      }
    }


    #region ----- Event handling routines -----
    /// <summary>
    /// Processes dialog-key events (escape, tab, etc.).
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="keyEventArgs">
    /// The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.
    /// </param>
    protected override void ProcessTextAreaKey(object sender, KeyEventArgs keyEventArgs)
    {
      if (!Visible)
        return;

      switch (keyEventArgs.KeyData)
      {
        case Keys.Down:
          if (DataProvider != null && DataProvider.InsightDataCount > 0)
          {
            CurrentData = (CurrentData + 1) % DataProvider.InsightDataCount;
            Refresh();
          }
          keyEventArgs.Handled = true;
          return;

        case Keys.Up:
          if (DataProvider != null && DataProvider.InsightDataCount > 0)
          {
            CurrentData = (CurrentData + DataProvider.InsightDataCount - 1) % DataProvider.InsightDataCount;
            Refresh();
          }
          keyEventArgs.Handled = true;
          return;
      }

      base.ProcessTextAreaKey(sender, keyEventArgs);
    }


    /// <summary>
    /// Handles changes of the caret position.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void CaretOffsetChanged(object sender, EventArgs e)
    {
      // move the window under the caret (don't change the x position)
      TextLocation caretPos = TextEditorControl.ActiveTextAreaControl.Caret.Position;

      int xpos = TextEditorControl.ActiveTextAreaControl.TextArea.TextView.GetDrawingXPos(caretPos.Y, caretPos.X);
      int ypos = (TextEditorControl.Document.GetVisibleLine(caretPos.Y) + 1) * TextEditorControl.ActiveTextAreaControl.TextArea.TextView.LineHeight - TextEditorControl.ActiveTextAreaControl.TextArea.VirtualTop.Y;
      int rulerHeight = TextEditorControl.TextEditorProperties.ShowHorizontalRuler ? TextEditorControl.ActiveTextAreaControl.TextArea.TextView.LineHeight : 0;

      Point p = TextEditorControl.ActiveTextAreaControl.PointToScreen(new Point(xpos, ypos + rulerHeight));
      if (p.Y != Location.Y)
        Location = p;

      while (DataProvider != null && DataProvider.CaretOffsetChanged())
        CloseCurrentDataProvider();
    }


    /// <summary>
    /// Raises the <see cref="Control.MouseDown"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="MouseEventArgs"></see> that contains the event data.</param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
      base.OnMouseDown(e);
      TextEditorControl.ActiveTextAreaControl.TextArea.Focus();
      if (TipPainterTools.DrawingRectangle1.Contains(e.X, e.Y))
      {
        CurrentData = (CurrentData + DataProvider.InsightDataCount - 1) % DataProvider.InsightDataCount;
        Refresh();
      }
      if (TipPainterTools.DrawingRectangle2.Contains(e.X, e.Y))
      {
        CurrentData = (CurrentData + 1) % DataProvider.InsightDataCount;
        Refresh();
      }
    }


    /// <summary>
    /// Handles the mouse wheel.
    /// </summary>
    /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    public void HandleMouseWheel(MouseEventArgs e)
    {
      if (DataProvider != null && DataProvider.InsightDataCount > 0)
      {
        int distance = _mouseWheelHandler.GetScrollAmount(e);
        if (TextEditorControl.TextEditorProperties.MouseWheelScrollDown)
          distance = -distance;
        if (distance > 0)
        {
          CurrentData = (CurrentData + 1) % DataProvider.InsightDataCount;
        }
        else if (distance < 0)
        {
          CurrentData = (CurrentData + DataProvider.InsightDataCount - 1) % DataProvider.InsightDataCount;
        }
        Refresh();
      }
    }
    #endregion


    #region ----- Insight Window Drawing routines -----
    /// <summary>
    /// Raises the <see cref="Control.Paint"/> event.
    /// </summary>
    /// <param name="pe">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
    protected override void OnPaint(PaintEventArgs pe)
    {
      string description;
      string methodCountMessage = null;

      if (DataProvider == null || DataProvider.InsightDataCount < 1)
      {
        description = "Unknown Method";
      }
      else
      {
        if (DataProvider.InsightDataCount > 1)
          methodCountMessage = TextEditorControl.GetRangeDescription(CurrentData + 1, DataProvider.InsightDataCount);
     
        description = DataProvider.GetInsightData(CurrentData);
      }

      DrawingSize = TipPainterTools.GetDrawingSizeHelpTipFromCombinedDescription(this, pe.Graphics, Font, methodCountMessage, description);
      if (DrawingSize != Size)
        SetLocation();
      else
        TipPainterTools.DrawHelpTipFromCombinedDescription(this, pe.Graphics, Font, methodCountMessage, description);
    }


    /// <summary>
    /// Is called when the control paints the background.
    /// </summary>
    /// <param name="pe">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
    protected override void OnPaintBackground(PaintEventArgs pe)
    {
      pe.Graphics.FillRectangle(SystemBrushes.Info, pe.ClipRectangle);
    }
    #endregion


    #region ----- InsightDataProvider handling -----
    private readonly Stack<InsightDataProviderStackElement> insightDataProviderStack = new Stack<InsightDataProviderStackElement>();


    int CurrentData
    {
      get { return insightDataProviderStack.Peek().CurrentData; }
      set { insightDataProviderStack.Peek().CurrentData = value; }
    }


    IInsightDataProvider DataProvider
    {
      get
      {
        if (insightDataProviderStack.Count == 0)
          return null;

        return insightDataProviderStack.Peek().DataProvider;
      }
    }


    /// <summary>
    /// Adds the insight data provider.
    /// </summary>
    /// <param name="provider">The provider.</param>
    /// <param name="fileName">Name of the file.</param>
    public void AddInsightDataProvider(IInsightDataProvider provider, string fileName)
    {
      provider.SetupDataProvider(fileName, TextEditorControl.ActiveTextAreaControl.TextArea);
      if (provider.InsightDataCount > 0)
        insightDataProviderStack.Push(new InsightDataProviderStackElement(provider));
    }


    void CloseCurrentDataProvider()
    {
      insightDataProviderStack.Pop();
      if (insightDataProviderStack.Count == 0)
        Close();
      else
        Refresh();
    }


    private class InsightDataProviderStackElement
    {
      public readonly IInsightDataProvider DataProvider;
      public int CurrentData;

      public InsightDataProviderStackElement(IInsightDataProvider dataProvider)
      {
        DataProvider = dataProvider;
        CurrentData = Math.Max(dataProvider.DefaultIndex, 0);
      }
    }
    #endregion
  }
}
