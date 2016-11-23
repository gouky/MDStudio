using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using DigitalRune.Windows.TextEditor.Actions;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Formatting
{
  /// <summary>
  /// This class currently inserts the closing tags to typed opening tags
  /// and does smart indentation for xml files.
  /// </summary>
  public class XmlFormattingStrategy : DefaultFormattingStrategy
  {
    /// <summary>
    /// Formats the line.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line number.</param>
    /// <param name="caretOffset">The caret offset.</param>
    /// <param name="charTyped">The character typed.</param>
    public override void FormatLine(TextArea textArea, int line, int caretOffset, char charTyped)
    {
      textArea.Document.UndoStack.StartUndoGroup();
      try
      {
        if (charTyped == '>')
        {
          StringBuilder stringBuilder = new StringBuilder();
          int offset = Math.Min(caretOffset - 2, textArea.Document.TextLength - 1);
          while (true)
          {
            if (offset < 0)
            {
              break;
            }
            char ch = textArea.Document.GetCharAt(offset);
            if (ch == '<')
            {
              string reversedTag = stringBuilder.ToString().Trim();
              if (!reversedTag.StartsWith("/") && !reversedTag.EndsWith("/"))
              {
                bool validXml = true;
                try
                {
                  XmlDocument doc = new XmlDocument();
                  doc.LoadXml(textArea.Document.TextContent);
                }
                catch (Exception)
                {
                  validXml = false;
                }
                // only insert the tag, if something is missing
                if (!validXml)
                {
                  StringBuilder tag = new StringBuilder();
                  for (int i = reversedTag.Length - 1; i >= 0 && !Char.IsWhiteSpace(reversedTag[i]); --i)
                  {
                    tag.Append(reversedTag[i]);
                  }
                  string tagString = tag.ToString();
                  if (tagString.Length > 0 && !tagString.StartsWith("!") && !tagString.StartsWith("?"))
                  {
                    textArea.Document.Insert(caretOffset, "</" + tagString + ">");
                  }
                }
              }
              break;
            }
            stringBuilder.Append(ch);
            --offset;
          }
        }
      }
      catch (Exception e)
      { 
        // Insanity check
        Debug.Assert(false, e.ToString());
      }
      if (charTyped == '\n')
      {
        textArea.Caret.Column = IndentLine(textArea, line);
      }
      textArea.Document.UndoStack.EndUndoGroup();
    }


    /// <summary>
    /// 'Smart' line indentation for XML.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <param name="line">The line.</param>
    /// <returns>
    /// The number of whitespaces which are before a non-whitespace character in the line.
    /// </returns>
    protected override int SmartIndentLine(TextArea textArea, int line)
    {
      if (line <= 0) 
        return AutoIndentLine(textArea, line);

      try
      {
        TryIndent(textArea, line, line);
        return GetIndentation(textArea, line).Length;
      }
      catch (XmlException)
      {
        return AutoIndentLine(textArea, line);
      }
    }


    /// <summary>
    /// This function sets the indent level in a range of lines.
    /// </summary>
    public override void IndentLines(TextArea textArea, int begin, int end)
    {
      textArea.Document.UndoStack.StartUndoGroup();
      try
      {
        TryIndent(textArea, begin, end);
      }
      catch (XmlException)
      {
        // TODO: Log exception
      }
      finally
      {
        textArea.Document.UndoStack.EndUndoGroup();
      }
    }


    #region ----- Smart Indentation -----
    private static void TryIndent(TextArea textArea, int begin, int end)
    {
      string currentIndentation = "";
      Stack tagStack = new Stack();
      IDocument document = textArea.Document;
      string tab = Tab.GetIndentationString(document);
      int nextLine = begin; // in text editor coordinates
      bool wasEmptyElement = false;
      XmlNodeType lastType = XmlNodeType.XmlDeclaration;

      // TextReader line number begin with 1, text editor line numbers with 0
      using (StringReader stringReader = new StringReader(document.TextContent))
      {
        XmlTextReader reader = new XmlTextReader(stringReader);
        reader.XmlResolver = null; // prevent XmlTextReader from loading external DTDs
        while (reader.Read())
        {
          if (wasEmptyElement)
          {
            if (tagStack.Count == 0)
              currentIndentation = "";
            else
              currentIndentation = (string) tagStack.Pop();
          }

          if (reader.NodeType == XmlNodeType.EndElement)
          {
            // Indent lines before closing tag.
            while (nextLine + 1 < reader.LineNumber)
            {
              // Set indentation of 'nextLine'
              LineSegment line = document.GetLineSegment(nextLine);
              string lineText = document.GetText(line);

              string newText = currentIndentation + lineText.Trim();

              if (newText != lineText)
                document.Replace(line.Offset, line.Length, newText);

              nextLine += 1;
            }

            if (tagStack.Count == 0)
              currentIndentation = "";
            else
              currentIndentation = (string)tagStack.Pop();
          }

          while (reader.LineNumber > nextLine)
          { 
            // Caution: here we compare 1-based and 0-based line numbers
            if (nextLine > end) 
              break;

            if (lastType == XmlNodeType.CDATA || lastType == XmlNodeType.Comment)
            {
              nextLine += 1;
              continue;
            }

            // Set indentation of 'nextLine'
            LineSegment line = document.GetLineSegment(nextLine);
            string lineText = document.GetText(line);

            string newText;
            // Special case: Opening tag has closing bracket on extra line: remove one indentation level.
            if (lineText.Trim() == ">")
              newText = (string) tagStack.Peek() + lineText.Trim();
            else
              newText = currentIndentation + lineText.Trim();

            if (newText != lineText)
              document.Replace(line.Offset, line.Length, newText);

            nextLine += 1;
          }

          if (reader.LineNumber > end)
            break;

          wasEmptyElement = reader.NodeType == XmlNodeType.Element && reader.IsEmptyElement;
          string attribIndent = null;

          if (reader.NodeType == XmlNodeType.Element)
          {
            tagStack.Push(currentIndentation);
            if (reader.LineNumber < begin)
              currentIndentation = GetIndentation(textArea, reader.LineNumber - 1);
            if (reader.Name.Length < 16)
              attribIndent = currentIndentation + new String(' ', 2 + reader.Name.Length);
            else
              attribIndent = currentIndentation + tab;
            currentIndentation += tab;
          }

          lastType = reader.NodeType;
          if (reader.NodeType == XmlNodeType.Element && reader.HasAttributes)
          {
            int startLine = reader.LineNumber;
            reader.MoveToAttribute(0); // move to first attribute
            if (reader.LineNumber != startLine)
              attribIndent = currentIndentation; // change to tab-indentation
            reader.MoveToAttribute(reader.AttributeCount - 1);
            while (reader.LineNumber > nextLine)
            { 
              // caution: here we compare 1-based and 0-based line numbers
              if (nextLine > end) 
                break;

              // set indentation of 'nextLine'
              LineSegment line = document.GetLineSegment(nextLine);
              string lineText = document.GetText(line);
              string newText = attribIndent + lineText.Trim();
              if (newText != lineText)
                document.Replace(line.Offset, line.Length, newText);

              nextLine += 1;
            }
          }
        }
        reader.Close();
      }
    }
    #endregion
  }
}
