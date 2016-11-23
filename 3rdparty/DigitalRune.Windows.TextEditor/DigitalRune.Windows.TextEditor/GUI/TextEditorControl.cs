using System;
using System.ComponentModel;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Completion;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Insight;


namespace DigitalRune.Windows.TextEditor
{
  public partial class TextEditorControl : UserControl
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    private Panel _textAreaPanel = new Panel();
    private TextAreaControl _activeTextAreaControl;
    private readonly TextAreaControl _primaryTextArea;
    private TextAreaControl _secondaryTextArea;
    private Splitter _textAreaSplitter;
    private bool _inHandleKeyPress;
    #endregion


    //--------------------------------------------------------------
    #region Properties & Events
    //--------------------------------------------------------------
    /// <summary>
    /// Gets the active text area.
    /// </summary>
    /// <value>The active text area.</value>
    [Browsable(false)]
    public TextAreaControl ActiveTextAreaControl
    {
      get { return _activeTextAreaControl; }
      protected set
      {
        if (_activeTextAreaControl != value)
        {
          _activeTextAreaControl = value;

          if (ActiveTextAreaControlChanged != null)
            ActiveTextAreaControlChanged(this, EventArgs.Empty);
        }
      }
    }


    /// <summary>
    /// Gets a value indicating whether undo is enabled (something on the undo-stack).
    /// </summary>
    /// <value><see langword="true"/> if undo is enabled; otherwise, <see langword="false"/>.</value>
    [Browsable(false)]
    public bool EnableUndo
    {
      get { return Document.UndoStack.CanUndo; }
    }


    /// <summary>
    /// Gets a value indicating whether redo is enabled (something on the redo-stack).
    /// </summary>
    /// <value><see langword="true"/> if redo is enabled; otherwise, <see langword="false"/>.</value>
    [Browsable(false)]    
    public bool EnableRedo
    {
      get { return Document.UndoStack.CanRedo; }
    }


    /// <summary>
    /// Gets a value indicating whether 'Copy' is enabled.
    /// </summary>
    /// <value><see langword="true"/> if 'Copy' is enabled; otherwise, <see langword="false"/>.</value>
    [Browsable(false)]    
    public bool EnableCopy
    {
      get { return ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCopy; }      
    }


    /// <summary>
    /// Gets a value indicating whether 'Cut' is enabled.
    /// </summary>
    /// <value><see langword="true"/> if 'Cut' is enabled; otherwise, <see langword="false"/>.</value>
    [Browsable(false)]
    public bool EnableCut
    {
      get { return ActiveTextAreaControl.TextArea.ClipboardHandler.EnableCut; }
    }


    /// <summary>
    /// Gets a value indicating whether 'Paste' is enabled (i.e. something is in the clipboard).
    /// </summary>
    /// <value><see langword="true"/> if 'Paste' is enabled; otherwise, <see langword="false"/>.</value>
    [Browsable(false)]
    public bool EnablePaste
    {
      get { return ActiveTextAreaControl.TextArea.ClipboardHandler.EnablePaste; }
    }


    /// <summary>
    /// Gets a value indicating whether 'Delete' is enabled (i.e. something is in the clipboard).
    /// </summary>
    /// <value><see langword="true"/> if 'Delete' is enabled; otherwise, <see langword="false"/>.</value>
    [Browsable(false)]
    public bool EnableDelete
    {
      get { return ActiveTextAreaControl.TextArea.ClipboardHandler.EnableDelete; }
    }


    /// <summary>
    /// Gets a value indicating whether 'Select All' is enabled.
    /// </summary>
    /// <value><see langword="true"/> if 'Select All' is enabled; otherwise, <see langword="false"/>.</value>
    [Browsable(false)]
    public bool EnableSelectAll
    {
      get { return ActiveTextAreaControl.TextArea.ClipboardHandler.EnableSelectAll; }
    }


