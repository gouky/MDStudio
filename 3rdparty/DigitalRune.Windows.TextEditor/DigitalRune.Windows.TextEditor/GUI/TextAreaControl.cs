using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Properties;
using DigitalRune.Windows.TextEditor.Utilities;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// Container control for text area, horizontal ruler, and scrollbars.
  /// </summary>
  /// <remarks>
  /// This contains a <see cref="TextArea"/> control and adds a horizontal ruler and scrollbars.
  /// It manages the scrolling/positioning (via scrollbars, mouse wheel, or code) and resizing 
  /// of the <see cref="TextArea"/>.
  /// </remarks>
  [ToolboxItem(false)]
  public class TextAreaControl : Panel
  {
    private bool _disposed;

    // Parent control
    private TextEditorControl _motherTextEditorControl;

    // Child controls
    private readonly TextArea _textArea;
    private HRuler _hRuler;
    private VScrollBar _vScrollBar = new VScrollBar();
    private HScrollBar _hScrollBar = new HScrollBar();

    private bool _doHandleMousewheel = true;
    private bool _adjustScrollBarsOnNextUpdate;
    private Point _scrollToPosOnNextUpdate;
    private int[] _lineLengthCache;
    private const int _LineLengthCacheAdditionalSize = 100;
    private const int _scrollMarginHeight = 3;
    private readonly MouseWheelHandler _mouseWheelHandler = new MouseWheelHandler();


    /// <summary>
    /// Gets the mother text editor control.
    /// </summary>
    /// <value>The mother text editor control.</value>
    public TextEditorControl MotherTextEditorControl
    {
      get { return _motherTextEditorControl; }
    }


    private IDocument Document
    {
      get
      {
        if (_motherTextEditorControl != null)
          return _motherTextEditorControl.Document;

        return null;
      }
    }


    /// <summary>
    /// Gets the text editor properties.
    /// </summary>
    /// <value>The text editor properties.</value>
    public ITextEditorProperties TextEditorProperties
    {
      get
      {
        if (_motherTextEditorControl != null)
          return _motherTextEditorControl.TextEditorProperties;
        return null;
      }
    }


    /// <summary>
    /// Gets the text area.
    /// </summary>
    /// <value>The text area.</value>
    public TextArea TextArea
    {
      get { return _textArea; }
    }


    /// <summary>
    /// Gets the caret.
    /// </summary>
    /// <value>The caret.</value>
    public Caret Caret
    {
      get { return _textArea.Caret; }
    }


    /// <summary>
    /// Gets the vertical scroll bar.
    /// </summary>
    /// <value>The vertical scroll bar.</value>
    public VScrollBar VScrollBar
    {
      get { return _vScrollBar; }
    }


    /// <summary>
    /// Gets the horizontal scroll bar.
    /// </summary>
    /// <value>The horizontal scroll bar.</value>
    public HScrollBar HScrollBar
    {
      get { return _hScrollBar; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to handle the mouse wheel.
    /// </summary>
    /// <value><see langword="true"/> if the mouse wheel is handle by this control; otherwise, <see langword="false"/>.</value>
    public bool DoHandleMousewheel
    {
      get { return _doHandleMousewheel; }
      set { _doHandleMousewheel = value; }
    }


    /// <summary>
    /// Occurs when the context menu is requested.
    /// </summary>
    public event MouseEventHandler ContextMenuRequest;


    /// <summary>
    /// Initializes a new instance of the <see cref="TextAreaControl"/> class.
    /// </summary>
    /// <param name="motherTextEditorControl">The mother text editor control.</param>
    public TextAreaControl(TextEditorControl motherTextEditorControl)
    {
      _motherTextEditorControl = motherTextEditorControl;
      _textArea = new TextArea(motherTextEditorControl, this);
      Controls.Add(_textArea);

      _vScrollBar.ValueChanged += VScrollBarValueChanged;
      _hScrollBar.ValueChanged += HScrollBarValueChanged;
      SetScrollBars();

      ResizeRedraw = true;

      Document.TextContentChanged += DocumentTextContentChanged;
      Document.DocumentChanged += AdjustScrollBarsOnDocumentChange;
      Document.UpdateCommited += DocumentUpdateCommitted;
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
          Document.TextContentChanged -= DocumentTextContentChanged;
          Document.DocumentChanged -= AdjustScrollBarsOnDocumentChange;
          Document.UpdateCommited -= DocumentUpdateCommitted;
          _motherTextEditorControl = null;
          if (_vScrollBar != null)
          {
            _vScrollBar.Dispose();
            _vScrollBar = null;
          }
          if (_hScrollBar != null)
          {
            _hScrollBar.Dispose();
            _hScrollBar = null;
          }
          if (_hRuler != null)
          {
            _hRuler.Dispose();
            _hRuler = null;
          }
        }
      }
      base.Dispose(disposing);
    }


    void DocumentTextContentChanged(object sender, EventArgs e)
    {
      // after the text content is changed abruptly, we need to validate the
      // caret position - otherwise the caret position is invalid for a short amount
      // of time, which can break client code that expects that the caret position is always valid
      Caret.ValidateCaretPos();
    }


    private void SetScrollBars()
    {
      if (_textArea.TextEditorProperties.ShowScrollBars)
      {
        if (!Controls.Contains(_hScrollBar))
          Controls.Add(_hScrollBar);

        if (!Controls.Contains(_vScrollBar))
          Controls.Add(_vScrollBar);

        ResizeTextArea();
      }
      else
      {
        if (Controls.Contains(_hScrollBar))
          Controls.Remove(_hScrollBar);

        if (Controls.Contains(_vScrollBar))
          Controls.Remove(_vScrollBar);

        ResizeTextArea();
      }
    }

    /// <summary>
    /// Raises the <see cref="Control.Resize"></see> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"></see> that contains the event data.</param>
    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      ResizeTextArea();
    }


    /// <summary>
    /// Resizes the text area.
    /// </summary>
    public void ResizeTextArea()
    {
      int y = 0;
      int h = 0;
      if (_hRuler != null)
      {
        _hRuler.Width = Width - SystemInformation.HorizontalScrollBarArrowWidth;
        y = _hRuler.Bounds.Bottom;
        h = _hRuler.Bounds.Height;
      }

      if (_textArea.TextEditorProperties.ShowScrollBars)
      {
        _textArea.Bounds = new Rectangle(0, y,
                                        Width - SystemInformation.HorizontalScrollBarArrowWidth,
                                        Height - SystemInformation.VerticalScrollBarArrowHeight - h);
        SetScrollBarBounds();
      }
      else
      {
        _textArea.Bounds = new Rectangle(0, y, Width, Height - h);
      }
    }


    /// <summary>
    /// Sets the scroll bar bounds.
    /// </summary>
    public void SetScrollBarBounds()
    {
      _vScrollBar.Bounds = new Rectangle(_textArea.Bounds.Right, 0, SystemInformation.HorizontalScrollBarArrowWidth, Height - SystemInformation.VerticalScrollBarArrowHeight);
      _hScrollBar.Bounds = new Rectangle(0, _textArea.Bounds.Bottom, Width - SystemInformation.HorizontalScrollBarArrowWidth, SystemInformation.VerticalScrollBarArrowHeight);
    }


    void AdjustScrollBarsOnDocumentChange(object sender, DocumentEventArgs e)
    {
      if (_motherTextEditorControl.IsInUpdate == false)
      {
        AdjustScrollBarsClearCache();
        AdjustScrollBars();
      }
      else
      {
        _adjustScrollBarsOnNextUpdate = true;
      }
    }


    void DocumentUpdateCommitted(object sender, EventArgs e)
    {
      if (_motherTextEditorControl.IsInUpdate == false)
      {
        Caret.ValidateCaretPos();

        // Adjust acroll bars
        if (!_scrollToPosOnNextUpdate.IsEmpty)
        {
          ScrollTo(_scrollToPosOnNextUpdate.Y, _scrollToPosOnNextUpdate.X);
        }
        if (_adjustScrollBarsOnNextUpdate)
        {
          AdjustScrollBarsClearCache();
          AdjustScrollBars();
        }
      }
    }


    void AdjustScrollBarsClearCache()
    {
      if (_lineLengthCache != null)
      {
        if (_lineLengthCache.Length < Document.TotalNumberOfLines + 2 * _LineLengthCacheAdditionalSize)
        {
          _lineLengthCache = null;
        }
        else
        {
          Array.Clear(_lineLengthCache, 0, _lineLengthCache.Length);
        }
      }
    }


    /// <summary>
    /// Adjusts the scroll bars.
    /// </summary>
    public void AdjustScrollBars()
    {
      _adjustScrollBarsOnNextUpdate = false;
      _vScrollBar.Minimum = 0;
      // number of visible lines in document (folding!)
      _vScrollBar.Maximum = _textArea.MaxVScrollValue;
      int max = 0;

      int firstLine = _textArea.TextView.FirstLogicalLine;
      int lastLine = Document.GetFirstLogicalLine(_textArea.TextView.FirstPhysicalLine + _textArea.TextView.NumberOfVisibleLines);
      if (lastLine >= Document.TotalNumberOfLines)
        lastLine = Document.TotalNumberOfLines - 1;

      if (_lineLengthCache == null || _lineLengthCache.Length <= lastLine)
      {
        _lineLengthCache = new int[lastLine + _LineLengthCacheAdditionalSize];
      }

      for (int lineNumber = firstLine; lineNumber <= lastLine; lineNumber++)
      {
        LineSegment lineSegment = Document.GetLineSegment(lineNumber);
        if (Document.FoldingManager.IsLineVisible(lineNumber))
        {
          if (_lineLengthCache[lineNumber] > 0)
          {
            max = Math.Max(max, _lineLengthCache[lineNumber]);
          }
          else
          {
            int visualLength = _textArea.TextView.GetVisualColumnFast(lineSegment, lineSegment.Length);
            _lineLengthCache[lineNumber] = Math.Max(1, visualLength);
            max = Math.Max(max, visualLength);
          }
        }
      }
      _hScrollBar.Minimum = 0;
      _hScrollBar.Maximum = (Math.Max(max + 20, _textArea.TextView.NumberOfVisibleColumns - 1));

      _vScrollBar.LargeChange = Math.Max(0, _textArea.TextView.DrawingPosition.Height);
      _vScrollBar.SmallChange = Math.Max(0, _textArea.TextView.LineHeight);

      _hScrollBar.LargeChange = Math.Max(0, _textArea.TextView.NumberOfVisibleColumns - 1);
      _hScrollBar.SmallChange = Math.Max(0, _textArea.TextView.SpaceWidth);
    }


    /// <summary>
    /// Notifies all child controls that the properties of the text edtiors have changed.
    /// </summary>
    public void OptionsChanged()
    {
      _textArea.OptionsChanged();

      if (_textArea.TextEditorProperties.ShowHorizontalRuler)
      {
        if (_hRuler == null)
        {
          _hRuler = new HRuler(_textArea);
          Controls.Add(_hRuler);
          ResizeTextArea();
        }
        else
        {
          _hRuler.Invalidate();
        }
      }
      else
      {
        if (_hRuler != null)
        {
          Controls.Remove(_hRuler);
          _hRuler.Dispose();
          _hRuler = null;
          ResizeTextArea();
        }
      }

      SetScrollBars();
      AdjustScrollBars();
    }


    void VScrollBarValueChanged(object sender, EventArgs e)
    {
      _textArea.VirtualTop = new Point(_textArea.VirtualTop.X, _vScrollBar.Value);
      Invalidate();
      AdjustScrollBars();
    }


    void HScrollBarValueChanged(object sender, EventArgs e)
    {
      _textArea.VirtualTop = new Point(_hScrollBar.Value * _textArea.TextView.ColumnWidth, _textArea.VirtualTop.Y);
      _textArea.Invalidate();
    }


    /// <summary>
    /// Handles mouse wheel events.
    /// </summary>
    /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    public void HandleMouseWheel(MouseEventArgs e)
    {
      int scrollDistance = _mouseWheelHandler.GetScrollAmount(e);
      if (scrollDistance == 0)
        return;
      if ((ModifierKeys & Keys.Control) != 0 && TextEditorProperties.MouseWheelTextZoom)
      {
        if (scrollDistance > 0)
        {
          _motherTextEditorControl.Font = new Font(_motherTextEditorControl.Font.Name, _motherTextEditorControl.Font.Size + 1);
        }
        else
        {
          _motherTextEditorControl.Font = new Font(_motherTextEditorControl.Font.Name, Math.Max(6, _motherTextEditorControl.Font.Size - 1));
        }
      }
      else
      {
        if (TextEditorProperties.MouseWheelScrollDown)
          scrollDistance = -scrollDistance;
        int newValue = _vScrollBar.Value + _vScrollBar.SmallChange * scrollDistance;
        _vScrollBar.Value = Math.Max(_vScrollBar.Minimum, Math.Min(_vScrollBar.Maximum - _vScrollBar.LargeChange + 1, newValue));
      }
    }


    /// <summary>
    /// Raises the <see cref="Control.MouseWheel"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="MouseEventArgs"></see> that contains the event data.</param>
    protected override void OnMouseWheel(MouseEventArgs e)
    {
      base.OnMouseWheel(e);
      if (DoHandleMousewheel)
        HandleMouseWheel(e);
    }


    /// <summary>
    /// Scrolls to current caret position.
    /// </summary>
    public void ScrollToCaret()
    {
      ScrollTo(_textArea.Caret.Line, _textArea.Caret.Column);
    }


    /// <summary>
    /// Scrolls to a given line/column.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="column">The column.</param>
    public void ScrollTo(int line, int column)
    {
      if (_motherTextEditorControl.IsInUpdate)
      {
        _scrollToPosOnNextUpdate = new Point(column, line);
        return;
      }
      else
      {
        _scrollToPosOnNextUpdate = Point.Empty;
      }

      ScrollTo(line);

      int curCharMin = _hScrollBar.Value - _hScrollBar.Minimum;
      int curCharMax = curCharMin + _textArea.TextView.NumberOfVisibleColumns;

      int pos = _textArea.TextView.GetVisualColumn(line, column);

      if (_textArea.TextView.NumberOfVisibleColumns < 0)
      {
        _hScrollBar.Value = 0;
      }
      else
      {
        if (pos < curCharMin)
        {
          _hScrollBar.Value = Math.Max(0, pos - _scrollMarginHeight);
        }
        else
        {
          if (pos > curCharMax)
          {
            _hScrollBar.Value = Math.Max(0, Math.Min(_hScrollBar.Maximum, (pos - _textArea.TextView.NumberOfVisibleColumns + _scrollMarginHeight)));
          }
        }
      }
    }


    /// <summary>
    /// Scrolls to a given line.
    /// </summary>
    /// <param name="line">The line.</param>
    public void ScrollTo(int line)
    {
      line = Math.Max(0, Math.Min(Document.TotalNumberOfLines - 1, line));
      line = Document.GetVisibleLine(line);
      int curLineMin = _textArea.TextView.FirstPhysicalLine;
      if (_textArea.TextView.RemainderOfFirstVisibleLine > 0)
      {
        curLineMin++;
      }

      if (line - _scrollMarginHeight + 3 < curLineMin)
      {
        _vScrollBar.Value = Math.Max(0, Math.Min(_vScrollBar.Maximum, (line - _scrollMarginHeight + 3) * _textArea.TextView.LineHeight));
        VScrollBarValueChanged(this, EventArgs.Empty);
      }
      else
      {
        int curLineMax = curLineMin + _textArea.TextView.NumberOfVisibleLines;
        if (line + _scrollMarginHeight - 1 > curLineMax)
        {
          if (_textArea.TextView.NumberOfVisibleLines == 1)
          {
            _vScrollBar.Value = Math.Max(0, Math.Min(_vScrollBar.Maximum, (line - _scrollMarginHeight - 1) * _textArea.TextView.LineHeight));
          }
          else
          {
            _vScrollBar.Value = Math.Min(_vScrollBar.Maximum,
                                        (line - _textArea.TextView.NumberOfVisibleLines + _scrollMarginHeight - 1) * _textArea.TextView.LineHeight);
          }
          VScrollBarValueChanged(this, EventArgs.Empty);
        }
      }
    }


    /// <summary>
    /// Scroll so that the specified line is centered.
    /// </summary>
    /// <param name="line">Line to center view on</param>
    /// <param name="treshold">
    /// If this action would cause scrolling by less than or equal to
    /// <paramref name="treshold"/> lines in any direction, don't scroll.  Use -1 
    /// to always center the view.
    /// </param>
    public void CenterViewOn(int line, int treshold)
    {
      line = Math.Max(0, Math.Min(Document.TotalNumberOfLines - 1, line));
      // convert line to visible line:
      line = Document.GetVisibleLine(line);
      // subtract half the visible line count
      line -= _textArea.TextView.NumberOfVisibleLines / 2;

      int curLineMin = _textArea.TextView.FirstPhysicalLine;
      if (_textArea.TextView.RemainderOfFirstVisibleLine > 0)
      {
        curLineMin++;
      }
      if (Math.Abs(curLineMin - line) > treshold)
      {
        // scroll:
        _vScrollBar.Value = Math.Max(0, Math.Min(_vScrollBar.Maximum, (line - _scrollMarginHeight + 3) * _textArea.TextView.LineHeight));
        VScrollBarValueChanged(this, EventArgs.Empty);
      }
    }


    /// <summary>
    /// Jumps to a given position in the document.
    /// </summary>
    /// <param name="line">The line.</param>
    public void JumpTo(int line)
    {
      line = Math.Min(line, Document.TotalNumberOfLines - 1);
      string text = Document.GetText(Document.GetLineSegment(line));
      JumpTo(line, text.Length - text.TrimStart().Length);
    }


    /// <summary>
    /// Jumps to a given position in the document.
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="column">The column.</param>
    public void JumpTo(int line, int column)
    {
      _textArea.Focus();
      _textArea.SelectionManager.ClearSelection();
      _textArea.Caret.Position = new TextLocation(column, line);
      _textArea.SetDesiredColumn();
      ScrollToCaret();
    }


    /// <summary>
    /// WNDs the proc.
    /// </summary>
    /// <param name="m">The m.</param>
    protected override void WndProc(ref Message m)
    {
      if (m.Msg == 0x007B)
      { 
        // handle WM_CONTEXTMENU
        if (ContextMenuRequest != null)
        {
          long lParam = m.LParam.ToInt64();
          int x = unchecked((short) (lParam & 0xffff));
          int y = unchecked((short) ((lParam & 0xffff0000) >> 16));
          if (x == -1 && y == -1)
          {
            Point pos = Caret.ScreenPosition;
            ContextMenuRequest(this, new MouseEventArgs(MouseButtons.None, 0, pos.X, pos.Y + _textArea.TextView.LineHeight, 0));
          }
          else
          {
            Point pos = PointToClient(new Point(x, y));
            ContextMenuRequest(this, new MouseEventArgs(MouseButtons.Right, 1, pos.X, pos.Y, 0));
          }
        }
      }
      base.WndProc(ref m);
    }
  }
}
