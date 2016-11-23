using System;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;

namespace DigitalRune.Windows.TextEditor.Markers
{
  /// <summary>
  /// Marks a part of a document.
  /// </summary>
  public class Marker : ISegment
  {
    private int _offset = -1;
    private int _length = -1;
    private readonly MarkerType _markerType;
    private readonly Color _color;
    private readonly Color _textColor;
    private readonly bool _overrideTextColor;
    private string _toolTip;
    private bool _isReadOnly;


    /// <summary>
    /// Gets or sets the offset.
    /// </summary>
    /// <value>The offset where the span begins</value>
    public int Offset
    {
      get { return _offset; }
      set { _offset = value; }
    }


    /// <summary>
    /// Gets or sets the length.
    /// </summary>
    /// <value>The length of the span</value>
    public int Length
    {
      get { return _length; }
      set { _length = value; }
    }


    /// <summary>
    /// Gets the type of the text marker.
    /// </summary>
    /// <value>The type of the text marker.</value>
    public MarkerType MarkerType
    {
      get { return _markerType; }
    }


    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>The color.</value>
    public Color Color
    {
      get { return _color; }
    }


    /// <summary>
    /// Gets the foreground color of the text.
    /// </summary>
    /// <value>The foreground color of the text.</value>
    /// <remarks>
    /// Only relevant when <see cref="MarkerType"/> is <c>SolidBlock</c>.
    /// </remarks>
    public Color TextColor
    {
      get { return _textColor; }
    }


    /// <summary>
    /// Gets a value indicating whether to override the foreground color of the text.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if the foreground color of the text shall be overriden; otherwise, 
    /// <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// Only relevant when <see cref="MarkerType"/> is <c>SolidBlock</c>.
    /// </remarks>
    public bool OverrideTextColor
    {
      get { return _overrideTextColor; }
    }



    /// <summary>
    /// Marks the text segment as read-only.
    /// </summary>
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }


    /// <summary>
    /// Gets or sets the tool tip of the text marker.
    /// </summary>
    /// <value>The tool tip.</value>
    public string ToolTip
    {
      get { return _toolTip; }
      set { _toolTip = value; }
    }


    /// <summary>
    /// Gets the last offset that is inside the marker region.
    /// </summary>
    /// <value>The end offset.</value>
    public int EndOffset
    {
      get { return Offset + Length - 1; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Marker"/> class.
    /// </summary>
    /// <param name="offset">The offset of the marked region.</param>
    /// <param name="length">The length of the marked region.</param>
    /// <param name="markerType">Type of the text marker.</param>
    public Marker(int offset, int length, MarkerType markerType)
      : this(offset, length, markerType, Color.Red)
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Marker"/> class.
    /// </summary>
    /// <param name="offset">The offset of the marked region.</param>
    /// <param name="length">The length of the marked region.</param>
    /// <param name="markerType">Type of the text marker.</param>
    /// <param name="color">The color of the text marker.</param>
    public Marker(int offset, int length, MarkerType markerType, Color color)
    {
      if (length < 1) 
        length = 1;

      _offset = offset;
      _length = length;
      _markerType = markerType;
      _color = color;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Marker"/> class.
    /// </summary>
    /// <param name="offset">The offset of the marked region.</param>
    /// <param name="length">The length of the marked region.</param>
    /// <param name="markerType">Type of the text marker.</param>
    /// <param name="color">The color of the text marker.</param>
    /// <param name="foreColor">The foreground color of the text.</param>
    public Marker(int offset, int length, MarkerType markerType, Color color, Color foreColor)
    {
      if (length < 1) 
        length = 1;

      _offset = offset;
      _length = length;
      _markerType = markerType;
      _color = color;
      _textColor = foreColor;
      _overrideTextColor = true;
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[Marker: Offset = {0}, Length = {1}]", Offset, Length);
    }


    /// <summary>
    /// Determines whether the given marker is read-only.
    /// </summary>
    /// <param name="marker">The marker.</param>
    /// <returns>
    /// 	<see langword="true"/> if the specified marker is read-only; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool IsReadOnlyPredicate(Marker marker)
    {
      return marker.IsReadOnly;
    }
  }
}