    /// <summary>
    /// Gets a value indicating whether this control is split into two views.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this control is into two views; otherwise, <see langword="false"/>.
    /// </value>
    [Browsable(false)]
    public bool IsViewSplit
    {
      get { return _secondaryTextArea != null; }
    }


    /// <summary>
    /// Occurs when the active text area changes.
    /// </summary>
    [Category("Misc")]
    [Description("Occurs when the active text area changes.")]
    public event EventHandler ActiveTextAreaControlChanged;


    /// <summary>
    /// Occurs when the selection within a text area is changed.
    /// </summary>
    [Category("Document")]
    [Description("Occurs when the selection within a text area is changed.")]
    public event EventHandler SelectionChanged;


    /// <summary>
    /// Occurs when the a dialog key (Tab, Return, Escape, arrow keys, etc.) is pressed on 
    /// the <see cref="TextArea"/>.
    /// </summary>
    [Category("Key")]
    [Description("Occurs when the a dialog key (Tab, Return, Escape, arrow keys, etc.) is pressed on the text area.")]
    public event KeyEventHandler TextAreaDialogKeyPress;


    /// <summary>
    /// Occurs when the a standard key (non-dialog key) is pressed on the <see cref="TextArea"/>.
    /// </summary>
    [Category("Key")]
    [Description("Occurs when the a standard key (non-dialog key) is pressed on the text area.")]
    public event KeyPressEventHandler TextAreaKeyPress;


    /// <summary>
    /// Occurs when the context menu is requested.
    /// </summary>
    [Category("Misc")]
    [Description("Occurs when the context menu is requested.")]
    public event MouseEventHandler ContextMenuRequest;
    #endregion


    //--------------------------------------------------------------
    #region Creation and Cleanup
    //--------------------------------------------------------------
    /// <summary>
    /// Initializes a new instance of the <see cref="TextEditorControl"/> class.
    /// </summary>
    public TextEditorControl()
    {
      GenerateDefaultActions();
      HighlightingManager.Manager.SyntaxHighlightingReloaded += OnReloadHighlighting;

      SetStyle(ControlStyles.ContainerControl, true);
      _textAreaPanel.Dock = DockStyle.Fill;

      Document = (new DocumentFactory()).CreateDocument();
      Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy();

      _primaryTextArea = new TextAreaControl(this);
      _activeTextAreaControl = _primaryTextArea;
      _primaryTextArea.TextArea.GotFocus += delegate { ActiveTextAreaControl = _primaryTextArea; };
      _primaryTextArea.Dock = DockStyle.Fill;
      _textAreaPanel.Controls.Add(_primaryTextArea);
      InitializeTextAreaControl(_primaryTextArea);
      Controls.Add(_textAreaPanel);
      ResizeRedraw = true;
      Document.UpdateCommited += CommitUpdateRequested;
      OptionsChanged();
    }


    /// <summary>
    /// Releases unmanaged and - optionally - managed resources
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release both managed and unmanaged resources; <see langword="false"/> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        // Clean up managed resources
        OnCompletionWindowClosed(this, EventArgs.Empty);
        OnInsightWindowClosed(this, EventArgs.Empty);

        if (_printDocument != null)
        {
          _printDocument.BeginPrint -= BeginPrint;
          _printDocument.PrintPage -= PrintPage;
          _printDocument = null;
        }
        Document.UndoStack.ClearAll();
        Document.UpdateCommited -= CommitUpdateRequested;
        if (_textAreaPanel != null)
        {
          if (_secondaryTextArea != null)
          {
            _secondaryTextArea.Dispose();
            _textAreaSplitter.Dispose();
            _secondaryTextArea = null;
            _textAreaSplitter = null;
          }
          if (_primaryTextArea != null)
          {
            _primaryTextArea.Dispose();
          }
          _textAreaPanel.Dispose();
          _textAreaPanel = null;
        }

