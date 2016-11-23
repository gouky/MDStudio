using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Formatting
{
  /// <summary>
  /// Interface used for the indentation class to access the document.
  /// </summary>
  interface IDocumentAccessor
  {
    /// <summary>Gets if something was changed in the document.</summary>
    bool Dirty { get; }

    /// <summary>
    /// Gets if the current line is read only (because it is not in the
    /// selected text region)
    /// </summary>
    bool ReadOnly { get; }

    /// <summary>Gets the number of the current line.</summary>
    int LineNumber { get; }

    /// <summary>Gets or sets the text of the current line.</summary>
    string Text { get; set; }

    /// <summary>Advances to the next line.</summary>
    bool Next();
  }


  sealed class DocumentAccessor : IDocumentAccessor
  {
    private readonly IDocument _document;
    private readonly int _minLine;
    private readonly int _maxLine;
    private int _changedLines;
    private int _lineNumber = -1;
    private bool _dirty;
    private bool _lineDirty;
    private string _text;
    private LineSegment _line;


    public bool ReadOnly
    {
      get { return _lineNumber < _minLine; }
    }


    public bool Dirty
    {
      get { return _dirty; }
    }


    public int LineNumber
    {
      get { return _lineNumber; }
    }


    public int ChangedLines
    {
      get { return _changedLines; }
    }


    public string Text
    {
      get { return _text; }
      set
      {
        if (_lineNumber < _minLine) 
          return;
        _text = value;
        _dirty = true;
        _lineDirty = true;
      }
    }


    public DocumentAccessor(IDocument document)
    {
      _document = document;
      _minLine = 0;
      _maxLine = _document.TotalNumberOfLines - 1;
    }


    public DocumentAccessor(IDocument document, int minLine, int maxLine)
    {
      _document = document;
      _minLine = minLine;
      _maxLine = maxLine;
    }


    public bool Next()
    {
      if (_lineDirty)
      {
        _document.Replace(_line.Offset, _line.Length, _text);
        _lineDirty = false;
        ++_changedLines;
      }
      ++_lineNumber;
      if (_lineNumber > _maxLine) 
        return false;
      _line = _document.GetLineSegment(_lineNumber);
      _text = _document.GetText(_line);
      return true;
    }
  }


  sealed class FileAccessor : IDisposable, IDocumentAccessor
  {
    private readonly string _filename;
    private FileStream _fileStream;
    private readonly StreamReader _reader;
    private readonly List<string> _lines = new List<string>();
    private bool _dirty;
    private int _lineNumber;
    private string _text = "";


    public bool Dirty
    {
      get { return _dirty; }
    }


    public bool ReadOnly
    {
      get { return false; }
    }


    public int LineNumber
    {
      get { return _lineNumber; }
    }


    public string Text
    {
      get { return _text; }
      set
      {
        _dirty = true;
        _text = value;
      }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FileAccessor"/> class.
    /// </summary>
    /// <param name="filename">The file name.</param>
    /// <param name="encoding">The default encoding.</param>
    public FileAccessor(string filename, Encoding encoding)
    {
      _filename = filename;
      _fileStream = new FileStream(filename, FileMode.Open, FileAccess.Read);
      _reader = Utilities.FileReader.OpenStream(_fileStream, encoding);
    }


    public bool Next()
    {
      if (_lineNumber > 0)
        _lines.Add(_text);

      _text = _reader.ReadLine();
      ++_lineNumber;
      return _text != null;
    }


    void IDisposable.Dispose()
    {
      Close();
    }


    /// <summary>
    /// Closes the file.
    /// </summary>
    public void Close()
    {
      Encoding encoding = _reader.CurrentEncoding;
      _reader.Close();
      _fileStream.Close();
      if (_dirty)
      {
        _fileStream = new FileStream(_filename, FileMode.Create, FileAccess.Write, FileShare.None);
        using (StreamWriter w = new StreamWriter(_fileStream, encoding))
          foreach (string line in _lines)
            w.WriteLine(line);

        _fileStream.Close();
      }
    }
  }


  sealed class StringAccessor : IDocumentAccessor
  {
    private readonly StringReader _reader;
    private readonly StringWriter _writer;
    private bool _dirty;
    private int _lineNumber;
    private string _text = "";


    public bool Dirty
    {
      get { return _dirty; }
    }


    public bool ReadOnly
    {
      get { return false; }
    }


    public string CodeOutput
    {
      get { return _writer.ToString(); }
    }


    public int LineNumber
    {
      get { return _lineNumber; }
    }


    public string Text
    {
      get { return _text; }
      set 
      { 
        _dirty = true;
        _text = value;
      }
    }


    public StringAccessor(string code)
    {
      _reader = new StringReader(code);
      _writer = new StringWriter();
    }


    public bool Next()
    {
      if (_lineNumber > 0)
        _writer.WriteLine(_text);

      _text = _reader.ReadLine();
      ++_lineNumber;
      return _text != null;
    }
  }
}
