using System;
using System.Diagnostics;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Types of words in a line.
  /// </summary>
  public enum TextWordType
  {
    /// <summary>A word.</summary>
    Word,
    /// <summary>A space.</summary>
    Space,
    /// <summary>A tab.</summary>
    Tab
  }


  /// <summary>
  /// This class represents single words with color information, two special versions of a word are
  /// spaces and tabs.
  /// </summary>
  public class TextWord
  {
    /// <summary>
    /// A space (special type of <see cref="TextWord"/>).
    /// </summary>
    public sealed class SpaceTextWord : TextWord
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.SpaceTextWord"/> class.
      /// </summary>
      public SpaceTextWord()
      {
        _length = 1;
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.SpaceTextWord"/> class.
      /// </summary>
      /// <param name="color">The color.</param>
      public SpaceTextWord(HighlightColor color)
      {
        _length = 1;
        SyntaxColor = color;
      }


      /// <summary>
      /// Gets the font.
      /// </summary>
      /// <param name="fontContainer">The font container.</param>
      /// <returns>Always <see langword="null"/>.</returns>
      public override Font GetFont(FontContainer fontContainer)
      {
        return null;
      }


      /// <summary>
      /// Gets the type.
      /// </summary>
      /// <value>The type (<see cref="TextWordType.Space"/>).</value>
      public override TextWordType Type
      {
        get { return TextWordType.Space; }
      }


      /// <summary>
      /// Gets a value indicating whether this instance is a whitespace.
      /// </summary>
      /// <value>
      /// 	<see langword="true"/>.
      /// </value>
      public override bool IsWhiteSpace
      {
        get { return true; }
      }
    }


    /// <summary>
    /// A tab (special type of <see cref="TextWord"/>).
    /// </summary>
    public sealed class TabTextWord : TextWord
    {
      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.TabTextWord"/> class.
      /// </summary>
      public TabTextWord()
      {
        _length = 1;
      }


      /// <summary>
      /// Initializes a new instance of the <see cref="TextWord.TabTextWord"/> class.
      /// </summary>
      /// <param name="color">The color.</param>
      public TabTextWord(HighlightColor color)
      {
        _length = 1;
        SyntaxColor = color;
      }

      /// <summary>
      /// Gets the font.
      /// </summary>
      /// <param name="fontContainer">The font container.</param>
      /// <returns>Always <see langword="null"/>.</returns>
      public override Font GetFont(FontContainer fontContainer)
      {
        return null;
      }


      /// <summary>
      /// Gets the type.
      /// </summary>
      /// <value>The type (<see cref="TextWordType.Tab"/>).</value>
      public override TextWordType Type
      {
        get { return TextWordType.Tab; }
      }


      /// <summary>
      /// Gets a value indicating whether this instance is a whitespace.
      /// </summary>
      /// <value>
      /// 	<see langword="true"/>.
      /// </value>
      public override bool IsWhiteSpace
      {
        get { return true; }
      }
    }


    private static readonly TextWord spaceWord = new SpaceTextWord();
    private static readonly TextWord tabWord = new TabTextWord();

    private HighlightColor _color;
    private readonly LineSegment _line;
    private readonly IDocument _document;
    private readonly int _offset;
    private int _length;
    private readonly bool _hasDefaultColor;


    /// <summary>
    /// Gets a space (special type of <see cref="TextWord"/>).
    /// </summary>
    /// <value>The space.</value>
    public static TextWord Space
    {
      get { return spaceWord; }
    }


    /// <summary>
    /// Gets a tab (special type of <see cref="TextWord"/>).
    /// </summary>
    /// <value>The tab.</value>
    public static TextWord Tab
    {
      get { return tabWord; }
    }


    /// <summary>
    /// Gets the offset of the word in the current line.
    /// </summary>
    /// <value>The offset of the word in the current line.</value>
    public int Offset
    {
      get { return _offset; }
    }


    /// <summary>
    /// Gets the length of the word.
    /// </summary>
    /// <value>The length of the word.</value>
    public int Length
    {
      get { return _length; }
    }


    /// <summary>
    /// Splits the word into two parts. 
    /// </summary>
    /// <param name="word">The word.</param>
    /// <param name="pos">The position, which lies in the range <c>[1, Length - 1]</c>.</param>
    /// <returns>The part after <paramref name="pos"/> is returned as a new <see cref="TextWord"/>.</returns>
    /// <remarks>
    /// The part before <paramref name="pos"/> is assigned to
    /// the reference parameter <paramref name="word"/>, the part after <paramref name="pos"/> is returned.
    /// </remarks>
    public static TextWord Split(ref TextWord word, int pos)
    {
#if DEBUG
      if (word.Type != TextWordType.Word)
        throw new ArgumentException("word.Type must be Word");

      if (pos <= 0)
        throw new ArgumentOutOfRangeException("pos", pos, "pos must be > 0");

      if (pos >= word.Length)
        throw new ArgumentOutOfRangeException("pos", pos, "pos must be < word.Length");
#endif
      TextWord after = new TextWord(word._document, word._line, word._offset + pos, word._length - pos, word._color, word._hasDefaultColor);
      word = new TextWord(word._document, word._line, word._offset, pos, word._color, word._hasDefaultColor);
      return after;
    }


    /// <summary>
    /// Gets a value indicating whether this instance has default color.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance has default color; otherwise, <see langword="false"/>.
    /// </value>
    public bool HasDefaultColor
    {
      get { return _hasDefaultColor; }
    }


    /// <summary>
    /// Gets the type.
    /// </summary>
    /// <value>The type.</value>
    public virtual TextWordType Type
    {
      get { return TextWordType.Word; }
    }


    /// <summary>
    /// Gets the word.
    /// </summary>
    /// <value>The word.</value>
    public string Word
    {
      get
      {
        if (_document == null)
          return String.Empty;

        return _document.GetText(_line.Offset + _offset, _length);
      }
    }


    /// <summary>
    /// Gets the font.
    /// </summary>
    /// <param name="fontContainer">The font container.</param>
    /// <returns>The font.</returns>
    public virtual Font GetFont(FontContainer fontContainer)
    {
      return _color.GetFont(fontContainer);
    }


    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
      get
      {
        if (_color == null)
          return Color.Black;
        else
          return _color.Color;
      }
    }

    /// <summary>
    /// Gets a value indicating whether this <see cref="TextWord"/> is bold.
    /// </summary>
    /// <value><see langword="true"/> if bold; otherwise, <see langword="false"/>.</value>
    public bool Bold
    {
      get
      {
        if (_color == null)
          return false;
        else
          return _color.Bold;
      }
    }


    /// <summary>
    /// Gets a value indicating whether this <see cref="TextWord"/> is italic.
    /// </summary>
    /// <value><see langword="true"/> if italic; otherwise, <see langword="false"/>.</value>
    public bool Italic
    {
      get
      {
        if (_color == null)
          return false;
        else
          return _color.Italic;
      }
    }


    /// <summary>
    /// Gets or sets the <see cref="HighlightColor"/>.
    /// </summary>
    /// <value>The color for the syntax highlighting.</value>
    public HighlightColor SyntaxColor
    {
      get { return _color; }
      set
      {
        Debug.Assert(value != null);
        _color = value;
      }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is a whitespace.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is whitespace; otherwise, <see langword="false"/>.
    /// </value>
    public virtual bool IsWhiteSpace
    {
      get { return false; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextWord"/> class.
    /// </summary>
    protected TextWord()
    {
      // Needed by the nested classes SpaceTextWord and TabTextWord.
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextWord"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="line">The line.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="color">The color.</param>
    /// <param name="hasDefaultColor">if set to <see langword="true"/> [has default color].</param>
    public TextWord(IDocument document, LineSegment line, int offset, int length, HighlightColor color, bool hasDefaultColor)
    {
      Debug.Assert(document != null);
      Debug.Assert(line != null);
      Debug.Assert(color != null);

      _document = document;
      _line = line;
      _offset = offset;
      _length = length;
      _color = color;
      _hasDefaultColor = hasDefaultColor;
    }


    /// <summary>
    /// Converts a <see cref="TextWord"/> instance to string (for debug purposes)
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return "[TextWord: Word = " + Word + ", Color = " + Color + "]";
    }
  }
}
