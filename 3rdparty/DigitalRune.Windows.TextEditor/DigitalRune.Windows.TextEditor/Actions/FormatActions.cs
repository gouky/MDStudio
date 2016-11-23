using System;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Selection;


namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Base class for all line formatting actions.
  /// </summary>
  public abstract class AbstractLineFormatAction : AbstractEditAction
  {
    private TextArea _textArea;

    /// <summary>
    /// Gets the <see cref="TextArea"/>.
    /// </summary>
    /// <value>The text area.</value>
    protected TextArea TextArea
    {
      get { return _textArea; }
    }


    /// <summary>
    /// Formats the specified lines.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="endLine">The end line.</param>
    /// <remarks>
    /// This method is called for all selections in the document.
    /// </remarks>
    abstract protected void Convert(IDocument document, int startLine, int endLine);


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.SelectionManager.SelectionIsReadonly)
        return;

      _textArea = textArea;
      textArea.BeginUpdate();
      textArea.Document.UndoStack.StartUndoGroup();
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        foreach (ISelection selection in textArea.SelectionManager.Selections)
          Convert(textArea.Document, selection.StartPosition.Y, selection.EndPosition.Y);
      }
      else
      {
        Convert(textArea.Document, 0, textArea.Document.TotalNumberOfLines - 1);
      }
      textArea.Document.UndoStack.EndUndoGroup();
      textArea.EndUpdate();
      textArea.Refresh();
    }
  }


  /// <summary>
  /// Base class for formatting actions on selections.
  /// </summary>
  public abstract class AbstractSelectionFormatAction : AbstractEditAction
  {
    private TextArea _textArea;

    /// <summary>
    /// Gets the <see cref="TextArea"/>.
    /// </summary>
    /// <value>The text area.</value>
    protected TextArea TextArea
    {
      get { return _textArea; }
    }


    /// <summary>
    /// Formats the specified region of the document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <remarks>
    /// This method is called for all selections in the document.
    /// </remarks>
    abstract protected void Convert(IDocument document, int offset, int length);


    /// <summary>
    /// Executes the action on a given <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.SelectionManager.SelectionIsReadonly)
        return;

      _textArea = textArea;
      textArea.BeginUpdate();
      textArea.Document.UndoStack.StartUndoGroup();
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        foreach (ISelection selection in textArea.SelectionManager.Selections)
          Convert(textArea.Document, selection.Offset, selection.Length);
      }
      else
      {
        Convert(textArea.Document, 0, textArea.Document.TextLength);
      }
      textArea.Document.UndoStack.EndUndoGroup();
      textArea.EndUpdate();
      textArea.Refresh();
    }
  }


  /// <summary>
  /// Removes all leading whitespaces for the current selections.
  /// </summary>
  public class RemoveLeadingWS : AbstractLineFormatAction
  {
    /// <summary>
    /// Formats the specified lines.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="endLine">The end line.</param>
    /// <remarks>
    /// This method is called for all selections in the document.
    /// </remarks>
    protected override void Convert(IDocument document, int startLine, int endLine)
    {
      for (int i = startLine; i < endLine; ++i)
      {
        LineSegment line = document.GetLineSegment(i);

        int removeNumber = 0;
        for (int x = line.Offset; x < line.Offset + line.Length && Char.IsWhiteSpace(document.GetCharAt(x)); ++x)
          ++removeNumber;

        if (removeNumber > 0)
        {
          document.Remove(line.Offset, removeNumber);
        }
      }
    }
  }


  /// <summary>
  /// Removes all the trailing whitespaces for the current selections.
  /// </summary>
  public class RemoveTrailingWS : AbstractLineFormatAction
  {
    /// <summary>
    /// Formats the specified lines.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="endLine">The end line.</param>
    /// <remarks>
    /// This method is called for all selections in the document.
    /// </remarks>
    protected override void Convert(IDocument document, int startLine, int endLine)
    {
      for (int i = endLine - 1; i >= startLine; --i)
      {
        LineSegment line = document.GetLineSegment(i);

        int removeNumber = 0;
        for (int x = line.Offset + line.Length - 1; x >= line.Offset && Char.IsWhiteSpace(document.GetCharAt(x)); --x)
          ++removeNumber;

        if (removeNumber > 0)
        {
          document.Remove(line.Offset + line.Length - removeNumber, removeNumber);
        }
      }
    }
  }


  /// <summary>
  /// Converts the current selection into upper-case characters.
  /// </summary>
  public class ToUpperCase : AbstractSelectionFormatAction
  {
    /// <summary>
    /// Converts the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startOffset">The start offset.</param>
    /// <param name="length">The length.</param>
    protected override void Convert(IDocument document, int startOffset, int length)
    {
      string what = document.GetText(startOffset, length).ToUpper();
      document.Replace(startOffset, length, what);
    }
  }


  /// <summary>
  /// Converts the current selection into lower-case characters.
  /// </summary>
  public class ToLowerCase : AbstractSelectionFormatAction
  {
    /// <summary>
    /// Converts the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startOffset">The start offset.</param>
    /// <param name="length">The length.</param>
    protected override void Convert(IDocument document, int startOffset, int length)
    {
      string what = document.GetText(startOffset, length).ToLower();
      document.Replace(startOffset, length, what);
    }
  }


  /// <summary>
  /// Inverts the case of the characters for the current selection.
  /// </summary>
  public class InvertCaseAction : AbstractSelectionFormatAction
  {
    /// <summary>
    /// Converts the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startOffset">The start offset.</param>
    /// <param name="length">The length.</param>
    protected override void Convert(IDocument document, int startOffset, int length)
    {
      StringBuilder what = new StringBuilder(document.GetText(startOffset, length));

      for (int i = 0; i < what.Length; ++i)
      {
        what[i] = Char.IsUpper(what[i]) ? Char.ToLower(what[i]) : Char.ToUpper(what[i]);
      }

      document.Replace(startOffset, length, what.ToString());
    }
  }


  /// <summary>
  /// Capitalizes all words in the current selection.
  /// </summary>
  public class CapitalizeAction : AbstractSelectionFormatAction
  {
    /// <summary>
    /// Converts the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startOffset">The start offset.</param>
    /// <param name="length">The length.</param>
    protected override void Convert(IDocument document, int startOffset, int length)
    {
      StringBuilder what = new StringBuilder(document.GetText(startOffset, length));

      for (int i = 0; i < what.Length; ++i)
      {
        if (!Char.IsLetter(what[i]) && i < what.Length - 1)
        {
          what[i + 1] = Char.ToUpper(what[i + 1]);
        }
      }
      document.Replace(startOffset, length, what.ToString());
    }

  }


  /// <summary>
  /// Converts all tabs in the current selection into spaces.
  /// </summary>
  public class ConvertTabsToSpaces : AbstractSelectionFormatAction
  {
    /// <summary>
    /// Converts the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startOffset">The start offset.</param>
    /// <param name="length">The length.</param>
    protected override void Convert(IDocument document, int startOffset, int length)
    {
      string what = document.GetText(startOffset, length);
      string spaces = new string(' ', document.TextEditorProperties.TabIndent);
      document.Replace(startOffset, length, what.Replace("\t", spaces));
    }
  }


  /// <summary>
  /// Converts all spaces in the current selection into tabs.
  /// </summary>
  public class ConvertSpacesToTabs : AbstractSelectionFormatAction
  {
    /// <summary>
    /// Converts the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startOffset">The start offset.</param>
    /// <param name="length">The length.</param>
    protected override void Convert(IDocument document, int startOffset, int length)
    {
      string what = document.GetText(startOffset, length);
      string spaces = new string(' ', document.TextEditorProperties.TabIndent);
      document.Replace(startOffset, length, what.Replace(spaces, "\t"));
    }
  }


  /// <summary>
  /// Converts all leadings tabs in the currently selected lines into spaces.
  /// </summary>
  public class ConvertLeadingTabsToSpaces : AbstractLineFormatAction
  {
    /// <summary>
    /// Converts the specified document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="y1">The y1.</param>
    /// <param name="y2">The y2.</param>
    protected override void Convert(IDocument document, int y1, int y2)
    {
      for (int i = y2; i >= y1; --i)
      {
        LineSegment line = document.GetLineSegment(i);

        if (line.Length > 0)
        {
          // count how many whitespace characters there are at the start
          int whiteSpace;
          for (whiteSpace = 0; whiteSpace < line.Length && Char.IsWhiteSpace(document.GetCharAt(line.Offset + whiteSpace)); whiteSpace++)
          {
            // deliberately empty
          }

          if (whiteSpace > 0)
          {
            string newLine = document.GetText(line.Offset, whiteSpace);
            string newPrefix = newLine.Replace("\t", new string(' ', document.TextEditorProperties.TabIndent));
            document.Replace(line.Offset, whiteSpace, newPrefix);
          }
        }
      }
    }
  }


  /// <summary>
  /// Converts all leadings spaces in the currently selected lines into tabs.
  /// </summary>
  public class ConvertLeadingSpacesToTabs : AbstractLineFormatAction
  {
    /// <summary>
    /// Formats the specified lines.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="endLine">The end line.</param>
    /// <remarks>
    /// This method is called for all selections in the document.
    /// </remarks>
    protected override void Convert(IDocument document, int startLine, int endLine)
    {
      for (int i = endLine; i >= startLine; --i)
      {
        LineSegment line = document.GetLineSegment(i);
        if (line.Length > 0)
        {
          // note: some users may prefer a more radical ConvertLeadingSpacesToTabs that
          // means there can be no spaces before the first character even if the spaces
          // didn't add up to a whole number of tabs
          string newLine = TextHelper.LeadingWhitespaceToTabs(document.GetText(line.Offset, line.Length), document.TextEditorProperties.TabIndent);
          document.Replace(line.Offset, line.Length, newLine);
        }
      }
    }
  }


  /// <summary>
  /// Intents the current selection.
  /// </summary>
  public class IndentSelection : AbstractLineFormatAction
  {
    /// <summary>
    /// Formats the specified lines.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="startLine">The start line.</param>
    /// <param name="endLine">The end line.</param>
    /// <remarks>
    /// This method is called for all selections in the document.
    /// </remarks>
    protected override void Convert(IDocument document, int startLine, int endLine)
    {
      document.FormattingStrategy.IndentLines(TextArea, startLine, endLine);
    }
  }
}
