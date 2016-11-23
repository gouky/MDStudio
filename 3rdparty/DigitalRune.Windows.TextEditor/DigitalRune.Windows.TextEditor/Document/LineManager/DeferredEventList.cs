using System.Collections.Generic;
using System.Collections.ObjectModel;


namespace DigitalRune.Windows.TextEditor.Document
{
	/// <summary>
	/// A list of events that are fired after the line manager has finished working.
	/// </summary>
	internal struct DeferredEventList
	{
		private List<LineSegment> _removedLines;
		private List<TextAnchor> _removedAnchors;


    /// <summary>
    /// Gets the removed lines.
    /// </summary>
    /// <value>The removed lines.</value>
    public ReadOnlyCollection<LineSegment> RemovedLines
	  {
      get { return (_removedLines != null) ? _removedLines.AsReadOnly() : null; }
	  }


    /// <summary>
    /// Gets the removed text anchors.
    /// </summary>
    /// <value>The removed <see cref="TextAnchor"/>s.</value>
	  public ReadOnlyCollection<TextAnchor> RemovedAnchors
	  {
	    get { return (_removedAnchors != null) ? _removedAnchors.AsReadOnly() : null; }
	  }


    /// <summary>
    /// Adds the removed line.
    /// </summary>
    /// <param name="line">The line.</param>
	  public void AddRemovedLine(LineSegment line)
		{
      if (_removedLines == null)
        _removedLines = new List<LineSegment>();
      _removedLines.Add(line);
		}
		

		public void AddDeletedAnchor(TextAnchor anchor)
		{
      if (_removedAnchors == null)
        _removedAnchors = new List<TextAnchor>();
      _removedAnchors.Add(anchor);
		}
		

		public void RaiseEvents()
		{
			// _removedLines is raised by the LineManager
      if (_removedAnchors != null) 
        foreach (TextAnchor anchor in _removedAnchors) 
					anchor.RaiseDeleted();
		}
	}
}
