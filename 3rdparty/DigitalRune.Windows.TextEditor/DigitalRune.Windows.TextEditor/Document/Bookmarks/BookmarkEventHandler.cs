using System;

namespace DigitalRune.Windows.TextEditor.Bookmarks
{
	/// <summary>
	/// Event arguments for bookmark related events.
	/// </summary>
	public class BookmarkEventArgs : EventArgs
	{
		private readonly Bookmark _bookmark;


    /// <summary>
    /// Gets the bookmark.
    /// </summary>
    /// <value>The bookmark.</value>
		public Bookmark Bookmark 
    {
			get { return _bookmark; }
		}


    /// <summary>
    /// Initializes a new instance of the <see cref="BookmarkEventArgs"/> class.
    /// </summary>
    /// <param name="bookmark">The bookmark.</param>
		public BookmarkEventArgs(Bookmark bookmark)
		{
			_bookmark = bookmark;
		}
	}
}
