using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Utilities;


namespace DigitalRune.Windows.TextEditor.Bookmarks
{
  /// <summary>
  /// This class handles the bookmarks for a buffer.
  /// </summary>
  public class BookmarkManager
  {
    private readonly IDocument _document;
#if DEBUG
		private readonly IList<Bookmark> _bookmarks = new CheckedList<Bookmark>();
#else
    private readonly IList<Bookmark> _bookmarks = new List<Bookmark>();
#endif
    IBookmarkFactory _factory;


    /// <summary>
    /// Gets the bookmarks.
    /// </summary>
    /// <value>The bookmarks.</value>
    public ReadOnlyCollection<Bookmark> Marks
    {
      get { return new ReadOnlyCollection<Bookmark>(_bookmarks); }
    }


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return _document; }
    }


    /// <summary>
    /// Gets or sets the bookmark factory.
    /// </summary>
    /// <value>The bookmark factory.</value>
    public IBookmarkFactory Factory
    {
      get { return _factory; }
      set { _factory = value; }
    }


    /// <summary>
    /// Occurs when bookmark is added.
    /// </summary>
    public event EventHandler<BookmarkEventArgs> Added;


    /// <summary>
    /// Occurs when bookmark is removed.
    /// </summary>
    public event EventHandler<BookmarkEventArgs> Removed;


    /// <summary>
    /// Creates a new instance of <see cref="BookmarkManager"/>
    /// </summary>
    /// <param name="document">The document.</param>
    internal BookmarkManager(IDocument document)
    {
      _document = document;
    }


    /// <summary>
    /// Sets the mark at the line <code>location.Line</code> if it is not set. If the
    /// line is already marked the mark is cleared.
    /// </summary>
    public void ToggleMarkAt(TextLocation location)
    {
      Bookmark newMark = (Factory != null) ? Factory.CreateBookmark(_document, location) : new Bookmark(_document, location);

      Type newMarkType = newMark.GetType();

      for (int i = 0; i < _bookmarks.Count; ++i)
      {
        Bookmark mark = _bookmarks[i];

        if (mark.LineNumber == location.Line && mark.CanToggle && mark.GetType() == newMarkType)
        {
          _bookmarks.RemoveAt(i);
          OnRemoved(new BookmarkEventArgs(mark));
          return;
        }
      }

      _bookmarks.Add(newMark);
      OnAdded(new BookmarkEventArgs(newMark));
    }


    /// <summary>
    /// Adds a bookmark.
    /// </summary>
    /// <param name="mark">The bookmark.</param>
    public void AddMark(Bookmark mark)
    {
      _bookmarks.Add(mark);
      OnAdded(new BookmarkEventArgs(mark));
    }


    /// <summary>
    /// Removes the bookmark.
    /// </summary>
    /// <param name="mark">The bookmark.</param>
    public void RemoveMark(Bookmark mark)
    {
      _bookmarks.Remove(mark);
      OnRemoved(new BookmarkEventArgs(mark));
    }


    /// <summary>
    /// Removes the bookmarks that match a certain criteria.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    public void RemoveMarks(Predicate<Bookmark> predicate)
    {
      for (int i = 0; i < _bookmarks.Count; ++i)
      {
        Bookmark bm = _bookmarks[i];
        if (predicate(bm))
        {
          _bookmarks.RemoveAt(i--);
          OnRemoved(new BookmarkEventArgs(bm));
        }
      }
    }


    /// <summary>
    /// Determines whether the specified line has a bookmark.
    /// </summary>
    /// <param name="lineNr">The line number.</param>
    /// <returns>
    /// true, if a mark at <paramref name="lineNr"/> exists, otherwise false.
    /// </returns>
    public bool IsMarked(int lineNr)
    {
      for (int i = 0; i < _bookmarks.Count; ++i)
        if (_bookmarks[i].LineNumber == lineNr)
          return true;

      return false;
    }


    /// <summary>
    /// Clears all bookmarks.
    /// </summary>
    public void Clear()
    {
      foreach (Bookmark mark in _bookmarks)
        OnRemoved(new BookmarkEventArgs(mark));

      _bookmarks.Clear();
    }


    /// <summary>
    /// Gets the first bookmark that matches a certain criteria.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The first matching bookmark.</returns>
    public Bookmark GetFirstMark(Predicate<Bookmark> predicate)
    {
      if (_bookmarks.Count < 1)
        return null;

      Bookmark first = null;
      for (int i = 0; i < _bookmarks.Count; ++i)
        if (predicate(_bookmarks[i]) && _bookmarks[i].Enabled && (first == null || _bookmarks[i].LineNumber < first.LineNumber))
          first = _bookmarks[i];

      return first;
    }


    /// <summary>
    /// Gets the last bookmark that matches a certain criteria.
    /// </summary>
    /// <param name="predicate">The predicate.</param>
    /// <returns>The last matching bookmark.</returns>
    public Bookmark GetLastMark(Predicate<Bookmark> predicate)
    {
      if (_bookmarks.Count < 1)
        return null;

      Bookmark last = null;
      for (int i = 0; i < _bookmarks.Count; ++i)
        if (predicate(_bookmarks[i]) && _bookmarks[i].Enabled && (last == null || _bookmarks[i].LineNumber > last.LineNumber))
          last = _bookmarks[i];

      return last;
    }


    /// <summary>
    /// Gets the next bookmark for a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <returns>The next bookmark afer the given line.</returns>
    public Bookmark GetNextMark(int curLineNr)
    {
      return GetNextMark(curLineNr, AcceptAnyMarkPredicate);
    }


    /// <summary>
    /// Gets the next bookmark for a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    /// The next bookmark for the given line; if it does not exist it returns <see cref="GetFirstMark"/>.
    /// </returns>
    public Bookmark GetNextMark(int curLineNr, Predicate<Bookmark> predicate)
    {
      if (_bookmarks.Count == 0)
        return null;

      Bookmark next = GetFirstMark(predicate);
      foreach (Bookmark mark in _bookmarks)
        if (predicate(mark) && mark.Enabled && mark.LineNumber > curLineNr)
          if (mark.LineNumber < next.LineNumber || next.LineNumber <= curLineNr)
            next = mark;

      return next;
    }


    /// <summary>
    /// Gets the bookmark before a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <returns>The previous bookmark.</returns>
    public Bookmark GetPrevMark(int curLineNr)
    {
      return GetPrevMark(curLineNr, AcceptAnyMarkPredicate);
    }


    /// <summary>
    /// Gets the bookmark before a given line.
    /// </summary>
    /// <param name="curLineNr">The line number.</param>
    /// <param name="predicate">The predicate.</param>
    /// <returns>
    /// The previous bookmark for the given line; if it does not exist it returns <see cref="GetLastMark"/>.
    /// </returns>
    public Bookmark GetPrevMark(int curLineNr, Predicate<Bookmark> predicate)
    {
      if (_bookmarks.Count == 0)
        return null;

      Bookmark prev = GetLastMark(predicate);

      foreach (Bookmark mark in _bookmarks)
        if (predicate(mark) && mark.Enabled && mark.LineNumber < curLineNr)
          if (mark.LineNumber > prev.LineNumber || prev.LineNumber >= curLineNr)
            prev = mark;

      return prev;
    }




    /// <summary>
    /// Raises the <see cref="Added"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Bookmarks.BookmarkEventArgs"/> instance containing the event data.</param>
    protected virtual void OnAdded(BookmarkEventArgs e)
    {
      if (Added != null)
        Added(this, e);
    }


    /// <summary>
    /// Raises the <see cref="Removed"/> event.
    /// </summary>
    /// <param name="e">The <see cref="DigitalRune.Windows.TextEditor.Bookmarks.BookmarkEventArgs"/> instance containing the event data.</param>
    protected virtual void OnRemoved(BookmarkEventArgs e)
    {
      if (Removed != null)
        Removed(this, e);
    }


    /// <summary>
    /// Accepts any mark.
    /// </summary>
    /// <param name="mark">The mark.</param>
    /// <returns>Always <see langword="true"/>.</returns>
    public static bool AcceptAnyMarkPredicate(Bookmark mark)
    {
      return true;
    }
  }
}
