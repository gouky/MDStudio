using System;
using System.Collections.Generic;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Markers
{
  /// <summary>
  /// Manages the list of markers and provides ways to retrieve markers for 
  /// specific positions.
  /// </summary>
  public sealed class MarkerStrategy
  {
    private readonly List<Marker> _textMarkers = new List<Marker>();
    private readonly IDocument _document;

    // Cache that stores: (key, value) = (offset, list of markers at offset)
    private readonly Dictionary<int, List<Marker>> markersTable = new Dictionary<int, List<Marker>>();


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return _document; }
    }


    /// <summary>
    /// Gets the text markers.
    /// </summary>
    /// <value>The text markers.</value>
    public IEnumerable<Marker> TextMarker
    {
      get { return _textMarkers.AsReadOnly(); }
    }


    /// <summary>
    /// Adds a text marker.
    /// </summary>
    /// <param name="item">The text marker.</param>
    public void AddMarker(Marker item)
    {
      markersTable.Clear();
      _textMarkers.Add(item);
    }


    /// <summary>
    /// Inserts a text marker.
    /// </summary>
    /// <param name="index">The index at which to insert the marker.</param>
    /// <param name="item">The text marker.</param>
    public void InsertMarker(int index, Marker item)
    {
      markersTable.Clear();
      _textMarkers.Insert(index, item);
    }


    /// <summary>
    /// Removes a text marker.
    /// </summary>
    /// <param name="item">The text marker.</param>
    public void RemoveMarker(Marker item)
    {
      markersTable.Clear();
      _textMarkers.Remove(item);
    }


    /// <summary>
    /// Removes all text markers.
    /// </summary>
    public void Clear()
    {
      markersTable.Clear();
      _textMarkers.Clear();
    }


    /// <summary>
    /// Removes all text markers that match a given criteria.
    /// </summary>
    /// <param name="match">The match.</param>
    public void RemoveAll(Predicate<Marker> match)
    {
      markersTable.Clear();
      _textMarkers.RemoveAll(match);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="MarkerStrategy"/> class.
    /// </summary>
    /// <param name="document">The document.</param>
    public MarkerStrategy(IDocument document)
    {
      _document = document;
      document.DocumentChanged += DocumentChanged;
    }


    /// <summary>
    /// Retrieves the text markers that contain a specific offset.
    /// </summary>
    /// <param name="offset">The offset in the document.</param>
    /// <returns>
    /// The text marker at <paramref name="offset"/>. Returns an empty list if
    /// no marker contains the offset.
    /// </returns>
    public List<Marker> GetMarkers(int offset)
    {
      if (!markersTable.ContainsKey(offset))
      {
        List<Marker> markers = new List<Marker>();
        for (int i = 0; i < _textMarkers.Count; ++i)
        {
          Marker marker = _textMarkers[i];
          if (marker.Offset <= offset && offset <= marker.EndOffset)
          {
            markers.Add(marker);
          }
        }
        markersTable[offset] = markers;
      }
      return markersTable[offset];
    }


    /// <summary>
    /// Retrieves all text markers in a given region.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <returns>A list of all text markers in this region.</returns>
    public List<Marker> GetMarkers(int offset, int length)
    {
      int endOffset = offset + length - 1;
      List<Marker> markers = new List<Marker>();
      for (int i = 0; i < _textMarkers.Count; ++i)
      {
        Marker marker = _textMarkers[i];

        if (marker.Offset <= offset && offset <= marker.EndOffset           // start in marker region
            || marker.Offset <= endOffset && endOffset <= marker.EndOffset  // end in marker region
            || offset <= marker.Offset && marker.Offset <= endOffset        // marker start in region
            || offset <= marker.EndOffset && marker.EndOffset <= endOffset) // marker end in region
        {
          markers.Add(marker);
        }
      }
      return markers;
    }


    /// <summary>
    /// Retrieves a list of all text markers at a given position.
    /// </summary>
    /// <param name="position">The position in the document.</param>
    /// <returns>A list of all text markers at <paramref name="position"/>.</returns>
    public List<Marker> GetMarkers(TextLocation position)
    {
      if (position.Y >= _document.TotalNumberOfLines || position.Y < 0)
        return new List<Marker>();

      LineSegment segment = _document.GetLineSegment(position.Y);
      return GetMarkers(segment.Offset + position.X);
    }


    private void DocumentChanged(object sender, DocumentEventArgs e)
    {
      // reset markers table
      markersTable.Clear();
      _document.UpdateSegmentListOnDocumentChange(_textMarkers, e);
    }
  }
}
