using System;
using DigitalRune.Windows.TextEditor.Bookmarks;


namespace DigitalRune.Windows.TextEditor.Actions
{
  /// <summary>
  /// Toggles a bookmark for the current line.
  /// </summary>
  public class ToggleBookmark : AbstractEditAction
  {
    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.Document.BookmarkManager.ToggleMarkAt(textArea.Caret.Position);
      textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, textArea.Caret.Line));
      textArea.Document.CommitUpdate();
    }
  }


  /// <summary>
  /// Jumps to the previous bookmark.
  /// </summary>
  public class GotoPrevBookmark : AbstractEditAction
  {
    private readonly Predicate<Bookmark> predicate;


    /// <summary>
    /// Initializes a new instance of the <see cref="GotoPrevBookmark"/> class.
    /// </summary>
    public GotoPrevBookmark()
    {
      predicate = BookmarkManager.AcceptAnyMarkPredicate;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="GotoPrevBookmark"/> class.
    /// </summary>
    /// <param name="predicate">
    /// A custom predicate to decide whether to accept the <see cref="Bookmark"/>
    /// or not.
    /// </param>
    public GotoPrevBookmark(Predicate<Bookmark> predicate)
    {
      this.predicate = predicate;
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      Bookmark mark = textArea.Document.BookmarkManager.GetPrevMark(textArea.Caret.Line, predicate);
      if (mark != null)
      {
        textArea.Caret.Position = mark.Location;
        textArea.SelectionManager.ClearSelection();
        textArea.SetDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Jumps to the next bookmark.
  /// </summary>
  public class GotoNextBookmark : AbstractEditAction
  {
    readonly Predicate<Bookmark> predicate;


    /// <summary>
    /// Initializes a new instance of the <see cref="GotoNextBookmark"/> class.
    /// </summary>
    public GotoNextBookmark()
    {
      predicate = BookmarkManager.AcceptAnyMarkPredicate;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="GotoNextBookmark"/> class.
    /// </summary>
    /// <param name="predicate">
    /// A custom predicate to decide whether to accept the <see cref="Bookmark"/>
    /// or not.
    /// </param>
    public GotoNextBookmark(Predicate<Bookmark> predicate)
    {
      this.predicate = predicate;
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      Bookmark mark = textArea.Document.BookmarkManager.GetNextMark(textArea.Caret.Line, predicate);
      if (mark != null)
      {
        textArea.Caret.Position = mark.Location;
        textArea.SelectionManager.ClearSelection();
        textArea.SetDesiredColumn();
      }
    }
  }


  /// <summary>
  /// Clears all bookmarks.
  /// </summary>
  public class ClearAllBookmarks : AbstractEditAction
  {
    private readonly Predicate<Bookmark> predicate;


    /// <summary>
    /// Initializes a new instance of the <see cref="ClearAllBookmarks"/> class.
    /// </summary>
    public ClearAllBookmarks()
    {
      predicate = BookmarkManager.AcceptAnyMarkPredicate;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ClearAllBookmarks"/> class.
    /// </summary>
    /// <param name="predicate">
    /// A custom predicate to decide whether to accept the <see cref="Bookmark"/>
    /// or not.
    /// </param>
    public ClearAllBookmarks(Predicate<Bookmark> predicate)
    {
      this.predicate = predicate;
    }


    /// <summary>
    /// Executes the action on a certain <see cref="TextArea"/>.
    /// </summary>
    /// <param name="textArea">The text area on which to execute the action.</param>
    public override void Execute(TextArea textArea)
    {
      textArea.Document.BookmarkManager.RemoveMarks(predicate);
      textArea.Document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
      textArea.Document.CommitUpdate();
    }
  }
}
