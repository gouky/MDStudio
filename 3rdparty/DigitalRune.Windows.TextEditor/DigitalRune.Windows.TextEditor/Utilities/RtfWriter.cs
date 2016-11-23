using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Selection;


namespace DigitalRune.Windows.TextEditor.Utilities
{
  /// <summary>
  /// Converts the selected text of a <see cref="TextArea"/> into Rich Text Format.
  /// </summary>
  public class RtfWriter
  {
    private Dictionary<string, int> _colors;
    private int _colorNum;
    private StringBuilder _colorString;


    /// <summary>
    /// Generates the Rich Text from the selection of a <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <returns>The selected text in Rich Text Format.</returns>
    public static string GenerateRtf(TextArea textArea)
    {
      RtfWriter rtfWriter = new RtfWriter();
      return rtfWriter.GenerateRtfInternal(textArea);
    }
    

    /// <summary>
    /// Generates the Rich Text from the selection of a <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    /// <returns>The selected text in Rich Text Format.</returns>
    private string GenerateRtfInternal(TextArea textArea)
    {
      _colors = new Dictionary<string, int>();
      _colorNum = 0;
      _colorString = new StringBuilder();


      StringBuilder rtf = new StringBuilder();

      rtf.Append(@"{\rtf1\ansi\ansicpg1252\deff0\deflang1031");
      BuildFontTable(textArea.Document, rtf);
      rtf.Append('\n');

      string fileContent = BuildFileContent(textArea);
      BuildColorTable(rtf);
      rtf.Append('\n');
      rtf.Append(@"\viewkind4\uc1\pard");
      rtf.Append(fileContent);
      rtf.Append("}");
      return rtf.ToString();
    }


    private void BuildColorTable(StringBuilder rtf)
    {
      rtf.Append(@"{\colortbl ;");
      rtf.Append(_colorString.ToString());
      rtf.Append("}");
    }


    private static void BuildFontTable(IDocument doc, StringBuilder rtf)
    {
      rtf.Append(@"{\fonttbl");
      rtf.Append(@"{\f0\fmodern\fprq1\fcharset0 " + doc.TextEditorProperties.Font.Name + ";}");
      rtf.Append("}");
    }


    private string BuildFileContent(TextArea textArea)
    {
      StringBuilder rtf = new StringBuilder();
      bool firstLine = true;
      Color curColor = Color.Black;
      bool oldItalic = false;
      bool oldBold = false;
      bool escapeSequence = false;

      foreach (ISelection selection in textArea.SelectionManager.Selections)
      {
        int selectionOffset = textArea.Document.PositionToOffset(selection.StartPosition);
        int selectionEndOffset = textArea.Document.PositionToOffset(selection.EndPosition);
        for (int i = selection.StartPosition.Y; i <= selection.EndPosition.Y; ++i)
        {
          LineSegment line = textArea.Document.GetLineSegment(i);
          int offset = line.Offset;
          if (line.Words == null)
          {
            continue;
          }

          foreach (TextWord word in line.Words)
          {
            switch (word.Type)
            {
              case TextWordType.Space:
                if (selection.ContainsOffset(offset))
                {
                  rtf.Append(' ');
                }
                ++offset;
                break;

              case TextWordType.Tab:
                if (selection.ContainsOffset(offset))
                {
                  rtf.Append(@"\tab");
                }
                ++offset;
                escapeSequence = true;
                break;

              case TextWordType.Word:
                Color c = word.Color;

                if (offset + word.Word.Length > selectionOffset && offset < selectionEndOffset)
                {
                  string colorstr = c.R + ", " + c.G + ", " + c.B;

                  if (!_colors.ContainsKey(colorstr))
                  {
                    _colors[colorstr] = ++_colorNum;
                    _colorString.Append(@"\red" + c.R + @"\green" + c.G + @"\blue" + c.B + ";");
                  }
                  if (c != curColor || firstLine)
                  {
                    rtf.Append(@"\cf" + _colors[colorstr]);
                    curColor = c;
                    escapeSequence = true;
                  }

                  if (oldItalic != word.Italic)
                  {
                    if (word.Italic)
                    {
                      rtf.Append(@"\i");
                    }
                    else
                    {
                      rtf.Append(@"\i0");
                    }
                    oldItalic = word.Italic;
                    escapeSequence = true;
                  }

                  if (oldBold != word.Bold)
                  {
                    if (word.Bold)
                    {
                      rtf.Append(@"\b");
                    }
                    else
                    {
                      rtf.Append(@"\b0");
                    }
                    oldBold = word.Bold;
                    escapeSequence = true;
                  }

                  if (firstLine)
                  {
                    rtf.Append(@"\f0\fs" + (Math.Round(textArea.TextEditorProperties.Font.Size * 2)));
                    firstLine = false;
                  }
                  if (escapeSequence)
                  {
                    rtf.Append(' ');
                    escapeSequence = false;
                  }
                  string printWord;
                  if (offset < selectionOffset)
                  {
                    printWord = word.Word.Substring(selectionOffset - offset);
                  }
                  else if (offset + word.Word.Length > selectionEndOffset)
                  {
                    printWord = word.Word.Substring(0, (offset + word.Word.Length) - selectionEndOffset);
                  }
                  else
                  {
                    printWord = word.Word;
                  }

                  rtf.Append(printWord.Replace(@"\", @"\\").Replace("{", "\\{").Replace("}", "\\}"));
                }
                offset += word.Length;
                break;
            }
          }
          if (offset < selectionEndOffset)
          {
            rtf.Append(@"\par");
          }
          rtf.Append('\n');
        }
      }

      return rtf.ToString();
    }
  }
}
