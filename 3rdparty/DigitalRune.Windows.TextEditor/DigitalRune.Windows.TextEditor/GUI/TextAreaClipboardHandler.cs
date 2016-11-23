using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Utilities;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// Performs the clipboard actions (cut, copy, paste, ...) for a text area.
  /// </summary>
  internal class TextAreaClipboardHandler
  {
    private const string LineSelectedType = "MSDEVLineSelect";  // This is the type VS 2003 and 2005 use for flagging a whole line copy 
    private readonly TextArea _textArea;


    /// <summary>
    /// Gets a value indicating whether Cut is enabled.
    /// </summary>
    /// <value><see langword="true"/> if Cut is enabled; otherwise, <see langword="false"/>.</value>
    public bool EnableCut
    {
      get { return _textArea.EnableCutOrPaste; }
    }


    /// <summary>
    /// Gets a value indicating whether Copy is enabled.
    /// </summary>
    /// <value><see langword="true"/> if copy is enabled; otherwise, <see langword="false"/>.</value>
    public bool EnableCopy
    {
      get { return true; }
    }


    /// <summary>
    /// Checks whether the clipboard contains text.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the clipboard contains text; otherwise <see langword="false"/>.
    /// </returns>
    public delegate bool ClipboardContainsTextDelegate();

    /// <summary>
    /// Is called when <see cref="EnablePaste"/> is queried and checks whether the clipboards 
    /// contains text. If this property is <see langword="null"/> (the default value), the text editor uses
    /// <see cref="Clipboard.ContainsText()"/>.
    /// </summary>
    /// <remarks>
    /// This property is useful if you want to prevent the default <see cref="Clipboard.ContainsText()"/> 
    /// behaviour that waits for the clipboard to be available - the clipboard might never become 
    /// available if it is owned by a process that is paused by the debugger.
    /// </remarks>
    public static ClipboardContainsTextDelegate GetClipboardContainsText;
		

    /// <summary>
    /// Gets a value indicating whether Paste is enabled (i.e. something is in the clipboard).
    /// </summary>
    /// <value><see langword="true"/> if Paste is enabled; otherwise, <see langword="false"/>.</value>
    public bool EnablePaste
    {
      get
      {
        if (!_textArea.EnableCutOrPaste)
          return false;

				ClipboardContainsTextDelegate d = GetClipboardContainsText;
        if (d != null)
        {
          return d();
        }

        try
        {
          return Clipboard.ContainsText();
        }
        catch (ExternalException)
        {
          return false;
        }
      }
    }


    /// <summary>
    /// Gets a value indicating whether 'delete' is enabled.
    /// </summary>
    /// <value><see langword="true"/> if 'delete' is enabled; otherwise, <see langword="false"/>.</value>
    public bool EnableDelete
    {
      get { return _textArea.SelectionManager.HasSomethingSelected && !_textArea.SelectionManager.SelectionIsReadonly; }
    }


    /// <summary>
    /// Gets a value indicating whether 'select all' is enabled.
    /// </summary>
    /// <value><see langword="true"/> if 'select all' is enabled; otherwise, <see langword="false"/>.</value>
    public bool EnableSelectAll
    {
      get { return true; }
    }


    /// <summary>
    /// Occurs when a text is copied into the clipboard.
    /// </summary>
    public event EventHandler<CopyTextEventArgs> CopyText;


    /// <summary>
    /// Initializes a new instance of the <see cref="TextAreaClipboardHandler"/> class.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public TextAreaClipboardHandler(TextArea textArea)
    {
      _textArea = textArea;
      textArea.SelectionManager.SelectionChanged += DocumentSelectionChanged;
    }


    void DocumentSelectionChanged(object sender, EventArgs e)
    {
      _textArea.MotherTextEditorControl.NotifySelectionChanged();
    }


    bool CopyTextToClipboard(string stringToCopy, bool asLine)
    {
      if (stringToCopy.Length > 0)
      {
        DataObject dataObject = new DataObject();
        dataObject.SetData(DataFormats.UnicodeText, true, stringToCopy);
        if (asLine)
        {
          MemoryStream lineSelected = new MemoryStream(1);
          lineSelected.WriteByte(1);
          dataObject.SetData(LineSelectedType, false, lineSelected);
        }
        // Default has no highlighting, therefore we don't need RTF output
        if (_textArea.Document.HighlightingStrategy.Name != "Default")
        {
          dataObject.SetData(DataFormats.Rtf, RtfWriter.GenerateRtf(_textArea));
        }
        OnCopyText(new CopyTextEventArgs(stringToCopy));

        SafeSetClipboard(dataObject);
        return true;
      }
      else
      {
        return false;
      }
    }


    [ThreadStatic]
    private static int _safeSetClipboardDataVersion;


    static void SafeSetClipboard(object dataObject)
    {
      // Work around ExternalException bug. (SD2-426)
      // Best reproducible inside Virtual PC.
      int version = unchecked(++_safeSetClipboardDataVersion);
      try
      {
        Clipboard.SetDataObject(dataObject, true);
      }
      catch (ExternalException)
      {
        Timer timer = new Timer();
        timer.Interval = 100;
        timer.Tick += delegate
        {
          timer.Stop();
          timer.Dispose();
          if (_safeSetClipboardDataVersion == version)
          {
            try
            {
              Clipboard.SetDataObject(dataObject, true, 10, 50);
            }
            catch (ExternalException) { }
          }
        };
        timer.Start();
      }
    }


    bool CopyTextToClipboard(string stringToCopy)
    {
      return CopyTextToClipboard(stringToCopy, false);
    }


    /// <summary>
    /// Cuts the selected text and puts it in the clipboard.
    /// </summary>
    public void Cut()
    {
      if (_textArea.SelectionManager.HasSomethingSelected)
      {
        if (CopyTextToClipboard(_textArea.SelectionManager.SelectedText))
        {
          if (_textArea.SelectionManager.SelectionIsReadonly)
            return;
          // Remove text
          _textArea.BeginUpdate();
          _textArea.Caret.Position = _textArea.SelectionManager.Selections[0].StartPosition;
          _textArea.SelectionManager.RemoveSelectedText();
          _textArea.EndUpdate();
        }
      }
      else if (_textArea.Document.TextEditorProperties.CutCopyWholeLine)
      {
        // No text was selected, select and cut the entire line
        int curLineNr = _textArea.Document.GetLineNumberForOffset(_textArea.Caret.Offset);
        LineSegment lineWhereCaretIs = _textArea.Document.GetLineSegment(curLineNr);
        string caretLineText = _textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
        _textArea.SelectionManager.SetSelection(_textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset), _textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset + lineWhereCaretIs.TotalLength));
        if (CopyTextToClipboard(caretLineText, true))
        {
          if (_textArea.SelectionManager.SelectionIsReadonly)
            return;
          // remove line
          _textArea.BeginUpdate();
          _textArea.Caret.Position = _textArea.Document.OffsetToPosition(lineWhereCaretIs.Offset);
          _textArea.SelectionManager.RemoveSelectedText();
          _textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr)));
          _textArea.EndUpdate();
        }
      }
    }


    /// <summary>
    /// Copies the selected text into the clipboard.
    /// </summary>
    public void Copy()
    {
      if (!CopyTextToClipboard(_textArea.SelectionManager.SelectedText) && _textArea.Document.TextEditorProperties.CutCopyWholeLine)
      {
        // No text was selected, select the entire line, copy it, and then deselect
        int curLineNr = _textArea.Document.GetLineNumberForOffset(_textArea.Caret.Offset);
        LineSegment lineWhereCaretIs = _textArea.Document.GetLineSegment(curLineNr);
        string caretLineText = _textArea.Document.GetText(lineWhereCaretIs.Offset, lineWhereCaretIs.TotalLength);
        CopyTextToClipboard(caretLineText, true);
      }
    }


    /// <summary>
    /// Pastes the content of the clipboard into the document.
    /// </summary>
    public void Paste()
    {
      if (!_textArea.EnableCutOrPaste)
        return;

      // Clipboard.GetDataObject may throw an exception...
      for (int i = 0; ; i++)
      {
        try
        {
          IDataObject data = Clipboard.GetDataObject();
          if (data == null)
            return;
          bool fullLine = data.GetDataPresent(LineSelectedType);
          if (data.GetDataPresent(DataFormats.UnicodeText))
          {
            string text = (string) data.GetData(DataFormats.UnicodeText);
            if (text.Length > 0)
            {
              _textArea.Document.UndoStack.StartUndoGroup();
              try
              {
                if (_textArea.SelectionManager.HasSomethingSelected)
                {
                  _textArea.Caret.Position = _textArea.SelectionManager.Selections[0].StartPosition;
                  _textArea.SelectionManager.RemoveSelectedText();
                }
                if (fullLine)
                {
                  int col = _textArea.Caret.Column;
                  _textArea.Caret.Column = 0;
                  if (!_textArea.IsReadOnly(_textArea.Caret.Offset))
                    _textArea.InsertString(text);
                  _textArea.Caret.Column = col;
                }
                else
                {
                  // _textArea.EnableCutOrPaste already checked readonly for this case
                  _textArea.InsertString(text);
                }
              }
              finally
              {
                _textArea.Document.UndoStack.EndUndoGroup();
              }
            }
          }
          return;
        }
        catch (ExternalException)
        {
          // GetDataObject does not provide RetryTimes parameter
          if (i > 5) throw;
        }
      }
    }


    /// <summary>
    /// Deletes the currently selected text.
    /// </summary>
    public void Delete()
    {
      new Actions.Delete().Execute(_textArea);
    }


    /// <summary>
    /// Selects the entire document.
    /// </summary>
    public void SelectAll()
    {
      new Actions.SelectWholeDocument().Execute(_textArea);
    }


    /// <summary>
    /// Raises the <see cref="CopyText"/> event.
    /// </summary>
    /// <param name="e">The <see cref="CopyTextEventArgs"/> instance containing the event data.</param>
    protected virtual void OnCopyText(CopyTextEventArgs e)
    {
      if (CopyText != null)
        CopyText(this, e);
    }
  }


  /// <summary>
  /// Event arguments for <see cref="TextAreaClipboardHandler.CopyText"/> event.
  /// </summary>
  internal class CopyTextEventArgs : EventArgs
  {
    private readonly string _text;

    /// <summary>
    /// Gets the text.
    /// </summary>
    /// <value>The text.</value>
    public string Text
    {
      get { return _text; }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CopyTextEventArgs"/> class.
    /// </summary>
    /// <param name="text">The text.</param>
    public CopyTextEventArgs(string text)
    {
      _text = text;
    }
  }
}
