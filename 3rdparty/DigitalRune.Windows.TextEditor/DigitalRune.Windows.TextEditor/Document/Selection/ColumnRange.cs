using System;

namespace DigitalRune.Windows.TextEditor.Selection
{
  /// <summary>
  /// Defines a column range.
  /// </summary>
  public class ColumnRange
  {
    /// <summary>
    /// Defines a <see cref="ColumnRange"/> that contains no columns.
    /// </summary>
    /// <remarks>
    /// The properties <see cref="StartColumn"/> and <see cref="EndColumn"/>
    /// are meaningless in this case. (But they are used internally.)
    /// </remarks>
    public static readonly ColumnRange NoColumn = new ColumnRange(-2, -2);

    /// <summary>
    /// Defines a <see cref="ColumnRange"/> that contains all columns of line.
    /// </summary>
    /// <remarks>
    /// The properties <see cref="StartColumn"/> and <see cref="EndColumn"/>
    /// are meaningless in this case. (But they are used internally.)
    /// </remarks>
    public static readonly ColumnRange WholeColumn = new ColumnRange(-1, -1);

    private int _startColumn;
    private int _endColumn;


    /// <summary>
    /// Gets or sets the start column.
    /// </summary>
    /// <value>The start column.</value>
    public int StartColumn
    {
      get { return _startColumn; }
      set { _startColumn = value; }
    }


    /// <summary>
    /// Gets or sets the end column.
    /// </summary>
    /// <value>The end column.</value>
    public int EndColumn
    {
      get { return _endColumn; }
      set { _endColumn = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnRange"/> class.
    /// </summary>
    /// <param name="startColumn">The start column.</param>
    /// <param name="endColumn">The end column.</param>
    public ColumnRange(int startColumn, int endColumn)
    {
      _startColumn = startColumn;
      _endColumn = endColumn;
    }


    /// <summary>
    /// Serves as a hash function for a particular type. <see cref="Object.GetHashCode"></see> is suitable for use in hashing algorithms and data structures like a hash table.
    /// </summary>
    /// <returns>
    /// A hash code for the current <see cref="Object"></see>.
    /// </returns>
    public override int GetHashCode()
    {
      return _startColumn + (_endColumn << 16);
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
      if (obj is ColumnRange)
      {
        return ((ColumnRange) obj)._startColumn == _startColumn &&
               ((ColumnRange) obj)._endColumn == _endColumn;
      }
      return false;
    }


    /// <summary>
    /// Returns a <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="String"></see> that represents the current <see cref="Object"></see>.
    /// </returns>
    public override string ToString()
    {
      return String.Format("[ColumnRange: StartColumn={0}, EndColumn={1}]", _startColumn, _endColumn);
    }
  }
}
