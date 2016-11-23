using System;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Bookmarks
{
  /// <summary>
  /// Description of Bookmark.
  /// </summary>
  public class Bookmark

  {
      enum BookmarkType
      {
          kMark,
          kBreakpoint
      };

    private IDocument _document;
    private TextAnchor _anchor;
    private TextLocation _location;
    private bool _enabled = true;
    private BookmarkType _type = BookmarkType.kMark;


    /// <summary>
    /// Gets if the bookmark can be toggled off using the 'set/unset bookmark' command.
    /// </summary>
    public virtual bool CanToggle
    {
      get { return true; }
    }

    /// <summary>
    /// Gets or sets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return _document; }
      set
      {
				if (_document != value) 
        {
					if (_anchor != null) 
          {
						_location = _anchor.Location;
						_anchor = null;
					}
					_document = value;
					CreateAnchor();
					OnDocumentChanged(EventArgs.Empty);
				}
      }
    }


    void CreateAnchor()
    {
      if (_document != null)
      {
        LineSegment line = _document.GetLineSegment(Math.Max(0, Math.Min(_location.Line, _document.TotalNumberOfLines - 1)));
        _anchor = line.CreateAnchor(Math.Max(0, Math.Min(_location.Column, line.Length)));
        // after insertion: keep bookmarks after the initial whitespace (see DefaultFormattingStrategy.SmartReplaceLine)
        _anchor.MovementType = AnchorMovementType.AfterInsertion;
        _anchor.Deleted += OnAnchorDeleted;
      }
    }


    void OnAnchorDeleted(object sender, EventArgs e)
    {
      _document.BookmarkManager.RemoveMark(this);
    }


    /// <summary>
    /// Gets the <see cref="TextAnchor"/> used for this bookmark.
    /// </summary>
    /// <value>The anchor used for this bookmark. <see langword="null"/> if the bookmark is not connected to a document.</value>
    public TextAnchor Anchor
    {
      get { return _anchor; }
    }

    /// <summary>
    /// Gets or sets the location of this bookmark.
    /// </summary>
    /// <value>The location of this bookmark.</value>
    public TextLocation Location
    {
      get
      {
        if (_anchor != null)
          return _anchor.Location;
        else
          return _location;
      }
      set
      {
        _location = value;
        CreateAnchor();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether this bookmark is enabled.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this bookmark is enabled; otherwise, <see langword="false"/>.
    /// </value>
    public bool Enabled
    {
      get { return _enabled; }
      set
      {
        if (_enabled != value)
        {
          _enabled = value;
          if (_document != null)
          {
            _document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, LineNumber));
            _document.CommitUpdate();
          }
          OnEnabledChanged(EventArgs.Empty);
        }
      }
    }


    /// <summary>
    /// Gets the line number of this bookmark.
    /// </summary>
    /// <value>The line number of this bookmark.</value>
    public int LineNumber
    {
      get { return _anchor != null ? _anchor.LineNumber : _location.Line; }
    }


    /// <summary>
    /// Gets the column number of this bookmark.
    /// </summary>
    /// <value>The column number of this bookmark.</value>
    public int ColumnNumber
    {
      get { return _anchor != null ? _anchor.ColumnNumber : _location.Column; }
    }


    /// <summary>
    /// Occurs when the document is has been changed.
    /// </summary>
    public event EventHandler DocumentChanged;


    /// <summary>
    /// Occurs when the <see cref="Enabled"/> property has been changed.
    /// </summary>
    public event EventHandler EnabledChanged;


    /// <summary>
    /// Initializes a new instance of the <see cref="Bookmark"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="location">The location of the bookmark.</param>
    public Bookmark(IDocument document, TextLocation location)
      : this(document, location, true)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Bookmark"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="location">The location of the bookmark.</param>
    /// <param name="enabled">If set to <see langword="true"/> this bookmark is enabled.</param>
    public Bookmark(IDocument document, TextLocation location, bool enabled)
    {
      _document = document;
      _enabled = enabled;
      Location = location;
    }


    /// <summary>
    /// Raises the <see cref="DocumentChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnDocumentChanged(EventArgs e)
    {
      if (DocumentChanged != null)
        DocumentChanged(this, e);
    }


    /// <summary>
    /// Raises the <see cref="EnabledChanged"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnEnabledChanged(EventArgs e)
    {
      if (EnabledChanged != null)
        EnabledChanged(this, e);
    }


    /// <summary>
    /// Handles a click on this bookmark on the <see cref="IconMargin"/>.
    /// </summary>
    /// <param name="parent">The parent control.</param>
    /// <param name="e">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    /// <returns><see langword="true"/> if mouse click has been handled and the bookmark is removed.</returns>
    /// <remarks>
    /// Per default a left-click on the bookmark removes it from the icon margin.
    /// </remarks>
    public virtual bool Click(Control parent, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left && CanToggle)
      {
        _document.BookmarkManager.RemoveMark(this);
        return true;
      }
      return false;
    }


    /// <summary>
    /// Draws the bookmark on the <see cref="IconMargin"/>.
    /// </summary>
    /// <param name="g">The <see cref="Graphics"/> context.</param>
    /// <param name="rectangle">The bounding rectangle.</param>
    public virtual void Draw(Graphics g, Rectangle rectangle)
    {
        switch (_type)
        {
            case BookmarkType.kMark:
                BookmarkRenderer.DrawBookmark(g, rectangle, _enabled);
                break;

            case BookmarkType.kBreakpoint:
                BookmarkRenderer.DrawBreakpoint(g, rectangle, true, true);
                break;
        }
    }
  }
}
