using System;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Folding
{
  /// <summary>
  /// Describes a section of the text that can be collapsed (folded).
  /// </summary>
  public class Fold : ISegment, IComparable<Fold>
  {
    private int _offset = -1;
    private int _length = -1;
    private bool isFolded;
    private readonly string _foldText = "...";
    private readonly IDocument _document;
    private int _startLine;
    private int _startColumn;
    private int _endLine;
    private int _endColumn;


    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    public int Offset
    {
      get { return _offset; }
      set
      {
        _offset = value; 
        Update();
      }
    }


    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length of the span</value>
    public int Length
    {
      get { return _length; }
      set
      {
        _length = value; 
        Update();
      }
    }


    /// <summary>
    /// Gets the start line.
    /// </summary>
    /// <value>The start line.</value>
    public int StartLine
    {
      get { return _startLine; }
    }


    /// <summary>
    /// Gets the start column.
    /// </summary>
    /// <value>The start column.</value>
    public int StartColumn
    {
      get { return _startColumn; }
    }


    /// <summary>
    /// Gets the end line.
    /// </summary>
    /// <value>The end line.</value>
    public int EndLine
    {
      get { return _endLine; }
    }


    /// <summary>
    /// Gets the end column.
    /// </summary>
    /// <value>The end column.</value>
    public int EndColumn
    {
      get { return _endColumn; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether this fold is collapsed (folded).
    /// </summary>
    /// <value><see langword="true"/> if this fold is collapsed (folded); otherwise, <see langword="false"/>.</value>
    public bool IsFolded
    {
      get { return isFolded; }
      set
      {
        if (isFolded != value)
        {
          _document.FoldingManager.ClearCache();
          isFolded = value;
        }
      }
    }


    /// <summary>
    /// Gets the label of the fold (shown when folded).
    /// </summary>
    /// <value>The label of the fold.</value>
    public string FoldText
    {
      get { return _foldText; }
    }


    /// <summary>
    /// Gets the inner text.
    /// </summary>
    /// <value>The inner text.</value>
    public string InnerText
    {
      get { return _document.GetText(_offset, _length); }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Fold"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="foldText">The label of the folding (shown when folded).</param>
    /// <param name="isFolded">Flag that defines whether the text is folded or not.</param>
    public Fold(IDocument document, int offset, int length, string foldText, bool isFolded)
    {
      _document = document;
      _offset = offset;
      _length = length;
      _foldText = foldText;
      this.isFolded = isFolded;
      Update();
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Fold"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="startColumn">The start column.</param>
    /// <param name="endLine">The end line.</param>
    /// <param name="endColumn">The end column.</param>
    public Fold(IDocument document, int startLine, int startColumn, int endLine, int endColumn)
      : this(document, startLine, startColumn, endLine, endColumn, "...")
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Fold"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="startColumn">The start column.</param>
    /// <param name="endLine">The end line.</param>
    /// <param name="endColumn">The end column.</param>
    /// <param name="foldText">The label of the folding (shown when folded).</param>
    public Fold(IDocument document, int startLine, int startColumn, int endLine, int endColumn, string foldText)
      : this(document, startLine, startColumn, endLine, endColumn, foldText, false)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Fold"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="startColumn">The start column.</param>
    /// <param name="endLine">The end line.</param>
    /// <param name="endColumn">The end column.</param>
    /// <param name="foldText">The label of the folding (shown when folded).</param>
    /// <param name="isFolded">Flag that defines whether the text is folded or not.</param>
    public Fold(IDocument document, int startLine, int startColumn, int endLine, int endColumn, string foldText, bool isFolded)
    {
      _document = document;

      startLine = Math.Min(document.TotalNumberOfLines - 1, Math.Max(startLine, 0));
      ISegment startLineSegment = document.GetLineSegment(startLine);

      endLine = Math.Min(document.TotalNumberOfLines - 1, Math.Max(endLine, 0));
      ISegment endLineSegment = document.GetLineSegment(endLine);

      // Prevent the region from completely disappearing
      if (string.IsNullOrEmpty(foldText))
      {
        foldText = "...";
      }

      _foldText = foldText;
      _offset = startLineSegment.Offset + Math.Min(startColumn, startLineSegment.Length);
      _length = (endLineSegment.Offset + Math.Min(endColumn, endLineSegment.Length)) - Offset;
      this.isFolded = isFolded;
      Update();
    }


    private void Update()
    {
      TextLocation location = GetPointForOffset(_document, _offset);
      _startLine = location.Line;
      _startColumn = location.Column;
      location = GetPointForOffset(_document, _offset + Length);
      _endLine = location.Line;
      _endColumn = location.Column;
    }


    static TextLocation GetPointForOffset(IDocument document, int offset)
    {
      if (offset > document.TextLength)
      {
        return new TextLocation(1, document.TotalNumberOfLines + 1);
      }
      else if (offset < 0)
      {
        return new TextLocation(-1, -1);
      }
      else
      {
        int line = document.GetLineNumberForOffset(offset);
        int column = offset - document.GetLineSegment(line).Offset;
        return new TextLocation(column, line);
      }
    }

    
    /// <summary>
    /// Compares the current object with another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// A 32-bit signed integer that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the other parameter.Zero This object is equal to other. Greater than zero This object is greater than other.
    /// </returns>
    public int CompareTo(Fold other)
    {
      if (_offset != other.Offset)
        return _offset.CompareTo(other.Offset);

      return _length.CompareTo(other.Length);
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[Fold: Offset = {0}, Length = {1}, Text = {2}, IsFolded = {3}]", Offset, Length, _foldText, isFolded);
    }
  }
}
