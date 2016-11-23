using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Folding
{
  /// <summary>
  /// A simple folding strategy for HLSL files.
  /// </summary>
  public class HlslFoldingStrategy : IFoldingStrategy
  {
    private static readonly Dictionary<string, object> _blocks;


    /// <summary>
    /// Initializes a new instance of the <see cref="HlslFoldingStrategy"/> class.
    /// </summary>
    static HlslFoldingStrategy()
    {
      _blocks = new Dictionary<string, object>();
      _blocks["asm"] = null;
      _blocks["BlendState"] = null;
      _blocks["cbuffer"] = null;
      _blocks["DepthStencilState"] = null;
      _blocks["interface"] = null;
      _blocks["pass"] = null;
      _blocks["RasterizerState"] = null;
      _blocks["struct"] = null;
      _blocks["sampler_state"] = null;
      _blocks["stateblock_state"] = null;
      _blocks["SamplerState"] = null;
      _blocks["SamplerComparisonState"] = null;
      _blocks["tbuffer"] = null;
      _blocks["technique"] = null;
      _blocks["technique10"] = null;
    }


    /// <summary>
    /// Generates the fold markers.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="fileName">Name of the file.</param>
    /// <param name="parseInformation">The parse information.</param>
    /// <returns>A list containing all foldings.</returns>
    public List<Fold> GenerateFolds(IDocument document, string fileName, object parseInformation)
    {
      List<Fold> foldMarkers = new List<Fold>();
      MarkBlocks(document, foldMarkers);
      return foldMarkers;
    }


    /// <summary>
    /// Marks all code blocks (namespaces, classes, methods, etc.) in the document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="foldMarkers">The fold markers.</param>
    private static void MarkBlocks(IDocument document, List<Fold> foldMarkers)
    {
      int offset = 0;
      while (offset < document.TextLength)
      {
        switch (document.GetCharAt(offset))
        {
          case '/':
            offset = SkipComment(document, offset);
            break;
          case '{':
            offset = MarkMethod(document, offset, foldMarkers);
            break;
          default:
            offset = MarkBlock(document, offset, foldMarkers);
            break;
        }
      }
    }


    /// <summary>
    /// Skips any comments that start at the current offset.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <returns>The index of the next character after the comments.</returns>
    private static int SkipComment(IDocument document, int offset)
    {
      if (offset >= document.TextLength - 1)
        return offset + 1;  // End of document, skip to end of document

      char current = document.GetCharAt(offset);
      char next = document.GetCharAt(offset + 1);

      if (current == '/' && next == '/')
      {
        // Skip line comment "//"
        LineSegment line = document.GetLineSegmentForOffset(offset);
        int offsetOfNextLine = line.Offset + line.TotalLength;
        return offsetOfNextLine;
      }
      else if (current == '/' && next == '*')
      {
        // Skip block comment "/* ... */"
        offset += 2;
        while (offset + 1 < document.TextLength)
        {
          if (document.GetCharAt(offset) == '*' && document.GetCharAt(offset + 1) == '/')
          {
            offset = offset + 2;
            break;  // End loop
          }
          offset++;
        }
      }
      else
      {
        offset++;
      }
      return offset;
    }


    /// <summary>
    /// Marks the method whose block starts at the given offset.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset of the method body ('{').</param>
    /// <param name="foldMarkers">The fold markers.</param>
    /// <returns>The index of the next character after the method.</returns>
    private static int MarkMethod(IDocument document, int offset, List<Fold> foldMarkers)
    {
      if (offset >= document.TextLength)
        return offset;

      int startOffset = offset;
      while (startOffset - 1 > 0 && Char.IsWhiteSpace(document.GetCharAt(startOffset - 1)))
        startOffset--;

      int offsetOfClosingBracket = TextHelper.FindClosingBracket(document, offset + 1, '{', '}');
      if (offsetOfClosingBracket > 0)
      {
        // Check whether next character is ';'
        int offsetOfNextCharacter = TextHelper.FindFirstNonWhitespace(document, offsetOfClosingBracket + 1);
        if (offsetOfNextCharacter < document.TextLength && document.GetCharAt(offsetOfNextCharacter) == ';')
          return offset + 1;

        int length = offsetOfClosingBracket - startOffset + 1;
        foldMarkers.Add(new Fold(document, startOffset, length, "{...}", false));

        // Skip to offset after '}'. (Ignore nested blocks.)
        offset = offsetOfClosingBracket + 1;
      }
      else
      {
        offset++;
      }
      return offset;
    }


    /// <summary>
    /// Marks the block that starts at the current offset.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset of the identifier.</param>
    /// <param name="foldMarkers">The fold markers.</param>
    /// <returns>The index of the next character after the block.</returns>
    private static int MarkBlock(IDocument document, int offset, List<Fold> foldMarkers)
    {
      if (offset >= document.TextLength)
        return offset;

      string word = TextHelper.GetIdentifierAt(document, offset);
      if (_blocks.ContainsKey(word))
      {
        offset += word.Length;
        while (offset < document.TextLength)
        {
          char c = document.GetCharAt(offset);
          if (c == '}' || c == ';')
          {
            offset++;
            break;
          }
          if (c == '{')
          {
            int startOffset = offset;
            while (Char.IsWhiteSpace(document.GetCharAt(startOffset - 1)))
              startOffset--;

            int offsetOfClosingBracket = TextHelper.FindClosingBracket(document, offset + 1, '{', '}');
            if (offsetOfClosingBracket > 0)
            {
              int length = offsetOfClosingBracket - startOffset + 1;
              foldMarkers.Add(new Fold(document, startOffset, length, "{...}", false));

              // Skip to offset after '{'.
              offset++;
              break;
            }
          }
          offset++;
        }
      }
      else
      {
        if (word.Length > 0)
        {
          // Skip to next word
          offset += word.Length;
        }
        else
        {
          // Skip to next character
          offset++;
        }
      }
      return offset;
    }
  }
}
