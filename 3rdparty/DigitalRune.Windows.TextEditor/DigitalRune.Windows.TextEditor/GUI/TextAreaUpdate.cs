using System;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// This enum describes all implemented request types
  /// </summary>
  public enum TextAreaUpdateType
  {
    /// <summary>
    /// Update the whole text area.
    /// </summary>
    WholeTextArea,
    /// <summary>
    /// Update a single line.
    /// </summary>
    SingleLine,
    /// <summary>
    /// Update single position.
    /// </summary>
    SinglePosition,
    /// <summary>
    /// Update from current position to the end of the line.
    /// </summary>
    PositionToLineEnd,
    /// <summary>
    /// Update from current position to the end.
    /// </summary>
    PositionToEnd,
    /// <summary>
    /// Updates the line between.
    /// </summary>
    LinesBetween
  }


  /// <summary>
  /// This class is used to request an update of the text area.
  /// </summary>
  public class TextAreaUpdate
  {
    private readonly TextLocation _position;
    private readonly TextAreaUpdateType _type;


    /// <summary>
    /// Gets the type of the text area update.
    /// </summary>
    /// <value>The type of the text area update.</value>
    public TextAreaUpdateType TextAreaUpdateType
    {
      get { return _type; }
    }


    /// <summary>
    /// Gets the position.
    /// </summary>
    /// <value>The position.</value>
    public TextLocation Position
    {
      get { return _position; }
    }


    /// <summary>
    /// Creates a new instance of <see cref="TextAreaUpdate"/>
    /// </summary>
    /// <param name="type">The type.</param>
    public TextAreaUpdate(TextAreaUpdateType type)
    {
      _type = type;
    }


    /// <summary>
    /// Creates a new instance of <see cref="TextAreaUpdate"/>
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="position">The position.</param>
    public TextAreaUpdate(TextAreaUpdateType type, TextLocation position)
    {
      _type = type;
      _position = position;
    }


    /// <summary>
    /// Creates a new instance of <see cref="TextAreaUpdate"/>
    /// </summary>
    /// <param name="type">The type.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="endLine">The end line.</param>
    public TextAreaUpdate(TextAreaUpdateType type, int startLine, int endLine)
    {
      _type = type;
      _position = new TextLocation(startLine, endLine);
    }


    /// <summary>
    /// Creates a new instance of <see cref="TextAreaUpdate"/>
    /// </summary>	
    public TextAreaUpdate(TextAreaUpdateType type, int singleLine)
    {
      _type = type;
      _position = new TextLocation(0, singleLine);
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[TextAreaUpdate: Type={0}, Position={1}]", _type, _position);
    }
  }
}
