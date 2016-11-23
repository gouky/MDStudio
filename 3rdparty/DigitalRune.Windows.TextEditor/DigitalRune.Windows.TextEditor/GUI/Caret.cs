using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// In this enumeration are all caret modes listed.
  /// </summary>
  public enum CaretMode
  {
    /// <summary>
    /// If the caret is in insert mode typed characters will be
    /// inserted at the caret position
    /// </summary>
    InsertMode,

    /// <summary>
    /// If the caret is in overwrite mode typed characters will
    /// overwrite the character at the caret position
    /// </summary>
    OverwriteMode
  }


  /// <summary>
  /// Renders the caret.
  /// </summary>
  public class Caret : IDisposable
  {
    /// <summary>
    /// Value indicating whether the current caret position needs to be validated.
    /// </summary>
    /// <remarks>
    /// The caret uses 'lazy validation': When a new caret position is set, the new position is not 
    /// immediately updated. The position is validated as soon as someone accesses the position (via 
    /// <see cref="Line"/>, <see cref="Column"/>, etc.). This is necessary for the following reasons: 
    /// If someone sets, <see cref="Line"/> and  <see cref="Column"/> independently the position 
    /// might be invalid after the first component is set. Therefore the validation needs to be 
    /// delayed until all components are set. Another reason is that during a longer update the 
    /// document might invalid, so the caret cannot be validated immediately. 
    /// </remarks>
    private bool _isValidationRequired;

    private int _line;
    private int _oldLine = -1;
    private int _column;
    private int _desiredColumn;
    private CaretMode _caretMode;
    private static bool _caretCreated;
    private bool _hidden = true;
    private TextArea _textArea;
    private Point _currentPos = new Point(-1, -1);
    private Ime _ime;
    private bool _firePositionChangedAfterUpdateEnd;
    private bool _outstandingUpdate;


    /// <summary>
    /// Gets or sets the desired column (in pixels).
    /// </summary>
    /// <value>
    /// The 'prefered' xPos in which the caret moves, when it is moved
    /// up/down. Measured in pixels, not in characters!
    /// </value>
    public int DesiredColumn
    {
      get { return _desiredColumn; }
      set { _desiredColumn = value; }
    }


    /// <summary>
    /// Gets or sets the caret mode.
    /// </summary>
    /// <value>The current caret mode.</value>
    public CaretMode CaretMode
    {
      get { return _caretMode; }
      set
      {
        _caretMode = value;
        OnCaretModeChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Gets or sets the line.
    /// </summary>
    /// <value>The line.</value>
    public int Line
    {
      get
      {
        if (_isValidationRequired)
          ValidateCaretPos();

        return _line;
      }

      set
      {
        _line = value;
        _isValidationRequired = true;
        UpdateCaretPosition();
        OnPositionChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Gets or sets the column.
    /// </summary>
    /// <value>The column.</value>
    public int Column
    {
      get
      {
        if (_isValidationRequired)
          ValidateCaretPos();

        return _column;
      }
      set
      {
        _column = value;
        _isValidationRequired = true;
        UpdateCaretPosition();
        OnPositionChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    /// <value>The position.</value>
    public TextLocation Position
    {
      get
      {
        if (_isValidationRequired)
          ValidateCaretPos();

        return new TextLocation(_column, _line);
      }

      set
      {
        _line = value.Y;
        _column = value.X;
        _isValidationRequired = true;
        UpdateCaretPosition();
        OnPositionChanged(EventArgs.Empty);
      }
    }


    /// <summary>
    /// Gets the offset in the text buffer.
    /// </summary>
    /// <value>The offset in the text buffer.</value>
    public int Offset
    {
      get { return _textArea.Document.PositionToOffset(Position); }
    }


    /// <remarks>
    /// Is called each time the caret is moved.
    /// </remarks>
    public event EventHandler PositionChanged;


    /// <remarks>
    /// Is called each time the CaretMode has changed.
    /// </remarks>
    public event EventHandler CaretModeChanged;


    /// <summary>
    /// Initializes a new instance of the <see cref="Caret"/> class.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public Caret(TextArea textArea)
    {
      _textArea = textArea;
      textArea.GotFocus += GotFocus;
      textArea.LostFocus += LostFocus;
    }


    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      DisposeCaret();
      _textArea.GotFocus -= GotFocus;
      _textArea.LostFocus -= LostFocus;
      _textArea = null;
    }


    /// <summary>
    /// Validates the position.
    /// </summary>
    /// <param name="pos">The position.</param>
    /// <returns><paramref name="pos"/> or the closest valid position.</returns>
    public TextLocation ValidatePosition(TextLocation pos)
    {
      int line = Math.Max(0, Math.Min(_textArea.Document.TotalNumberOfLines - 1, pos.Y));
      int column = Math.Max(0, pos.X);

      if (column == int.MaxValue || !_textArea.TextEditorProperties.AllowCaretBeyondEOL)
      {
        LineSegment lineSegment = _textArea.Document.GetLineSegment(line);
        column = Math.Min(column, lineSegment.Length);
      }
      return new TextLocation(column, line);
    }


    /// <summary>
    /// Validates the caret position.
    /// </summary>
    /// <remarks>
    /// If the caret position is outside the document text bounds
    /// it is set to the correct position by calling ValidateCaretPos.
    /// </remarks>
    public void ValidateCaretPos()
    {
      _line = Math.Max(0, Math.Min(_textArea.Document.TotalNumberOfLines - 1, _line));
      _column = Math.Max(0, _column);

      if (_column == int.MaxValue || !_textArea.TextEditorProperties.AllowCaretBeyondEOL)
      {
        LineSegment lineSegment = _textArea.Document.GetLineSegment(_line);
        _column = Math.Min(_column, lineSegment.Length);
      }

      _isValidationRequired = false;
    }


    /// <summary>
    /// Creates the caret.
    /// </summary>
    void CreateCaret()
    {
      while (!_caretCreated)
      {
        switch (_caretMode)
        {
          case CaretMode.InsertMode:
            _caretCreated = CreateCaret(_textArea.Handle, IntPtr.Zero, 2, _textArea.TextView.FontHeight);
            break;
          case CaretMode.OverwriteMode:
            _caretCreated = CreateCaret(_textArea.Handle, IntPtr.Zero, _textArea.TextView.SpaceWidth, _textArea.TextView.FontHeight);
            break;
        }
      }
      if (_currentPos.X < 0)
      {
        ValidateCaretPos();
        _currentPos = ScreenPosition;
      }
      SetCaretPos(_currentPos.X, _currentPos.Y);
      ShowCaret(_textArea.Handle);
    }


    /// <summary>
    /// Recreates the caret.
    /// </summary>
    public void RecreateCaret()
    {
      DisposeCaret();
      if (!_hidden)
        CreateCaret();
    }


    /// <summary>
    /// Disposes the caret.
    /// </summary>
    void DisposeCaret()
    {
      if (_caretCreated)
      {
        _caretCreated = false;
        HideCaret(_textArea.Handle);
        DestroyCaret();
      }
    }


    void GotFocus(object sender, EventArgs e)
    {
      _hidden = false;
      if (!_textArea.MotherTextEditorControl.IsInUpdate)
      {
        CreateCaret();
        UpdateCaretPosition();
      }
    }


    void LostFocus(object sender, EventArgs e)
    {
      _hidden = true;
      DisposeCaret();
    }


    /// <summary>
    /// Gets the screen position.
    /// </summary>
    /// <value>The screen position.</value>
    public Point ScreenPosition
    {
      get
      {
        if (_isValidationRequired)
          ValidateCaretPos();

        int xpos = _textArea.TextView.GetDrawingXPos(_line, _column);
        int ypos = (_textArea.Document.GetVisibleLine(_line)) * _textArea.TextView.LineHeight - _textArea.TextView.TextArea.VirtualTop.Y;
        return new Point(_textArea.TextView.DrawingPosition.X + xpos,
                         _textArea.TextView.DrawingPosition.Y + ypos);
      }
    }


    internal void OnEndUpdate()
    {
      if (_outstandingUpdate)
        UpdateCaretPosition();
    }


    /// <summary>
    /// Updates the caret position.
    /// </summary>
    public void UpdateCaretPosition()
    {
      if (_textArea.MotherTextAreaControl.TextEditorProperties.LineViewerStyle == LineViewerStyle.FullRow && _oldLine != _line)
      {
        _textArea.UpdateLine(_oldLine);
        _textArea.UpdateLine(_line);
      }
      _oldLine = _line;

      if (_hidden || _textArea.MotherTextEditorControl.IsInUpdate)
      {
        _outstandingUpdate = true;
        return;
      }
      else
      {
        _outstandingUpdate = false;
      }

      ValidateCaretPos();
      int lineNr = _line;
      int xpos = _textArea.TextView.GetDrawingXPos(lineNr, _column);
      Point pos = ScreenPosition;
      if (xpos >= 0)
      {
        CreateCaret();
        bool success = SetCaretPos(pos.X, pos.Y);
        if (!success)
        {
          DestroyCaret();
          _caretCreated = false;
          UpdateCaretPosition();
        }
      }
      else
      {
        DestroyCaret();
      }
      // set the input method editor location
      if (_ime == null)
      {
        _ime = new Ime(_textArea.Handle, _textArea.Document.TextEditorProperties.Font);
      }
      else
      {
        _ime.HWnd = _textArea.Handle;
        _ime.Font = _textArea.Document.TextEditorProperties.Font;
      }
      _ime.SetIMEWindowLocation(pos.X, pos.Y);

      _currentPos = pos;
    }

    #region Native caret functions
    [DllImport("User32.dll")]
    static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);

    [DllImport("User32.dll")]
    static extern bool SetCaretPos(int x, int y);

    [DllImport("User32.dll")]
    static extern bool DestroyCaret();

    [DllImport("User32.dll")]
    static extern bool ShowCaret(IntPtr hWnd);

    [DllImport("User32.dll")]
    static extern bool HideCaret(IntPtr hWnd);
    #endregion


    void FirePositionChangedAfterUpdateEnd(object sender, EventArgs e)
    {
      OnPositionChanged(EventArgs.Empty);
    }


    /// <summary>
    /// Raises the <see cref="PositionChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnPositionChanged(EventArgs e)
    {
      if (_textArea.MotherTextEditorControl.IsInUpdate)
      {
        if (_firePositionChangedAfterUpdateEnd == false)
        {
          _firePositionChangedAfterUpdateEnd = true;
          _textArea.Document.UpdateCommited += FirePositionChangedAfterUpdateEnd;
        }
        return;
      }

      if (_firePositionChangedAfterUpdateEnd)
      {
        _textArea.Document.UpdateCommited -= FirePositionChangedAfterUpdateEnd;
        _firePositionChangedAfterUpdateEnd = false;
      }

      List<Fold> foldings = _textArea.Document.FoldingManager.GetFoldsFromPosition(_line, _column);
      bool shouldUpdate = false;
      foreach (Fold fold in foldings)
      {
        shouldUpdate |= fold.IsFolded;
        fold.IsFolded = false;
      }

      if (shouldUpdate)
      {
        _textArea.Document.FoldingManager.NotifyFoldingChanged(EventArgs.Empty);
      }

      if (PositionChanged != null)
      {
        PositionChanged(this, e);
      }
      _textArea.ScrollToCaret();
    }


    /// <summary>
    /// Raises the <see cref="CaretModeChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnCaretModeChanged(EventArgs e)
    {
      if (CaretModeChanged != null)
      {
        CaretModeChanged(this, e);
      }
      HideCaret(_textArea.Handle);
      DestroyCaret();
      _caretCreated = false;
      CreateCaret();
      ShowCaret(_textArea.Handle);
    }
  }
}