        HighlightingManager.Manager.SyntaxHighlightingReloaded -= OnReloadHighlighting;
        Document.HighlightingStrategy = null;
        Document.UndoStack.TextEditorControl = null;
      }
      base.Dispose(disposing);
    }


    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------
    /// <summary>
    /// Initializes the text area control.
    /// </summary>
    /// <param name="newControl">The new control.</param>
    /// <remarks>
    /// <para>
    /// A <see cref="TextEditorControl"/> can contain multiple <see cref="TextAreaControl"/>s. 
    /// E.g. for showing a split-view of the document.
    /// </para>
    /// <para>
    /// This method is called when a new <see cref="TextAreaControl"/> is created.
    /// </para>
    /// </remarks>
    protected void InitializeTextAreaControl(TextAreaControl newControl)
    {
      newControl.ContextMenuRequest += TextArea_ContextMenuRequest;
      newControl.MouseWheel += TextArea_MouseWheel;
      newControl.TextArea.KeyPress += TextArea_KeyPress;
      newControl.TextArea.DialogKeyPress += TextArea_DialogKeyPress;
      newControl.TextArea.ToolTipRequest += TextArea_ToolTipRequest;
      newControl.DoHandleMousewheel = false;
    }


    /// <summary>
    /// Notifies all views that options have changed.
    /// </summary>
    public void OptionsChanged()
    {
      _primaryTextArea.OptionsChanged();
      if (_secondaryTextArea != null)
        _secondaryTextArea.OptionsChanged();
    }


    /// <summary>
    /// Shows/hides a vertical split view of the document.
    /// </summary>
    public void ToggleSplitView()
    {
      if (_secondaryTextArea == null)
      {
        _secondaryTextArea = new TextAreaControl(this) 
        {
          Dock = DockStyle.Bottom, 
          Height = (Height / 2)
        };
        _secondaryTextArea.TextArea.GotFocus += delegate { ActiveTextAreaControl = _secondaryTextArea; };

        _textAreaSplitter = new Splitter 
        {
          BorderStyle = BorderStyle.FixedSingle, 
          Height = 8, 
          Dock = DockStyle.Bottom
        };
        _textAreaPanel.Controls.Add(_textAreaSplitter);
        _textAreaPanel.Controls.Add(_secondaryTextArea);
        InitializeTextAreaControl(_secondaryTextArea);
        _secondaryTextArea.OptionsChanged();
      }
      else
      {
        ActiveTextAreaControl = _primaryTextArea;

        _textAreaPanel.Controls.Remove(_secondaryTextArea);
        _textAreaPanel.Controls.Remove(_textAreaSplitter);

        _secondaryTextArea.Dispose();
        _textAreaSplitter.Dispose();
        _secondaryTextArea = null;
        _textAreaSplitter = null;
      }
    }


    /// <summary>
    /// Undoes the last changes in the document.
    /// </summary>
    public void Undo()
    {
      if (Document.ReadOnly)
        return;

      if (Document.UndoStack.CanUndo)
      {
        BeginUpdate();

        Document.UndoStack.Undo();
        Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        _primaryTextArea.TextArea.UpdateMatchingBracket();
        if (_secondaryTextArea != null)
          _secondaryTextArea.TextArea.UpdateMatchingBracket();

        EndUpdate();
      }
    }


    /// <summary>
    /// Redoes the last undone changes.
    /// </summary>
    public void Redo()
    {
      if (Document.ReadOnly)
        return;

      if (Document.UndoStack.CanRedo)
      {
        BeginUpdate();

        Document.UndoStack.Redo();
        Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
        _primaryTextArea.TextArea.UpdateMatchingBracket();
        if (_secondaryTextArea != null)
          _secondaryTextArea.TextArea.UpdateMatchingBracket();

        EndUpdate();
      }
    }


    /// <summary>
    /// Sets the syntax highlighting mode.
    /// </summary>
    /// <param name="name">The name of the syntax highlighting mode.</param>
    public void SetHighlighting(string name)
    {
      Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(name);
    }


    internal void NotifySelectionChanged()
    {
      OnSelectionChanged(EventArgs.Empty);
    }


    /// <summary>
    /// Raises the <see cref="SelectionChanged" /> event.
    /// </summary>
    /// <param name="e"><see cref="EventArgs" /> object that provides the arguments for the event.</param>
    protected virtual void OnSelectionChanged(EventArgs e)
    {
      EventHandler handler = SelectionChanged;

      if (handler != null)
        handler(this, e);
    }


    void TextArea_MouseWheel(object sender, MouseEventArgs e)
    {
      TextAreaControl textAreaControl = (TextAreaControl) sender;
      if (_insightWindow != null && !_insightWindow.IsDisposed && _insightWindow.Visible)
      {
        _insightWindow.HandleMouseWheel(e);
      }
      else if (completionWindow != null && !completionWindow.IsDisposed && completionWindow.Visible)
      {
        completionWindow.HandleMouseWheel(e);
      }
      else
      {
        textAreaControl.HandleMouseWheel(e);
      }
    }


    private void TextArea_DialogKeyPress(object sender, KeyEventArgs keyEventArgs)
    {
      OnTextAreaDialogKeyPress(keyEventArgs);
    }


    /// <summary>
    /// Raises the <see cref="TextAreaDialogKeyPress"/> event.
    /// </summary>
    /// <param name="keyEventArgs">
    /// The <see cref="System.Windows.Forms.KeyEventArgs"/> instance  containing the event data.
    /// </param>
    protected virtual void OnTextAreaDialogKeyPress(KeyEventArgs keyEventArgs)
    {
      KeyEventHandler handler = TextAreaDialogKeyPress;

      if (handler != null)
        handler(this, keyEventArgs);
    }


    private void TextArea_KeyPress(object sender, KeyPressEventArgs keyEventArgs)
    {
      if (_inHandleKeyPress)
        return;

      // First, let all subscribers handle the key event.
      OnTextAreaKeyPress(keyEventArgs);
      if (keyEventArgs.Handled)
        return;


      _inHandleKeyPress = true;
      try
      {
        char ch = keyEventArgs.KeyChar;
        if (CompletionWindowVisible)
        {
          // Forward key event to completion window
          if (completionWindow.ProcessKeyEvent(ch))
            keyEventArgs.Handled = true;
        }

        if (EnableMethodInsight && (ch == '(' || ch == ','))
        {
          // Request insight window
          InsightEventArgs e = new InsightEventArgs(ch);
          OnInsightRequest(e);
        }
        else if (!keyEventArgs.Handled && EnableCompletion)
        {
          // Request completion window
          CompletionEventArgs e = new CompletionEventArgs(ch);
          OnCompletionRequest(e);
        }
      }
      //catch (Exception exception)
      //{
      //  // TODO: Log exception       
      //}
      finally
      {
        _inHandleKeyPress = false;
      }
    }


    /// <summary>
    /// Raises the <see cref="TextAreaKeyPress"/> event.
    /// </summary>
    /// <param name="keyEventArgs">
    /// The <see cref="System.Windows.Forms.KeyPressEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnTextAreaKeyPress(KeyPressEventArgs keyEventArgs)
    {
      KeyPressEventHandler handler = TextAreaKeyPress;

      if (handler != null)
        handler(this, keyEventArgs);
    }


    private void TextArea_ContextMenuRequest(object sender, MouseEventArgs mouseEventArgs)
    {
      OnContextMenuRequest(mouseEventArgs);
    }


    /// <summary>
    /// Raises the <see cref="ContextMenuRequest"/> event.
    /// </summary>
    /// <param name="mouseEventArgs">
    /// The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.
    /// </param>
    protected virtual void OnContextMenuRequest(MouseEventArgs mouseEventArgs)
    {
      MouseEventHandler handler = ContextMenuRequest;

      if (handler != null)
        handler(this, mouseEventArgs);
    }
    #endregion
  }
}