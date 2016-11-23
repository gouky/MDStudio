using System;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Provides the base functionality of all completion windows.
  /// </summary>
  public abstract class AbstractCompletionWindow : Form
  {
    /// <summary>Indicates whether to add drop shadow.</summary>
    private static readonly bool _addDropShadow;
    private readonly Form _parentForm;
    private TextEditorControl _textEditorControl;
    private Size _drawingSize;
    private Rectangle _workingScreen;


    /// <summary>
    /// Gets the create params.
    /// </summary>
    /// <value>The create params.</value>
    protected override CreateParams CreateParams
    {
      get
      {
        CreateParams createParams = base.CreateParams;
        AddShadowToWindow(createParams);
        return createParams;
      }
    }


    /// <summary>
    /// The size of the completion window.
    /// </summary>
    protected Size DrawingSize
    {
      get { return _drawingSize; }
      set { _drawingSize = value; }
    }


    /// <summary>
    /// Gets a value indicating whether the window will be activated when it is shown.
    /// </summary>
    /// <returns>True if the window will not be activated when it is shown; otherwise, false. The default is false.</returns>
    protected override bool ShowWithoutActivation
    {
      get { return true; }
    }


    /// <summary>
    /// The text editor control.
    /// </summary>
    protected TextEditorControl TextEditorControl
    {
      get { return _textEditorControl; }
      set { _textEditorControl = value; }
    }


    static AbstractCompletionWindow()
    {
      // Test whether OS version allows/requires drop shadow
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
      {
        Version ver = Environment.OSVersion.Version;
        if (ver.Major > 5 || ver.Major == 5 && ver.Minor >= 1)
        {
          _addDropShadow = true;
        }
      }
    }

    
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractCompletionWindow"/> class.
    /// </summary>
    /// <param name="parentForm">The parent form.</param>
    /// <param name="control">The text editor control.</param>
    protected AbstractCompletionWindow(Form parentForm, TextEditorControl control)
    {
      _workingScreen = Screen.GetWorkingArea(parentForm);
      _parentForm = parentForm;
      _textEditorControl = control;

      SetLocation();
      StartPosition = FormStartPosition.Manual;
      FormBorderStyle = FormBorderStyle.None;
      ShowInTaskbar = false;
      MinimumSize = new Size(1, 1);
      Size = new Size(1, 1);
    }


    /// <overloads>
    /// <summary>
    /// Sets the location of the window relative to a text location.
    /// </summary>
    /// </overloads>
    /// <summary>
    /// Sets the location relative to the current caret position.
    /// </summary>
    public void SetLocation()
    {
      TextLocation caretLocation = TextEditorControl.ActiveTextAreaControl.TextArea.Caret.Position;
      SetLocation(caretLocation);
    }


    /// <summary>
    /// Sets the location relative to the specified text.
    /// </summary>
    /// <param name="caretLocation">The text location.</param>
    protected virtual void SetLocation(TextLocation caretLocation)
    {
      TextArea textArea = TextEditorControl.ActiveTextAreaControl.TextArea;

      int xpos = textArea.TextView.GetDrawingXPos(caretLocation.Y, caretLocation.X);
      int rulerHeight = textArea.TextEditorProperties.ShowHorizontalRuler ? textArea.TextView.LineHeight : 0;
      Point pos = new Point(textArea.TextView.DrawingPosition.X + xpos,
                            textArea.TextView.DrawingPosition.Y
                            + (textArea.Document.GetVisibleLine(caretLocation.Y)) * textArea.TextView.LineHeight 
                            - textArea.TextView.TextArea.VirtualTop.Y + textArea.TextView.LineHeight + rulerHeight);

      Point location = TextEditorControl.ActiveTextAreaControl.PointToScreen(pos);

      // set bounds
      Rectangle bounds = new Rectangle(location, _drawingSize);

      if (!_workingScreen.Contains(bounds))
      {
        if (bounds.Right > _workingScreen.Right)
          bounds.X = _workingScreen.Right - bounds.Width;

        if (bounds.Left < _workingScreen.Left)
          bounds.X = _workingScreen.Left;

        if (bounds.Top < _workingScreen.Top)
          bounds.Y = _workingScreen.Top;

        if (bounds.Bottom > _workingScreen.Bottom)
        {
          bounds.Y = bounds.Y - bounds.Height - TextEditorControl.ActiveTextAreaControl.TextArea.TextView.LineHeight;
          if (bounds.Bottom > _workingScreen.Bottom)
            bounds.Y = _workingScreen.Bottom - bounds.Height;
        }
      }
      Bounds = bounds;
    }


    /// <summary>
    /// Adds a shadow to the create params if it is supported by the operating system.
    /// </summary>
    /// <param name="createParams">The create params.</param>
    public static void AddShadowToWindow(CreateParams createParams)
    {
      if (_addDropShadow)
        createParams.ClassStyle |= 0x00020000; // set CS_DROPSHADOW
    }


    /// <summary>
    /// Shows the completion window.
    /// </summary>
    protected void ShowCompletionWindow()
    {
      if (IsDisposed)
        return; 

      Owner = _parentForm;
      Enabled = true;
      Show();

      TextEditorControl.Focus();

      if (_parentForm != null)
        _parentForm.LocationChanged += ParentFormLocationChanged;

      TextEditorControl.ActiveTextAreaControl.VScrollBar.ValueChanged += ParentFormLocationChanged;
      TextEditorControl.ActiveTextAreaControl.HScrollBar.ValueChanged += ParentFormLocationChanged;
      TextEditorControl.ActiveTextAreaControl.TextArea.DialogKeyPress += ProcessTextAreaKey;
      TextEditorControl.ActiveTextAreaControl.TextArea.KeyDown += OnKeyDown;
      TextEditorControl.ActiveTextAreaControl.TextArea.KeyUp += OnKeyUp;
      TextEditorControl.ActiveTextAreaControl.Caret.PositionChanged += CaretOffsetChanged;
      TextEditorControl.ActiveTextAreaControl.TextArea.LostFocus += TextEditorLostFocus;
      TextEditorControl.Resize += ParentFormLocationChanged;

      foreach (Control c in Controls)
        c.MouseMove += ControlMouseMove;
    }


    void ParentFormLocationChanged(object sender, EventArgs e)
    {
      SetLocation();
    }


    /// <summary>
    /// Processes key-events.
    /// </summary>
    /// <param name="ch">The character pressed.</param>
    /// <returns>
    /// <see langword="true"/> if the key has been handled by the completion window; 
    /// otherwise <see langword="false"/>.
    /// </returns>
    public virtual bool ProcessKeyEvent(char ch)
    {
      return false;
    }


    /// <summary>
    /// Processes dialog-key events (escape, tab, etc.).
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="keyEventArgs">
    /// The <see cref="System.Windows.Forms.KeyEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void ProcessTextAreaKey(object sender, KeyEventArgs keyEventArgs)
    {
      if (!Visible)
        return;

      switch (keyEventArgs.KeyData)
      {
        case Keys.Escape:
          Close();
          keyEventArgs.Handled = true;
          return;
      }
    }


    private void OnKeyDown(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.ControlKey && Visible)
      {
        Opacity = 0.1;
        OnOpacityChanged();
      }
    }


    private void OnKeyUp(object sender, KeyEventArgs e)
    {
      if (e.KeyCode == Keys.ControlKey && Visible)
      {
        Opacity = 1;
        OnOpacityChanged();
      }
    }


    /// <summary>
    /// Called when <see cref="Form.Opacity"/> is changed.
    /// </summary>
    protected virtual void OnOpacityChanged()
    {
    }


    /// <summary>
    /// Handles changes of the caret position.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void CaretOffsetChanged(object sender, EventArgs e)
    {
    }


    /// <summary>
    /// Handles the lost-focus event of the <see cref="TextArea"/>.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected void TextEditorLostFocus(object sender, EventArgs e)
    {
      if (!TextEditorControl.ActiveTextAreaControl.TextArea.Focused && !ContainsFocus)
        Close();
    }


    /// <summary>
    /// Raises the <see cref="Form.Closed"></see> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"></see> that contains the event data.</param>
    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);

      // take out the inserted methods
      _parentForm.LocationChanged -= ParentFormLocationChanged;

      foreach (Control control in Controls)
        control.MouseMove -= ControlMouseMove;

      TextEditorControl.ActiveTextAreaControl.VScrollBar.ValueChanged -= ParentFormLocationChanged;
      TextEditorControl.ActiveTextAreaControl.HScrollBar.ValueChanged -= ParentFormLocationChanged;      
      TextEditorControl.ActiveTextAreaControl.TextArea.LostFocus -= TextEditorLostFocus;
      TextEditorControl.ActiveTextAreaControl.Caret.PositionChanged -= CaretOffsetChanged;
      TextEditorControl.ActiveTextAreaControl.TextArea.DialogKeyPress -= ProcessTextAreaKey;
      TextEditorControl.ActiveTextAreaControl.TextArea.KeyDown -= OnKeyDown;
      TextEditorControl.ActiveTextAreaControl.TextArea.KeyUp -= OnKeyUp;
      TextEditorControl.Resize -= ParentFormLocationChanged;
      Dispose();
    }


    /// <summary>
    /// Raises the <see cref="Control.MouseMove"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="MouseEventArgs"></see> that contains the event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      ControlMouseMove(this, e);
    }


    /// <summary>
    /// Invoked when the mouse moves over this form or any child control.
    /// Shows the mouse cursor on the text area if it has been hidden.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    /// <remarks>
    /// Derived classes should attach this handler to the MouseMove event
    /// of all created controls which are not added to the Controls
    /// collection.
    /// </remarks>
    protected void ControlMouseMove(object sender, MouseEventArgs e)
    {
      TextEditorControl.ActiveTextAreaControl.TextArea.ShowHiddenCursor(false);
    }
  }
}
