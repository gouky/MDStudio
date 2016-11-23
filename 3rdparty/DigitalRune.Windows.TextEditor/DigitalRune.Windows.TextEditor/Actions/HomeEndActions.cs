using System.Collections.Generic;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;


namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Moves the caret to start of the line.
  /// </summary>
  public class Home : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      LineSegment curLine;
      TextLocation newPos = textArea.Caret.Position;
      bool jumpedIntoFolding;
      do
      {
        curLine = textArea.Document.GetLineSegment(newPos.Y);

        if (TextHelper.IsEmptyLine(textArea.Document, newPos.Y))
        {
          newPos.X = newPos.X != 0 ? 0 : curLine.Length;
        }
        else
        {
          int firstCharOffset = TextHelper.FindFirstNonWhitespace(textArea.Document, curLine.Offset);
          int firstCharColumn = firstCharOffset - curLine.Offset;
          newPos.X = newPos.X == firstCharColumn ? 0 : firstCharColumn;
        }
        List<Fold> foldings = textArea.Document.FoldingManager.GetFoldsFromPosition(newPos.Y, newPos.X);
        jumpedIntoFolding = false;
        foreach (Fold fold in foldings)
        {
          if (fold.IsFolded)
          {
            newPos = new TextLocation(fold.StartColumn, fold.StartLine);
            jumpedIntoFolding = true;
            break;
          }
        }

      } while (jumpedIntoFolding);

      if (newPos != textArea.Caret.Position)
      {
        textArea.Caret.Position = newPos;
        textArea.SetDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Moves the caret to the end of the line.
  /// </summary>
  public class End : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      LineSegment curLine;
      TextLocation newPos = textArea.Caret.Position;
      bool jumpedIntoFolding;
      do
      {
        curLine = textArea.Document.GetLineSegment(newPos.Y);
        newPos.X = curLine.Length;

        List<Fold> foldings = textArea.Document.FoldingManager.GetFoldsFromPosition(newPos.Y, newPos.X);
        jumpedIntoFolding = false;
        foreach (Fold fold in foldings)
        {
          if (fold.IsFolded)
          {
            newPos = new TextLocation(fold.EndColumn, fold.EndLine);
            jumpedIntoFolding = true;
            break;
          }
        }
      } while (jumpedIntoFolding);

      if (newPos != textArea.Caret.Position)
      {
        textArea.Caret.Position = newPos;
        textArea.SetDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Moves the caret to the start of the document.
  /// </summary>
  public class MoveToStart : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      if (textArea.Caret.Line != 0 || textArea.Caret.Column != 0)
      {
        textArea.Caret.Position = new TextLocation(0, 0);
        textArea.SetDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Moves the document caret to the end of the document.
  /// </summary>
  public class MoveToEnd : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation endPos = textArea.Document.OffsetToPosition(textArea.Document.TextLength);
      if (textArea.Caret.Position != endPos)
      {
        textArea.Caret.Position = endPos;
        textArea.SetDesiredColumn();
      }
    }
  }
}
