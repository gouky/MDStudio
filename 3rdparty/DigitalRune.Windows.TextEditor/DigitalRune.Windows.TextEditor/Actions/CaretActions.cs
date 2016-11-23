using System;
using System.Collections.Generic;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;


namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Moves the caret one position to the left.
  /// </summary>
  public class CaretLeft : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation position = textArea.Caret.Position;
      List<Fold> foldings = textArea.Document.FoldingManager.GetFoldedFoldsWithEndAt(position.Y);
      Fold justBeforeCaret = null;
      foreach (Fold fold in foldings)
      {
        if (fold.EndColumn == position.X)
        {
          justBeforeCaret = fold;
          break; // the first folding found is the folding with the smallest start position
        }
      }

      if (justBeforeCaret != null)
      {
        position.Y = justBeforeCaret.StartLine;
        position.X = justBeforeCaret.StartColumn;
      }
      else
      {
        if (position.X > 0)
        {
          --position.X;
        }
        else if (position.Y > 0)
        {
          LineSegment lineAbove = textArea.Document.GetLineSegment(position.Y - 1);
          position = new TextLocation(lineAbove.Length, position.Y - 1);
        }
      }

      textArea.Caret.Position = position;
      textArea.SetDesiredColumn();
    }
  }


  /// <summary>
  /// Moves the caret one position to the right.
  /// </summary>
  public class CaretRight : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      LineSegment curLine = textArea.Document.GetLineSegment(textArea.Caret.Line);
      TextLocation position = textArea.Caret.Position;
      List<Fold> foldings = textArea.Document.FoldingManager.GetFoldedFoldsWithStartAt(position.Y);
      Fold justBehindCaret = null;
      foreach (Fold fold in foldings)
      {
        if (fold.StartColumn == position.X)
        {
          justBehindCaret = fold;
          break;
        }
      }
      if (justBehindCaret != null)
      {
        position.Y = justBehindCaret.EndLine;
        position.X = justBehindCaret.EndColumn;
      }
      else
      { 
        // no folding is interesting
        if (position.X < curLine.Length || textArea.TextEditorProperties.AllowCaretBeyondEOL)
        {
          ++position.X;
        }
        else if (position.Y + 1 < textArea.Document.TotalNumberOfLines)
        {
          ++position.Y;
          position.X = 0;
        }
      }
      textArea.Caret.Position = position;
      textArea.SetDesiredColumn();
    }
  }


  /// <summary>
  /// Moves the caret up to the previous line.
  /// </summary>
  public class CaretUp : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation position = textArea.Caret.Position;
      int lineNr = position.Y;
      int visualLine = textArea.Document.GetVisibleLine(lineNr);
      if (visualLine > 0)
      {
        int xpos = textArea.TextView.GetDrawingXPos(lineNr, position.X);
        Point pos = new Point(xpos, textArea.TextView.DrawingPosition.Y + (visualLine - 1) * textArea.TextView.LineHeight - textArea.TextView.TextArea.VirtualTop.Y);
        textArea.Caret.Position = textArea.TextView.GetLogicalPosition(pos);
        textArea.SetCaretToDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Moves the caret down to the next line.
  /// </summary>
  public class CaretDown : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation position = textArea.Caret.Position;
      int lineNr = position.Y;
      int visualLine = textArea.Document.GetVisibleLine(lineNr);
      if (visualLine < textArea.Document.GetVisibleLine(textArea.Document.TotalNumberOfLines))
      {
        Point pos = new Point(textArea.TextView.GetDrawingXPos(lineNr, position.X),
                              textArea.TextView.DrawingPosition.Y
                              + (visualLine + 1) * textArea.TextView.LineHeight
                              - textArea.TextView.TextArea.VirtualTop.Y);
        textArea.Caret.Position = textArea.TextView.GetLogicalPosition(pos);
        textArea.SetCaretToDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Moves the caret to the beginning of the next word.
  /// </summary>
  public class WordRight : CaretRight
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      LineSegment line = textArea.Document.GetLineSegment(textArea.Caret.Position.Y);
      TextLocation oldPos = textArea.Caret.Position;
      TextLocation newPos;
      if (textArea.Caret.Column >= line.Length)
      {
        newPos = new TextLocation(0, textArea.Caret.Line + 1);
      }
      else
      {
        int nextWordStart = TextHelper.FindNextWordStart(textArea.Document, textArea.Caret.Offset);
        newPos = textArea.Document.OffsetToPosition(nextWordStart);
      }

      // handle fold markers
      List<Fold> foldings = textArea.Document.FoldingManager.GetFoldsFromPosition(newPos.Y, newPos.X);
      foreach (Fold marker in foldings)
      {
        if (marker.IsFolded)
        {
          if (oldPos.X == marker.StartColumn && oldPos.Y == marker.StartLine)
            newPos = new TextLocation(marker.EndColumn, marker.EndLine);
          else
            newPos = new TextLocation(marker.StartColumn, marker.StartLine);

          break;
        }
      }

      textArea.Caret.Position = newPos;
      textArea.SetDesiredColumn();
    }
  }


  /// <summary>
  /// Moves the caret to the beginning of the previous word.
  /// </summary>
  public class WordLeft : CaretLeft
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      TextLocation oldPos = textArea.Caret.Position;
      if (textArea.Caret.Column == 0)
      {
        base.Execute(textArea);
      }
      else
      {
        int prevWordStart = TextHelper.FindPrevWordStart(textArea.Document, textArea.Caret.Offset);

        TextLocation newPos = textArea.Document.OffsetToPosition(prevWordStart);

        // handle fold markers
        List<Fold> foldings = textArea.Document.FoldingManager.GetFoldsFromPosition(newPos.Y, newPos.X);
        foreach (Fold marker in foldings)
        {
          if (marker.IsFolded)
          {
            if (oldPos.X == marker.EndColumn && oldPos.Y == marker.EndLine)
              newPos = new TextLocation(marker.StartColumn, marker.StartLine);
            else
              newPos = new TextLocation(marker.EndColumn, marker.EndLine);

            break;
          }
        }
        textArea.Caret.Position = newPos;
        textArea.SetDesiredColumn();
      }
    }
  }



  /// <summary>
  /// Scrolls the <see cref="TextArea"/> up by one line.
  /// </summary>
  public class ScrollLineUp : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.AutoClearSelection = false;

      textArea.MotherTextAreaControl.VScrollBar.Value = Math.Max(textArea.MotherTextAreaControl.VScrollBar.Minimum, textArea.VirtualTop.Y - textArea.TextView.LineHeight);
    }
  }


  /// <summary>
  /// Scrolls the <see cref="TextArea"/> down by one line.
  /// </summary>
  public class ScrollLineDown : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.AutoClearSelection = false;
      textArea.MotherTextAreaControl.VScrollBar.Value = Math.Min(textArea.MotherTextAreaControl.VScrollBar.Maximum, textArea.VirtualTop.Y + textArea.TextView.LineHeight);
    }
  }
}
