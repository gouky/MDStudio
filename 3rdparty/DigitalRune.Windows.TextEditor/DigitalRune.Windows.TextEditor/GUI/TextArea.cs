using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Actions;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Markers;
using DigitalRune.Windows.TextEditor.Selection;
using DigitalRune.Windows.TextEditor.Completion;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// The text area control.
  /// </summary>
  /// <remarks>
  /// The <see cref="TextArea"/> controls the editing in the visible part of the
  /// document. This control renders all the elements that are tightly coupled 
  /// with currently edited text: text view, line number margin, folding 
  /// margin and icon bar margin. (Scrollbars and the horizontal ruler are not 
  /// part of this control. They are part of the <see cref="TextAreaControl"/>.)
  /// </remarks>
  [ToolboxItem(false)]
  public class TextArea : Control
  {
    private bool _disposed;

    private TextAreaControl _motherTextAreaControl;
    private TextEditorControl _motherTextEditorControl;

    // Margins
    private readonly IconMargin _iconMargin;
    private readonly LineNumberMargin _lineNumberMargin;
    private readonly FoldMargin _foldMargin;
    private readonly TextView _textView;
    private readonly List<AbstractMargin> _leftMargins = new List<AbstractMargin>();

    private Point _virtualTop = new Point(0, 0);
    private readonly Caret _caret;
    private readonly List<BracketHighlightingScheme> _bracketHighlightingSchemes = new List<BracketHighlightingScheme>();
    private readonly TextAreaClipboardHandler _textAreaClipboardHandler;
    private readonly SelectionManager _selectionManager;
    private bool _autoClearSelection;

    private bool _mouseCursorHidden;
    /// <summary>
    /// The position where the mouse cursor was when it was hidden. Sometimes the text editor gets MouseMove
    /// events when typing text even if the mouse is not moved.
    /// </summary>
    private Point _mouseCursorHidePosition;

    private Point _mousePosition = new Point(0, 0);
    internal Point MousePositionInternal
    {
      get { return _mousePosition; }
      set { _mousePosition = value; }
    }

    private AbstractMargin _lastMouseInMargin;

    // static because the mouse can only be in one text area and we don't want to have
    // tooltips of text areas from inactive tabs floating around.
    private static DeclarationViewWindow _toolTip;
    private static string _oldToolTip;
    private bool _toolTipActive;
    /// <summary>
    /// Rectangle in text area that caused the current tool tip.
    /// Prevents tooltip from re-showing when it was closed because of a click or keyboard
    /// input and the mouse was not used.
    /// </summary>
    private Rectangle _toolTipRectangle;


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    [Browsable(false)]
    public IDocument Document
    {
      get { return _motherTextEditorControl.Document; }
    }


    /// <summary>
    /// Gets the text editor properties.
    /// </summary>
    /// <value>The text editor properties.</value>
    public ITextEditorProperties TextEditorProperties
    {
      get { return _motherTextEditorControl.TextEditorProperties; }
    }


    /// <summary>
    /// Gets the mother text editor control.
    /// </summary>
    /// <value>The mother text editor control.</value>
    public TextEditorControl MotherTextEditorControl
    {
      get { return _motherTextEditorControl; }
    }


    /// <summary>
    /// Gets the mother text area control.
    /// </summary>
    /// <value>The mother text area control.</value>
    public TextAreaControl MotherTextAreaControl
    {
      get { return _motherTextAreaControl; }
    }


    /// <summary>
    /// Gets the left margins.
    /// </summary>
    /// <value>The left margins.</value>
    [Browsable(false)]
    internal IList<AbstractMargin> LeftMargins
    {
      get { return _leftMargins.AsReadOnly(); }
    }


    /// <summary>
    /// Gets the icon bar margin.
    /// </summary>
    /// <value>The icon bar margin.</value>
    internal IconMargin IconMargin
    {
      get { return _iconMargin; }
    }


    /// <summary>
    /// Gets the gutter margin.
    /// </summary>
    /// <value>The gutter margin.</value>
    internal LineNumberMargin LineNumberMargin
    {
      get { return _lineNumberMargin; }
    }


    /// <summary>
    /// Gets the fold margin.
    /// </summary>
    /// <value>The fold margin.</value>
    internal FoldMargin FoldMargin
    {
      get { return _foldMargin; }
    }


    /// <summary>
    /// Gets the text view.
    /// </summary>
    /// <value>The text view.</value>
    internal TextView TextView
    {
      get { return _textView; }
    }


    /// <summary>
    /// Gets or sets the virtual top.
    /// </summary>
    /// <value>The virtual top.</value>
    public Point VirtualTop
    {
      get { return _virtualTop; }
      set
      {
        Point newVirtualTop = new Point(value.X, Math.Min(MaxVScrollValue, Math.Max(0, value.Y)));
        if (_virtualTop != newVirtualTop)
        {
          _virtualTop = newVirtualTop;
          _motherTextAreaControl.VScrollBar.Value = _virtualTop.Y;
          Invalidate();
        }
        _caret.UpdateCaretPosition();
      }
    }


    /// <summary>
    /// Gets the maximal value for vertical scroll bar.
    /// </summary>
    /// <value>The maximal value for the vertical scroll bar.</value>
    public int MaxVScrollValue
    {
      get { return (Document.GetVisibleLine(Document.TotalNumberOfLines - 1) + 1 + TextView.NumberOfVisibleLines * 2 / 3) * TextView.LineHeight; }
    }


    /// <summary>
    /// Gets the caret.
    /// </summary>
    /// <value>The caret.</value>
    public Caret Caret
    {
      get { return _caret; }
    }


    /// <summary>
    /// Gets the selection manager.
    /// </summary>
    /// <value>The selection manager.</value>
    public SelectionManager SelectionManager
    {
      get { return _selectionManager; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to automatically clear selections.
    /// </summary>
    /// <value><see langword="true"/> if selections should be cleared automatically; otherwise, <see langword="false"/>.</value>
    public bool AutoClearSelection
    {
      get { return _autoClearSelection; }
      set { _autoClearSelection = value; }
    }


    /// <summary>
    /// Gets a value indicating whether cut or paste is allowed at the current 
    /// selection/position.
    /// </summary>
    /// <value><see langword="true"/> if cut/paste is enabled; otherwise, <see langword="false"/>.</value>
    public bool EnableCutOrPaste
    {
      get
      {
        if (_motherTextAreaControl == null)
          return false;

        if (SelectionManager.HasSomethingSelected)
          return !SelectionManager.SelectionIsReadonly;
        else
          return !IsReadOnly(Caret.Offset);
      }
    }


    /// <summary>
    /// Gets the clipboard handler.
    /// </summary>
    /// <value>The clipboard handler.</value>
    internal TextAreaClipboardHandler ClipboardHandler
    {
      get { return _textAreaClipboardHandler; }
    }


    /// <summary>
    /// Occurs when a dialog key (Tab, Escape, Return, arrow keys, etc.) is pressed.
    /// </summary>
    public event KeyEventHandler DialogKeyPress;


    /// <summary>
    /// Occurs when tool tip is requested.
    /// </summary>
    public event EventHandler<ToolTipRequestEventArgs> ToolTipRequest;


    /// <summary>
    /// Initializes a new instance of the <see cref="TextArea"/> class.
    /// </summary>
    /// <param name="motherTextEditorControl">The mother text editor control.</param>
    /// <param name="motherTextAreaControl">The mother text area control.</param>
    public TextArea(TextEditorControl motherTextEditorControl, TextAreaControl motherTextAreaControl)
    {
      _motherTextAreaControl = motherTextAreaControl;
      _motherTextEditorControl = motherTextEditorControl;
      _caret = new Caret(this);
      _selectionManager = new SelectionManager(Document, this);
      _textAreaClipboardHandler = new TextAreaClipboardHandler(this);
      ResizeRedraw = true;

      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.Opaque, false);
      SetStyle(ControlStyles.ResizeRedraw, true);
      SetStyle(ControlStyles.Selectable, true);

      _lineNumberMargin = new LineNumberMargin(this);
      _foldMargin = new FoldMargin(this);
      _iconMargin = new IconMargin(this);
      _leftMargins.AddRange(new AbstractMargin[] { _iconMargin, _lineNumberMargin, _foldMargin });
      _textView = new TextView(this);
      OptionsChanged();

      new TextAreaMouseHandler(this).Attach();
      new TextAreaDragDropHandler().Attach(this);

      _bracketHighlightingSchemes.Add(new BracketHighlightingScheme('{', '}'));
      _bracketHighlightingSchemes.Add(new BracketHighlightingScheme('(', ')'));
      _bracketHighlightingSchemes.Add(new BracketHighlightingScheme('[', ']'));

      _caret.PositionChanged += SearchMatchingBracket;
      Document.TextContentChanged += TextContentChanged;
      Document.FoldingManager.FoldingChanged += DocumentFoldingsChanged;
    }


    /// <summary>
    /// Inserts the left margin.
    /// </summary>
    /// <param name="index">The index.</param>
    /// <param name="margin">The margin.</param>
    internal void InsertLeftMargin(int index, AbstractMargin margin)
    {
      _leftMargins.Insert(index, margin);
      Refresh();
    }


    /// <summary>
    /// Updates the matching brackets.
    /// </summary>
    public void UpdateMatchingBracket()
    {
      SearchMatchingBracket(null, null);
    }


    void TextContentChanged(object sender, EventArgs e)
    {
      Caret.Position = new TextLocation(0, 0);
      SelectionManager.Selections.Clear();
    }


    void SearchMatchingBracket(object sender, EventArgs e)
    {
      if (!TextEditorProperties.ShowMatchingBracket)
      {
        _textView.Highlight = null;
        return;
      }
      int oldLine1 = -1, oldLine2 = -1;
      if (_textView.Highlight != null && _textView.Highlight.OpeningBrace.Y >= 0 && _textView.Highlight.OpeningBrace.Y < Document.TotalNumberOfLines)
      {
        oldLine1 = _textView.Highlight.OpeningBrace.Y;
      }
      if (_textView.Highlight != null && _textView.Highlight.ClosingBrace.Y >= 0 && _textView.Highlight.ClosingBrace.Y < Document.TotalNumberOfLines)
      {
        oldLine2 = _textView.Highlight.ClosingBrace.Y;
      }
      _textView.Highlight = FindMatchingBracketHighlight();
      if (oldLine1 >= 0)
        UpdateLine(oldLine1);
      if (oldLine2 >= 0 && oldLine2 != oldLine1)
        UpdateLine(oldLine2);
      if (_textView.Highlight != null)
      {
        int newLine1 = _textView.Highlight.OpeningBrace.Y;
        int newLine2 = _textView.Highlight.ClosingBrace.Y;
        if (newLine1 != oldLine1 && newLine1 != oldLine2)
          UpdateLine(newLine1);
        if (newLine2 != oldLine1 && newLine2 != oldLine2 && newLine2 != newLine1)
          UpdateLine(newLine2);
      }
    }


    /// <summary>
    /// Finds the pair matching brackets that need to be highlighted.
    /// </summary>
    /// <returns>A pair of matching brackets (or <see langword="null"/>).</returns>
    internal Highlight FindMatchingBracketHighlight()
    {
      if (Caret.Offset == 0)
        return null;
      foreach (BracketHighlightingScheme bracketscheme in _bracketHighlightingSchemes)
      {
        Highlight highlight = bracketscheme.GetHighlight(Document, Caret.Offset - 1);
        if (highlight != null)
          return highlight;
      }
      return null;
    }


    /// <summary>
    /// Sets the desired column.
    /// </summary>
    public void SetDesiredColumn()
    {
      Caret.DesiredColumn = TextView.GetDrawingXPos(Caret.Line, Caret.Column) + VirtualTop.X * _textView.ColumnWidth;
    }


    /// <summary>
    /// Sets the caret to the desired column.
    /// </summary>
    public void SetCaretToDesiredColumn()
    {
      Fold dummy;
      Caret.Position = _textView.GetLogicalColumn(Caret.Line, Caret.DesiredColumn + VirtualTop.X, out dummy);
    }


    /// <summary>
    /// Notifies all client controls that the text editor properties have changed.
    /// </summary>
    public void OptionsChanged()
    {
      UpdateMatchingBracket();
      _textView.OptionsChanged();
      _caret.RecreateCaret();
      _caret.UpdateCaretPosition();
      Refresh();
    }


    /// <summary>
    /// Raises the <see cref="Control.MouseLeave"></see> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"></see> that contains the event data.</param>
    protected override void OnMouseLeave(EventArgs e)
    {
      base.OnMouseLeave(e);
      Cursor = Cursors.Default;
      if (_lastMouseInMargin != null)
      {
        _lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);
        _lastMouseInMargin = null;
      }
      CloseToolTip();
    }


    /// <summary>
    /// Raises the <see cref="Control.MouseDown"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="MouseEventArgs"></see> that contains the event data.</param>
    protected override void OnMouseDown(MouseEventArgs e)
    {
      // this corrects weird problems when text is selected,
      // then a menu item is selected, then the text is
      // clicked again - it correctly synchronizes the
      // click position
      _mousePosition = new Point(e.X, e.Y);

      base.OnMouseDown(e);
      CloseToolTip();

      foreach (AbstractMargin margin in _leftMargins)
      {
        if (margin.DrawingPosition.Contains(e.X, e.Y))
        {
          margin.HandleMouseDown(e);
        }
      }
    }


    /// <summary>
    /// Shows the mouse cursor if it has been hidden.
    /// </summary>
    /// <param name="forceShow"><see langword="true"/> to always show the cursor or <see langword="false"/> to show it only if it has been moved since it was hidden.</param>
    internal void ShowHiddenCursor(bool forceShow)
    {
      if (_mouseCursorHidden)
      {
        if (_mouseCursorHidePosition != Cursor.Position || forceShow)
        {
          Cursor.Show();
          _mouseCursorHidden = false;
        }
      }
    }
		

    void SetToolTip(string text, int lineNumber)
    {
      if (_toolTip == null || _toolTip.IsDisposed)
        _toolTip = new DeclarationViewWindow(FindForm());
      if (_oldToolTip == text)
        return;
      if (text == null)
      {
        _toolTip.Hide();
      }
      else
      {
        Point p = MousePosition;
        Point cp = PointToClient(p);
        if (lineNumber >= 0)
        {
          lineNumber = Document.GetVisibleLine(lineNumber);
          p.Y = (p.Y - cp.Y) + (lineNumber * TextView.LineHeight) - _virtualTop.Y;
        }
        p.Offset(3, 3);
        _toolTip.Location = p;
        _toolTip.Description = text;
        _toolTip.HideOnClick = true;
        _toolTip.Show();
      }
      _oldToolTip = text;
    }


    /// <summary>
    /// Raises the <see cref="ToolTipRequest"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.ToolTipRequestEventArgs"/> instance containing the event data.</param>
    protected virtual void OnToolTipRequest(ToolTipRequestEventArgs e)
    {
      if (ToolTipRequest != null)
        ToolTipRequest(this, e);
    }


    void CloseToolTip()
    {
      if (_toolTipActive)
      {
        _toolTipActive = false;
        SetToolTip(null, -1);
      }
      ResetMouseEventArgs();
    }


    /// <summary>
    /// Raises the <see cref="Control.MouseHover"></see> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"></see> that contains the event data.</param>
    protected override void OnMouseHover(EventArgs e)
    {
      base.OnMouseHover(e);
      if (MouseButtons == MouseButtons.None)
        RequestToolTip(PointToClient(MousePosition));
      else
        CloseToolTip();
    }


    /// <summary>
    /// Requests the tool tip.
    /// </summary>
    /// <param name="mousePos">The mouse position.</param>
    protected void RequestToolTip(Point mousePos)
    {
      if (_toolTipRectangle.Contains(mousePos))
      {
        if (!_toolTipActive)
          ResetMouseEventArgs();
        return;
      }

      _toolTipRectangle = new Rectangle(mousePos.X - 4, mousePos.Y - 4, 8, 8);

      TextLocation logicPos = _textView.GetLogicalPosition(mousePos.X - _textView.DrawingPosition.Left,
                                                   mousePos.Y - _textView.DrawingPosition.Top);
      bool inDocument = _textView.DrawingPosition.Contains(mousePos) 
                        && logicPos.Y >= 0 
                        && logicPos.Y < Document.TotalNumberOfLines;
      ToolTipRequestEventArgs args = new ToolTipRequestEventArgs(mousePos, logicPos, inDocument);
      OnToolTipRequest(args);
      if (args.ToolTipShown)
      {
        _toolTipActive = true;
        SetToolTip(args.ToolTipText, inDocument ? logicPos.Y + 1 : -1);
      }
      else
      {
        CloseToolTip();
      }
    }

    // external interface to the attached event
    internal void RaiseMouseMove(MouseEventArgs e)
    {
      OnMouseMove(e);
    }


    /// <summary>
    /// Raises the <see cref="Control.MouseMove"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="MouseEventArgs"></see> that contains the event data.</param>
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!_toolTipRectangle.Contains(e.Location))
      {
        _toolTipRectangle = Rectangle.Empty;
        if (_toolTipActive)
          RequestToolTip(e.Location);
      }
      foreach (AbstractMargin margin in _leftMargins)
      {
        if (margin.DrawingPosition.Contains(e.X, e.Y))
        {
          Cursor = margin.Cursor;
          margin.HandleMouseMove(e);
          if (_lastMouseInMargin != margin)
          {
            if (_lastMouseInMargin != null)
              _lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);

            _lastMouseInMargin = margin;
          }
          return;
        }
      }
      if (_lastMouseInMargin != null)
      {
        _lastMouseInMargin.HandleMouseLeave(EventArgs.Empty);
        _lastMouseInMargin = null;
      }
      if (_textView.DrawingPosition.Contains(e.X, e.Y))
      {
        TextLocation realmousepos = TextView.GetLogicalPosition(e.X - TextView.DrawingPosition.X, e.Y - TextView.DrawingPosition.Y);
        if (SelectionManager.IsSelected(Document.PositionToOffset(realmousepos)) && MouseButtons == MouseButtons.None)
        {
          // mouse is hovering over a selection, so show default mouse
          Cursor = Cursors.Default;
        }
        else
        {
          // mouse is hovering over text area, not a selection, so show the textView cursor
          Cursor = _textView.Cursor;
        }
        return;
      }
      Cursor = Cursors.Default;
    }


    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.GotFocus"></see> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
    protected override void OnGotFocus(EventArgs e)
    {
      base.OnGotFocus(e);
      Invalidate();
    }


    /// <summary>
    /// Raises the <see cref="E:System.Windows.Forms.Control.LostFocus"></see> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"></see> that contains the event data.</param>
    protected override void OnLostFocus(EventArgs e)
    {
      base.OnLostFocus(e);
      Invalidate();
    }


    /// <summary>
    /// Raises the <see cref="Control.Paint"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="PaintEventArgs"></see> that contains the event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
      int currentXPos = 0;
      int currentYPos = 0;
      bool adjustScrollBars = false;
      Graphics g = e.Graphics;
      Rectangle clipRectangle = e.ClipRectangle;

      if (clipRectangle.Width <= 0 || clipRectangle.Height <= 0)
        return;

      bool isFullRepaint = clipRectangle.X == 0 && clipRectangle.Y == 0
        && clipRectangle.Width == Width && clipRectangle.Height == Height;

      g.TextRenderingHint = TextEditorProperties.TextRenderingHint;

      foreach (AbstractMargin margin in _leftMargins)
      {
        if (margin.IsVisible)
        {
          Rectangle marginRectangle = new Rectangle(currentXPos, currentYPos, margin.Size.Width, Height - currentYPos);
          if (marginRectangle != margin.DrawingPosition)
          {
            // margin changed size
            if (!isFullRepaint && !clipRectangle.Contains(marginRectangle))
            {
              Invalidate(); // do a full repaint
            }
            adjustScrollBars = true;
            margin.DrawingPosition = marginRectangle;
          }
          currentXPos += margin.DrawingPosition.Width;
          if (clipRectangle.IntersectsWith(marginRectangle))
          {
            marginRectangle.Intersect(clipRectangle);
            if (!marginRectangle.IsEmpty)
              margin.Draw(g, marginRectangle);
          }
        }
      }

      Rectangle textViewArea = new Rectangle(currentXPos, currentYPos, Width - currentXPos, Height - currentYPos);
      if (textViewArea != _textView.DrawingPosition)
      {
        adjustScrollBars = true;
        _textView.DrawingPosition = textViewArea;
        // update caret position (but outside of WM_PAINT!)
        BeginInvoke((MethodInvoker) _caret.UpdateCaretPosition);
      }
      if (clipRectangle.IntersectsWith(textViewArea))
      {
        textViewArea.Intersect(clipRectangle);
        if (!textViewArea.IsEmpty)
          _textView.Draw(g, textViewArea);
      }

      if (adjustScrollBars)
        _motherTextAreaControl.AdjustScrollBars();

      base.OnPaint(e);
    }


    void DocumentFoldingsChanged(object sender, EventArgs e)
    {
      _caret.UpdateCaretPosition();
      _motherTextAreaControl.AdjustScrollBars();
      Invalidate();
    }


    internal bool IsReadOnly(int offset)
		{
			if (Document.ReadOnly)
				return true;

			if (TextEditorProperties.SupportsReadOnlySegments)
				return Document.MarkerStrategy.GetMarkers(offset).Exists(Marker.IsReadOnlyPredicate);
			else
				return false;
		}


    internal bool IsReadOnly(int offset, int length)
		{
			if (Document.ReadOnly)
				return true;

			if (TextEditorProperties.SupportsReadOnlySegments)
        return Document.MarkerStrategy.GetMarkers(offset, length).Exists(Marker.IsReadOnlyPredicate);
			else
				return false;
		}


    #region keyboard handling methods
    /// <summary>
    /// Determines if a character is an input character that the control recognizes.
    /// </summary>
    /// <param name="charCode">The character to test.</param>
    /// <returns>
    /// true if the character should be sent directly to the control and not preprocessed; otherwise, false.
    /// </returns>
    protected override bool IsInputChar(char charCode)
    {
      // Fixes SD2-747: Form containing the text editor and a button with a shortcut
      return true;
    }


    /// <summary>
    /// Simulates a key press on the <see cref="TextArea"/>.
    /// </summary>
    /// <param name="keyChar">The key char.</param>
    public void SimulateKeyPress(char keyChar)
    {
      OnKeyPress(new KeyPressEventArgs(keyChar));
    }


    /// <summary>
    /// Raises the <see cref="Control.KeyPress"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="KeyPressEventArgs"></see> that contains the event data.</param>
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
      if (SelectionManager.HasSomethingSelected)
      {
        if (SelectionManager.SelectionIsReadonly)
          return;
      }
      else if (IsReadOnly(Caret.Offset))
      {
        return;
      }

      if (e.KeyChar < ' ')
        return;

      if (!_mouseCursorHidden && TextEditorProperties.HideMouseCursor)
      {
        if (ClientRectangle.Contains(PointToClient(Cursor.Position)))
        {
          _mouseCursorHidePosition = Cursor.Position;
          _mouseCursorHidden = true;
          Cursor.Hide();
        }
      }
      CloseToolTip();

      BeginUpdate();
      Document.UndoStack.StartUndoGroup();
      try
      {
        // INSERT char

        // First, let all subscribers of the KeyPress event react.
        base.OnKeyPress(e);

        if (!e.Handled)
        {
          switch (Caret.CaretMode)
          {
            case CaretMode.InsertMode:
              InsertChar(e.KeyChar);
              break;
            case CaretMode.OverwriteMode:
              ReplaceChar(e.KeyChar);
              break;
            default:
              Debug.Assert(false, "Unknown caret mode " + Caret.CaretMode);
              break;
          }
        }

        int currentLineNr = Caret.Line;
        Document.FormattingStrategy.FormatLine(this, currentLineNr, Document.PositionToOffset(Caret.Position), e.KeyChar);
        EndUpdate();
      }
      finally
      {
        Document.UndoStack.EndUndoGroup();
      }
      e.Handled = true;
    }



    /// <summary>
    /// Simulates the press of a dialog key on the <see cref="TextArea"/>.
    /// </summary>
    /// <param name="keyData">The key data.</param>
    public void SimulateDialogKeyPress(Keys keyData)
    {
      ProcessDialogKey(keyData);
    }


    /// <summary>
    /// Processes a dialog key.
    /// </summary>
    /// <param name="keyData">One of the <see cref="Keys"></see> values that represents the key to process.</param>
    /// <returns>
    /// true if the key was processed by the control; otherwise, false.
    /// </returns>
    protected override bool ProcessDialogKey(Keys keyData)
    {
      // Try, if a dialog key is handled by a subscriber of the DialogKeyPress event.
      KeyEventArgs keyEventArgs = new KeyEventArgs(keyData);
      OnDialogKeyPress(keyEventArgs);
      if (keyEventArgs.Handled)
        return true;

      // if not (or the process was 'silent'), use the standard edit actions
      IEditAction action = _motherTextEditorControl.GetEditAction(keyData);
      AutoClearSelection = true;
      if (action != null)
      {
        BeginUpdate();
        try
        {
          lock (Document)
          {
            action.Execute(this);
            if (SelectionManager.HasSomethingSelected && AutoClearSelection /*&& caretchanged*/)
              if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal)
                SelectionManager.ClearSelection();
          }
        }
        finally
        {
          EndUpdate();
        }
        return true;
      }

      return base.ProcessDialogKey(keyData);
    }


    /// <summary>
    /// Raises the <see cref="DialogKeyPress"/> event.
    /// </summary>
    /// <param name="keyEventArgs">
    /// The <see cref="System.Windows.Forms.KeyEventArgs"/> instance  containing the event data.
    /// </param>
    protected virtual void OnDialogKeyPress(KeyEventArgs keyEventArgs)
    {
      KeyEventHandler handler = DialogKeyPress;

      if (handler != null)
        handler(this, keyEventArgs);
    }
    #endregion


    /// <summary>
    /// Scrolls to the position of the caret.
    /// </summary>
    public void ScrollToCaret()
    {
      _motherTextAreaControl.ScrollToCaret();
    }


    /// <summary>
    /// Scrolls to certain line.
    /// </summary>
    /// <param name="line">The line.</param>
    public void ScrollTo(int line)
    {
      _motherTextAreaControl.ScrollTo(line);
    }


    /// <summary>
    /// Begin of update.
    /// </summary>
    public void BeginUpdate()
    {
      _motherTextEditorControl.BeginUpdate();
    }


    /// <summary>
    /// End of update.
    /// </summary>
    public void EndUpdate()
    {
      _motherTextEditorControl.EndUpdate();
    }


    static string GenerateWhitespaceString(int length)
    {
      return new String(' ', length);
    }


    /// <summary>
    /// Inserts a single character at the caret position
    /// </summary>
    /// <param name="ch">The character.</param>
    public void InsertChar(char ch)
    {
      bool updating = _motherTextEditorControl.IsInUpdate;
      if (!updating)
        BeginUpdate();

      // filter out foreign whitespace chars and replace them with standard space (ASCII 32)
      if (Char.IsWhiteSpace(ch) && ch != '\t' && ch != '\n')
        ch = ' ';

      Document.UndoStack.StartUndoGroup();
      if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
          SelectionManager.Selections.Count > 0)
      {
        Caret.Position = SelectionManager.Selections[0].StartPosition;
        SelectionManager.RemoveSelectedText();
      }
      LineSegment caretLine = Document.GetLineSegment(Caret.Line);
      int offset = Caret.Offset;
      // use desired column for generated whitespaces
      int dc = Caret.Column;

      if (caretLine.Length < dc && ch != '\n')
        Document.Insert(offset, GenerateWhitespaceString(dc - caretLine.Length) + ch);
      else
        Document.Insert(offset, ch.ToString());

      Document.UndoStack.EndUndoGroup();
      ++Caret.Column;

      if (!updating)
      {
        EndUpdate();
        UpdateLineToEnd(Caret.Line, Caret.Column);
      }

      // I prefer to set NOT the standard column, if you type something
      //			++Caret.DesiredColumn;
    }


    /// <summary>
    /// Inserts a whole string at the caret position
    /// </summary>
    /// <param name="str">The string.</param>
    public void InsertString(string str)
    {
      bool updating = _motherTextEditorControl.IsInUpdate;
      if (!updating)
        BeginUpdate();

      try
      {
        Document.UndoStack.StartUndoGroup();
        if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal &&
            SelectionManager.Selections.Count > 0)
        {
          Caret.Position = SelectionManager.Selections[0].StartPosition;
          SelectionManager.RemoveSelectedText();
        }

        int oldOffset = Document.PositionToOffset(Caret.Position);
        int oldLine = Caret.Line;
        LineSegment caretLine = Document.GetLineSegment(Caret.Line);
        if (caretLine.Length < Caret.Column)
        {
          int whiteSpaceLength = Caret.Column - caretLine.Length;
          Document.Insert(oldOffset, GenerateWhitespaceString(whiteSpaceLength) + str);
          Caret.Position = Document.OffsetToPosition(oldOffset + str.Length + whiteSpaceLength);
        }
        else
        {
          Document.Insert(oldOffset, str);
          Caret.Position = Document.OffsetToPosition(oldOffset + str.Length);
        }
        Document.UndoStack.EndUndoGroup();
        if (oldLine != Caret.Line)
        {
          UpdateToEnd(oldLine);
        }
        else
        {
          UpdateLineToEnd(Caret.Line, Caret.Column);
        }
      }
      finally
      {
        if (!updating)
        {
          EndUpdate();
        }
      }
    }


    /// <summary>
    /// Replaces a character at the caret position
    /// </summary>
    /// <param name="ch">The character.</param>
    public void ReplaceChar(char ch)
    {
      bool updating = _motherTextEditorControl.IsInUpdate;
      if (!updating)
        BeginUpdate();

      if (Document.TextEditorProperties.DocumentSelectionMode == DocumentSelectionMode.Normal && SelectionManager.Selections.Count > 0)
      {
        Caret.Position = SelectionManager.Selections[0].StartPosition;
        SelectionManager.RemoveSelectedText();
      }

      int lineNr = Caret.Line;
      LineSegment line = Document.GetLineSegment(lineNr);
      int offset = Document.PositionToOffset(Caret.Position);
      if (offset < line.Offset + line.Length)
        Document.Replace(offset, 1, ch.ToString());
      else
        Document.Insert(offset, ch.ToString());

      if (!updating)
      {
        EndUpdate();
        UpdateLineToEnd(lineNr, Caret.Column);
      }
      ++Caret.Column;
      //			++Caret.DesiredColumn;
    }


    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="Control"></see> and its child controls and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (!_disposed)
        {
          _disposed = true;
          if (_caret != null)
          {
            _caret.PositionChanged -= SearchMatchingBracket;
            _caret.Dispose();
          }
          Document.TextContentChanged -= TextContentChanged;
          Document.FoldingManager.FoldingChanged -= DocumentFoldingsChanged;
          _motherTextAreaControl = null;
          _motherTextEditorControl = null;
          foreach (AbstractMargin margin in _leftMargins)
          {
            if (margin is IDisposable)
              (margin as IDisposable).Dispose();
          }
          _textView.Dispose();
        }
      }
      base.Dispose(disposing);
    }


    #region UPDATE Commands
    internal void UpdateLine(int line)
    {
      UpdateLines(0, line, line);
    }


    internal void UpdateLines(int lineBegin, int lineEnd)
    {
      UpdateLines(0, lineBegin, lineEnd);
    }

    
    internal void UpdateToEnd(int lineBegin)
    {
      lineBegin = Document.GetVisibleLine(lineBegin);
      int y = Math.Max(0, lineBegin * _textView.LineHeight);
      y = Math.Max(0, y - _virtualTop.Y);
      Rectangle r = new Rectangle(0, y, Width, Height - y);
      Invalidate(r);
    }


    internal void UpdateLineToEnd(int lineNr, int xStart)
    {
      UpdateLines(xStart, lineNr, lineNr);
    }


    internal void UpdateLine(int line, int begin, int end)
    {
      UpdateLines(line, line);
    }


    int FirstPhysicalLine
    {
      get { return VirtualTop.Y / _textView.LineHeight; }
    }


    internal void UpdateLines(int xPos, int lineBegin, int lineEnd)
    {
      InvalidateLines(lineBegin, lineEnd);
    }


    void InvalidateLines(int lineBegin, int lineEnd)
    {
      lineBegin = Math.Max(Document.GetVisibleLine(lineBegin), FirstPhysicalLine);
      lineEnd = Math.Min(Document.GetVisibleLine(lineEnd), FirstPhysicalLine + _textView.NumberOfVisibleLines);
      int y = Math.Max(0, lineBegin * _textView.LineHeight);
      int height = Math.Min(_textView.DrawingPosition.Height, (1 + lineEnd - lineBegin) * (_textView.LineHeight + 1));
      Rectangle r = new Rectangle(0, y - 1 - _virtualTop.Y, Width, height + 3);
      Invalidate(r);
    }
    #endregion
  }
}
