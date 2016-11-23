using System;
using System.Diagnostics;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Selection;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Inserts a tab/indent.
  /// </summary>
  public class Tab : AbstractEditAction
  {
    /// <summary>
    /// Gets the indentation string of the document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <returns>The indentation string.</returns>
    public static string GetIndentationString(IDocument document)
    {
      return GetIndentationString(document, null);
    }


    /// <summary>
    /// Gets the indentation string.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="textArea">The text area.</param>
    /// <returns>The indentation string.</returns>
    public static string GetIndentationString(IDocument document, TextArea textArea)
    {
      StringBuilder indent = new StringBuilder();

      if (document.TextEditorProperties.ConvertTabsToSpaces)
      {
        int tabIndent = document.TextEditorProperties.IndentationSize;
        if (textArea != null)
        {
          int column = textArea.TextView.GetVisualColumn(textArea.Caret.Line, textArea.Caret.Column);
          indent.Append(new String(' ', tabIndent - column % tabIndent));
        }
        else
        {
          indent.Append(new String(' ', tabIndent));
        }
      }
      else
      {
        indent.Append('\t');
      }
      return indent.ToString();
    }


    static void InsertTabs(IDocument document, ISelection selection, int y1, int y2)
    {
      string indentationString = GetIndentationString(document);
      for (int i = y2; i >= y1; --i)
      {
        LineSegment line = document.GetLineSegment(i);
        if (i == y2 && i == selection.EndPosition.Y && selection.EndPosition.X == 0)
        {
          continue;
        }

        // this bit is optional - but useful if you are using block tabbing to sort out
        // a source file with a mixture of tabs and spaces
        //				string newLine = document.GetText(line.Offset,line.Length);
        //				document.Replace(line.Offset,line.Length,newLine);
        //				++redocounter;

        document.Insert(line.Offset, indentationString);
      }
    }


    static void InsertTabAtCaretPosition(TextArea textArea)
    {
      switch (textArea.Caret.CaretMode)
      {
        case CaretMode.InsertMode:
          textArea.InsertString(GetIndentationString(textArea.Document, textArea));
          break;
        case CaretMode.OverwriteMode:
          string indentStr = GetIndentationString(textArea.Document, textArea);
          textArea.ReplaceChar(indentStr[0]);
          if (indentStr.Length > 1)
          {
            textArea.InsertString(indentStr.Substring(1));
          }
          break;
      }
      textArea.SetDesiredColumn();
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    /// <remarks>
    /// Executes this edit action
    /// </remarks>
    public override void Execute(TextArea textArea)
    {
      if (textArea.SelectionManager.SelectionIsReadonly)
        return;

      textArea.Document.UndoStack.StartUndoGroup();
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        foreach (ISelection selection in textArea.SelectionManager.Selections)
        {
          int startLine = selection.StartPosition.Y;
          int endLine = selection.EndPosition.Y;
          if (startLine != endLine)
          {
            textArea.BeginUpdate();
            InsertTabs(textArea.Document, selection, startLine, endLine);
            textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, startLine, endLine));
            textArea.EndUpdate();
          }
          else
          {
            InsertTabAtCaretPosition(textArea);
            break;
          }
        }
        textArea.Document.CommitUpdate();
        textArea.AutoClearSelection = false;
      }
      else
      {
        InsertTabAtCaretPosition(textArea);
      }
      textArea.Document.UndoStack.EndUndoGroup();
    }
  }


  /// <summary>
  /// Removes a tab (removes indentation).
  /// </summary>
  public class ShiftTab : AbstractEditAction
  {
    static void RemoveTabs(IDocument document, ISelection selection, int y1, int y2)
    {
      document.UndoStack.StartUndoGroup();
      for (int i = y2; i >= y1; --i)
      {
        LineSegment line = document.GetLineSegment(i);
        if (i == y2 && line.Offset == selection.EndOffset)
          continue;

        if (line.Length > 0)
        {
          if (line.Length > 0)
          {
            int charactersToRemove = 0;
            if (document.GetCharAt(line.Offset) == '\t')
            { 
              // first character is a tab - just remove it
              charactersToRemove = 1;
            }
            else if (document.GetCharAt(line.Offset) == ' ')
            {
              int leadingSpaces;
              int tabIndent = document.TextEditorProperties.IndentationSize;
              for (leadingSpaces = 1; leadingSpaces < line.Length && document.GetCharAt(line.Offset + leadingSpaces) == ' '; leadingSpaces++)
              {
                // deliberately empty
              }
              if (leadingSpaces >= tabIndent)
              {
                // just remove tabIndent
                charactersToRemove = tabIndent;
              }
              else if (line.Length > leadingSpaces && document.GetCharAt(line.Offset + leadingSpaces) == '\t')
              {
                // remove the leading spaces and the following tab as they add up
                // to just one tab stop
                charactersToRemove = leadingSpaces + 1;
              }
              else
              {
                // just remove the leading spaces
                charactersToRemove = leadingSpaces;
              }
            }
            if (charactersToRemove > 0)
            {
              document.Remove(line.Offset, charactersToRemove);
            }
          }
        }
      }
      document.UndoStack.EndUndoGroup();
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        foreach (ISelection selection in textArea.SelectionManager.Selections)
        {
          int startLine = selection.StartPosition.Y;
          int endLine = selection.EndPosition.Y;
          textArea.BeginUpdate();
          RemoveTabs(textArea.Document, selection, startLine, endLine);
          textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, startLine, endLine));
          textArea.EndUpdate();
        }
        textArea.AutoClearSelection = false;
      }
      else
      {
        // Pressing Shift-Tab with nothing selected the cursor will move back to the
        // previous tab stop. It will stop at the beginning of the line. Also, the desired
        // column is updated to that column.
        int tabIndent = textArea.Document.TextEditorProperties.IndentationSize;
        int currentColumn = textArea.Caret.Column;
        int remainder = currentColumn % tabIndent;
        textArea.Caret.DesiredColumn = (remainder == 0) ? Math.Max(0, currentColumn - tabIndent) : Math.Max(0, currentColumn - remainder);
        textArea.SetCaretToDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Comments/uncomments the current selection.
  /// </summary>
  public class ToggleComment : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Document.ReadOnly)
        return;

      if (textArea.Document.HighlightingStrategy.Properties.ContainsKey("LineComment"))
      {
        new ToggleLineComment().Execute(textArea);
      }
      else if (textArea.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentBegin") &&
                 textArea.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentEnd"))
      {
        new ToggleBlockComment().Execute(textArea);
      }
    }
  }


  /// <summary>
  /// Comments/uncomments the currently selected lines.
  /// </summary>
  public class ToggleLineComment : AbstractEditAction
  {
    int firstLine;
    int lastLine;

    void RemoveCommentAt(IDocument document, string comment, ISelection selection, int y1, int y2)
    {
      firstLine = y1;
      lastLine = y2;

      for (int i = y2; i >= y1; --i)
      {
        LineSegment line = document.GetLineSegment(i);
        if (selection != null && i == y2 && line.Offset == selection.Offset + selection.Length)
        {
          --lastLine;
          continue;
        }

        string lineText = document.GetText(line.Offset, line.Length);
        if (lineText.Trim().StartsWith(comment))
        {
          document.Remove(line.Offset + lineText.IndexOf(comment), comment.Length);
        }
      }
    }

    void SetCommentAt(IDocument document, string comment, ISelection selection, int y1, int y2)
    {
      firstLine = y1;
      lastLine = y2;

      for (int i = y2; i >= y1; --i)
      {
        LineSegment line = document.GetLineSegment(i);
        if (selection != null && i == y2 && line.Offset == selection.Offset + selection.Length)
        {
          --lastLine;
          continue;
        }
        document.Insert(line.Offset, comment);
      }
    }

    bool ShouldComment(IDocument document, string comment, ISelection selection, int startLine, int endLine)
    {
      for (int i = endLine; i >= startLine; --i)
      {
        LineSegment line = document.GetLineSegment(i);
        if (selection != null && i == endLine && line.Offset == selection.Offset + selection.Length)
        {
          --lastLine;
          continue;
        }
        string lineText = document.GetText(line.Offset, line.Length);
        if (!lineText.Trim().StartsWith(comment))
        {
          return true;
        }
      }
      return false;
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Document.ReadOnly)
        return;

      string comment = null;
      if (textArea.Document.HighlightingStrategy.Properties.ContainsKey("LineComment"))
        comment = textArea.Document.HighlightingStrategy.Properties["LineComment"];

      if (String.IsNullOrEmpty(comment))
        return;

      textArea.Document.UndoStack.StartUndoGroup();

      if (textArea.SelectionManager.HasSomethingSelected)
      {
        bool shouldComment = true;
        foreach (ISelection selection in textArea.SelectionManager.Selections)
        {
          if (!ShouldComment(textArea.Document, comment, selection, selection.StartPosition.Y, selection.EndPosition.Y))
          {
            shouldComment = false;
            break;
          }
        }

        foreach (ISelection selection in textArea.SelectionManager.Selections)
        {
          textArea.BeginUpdate();
          if (shouldComment)
            SetCommentAt(textArea.Document, comment, selection, selection.StartPosition.Y, selection.EndPosition.Y);
          else
            RemoveCommentAt(textArea.Document, comment, selection, selection.StartPosition.Y, selection.EndPosition.Y);
          
          textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.LinesBetween, firstLine, lastLine));
          textArea.EndUpdate();
        }
        textArea.Document.CommitUpdate();
        textArea.AutoClearSelection = false;
      }
      else
      {
        textArea.BeginUpdate();
        int caretLine = textArea.Caret.Line;
        if (ShouldComment(textArea.Document, comment, null, caretLine, caretLine))
          SetCommentAt(textArea.Document, comment, null, caretLine, caretLine);
        else
          RemoveCommentAt(textArea.Document, comment, null, caretLine, caretLine);

        textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, caretLine));
        textArea.EndUpdate();
      }
      textArea.Document.UndoStack.EndUndoGroup();
    }
  }


  /// <summary>
  /// Comments/uncomments the current selection.
  /// </summary>
  public class ToggleBlockComment : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Document.ReadOnly)
        return;

      string commentStart = null;
      if (textArea.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentBegin"))
        commentStart = textArea.Document.HighlightingStrategy.Properties["BlockCommentBegin"];

      string commentEnd = null;
      if (textArea.Document.HighlightingStrategy.Properties.ContainsKey("BlockCommentEnd"))
        commentEnd = textArea.Document.HighlightingStrategy.Properties["BlockCommentEnd"];

      if (String.IsNullOrEmpty(commentStart) || String.IsNullOrEmpty(commentEnd))
        return;

      int selectionStartOffset;
      int selectionEndOffset;

      if (textArea.SelectionManager.HasSomethingSelected)
      {
        selectionStartOffset = textArea.SelectionManager.Selections[0].Offset;
        selectionEndOffset = textArea.SelectionManager.Selections[textArea.SelectionManager.Selections.Count - 1].EndOffset;
      }
      else
      {
        selectionStartOffset = textArea.Caret.Offset;
        selectionEndOffset = selectionStartOffset;
      }

      BlockCommentRegion commentRegion = FindSelectedCommentRegion(textArea.Document, commentStart, commentEnd, selectionStartOffset, selectionEndOffset);

      textArea.Document.UndoStack.StartUndoGroup();
      if (commentRegion != null)
        RemoveComment(textArea.Document, commentRegion);
      else if (textArea.SelectionManager.HasSomethingSelected)
        SetCommentAt(textArea.Document, selectionStartOffset, selectionEndOffset, commentStart, commentEnd);
      textArea.Document.UndoStack.EndUndoGroup();

      textArea.Document.CommitUpdate();
      textArea.AutoClearSelection = false;
    }


    /// <summary>
    /// Finds the selected comment region.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="commentStart">The comment start.</param>
    /// <param name="commentEnd">The comment end.</param>
    /// <param name="selectionStartOffset">The start offset of the selection.</param>
    /// <param name="selectionEndOffset">The end offset of selection.</param>
    /// <returns>The description of the block comment.</returns>
    public static BlockCommentRegion FindSelectedCommentRegion(IDocument document, string commentStart, string commentEnd, int selectionStartOffset, int selectionEndOffset)
    {
      if (document.TextLength == 0)
        return null;

      // Find start of comment in selected text.
      int commentEndOffset;
      string selectedText = document.GetText(selectionStartOffset, selectionEndOffset - selectionStartOffset);

      int commentStartOffset = selectedText.IndexOf(commentStart);
      if (commentStartOffset >= 0)
        commentStartOffset += selectionStartOffset;

      // Find end of comment in selected text.
      if (commentStartOffset >= 0)
        commentEndOffset = selectedText.IndexOf(commentEnd, commentStartOffset + commentStart.Length - selectionStartOffset);
      else
        commentEndOffset = selectedText.IndexOf(commentEnd);

      if (commentEndOffset >= 0)
        commentEndOffset += selectionStartOffset;

      // Find start of comment before or partially inside the 
      // selected text.

      if (commentStartOffset == -1)
      {
        int offset = selectionEndOffset + commentStart.Length - 1;
        if (offset > document.TextLength)
          offset = document.TextLength;

        string text = document.GetText(0, offset);
        commentStartOffset = text.LastIndexOf(commentStart);
        if (commentStartOffset >= 0)
        {
          // Find end of comment before comment start.
          int commentEndBeforeStartOffset = text.IndexOf(commentEnd, commentStartOffset, selectionStartOffset - commentStartOffset);
          if (commentEndBeforeStartOffset > commentStartOffset)
            commentStartOffset = -1;
        }
      }

      // Find end of comment after or partially after the
      // selected text.

      if (commentEndOffset == -1)
      {
        int offset = selectionStartOffset + 1 - commentEnd.Length;
        if (offset < 0)
          offset = selectionStartOffset;

        string text = document.GetText(offset, document.TextLength - offset);
        commentEndOffset = text.IndexOf(commentEnd);
        if (commentEndOffset >= 0)
          commentEndOffset += offset;

      }

      if (commentStartOffset != -1 && commentEndOffset != -1)
        return new BlockCommentRegion(commentStart, commentEnd, commentStartOffset, commentEndOffset);

      return null;
    }


    static void SetCommentAt(IDocument document, int offsetStart, int offsetEnd, string commentStart, string commentEnd)
    {
      document.Insert(offsetEnd, commentEnd);
      document.Insert(offsetStart, commentStart);
    }


    static void RemoveComment(IDocument document, BlockCommentRegion commentRegion)
    {
      document.Remove(commentRegion.EndOffset, commentRegion.CommentEnd.Length);
      document.Remove(commentRegion.StartOffset, commentRegion.CommentStart.Length);
    }
  }


  /// <summary>
  /// Describes a commented region.
  /// </summary>
  public class BlockCommentRegion
  {
    private readonly string commentStart = String.Empty;
    private readonly string commentEnd = String.Empty;
    private readonly int startOffset = -1;
    private readonly int endOffset = -1;


    /// <summary>
    /// Initializes a new instance of the <see cref="BlockCommentRegion"/> class.
    /// </summary>
    /// <param name="commentStart">The comment start.</param>
    /// <param name="commentEnd">The comment end.</param>
    /// <param name="startOffset">The offset of the comment start.</param>
    /// <param name="endOffset">The offset of the comment end.</param>
    public BlockCommentRegion(string commentStart, string commentEnd, int startOffset, int endOffset)
    {
      this.commentStart = commentStart;
      this.commentEnd = commentEnd;
      this.startOffset = startOffset;
      this.endOffset = endOffset;
    }


    /// <summary>
    /// Gets the comment start string.
    /// </summary>
    /// <value>The comment start string.</value>
    public string CommentStart
    {
      get { return commentStart; }
    }


    /// <summary>
    /// Gets the comment end string.
    /// </summary>
    /// <value>The comment end string.</value>
    public string CommentEnd
    {
      get { return commentEnd; }
    }


    /// <summary>
    /// Gets the of the comment start string.
    /// </summary>
    /// <value>The offset of the comment start string.</value>
    public int StartOffset
    {
      get { return startOffset; }
    }


    /// <summary>
    /// Gets the offset of the comment end string.
    /// </summary>
    /// <value>The offset of the comment end string.</value>
    public int EndOffset
    {
      get { return endOffset; }
    }


    /// <summary>
    /// Serves as a hash function for a particular type.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="T:System.Object"/>.
    /// </returns>
    public override int GetHashCode()
    {
      int hashCode = 0;
      unchecked
      {
        if (commentStart != null) hashCode += 1000000007 * commentStart.GetHashCode();
        if (commentEnd != null) hashCode += 1000000009 * commentEnd.GetHashCode();
        hashCode += 1000000021 * startOffset.GetHashCode();
        hashCode += 1000000033 * endOffset.GetHashCode();
      }
      return hashCode;
    }


    /// <summary>
    /// Determines whether the specified <see cref="Object"></see> is equal to the current <see cref="Object"></see>.
    /// </summary>
    /// <param name="obj">The <see cref="Object"></see> to compare with the current <see cref="Object"></see>.</param>
    /// <returns>
    /// true if the specified <see cref="Object"></see> is equal to the current <see cref="Object"></see>; otherwise, false.
    /// </returns>
    public override bool Equals(object obj)
    {
      BlockCommentRegion other = obj as BlockCommentRegion;
      if (other == null) 
        return false;

      return commentStart == other.commentStart 
        && commentEnd == other.commentEnd 
        && startOffset == other.startOffset 
        && endOffset == other.endOffset;
    }
  }


  /// <summary>
  /// Removes the last character.
  /// </summary>
  public class Backspace : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        Delete.DeleteSelection(textArea);
      }
      else
      {
        if (textArea.Caret.Offset > 0 && !textArea.IsReadOnly(textArea.Caret.Offset - 1))
        {
          textArea.BeginUpdate();
          int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
          int curLineOffset = textArea.Document.GetLineSegment(curLineNr).Offset;

          if (curLineOffset == textArea.Caret.Offset)
          {
            LineSegment line = textArea.Document.GetLineSegment(curLineNr - 1);
            int lineEndOffset = line.Offset + line.Length;
            int lineLength = line.Length;
            textArea.Document.Remove(lineEndOffset, curLineOffset - lineEndOffset);
            textArea.Caret.Position = new TextLocation(lineLength, curLineNr - 1);
            textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr - 1)));
          }
          else
          {
            int caretOffset = textArea.Caret.Offset - 1;
            textArea.Caret.Position = textArea.Document.OffsetToPosition(caretOffset);
            textArea.Document.Remove(caretOffset, 1);

            textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToLineEnd, new TextLocation(textArea.Caret.Offset - textArea.Document.GetLineSegment(curLineNr).Offset, curLineNr)));
          }
          textArea.EndUpdate();
        }
      }
    }
  }


  /// <summary>
  /// Removes the next character.
  /// </summary>
  public class Delete : AbstractEditAction
  {
    /// <summary>
    /// Deletes the currently selected text.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    internal static void DeleteSelection(TextArea textArea)
    {
      Debug.Assert(textArea.SelectionManager.HasSomethingSelected);
      if (textArea.SelectionManager.SelectionIsReadonly)
        return;
      textArea.BeginUpdate();
      textArea.Caret.Position = textArea.SelectionManager.Selections[0].StartPosition;
      textArea.SelectionManager.RemoveSelectedText();
      textArea.ScrollToCaret();
      textArea.EndUpdate();
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        DeleteSelection(textArea);
      }
      else
      {
        if (textArea.IsReadOnly(textArea.Caret.Offset))
          return;

        if (textArea.Caret.Offset < textArea.Document.TextLength)
        {
          textArea.BeginUpdate();
          int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
          LineSegment curLine = textArea.Document.GetLineSegment(curLineNr);

          if (curLine.Offset + curLine.Length == textArea.Caret.Offset)
          {
            if (curLineNr + 1 < textArea.Document.TotalNumberOfLines)
            {
              LineSegment nextLine = textArea.Document.GetLineSegment(curLineNr + 1);

              textArea.Document.Remove(textArea.Caret.Offset, nextLine.Offset - textArea.Caret.Offset);
              textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr)));
            }
          }
          else
          {
            textArea.Document.Remove(textArea.Caret.Offset, 1);
          }
          textArea.UpdateMatchingBracket();
          textArea.EndUpdate();
        }
      }
    }
  }


  /// <summary>
  /// Moves the caret on page down.
  /// </summary>
  public class MovePageDown : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      int curLineNr = textArea.Caret.Line;
      int requestedLineNumber = Math.Min(textArea.Document.GetNextVisibleLineAfter(curLineNr, textArea.TextView.NumberOfVisibleLines), textArea.Document.TotalNumberOfLines - 1);

      if (curLineNr != requestedLineNumber)
      {
        textArea.Caret.Position = new TextLocation(0, requestedLineNumber);
        textArea.SetCaretToDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Moves the page up.
  /// </summary>
  public class MovePageUp : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      int curLineNr = textArea.Caret.Line;
      int requestedLineNumber = Math.Max(textArea.Document.GetNextVisibleLineBefore(curLineNr, textArea.TextView.NumberOfVisibleLines), 0);

      if (curLineNr != requestedLineNumber)
      {
        textArea.Caret.Position = new TextLocation(0, requestedLineNumber);
        textArea.SetCaretToDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Inserts a newline into the document.
  /// </summary>
  public class Return : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Document.ReadOnly)
      {
        return;
      }
      textArea.BeginUpdate();
      textArea.Document.UndoStack.StartUndoGroup();
      try
      {
        textArea.InsertString(Environment.NewLine);
        int curLineNr = textArea.Caret.Line;
        textArea.Document.FormattingStrategy.FormatLine(textArea, curLineNr, textArea.Caret.Offset, '\n');
        textArea.SetDesiredColumn();
        textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, curLineNr - 1)));
      }
      finally
      {
        textArea.Document.UndoStack.EndUndoGroup();
        textArea.EndUpdate();
      }
    }
  }


  /// <summary>
  /// Switches between Insert and Overwrite mode.
  /// </summary>
  public class ToggleEditMode : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Document.ReadOnly)
        return;

      switch (textArea.Caret.CaretMode)
      {
        case CaretMode.InsertMode:
          textArea.Caret.CaretMode = CaretMode.OverwriteMode;
          break;
        case CaretMode.OverwriteMode:
          textArea.Caret.CaretMode = CaretMode.InsertMode;
          break;
      }
    }
  }


  /// <summary>
  /// Undoes the last operation.
  /// </summary>
  public class Undo : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.MotherTextEditorControl.Undo();
    }
  }


  /// <summary>
  /// Redoes the last undone operation.
  /// </summary>
  public class Redo : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.MotherTextEditorControl.Redo();
    }
  }


  /// <summary>
  /// Removes the characters between the caret and the beginning of the previous word.
  /// </summary>
  public class WordBackspace : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      // if anything is selected we will just delete it first
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        Delete.DeleteSelection(textArea);
        return;
      }
      textArea.BeginUpdate();
      // now delete from the caret to the beginning of the word
      LineSegment line =
        textArea.Document.GetLineSegmentForOffset(textArea.Caret.Offset);
      // if we are not at the beginning of a line
      if (textArea.Caret.Offset > line.Offset)
      {
        int prevWordStart = TextHelper.FindPrevWordStart(textArea.Document,
                                                            textArea.Caret.Offset);
        if (prevWordStart < textArea.Caret.Offset)
        {
          if (!textArea.IsReadOnly(prevWordStart, textArea.Caret.Offset - prevWordStart))
          {
            textArea.Document.Remove(prevWordStart,
                                     textArea.Caret.Offset - prevWordStart);
            textArea.Caret.Position = textArea.Document.OffsetToPosition(prevWordStart);
          }
        }
      }
      // if we are now at the beginning of a line
      if (textArea.Caret.Offset == line.Offset)
      {
        // if we are not on the first line
        int curLineNr = textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset);
        if (curLineNr > 0)
        {
          // move to the end of the line above
          LineSegment lineAbove = textArea.Document.GetLineSegment(curLineNr - 1);
          int endOfLineAbove = lineAbove.Offset + lineAbove.Length;
          int charsToDelete = textArea.Caret.Offset - endOfLineAbove;
          if (!textArea.IsReadOnly(endOfLineAbove, charsToDelete))
          {
            textArea.Document.Remove(endOfLineAbove, charsToDelete);
            textArea.Caret.Position = textArea.Document.OffsetToPosition(endOfLineAbove);
          }
        }
      }
      textArea.SetDesiredColumn();
      textArea.EndUpdate();
      // if there are now less lines, we need this or there are redraw problems
      textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset))));
      textArea.Document.CommitUpdate();
    }
  }


  /// <summary>
  /// Removes characters between the caret and the beginning of the next word.
  /// </summary>
  public class DeleteWord : Delete
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.SelectionManager.HasSomethingSelected)
      {
        DeleteSelection(textArea);
        return;
      }
      // if anything is selected we will just delete it first
      textArea.BeginUpdate();
      // now delete from the caret to the beginning of the word
      LineSegment line = textArea.Document.GetLineSegmentForOffset(textArea.Caret.Offset);
      if (textArea.Caret.Offset == line.Offset + line.Length)
      {
        // if we are at the end of a line
        base.Execute(textArea);
      }
      else
      {
        int nextWordStart = TextHelper.FindNextWordStart(textArea.Document, textArea.Caret.Offset);
        if (nextWordStart > textArea.Caret.Offset)
        {
          if (!textArea.IsReadOnly(textArea.Caret.Offset, nextWordStart - textArea.Caret.Offset))
          {
            textArea.Document.Remove(textArea.Caret.Offset, nextWordStart - textArea.Caret.Offset);
            // cursor never moves with this command
          }
        }
      }
      textArea.UpdateMatchingBracket();
      textArea.EndUpdate();
      // if there are now less lines, we need this or there are redraw problems
      textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, textArea.Document.GetLineNumberForOffset(textArea.Caret.Offset))));
      textArea.Document.CommitUpdate();
    }
  }


  /// <summary>
  /// Deletes the current line.
  /// </summary>
  public class DeleteLine : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      int lineNr = textArea.Caret.Line;
      LineSegment line = textArea.Document.GetLineSegment(lineNr);
      if (textArea.IsReadOnly(line.Offset, line.Length))
        return;
      textArea.Document.Remove(line.Offset, line.TotalLength);
      textArea.Caret.Position = textArea.Document.OffsetToPosition(line.Offset);

      textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.PositionToEnd, new TextLocation(0, lineNr)));
      textArea.UpdateMatchingBracket();
      textArea.Document.CommitUpdate();
    }
  }


  /// <summary>
  /// Deletes all characters up to the end of the current line.
  /// </summary>
  public class DeleteToLineEnd : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      int lineNr = textArea.Caret.Line;
      LineSegment line = textArea.Document.GetLineSegment(lineNr);

      int numRemove = (line.Offset + line.Length) - textArea.Caret.Offset;
			if (numRemove > 0 && !textArea.IsReadOnly(textArea.Caret.Offset, numRemove))
      {
        textArea.Document.Remove(textArea.Caret.Offset, numRemove);
        textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, new TextLocation(0, lineNr)));
        textArea.Document.CommitUpdate();
      }
    }
  }


  /// <summary>
  /// Moves the caret to the matching brace.
  /// </summary>
  public class GotoMatchingBrace : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      Highlight highlight = textArea.FindMatchingBracketHighlight();
      if (highlight != null)
      {
        TextLocation p1 = new TextLocation(highlight.ClosingBrace.X + 1, highlight.ClosingBrace.Y);
        TextLocation p2 = new TextLocation(highlight.OpeningBrace.X + 1, highlight.OpeningBrace.Y);
        if (p1 == textArea.Caret.Position)
        {
          if (textArea.Document.TextEditorProperties.BracketMatchingStyle == BracketMatchingStyle.After)
            textArea.Caret.Position = p2;
          else
            textArea.Caret.Position = new TextLocation(p2.X - 1, p2.Y);
        }
        else
        {
          if (textArea.Document.TextEditorProperties.BracketMatchingStyle == BracketMatchingStyle.After)
            textArea.Caret.Position = p1;
          else
            textArea.Caret.Position = new TextLocation(p1.X - 1, p1.Y);
        }
        textArea.SetDesiredColumn();
      }
    }
  }
}
