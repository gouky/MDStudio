using System;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor.Formatting
{
  /// <summary>
  /// This class handles the auto and smart indenting in the text buffer while
  /// you type.
  /// </summary>
  public class DefaultFormattingStrategy : IFormattingStrategy
  {
    private static readonly char[] _whitespaceChars = { ' ', '\t' };


    /// <summary>
    /// Returns the whitespaces which are before a non-whitespace character in the line
    /// as a string.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <returns>
    /// The whitespaces which are before a non-whitespace character in the line
    /// as a string.
    /// </returns>
    protected static string GetIndentation(TextArea textArea, int lineNumber)
    {
      if (lineNumber < 0 || lineNumber > textArea.Document.TotalNumberOfLines)
        throw new ArgumentOutOfRangeException("lineNumber");

      string lineText = TextHelper.GetLineAsString(textArea.Document, lineNumber);
      StringBuilder whitespaces = new StringBuilder();

      foreach (char ch in lineText)
      {
        if (Char.IsWhiteSpace(ch))
          whitespaces.Append(ch);
        else
          break;
      }
      return whitespaces.ToString();
    }


    /// <summary>
    /// Could be overwritten to define more complex indentation.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <returns>
    /// The number of whitespaces which are before a non-whitespace character in the line.
    /// </returns>
    protected virtual int AutoIndentLine(TextArea textArea, int line)
    {
      string indentation = line != 0 ? GetIndentation(textArea, line - 1) : "";
      if (indentation.Length > 0)
      {
        string newLineText = indentation + TextHelper.GetLineAsString(textArea.Document, line).Trim();
        LineSegment oldLine = textArea.Document.GetLineSegment(line);
        SmartReplaceLine(textArea.Document, oldLine, newLineText);
      }
      return indentation.Length;
    }


    /// <summary>
    /// Replaces the text in a line.
    /// If only whitespace at the beginning and end of the line was changed, this method
    /// only adjusts the whitespace and doesn't replace the other text.
    /// </summary>
    public static void SmartReplaceLine(IDocument document, LineSegment line, string newLineText)
    {
      if (document == null)
        throw new ArgumentNullException("document");
      if (line == null)
        throw new ArgumentNullException("line");
      if (newLineText == null)
        throw new ArgumentNullException("newLineText");
      string newLineTextTrim = newLineText.Trim(_whitespaceChars);
      string oldLineText = document.GetText(line);
      if (oldLineText == newLineText)
        return;
      int pos = oldLineText.IndexOf(newLineTextTrim);
      if (newLineTextTrim.Length > 0 && pos >= 0)
      {
        document.UndoStack.StartUndoGroup();
        try
        {
          // find whitespace at beginning
          int startWhitespaceLength = 0;
          while (startWhitespaceLength < newLineText.Length)
          {
            char c = newLineText[startWhitespaceLength];
            if (c != ' ' && c != '\t')
              break;
            startWhitespaceLength++;
          }
          // find whitespace at end
          int endWhitespaceLength = newLineText.Length - newLineTextTrim.Length - startWhitespaceLength;

          // replace whitespace sections
          int lineOffset = line.Offset;
          document.Replace(lineOffset + pos + newLineTextTrim.Length, line.Length - pos - newLineTextTrim.Length, newLineText.Substring(newLineText.Length - endWhitespaceLength));
          document.Replace(lineOffset, pos, newLineText.Substring(0, startWhitespaceLength));
        }
        finally
        {
          document.UndoStack.EndUndoGroup();
        }
      }
      else
      {
        document.Replace(line.Offset, line.Length, newLineText);
      }
    }


    /// <summary>
    /// Could be overwritten to define more complex indentation.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <returns>
    /// The number of whitespaces which are before a non-whitespace character in the line.
    /// </returns>
    protected virtual int SmartIndentLine(TextArea textArea, int line)
    {
      return AutoIndentLine(textArea, line); // smart = auto-indent in normal texts
    }


    /// <summary>
    /// This function formats a specific line after a character is typed.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <param name="caretOffset">The caret offset.</param>
    /// <param name="ch">The character typed.</param>
    public virtual void FormatLine(TextArea textArea, int line, int caretOffset, char ch)
    {
      if (ch == '\n')
        textArea.Caret.Column = IndentLine(textArea, line);
    }


    /// <summary>
    /// This function sets the indentation level in a specific line.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <returns>
    /// The number of whitespaces which are before a non-whitespace character in the line.
    /// </returns>
    public int IndentLine(TextArea textArea, int line)
    {
      textArea.Document.UndoStack.StartUndoGroup();
      int result;
      switch (textArea.Document.TextEditorProperties.IndentStyle)
      {
        case IndentStyle.None:
          result = 0;
          break;
        case IndentStyle.Auto:
          result = AutoIndentLine(textArea, line);
          break;
        case IndentStyle.Smart:
          result = SmartIndentLine(textArea, line);
          break;
        default:
          throw new NotSupportedException("Unsupported value for IndentStyle: " + textArea.Document.TextEditorProperties.IndentStyle);
      }
      textArea.Document.UndoStack.EndUndoGroup();
      return result;
    }


    /// <summary>
    /// This function sets the indentation level in a range of lines.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="begin">The begin.</param>
    /// <param name="end">The end.</param>
    public virtual void IndentLines(TextArea textArea, int begin, int end)
    {
      textArea.Document.UndoStack.StartUndoGroup();

      for (int i = begin; i <= end; ++i)
        IndentLine(textArea, i);

      textArea.Document.UndoStack.EndUndoGroup();
    }


    /// <summary>
    /// Finds the offset of the opening bracket in the block defined by offset skipping
    /// brackets, strings and comments.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset of an position in the block (before the closing bracket).</param>
    /// <param name="openBracket">The character for the opening bracket.</param>
    /// <param name="closingBracket">The character for the closing bracket.</param>
    /// <returns>
    /// Returns the offset of the opening bracket or -1 if no matching bracket was found.
    /// </returns>
    public virtual int SearchBracketBackward(IDocument document, int offset, char openBracket, char closingBracket)
    {
      int brackets = -1;

      // first try "quick find" - find the matching bracket if there is no string/comment in the way
      for (int i = offset; i >= 0; --i)
      {
        char ch = document.GetCharAt(i);
        if (ch == openBracket)
        {
          ++brackets;
          if (brackets == 0) return i;
        }
        else if (ch == closingBracket)
        {
          --brackets;
        }
        else if (ch == '"')
        {
          break;
        }
        else if (ch == '\'')
        {
          break;
        }
        else if (ch == '/' && i > 0)
        {
          if (document.GetCharAt(i - 1) == '/') 
            break;
          if (document.GetCharAt(i - 1) == '*') 
            break;
        }
      }
      return -1;
    }


    /// <summary>
    /// Finds the offset of the closing bracket in the block defined by offset skipping
    /// brackets, strings and comments.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset of an position in the block (after the opening bracket).</param>
    /// <param name="openBracket">The character for the opening bracket.</param>
    /// <param name="closingBracket">The character for the closing bracket.</param>
    /// <returns>
    /// Returns the offset of the closing bracket or -1 if no matching bracket was found.
    /// </returns>
    public virtual int SearchBracketForward(IDocument document, int offset, char openBracket, char closingBracket)
    {
      int brackets = 1;
      // try "quick find" - find the matching bracket if there is no string/comment in the way
      for (int i = offset; i < document.TextLength; ++i)
      {
        char ch = document.GetCharAt(i);
        if (ch == openBracket)
        {
          ++brackets;
        }
        else if (ch == closingBracket)
        {
          --brackets;
          if (brackets == 0) 
            return i;
        }
        else if (ch == '"')
        {
          break;
        }
        else if (ch == '\'')
        {
          break;
        }
        else if (ch == '/' && i > 0)
        {
          if (document.GetCharAt(i - 1) == '/') 
            break;
        }
        else if (ch == '*' && i > 0)
        {
          if (document.GetCharAt(i - 1) == '/') 
            break;
        }
      }
      return -1;
    }
  }
}
