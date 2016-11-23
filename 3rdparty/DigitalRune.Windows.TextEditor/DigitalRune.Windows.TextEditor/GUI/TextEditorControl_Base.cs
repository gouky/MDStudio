using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Formatting;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// The text editor control.
  /// </summary>
  [ToolboxBitmap("DigitalRune.Windows.TextEditor.Resources.TextEditorControl.bmp")]
  [ToolboxItem(true)]
  public partial class TextEditorControl
  {
    private IDocument _document;
    private Encoding _encoding;
    private string _currentFileName;


    /// <summary>
    /// Gets or sets the text editor properties.
    /// </summary>
    /// <value>The text editor properties.</value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ITextEditorProperties TextEditorProperties
    {
      get { return Document.TextEditorProperties; }
      set
      {
        Document.TextEditorProperties = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the encoding.
    /// </summary>
    /// <value>Current file's character encoding</value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Encoding Encoding
    {
      get
      {
        if (_encoding == null)
          return TextEditorProperties.Encoding;
        return _encoding;
      }
      set { _encoding = value; }
    }


    /// <summary>
    /// Gets or sets the name of the current file.
    /// </summary>
    /// <value>The current file name</value>
    [Browsable(false)]
    [ReadOnly(true)]
    public string FileName
    {
      get { return _currentFileName; }
      set
      {
        if (_currentFileName != value)
        {
          _currentFileName = value;
          OnFileNameChanged(EventArgs.Empty);
        }
      }
    }


    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    /// <value>The current document</value>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IDocument Document
    {
      get { return _document; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        if (_document == value)
          return; 

        if (_document != null)
        {
          _document.DocumentAboutToBeChanged -= Document_DocumentAboutToBeChanged;
          _document.DocumentChanged -= Document_DocumentChanged;
          _document.TextContentChanged -= Document_TextContentChanged;
        }
        _document = value;
        _document.UndoStack.TextEditorControl = this;
        _document.DocumentAboutToBeChanged += Document_DocumentAboutToBeChanged;
        _document.DocumentChanged += Document_DocumentChanged;
        _document.TextContentChanged += Document_TextContentChanged;
      }
    }


    /// <summary>
    /// Gets or sets the content of the current document.
    /// </summary>
    /// <value>The content of the current document as <see cref="string"/>.</value>
    [EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [Editor("System.ComponentModel.Design.MultilineStringEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor))]
    public override string Text
    {
      get { return Document.TextContent; }
      set { Document.TextContent = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the document is read-only.
    /// </summary>
    /// <value>If set to <see langword="true"/> the contents can't be altered.</value>
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("If set to true the contents can't be altered.")]
    public bool IsReadOnly
    {
      get { return Document.ReadOnly; }
      set { Document.ReadOnly = value; }
    }


    /// <summary>
    /// Gets the default size of the control.
    /// </summary>
    /// <value>
    /// The default size of the control.
    /// </value>
    /// <remarks>
    /// Supposedly this is the way to do it according to .NET docs,
    /// as opposed to setting the size in the constructor
    /// </remarks>
    protected override Size DefaultSize
    {
      get { return new Size(100, 100); }
    }


    /// <summary>
    /// Occurs when the file name is changed.
    /// </summary>
    [Category("Document")]
    [Description("Occurs when the file name is changed.")]
    public event EventHandler FileNameChanged;


    /// <summary>
    /// Occurs when a document is about to be changed.
    /// </summary>
    [Category("Document")]
    [Description("Occurs when a document is about to be changed.")]
    public event EventHandler<DocumentEventArgs> DocumentAboutToBeChanged;


    /// <summary>
    /// Occurs when a document has been changed.
    /// </summary>
    [Category("Document")]
    [Description("Occurs when a document has been changed.")]
    public event EventHandler<DocumentEventArgs> DocumentChanged;


    /// <summary>
    /// Loads the file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public void LoadFile(string fileName)
    {
      LoadFile(fileName, true, true);
    }


    /// <summary>
    /// Loads a file.
    /// </summary>
    /// <param name="fileName">The name of the file to open.</param>
    /// <param name="autoLoadHighlighting">Automatically load the highlighting for the file.</param>
    /// <param name="autodetectEncoding">Automatically detect file encoding and set Encoding property to the detected encoding.</param>
    public void LoadFile(string fileName, bool autoLoadHighlighting, bool autodetectEncoding)
    {
      using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
      {
        LoadFile(fileName, fs, autoLoadHighlighting, autodetectEncoding);
      }
    }

    /// <summary>
    /// Loads a file from the specified stream.
    /// </summary>
    /// <param name="fileName">The name of the file to open. Used to find the correct highlighting strategy
    /// if autoLoadHighlighting is active, and sets the filename property to this value.</param>
    /// <param name="stream">The stream to actually load the file content from.</param>
    /// <param name="autoLoadHighlighting">Automatically load the highlighting for the file</param>
    /// <param name="autodetectEncoding">Automatically detect file encoding and set Encoding property to the detected encoding.</param>
    public void LoadFile(string fileName, Stream stream, bool autoLoadHighlighting, bool autodetectEncoding)
    {
      if (stream == null)
        throw new ArgumentNullException("stream");

      BeginUpdate();
      Document.TextContent = String.Empty;
      Document.UndoStack.ClearAll();
      Document.BookmarkManager.Clear();

      if (autoLoadHighlighting)
      {
        try
        {
          Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategyForFile(fileName);

          // Automatically set available formatting and folding strategies, where available
          switch (Document.HighlightingStrategy.Name)
          {
            case "C#":
              Document.FormattingStrategy = new CSharpFormattingStrategy();
              Document.FoldingManager.FoldingStrategy = new CSharpFoldingStrategy();
              break;
            case "Cg":
            case "HLSL":
              Document.FormattingStrategy = new HlslFormattingStrategy();
              Document.FoldingManager.FoldingStrategy = new HlslFoldingStrategy();
              break;
            case "XML":
              Document.FormattingStrategy = new XmlFormattingStrategy();
              Document.FoldingManager.FoldingStrategy = new XmlFoldingStrategy();
              break;
            default:
              break;
          }
        }
        catch (HighlightingDefinitionInvalidException ex)
        {
          MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      }


      if (autodetectEncoding)
      {
        Encoding encoding = Encoding;
        Document.TextContent = Utilities.FileReader.ReadFileContent(stream, ref encoding);
        Encoding = encoding;
      }
      else
      {
        using (StreamReader reader = new StreamReader(fileName, Encoding))
        {
          Document.TextContent = reader.ReadToEnd();
        }
      }

      FileName = fileName;
      Document.UpdateQueue.Clear();
      EndUpdate();

      OptionsChanged();
      Refresh();
    }


    /// <summary>
    /// Gets if the document can be saved with the current encoding without losing data.
    /// </summary>
    /// <returns>
    /// 	<see langword="true"/> if the document can be saved with the current encoding; otherwise, <see langword="false"/>.
    /// </returns>
    public bool CanSaveWithCurrentEncoding()
    {
      if (_encoding == null || Utilities.FileReader.IsUnicode(_encoding))
        return true;

      // not a unicode codepage
      string text = Document.TextContent;
      return _encoding.GetString(_encoding.GetBytes(text)) == text;
    }


    /// <summary>
    /// Saves the file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    public void SaveFile(string fileName)
    {
      using (FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
      {
        SaveFile(fs);
      }
      FileName = fileName;
    }

    /// <summary>
    /// Saves the text editor content into the specified stream.
    /// (Does not close the stream.)
    /// </summary>
    /// <param name="stream">The stream.</param>
    public void SaveFile(Stream stream)
    {
      StreamWriter streamWriter = new StreamWriter(stream, Encoding ?? Encoding.UTF8);

      // save line per line to apply the LineTerminator to all lines
      // (otherwise we might save files with mixed-up line endings)
      foreach (LineSegment line in Document.LineSegmentCollection)
      {
        streamWriter.Write(Document.GetText(line.Offset, line.Length));
        if (line.DelimiterLength > 0)
        {
          char charAfterLine = Document.GetCharAt(line.Offset + line.Length);
          if (charAfterLine != '\n' && charAfterLine != '\r')
            throw new InvalidOperationException("The document cannot be saved because it is corrupted.");
          // only save line terminator if the line has one
          streamWriter.Write(Document.TextEditorProperties.LineTerminator);
        }
      }
      streamWriter.Flush();
    }


    /// <summary>
    /// Gets the range description.
    /// </summary>
    /// <param name="selectedItem">The selected item.</param>
    /// <param name="itemCount">The item count.</param>
    /// <returns>The range description.</returns>
    public virtual string GetRangeDescription(int selectedItem, int itemCount)
    {
      // used in insight window
      // Localization ISSUES

      StringBuilder sb = new StringBuilder(selectedItem.ToString());
      sb.Append(" from ");
      sb.Append(itemCount.ToString());
      return sb.ToString();
    }


    /// <summary>
    /// Forces the control to invalidate its client area and immediately redraw itself and any child controls.
    /// </summary>
    /// <remarks>
    /// Overwritten refresh method that does nothing if the control is in
    /// an update cycle.
    /// </remarks>
    public override void Refresh()
    {
      if (IsInUpdate)
        return;

      base.Refresh();
    }


    /// <summary>
    /// Raises the <see cref="FileNameChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnFileNameChanged(EventArgs e)
    {
      if (FileNameChanged != null)
        FileNameChanged(this, e);
    }


    private void Document_DocumentAboutToBeChanged(object sender, DocumentEventArgs documentEventArgs)
    {
      OnDocumentAboutToBeChanged(documentEventArgs);
    }


    /// <summary>
    /// Raises the <see cref="DocumentAboutToBeChanged"/> event.
    /// </summary>
    /// <param name="documentEventArgs">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    protected virtual void OnDocumentAboutToBeChanged(DocumentEventArgs documentEventArgs)
    {
      EventHandler<DocumentEventArgs> handler = DocumentAboutToBeChanged;
      if (handler != null)
        handler(this, documentEventArgs);
    }


    private void Document_DocumentChanged(object sender, DocumentEventArgs documentEventArgs)
    {
      OnDocumentChanged(documentEventArgs);
    }


    /// <summary>
    /// Raises the <see cref="DocumentChanged"/> event.
    /// </summary>
    /// <param name="documentEventArgs">The <see cref="DigitalRune.Windows.TextEditor.Document.DocumentEventArgs"/> instance containing the event data.</param>
    protected virtual void OnDocumentChanged(DocumentEventArgs documentEventArgs)
    {
      EventHandler<DocumentEventArgs> handler = DocumentChanged;
      if (handler != null)
        handler(this, documentEventArgs);
    }


    private void Document_TextContentChanged(object sender, EventArgs e)
    {
      OnTextChanged(e);
    }


    /// <summary>
    /// Called when reloading the highlighting.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnReloadHighlighting(object sender, EventArgs e)
    {
      if (Document.HighlightingStrategy != null)
      {
        try
        {
          Document.HighlightingStrategy = HighlightingStrategyFactory.CreateHighlightingStrategy(Document.HighlightingStrategy.Name);
        }
        catch (HighlightingDefinitionInvalidException ex)
        {
          MessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        OptionsChanged();
      }
    }
  }
}
