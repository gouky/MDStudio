using System;
using System.Diagnostics;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Selection
{
  /// <summary>
  /// Default implementation of an <see cref="ISelection"/> interface.
  /// </summary>
  public class DefaultSelection : ISelection
  {
    private readonly IDocument _document;
    private bool _isRectangularSelection;
    private TextLocation _startPosition;
    private TextLocation _endPosition;


    /// <summary>
    /// Gets or sets the start position of the selection.
    /// </summary>
    /// <value>The start position of the selection.</value>
    public TextLocation StartPosition
    {
      get { return _startPosition; }
      set
      {
        DefaultDocument.ValidatePosition(_document, value);
        _startPosition = value;
      }
    }


    /// <summary>
    /// Gets or sets the end position of the selection.
    /// </summary>
    /// <value>The end position of the selection.</value>
    public TextLocation EndPosition
    {
      get { return _endPosition; }
      set
      {
        DefaultDocument.ValidatePosition(_document, value);
        _endPosition = value;
      }
    }


    /// <summary>
    /// Gets the offset of the selection.
    /// </summary>
    /// <value>The offset of the selection.</value>
    public int Offset
    {
      get { return _document.PositionToOffset(_startPosition); }
    }


    /// <summary>
    /// Gets the end offset of the selection.
    /// </summary>
    /// <value>The end offset of the selection.</value>
    public int EndOffset
    {
      get { return _document.PositionToOffset(_endPosition); }
    }


    /// <summary>
    /// Gets the length of the selection.
    /// </summary>
    /// <value>The length of the selection.</value>
    public int Length
    {
      get { return EndOffset - Offset; }
    }


    /// <summary>
    /// Gets a value indicating whether this selection is empty.
    /// </summary>
    /// <value>Returns true, if the selection is empty</value>
    public bool IsEmpty
    {
      get { return _startPosition == _endPosition; }
    }


    /// <summary>
    /// Gets a value indicating whether this is a rectangular selection.
    /// </summary>
    /// <value>Returns true, if the selection is rectangular.</value>
    // TODO : make this unused property used.
    public bool IsRectangularSelection
    {
      get { return _isRectangularSelection; }
      set { _isRectangularSelection = value; }
    }


    /// <summary>
    /// Gets the selected text.
    /// </summary>
    /// <value>The text which is selected by this selection.</value>
    public string SelectedText
    {
      get
      {
        if (_document != null)
        {
          if (Length < 0)
            return null;

          return _document.GetText(Offset, Length);
        }
        return null;
      }
    }


    /// <summary>
    /// Creates a new instance of <see cref="DefaultSelection"/>
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startPosition">The start position.</param>
    /// <param name="endPosition">The end position.</param>
    public DefaultSelection(IDocument document, TextLocation startPosition, TextLocation endPosition)
    {
      DefaultDocument.ValidatePosition(document, startPosition);
      DefaultDocument.ValidatePosition(document, endPosition);
      Debug.Assert(startPosition <= endPosition);
      _document = document;
      _startPosition = startPosition;
      _endPosition = endPosition;
    }


    /// <summary>
    /// Converts a <see cref="DefaultSelection"/> instance to string 
    /// (only for debugging purposes).
    /// </summary>
    public override string ToString()
    {
      return String.Format("[DefaultSelection : StartPosition={0}, EndPosition={1}]", _startPosition, _endPosition);
    }


    /// <summary>
    /// Determines whether this selection contains the specified position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>
    /// 	<see langword="true"/> if this selection contains the specified position; otherwise, <see langword="false"/>.
    /// </returns>
    public bool ContainsPosition(TextLocation position)
    {
      if (IsEmpty)
        return false;
      return _startPosition.Y < position.Y && position.Y < _endPosition.Y ||
        _startPosition.Y == position.Y && _startPosition.X <= position.X && (_startPosition.Y != _endPosition.Y || position.X <= _endPosition.X) ||
        _endPosition.Y == position.Y && _startPosition.Y != _endPosition.Y && position.X <= _endPosition.X;
    }


    /// <summary>
    /// Determines whether this selection contains the specified offset.
    /// </summary>
    /// <param name="offset">The specified offset.</param>
    /// <returns>
    /// <see langword="true"/> if this selection contains the specified offset; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public bool ContainsOffset(int offset)
    {
      return Offset <= offset && offset <= EndOffset;
    }
  }
}
