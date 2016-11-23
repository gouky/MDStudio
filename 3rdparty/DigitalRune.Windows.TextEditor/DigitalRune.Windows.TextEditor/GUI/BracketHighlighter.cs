using System;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// A pair of matching braces.
  /// </summary>
  internal class Highlight
  {
    private TextLocation _openingBrace;
    private TextLocation _closingBrace;


    /// <summary>
    /// Gets or sets the opening brace.
    /// </summary>
    /// <value>The opening brace.</value>
    public TextLocation OpeningBrace
    {
      get { return _openingBrace; }
      set { _openingBrace = value; }
    }


    /// <summary>
    /// Gets or sets the closing brace.
    /// </summary>
    /// <value>The closing brace.</value>
    public TextLocation ClosingBrace
    {
      get { return _closingBrace; }
      set { _closingBrace = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Highlight"/> class.
    /// </summary>
    /// <param name="openBrace">The opening brace.</param>
    /// <param name="closeBrace">The closing brace.</param>
    public Highlight(TextLocation openBrace, TextLocation closeBrace)
    {
      _openingBrace = openBrace;
      _closingBrace = closeBrace;
    }
  }


  /// <summary>
  /// Describes a bracket highlighting scheme.
  /// </summary>
  internal class BracketHighlightingScheme
  {
    private char _openTag;
    private char _closingTag;


    /// <summary>
    /// Gets or sets the opening tag.
    /// </summary>
    /// <value>The opening tag.</value>
    public char OpeningTag
    {
      get { return _openTag; }
      set { _openTag = value; }
    }


    /// <summary>
    /// Gets or sets the closing tag.
    /// </summary>
    /// <value>The closing tag.</value>
    public char ClosingTag
    {
      get { return _closingTag; }
      set { _closingTag = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="BracketHighlightingScheme"/> class.
    /// </summary>
    /// <param name="opentag">The opening tag.</param>
    /// <param name="closingtag">The closing tag.</param>
    public BracketHighlightingScheme(char opentag, char closingtag)
    {
      _openTag = opentag;
      _closingTag = closingtag;
    }


    /// <summary>
    /// Gets the highlight.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The matching bracers.</returns>
    public Highlight GetHighlight(IDocument document, int offset)
    {
      int searchOffset;
      if (document.TextEditorProperties.BracketMatchingStyle == BracketMatchingStyle.After)
        searchOffset = offset;
      else
        searchOffset = offset + 1;

      char word = document.GetCharAt(Math.Max(0, Math.Min(document.TextLength - 1, searchOffset)));

      TextLocation endP = document.OffsetToPosition(searchOffset);
      if (word == _openTag)
      {
        if (searchOffset < document.TextLength)
        {
          int bracketOffset = TextHelper.FindClosingBracket(document, searchOffset + 1, _openTag, _closingTag);
          if (bracketOffset >= 0)
          {
            TextLocation p = document.OffsetToPosition(bracketOffset);
            return new Highlight(p, endP);
          }
        }
      }
      else if (word == _closingTag)
      {
        if (searchOffset > 0)
        {
          int bracketOffset = TextHelper.FindOpeningBracket(document, searchOffset - 1, _openTag, _closingTag);
          if (bracketOffset >= 0)
          {
            TextLocation p = document.OffsetToPosition(bracketOffset);
            return new Highlight(p, endP);
          }
        }
      }
      return null;
    }
  }
}
