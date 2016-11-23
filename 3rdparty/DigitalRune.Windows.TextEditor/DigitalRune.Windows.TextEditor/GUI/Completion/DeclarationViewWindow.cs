using System;
using System.Drawing;
using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// A tool-tip window.
  /// </summary>
  public class DeclarationViewWindow : Form
  {
    private string _description = String.Empty;
    private bool _fixedWidth;


    /// <summary>
    /// Gets or sets the description that is shown in the <see cref="DeclarationViewWindow"/>.
    /// </summary>
    /// <value>The description shown in the window.</value>
    public string Description
    {
      get { return _description; }
      set
      {
        _description = value;
        if (value == null && Visible)
        {
          Visible = false;
        }
        else if (value != null)
        {
          if (!Visible) 
            Show();
          Refresh();
        }
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether whether the width is fixed.
    /// </summary>
    /// <value><see langword="true"/> if the width is fixed; otherwise, <see langword="false"/>.</value>
    public bool FixedWidth
    {
      get { return _fixedWidth; }
      set { _fixedWidth = value; }
    }


    /// <summary>
    /// Gets the required width for the declaration window when should be shown on the left.
    /// </summary>
    /// <param name="p">The position of the declaration window.</param>
    /// <returns>The required width.</returns>
    public int GetRequiredLeftHandSideWidth(Point p)
    {
      if (!String.IsNullOrEmpty(_description))
      {
        using (Graphics g = CreateGraphics())
        {
          Size s = TipPainterTools.GetLeftHandSideDrawingSizeHelpTipFromCombinedDescription(this, g, Font, null, _description, p);
          return s.Width;
        }
      }
      return 0;
    }
		

    /// <summary>
    /// Gets or sets a value indicating whether to close the window on mouse click.
    /// </summary>
    public bool HideOnClick;


    /// <summary>
    /// Initializes a new instance of the <see cref="DeclarationViewWindow"/> class.
    /// </summary>
    /// <param name="parent">The parent.</param>
    public DeclarationViewWindow(Form parent)
    {
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.ResizeRedraw, false);
      SetStyle(ControlStyles.Selectable, false);
      StartPosition = FormStartPosition.Manual;
      FormBorderStyle = FormBorderStyle.None;
      Owner = parent;
      ShowInTaskbar = false;
      Size = new Size(0, 0);
      base.CreateHandle();
    }


    /// <summary>
    /// Gets the create params.
    /// </summary>
    /// <value>The create params.</value>
    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams createParams = base.CreateParams;
        AbstractCompletionWindow.AddShadowToWindow(createParams);
        return createParams;
      }
    }


    /// <summary>
    /// Gets a value indicating whether the window will be activated when it is shown.
    /// </summary>
    /// <value></value>
    /// <returns>True if the window will not be activated when it is shown; otherwise, false. The default is false.</returns>
    protected override bool ShowWithoutActivation
    {
      get { return true; }
    }


    /// <summary>
    /// Raises the <see cref="Control.Click"></see> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"></see> that contains the event data.</param>
    protected override void OnClick(EventArgs e)
    {
      base.OnClick(e);
      if (HideOnClick) 
        Hide();
    }


    /// <summary>
    /// Raises the <see cref="Control.Paint"/> event.
    /// </summary>
    /// <param name="paintEventArgs">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
    protected override void OnPaint(PaintEventArgs paintEventArgs)
    {
      if (!String.IsNullOrEmpty(_description))
      {
        if (_fixedWidth)
        {
          TipPainterTools.DrawFixedWidthHelpTipFromCombinedDescription(this, paintEventArgs.Graphics, Font, null, _description);
        }
        else
        {
          TipPainterTools.DrawHelpTipFromCombinedDescription(this, paintEventArgs.Graphics, Font, null, _description);
        }
      }
    }


    /// <summary>
    /// Paints the background of the control. 
    /// </summary>
    /// <param name="paintEventArgs">
    /// A <see cref="System.Windows.Forms.PaintEventArgs"/> that contains 
    /// information about the control to paint.
    /// </param>
    protected override void OnPaintBackground(PaintEventArgs paintEventArgs)
    {
      paintEventArgs.Graphics.FillRectangle(SystemBrushes.Info, paintEventArgs.ClipRectangle);
    }
  }
}
