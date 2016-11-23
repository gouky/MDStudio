using System;

namespace DigitalRune.Windows.TextEditor.Document
{
  /// <summary>
  /// A line/column position. (Text editor lines/columns are counting from zero.)
  /// </summary>
  /// <remarks>
  /// For convenience: The line number can be accessed via <see cref="Y"/> or <see cref="Line"/>.
  /// And the column number can be accessed via <see cref="X"/> or <see cref="Column"/>. 
  /// </remarks>
  public struct TextLocation : IComparable<TextLocation>, IEquatable<TextLocation>
  {
    /// <summary>
    /// Represents no text location (-1, -1).
    /// </summary>
    public static readonly TextLocation Empty = new TextLocation(-1, -1);


    private int _x;
    private int _y;


    /// <summary>
    /// Initializes a new instance of the <see cref="TextLocation"/> struct.
    /// </summary>
    /// <param name="column">The column.</param>
    /// <param name="line">The line.</param>
    public TextLocation(int column, int line)
    {
      _x = column;
      _y = line;
    }


    /// <summary>
    /// Gets or sets the column.
    /// </summary>
    /// <value>The column.</value>
    public int X
    {
      get { return _x; }
      set { _x = value; }
    }


    /// <summary>
    /// Gets or sets the column.
    /// </summary>
    /// <value>The column.</value>
    public int Y
    {
      get { return _y; }
      set { _y = value; }
    }


    /// <summary>
    /// Gets or sets the line.
    /// </summary>
    /// <value>The line.</value>
    public int Line
    {
      get { return _y; }
      set { _y = value; }
    }


    /// <summary>
    /// Gets or sets the column.
    /// </summary>
    /// <value>The column.</value>
    public int Column
    {
      get { return _x; }
      set { _x = value; }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is empty.
    /// </summary>
    /// <value><see langword="true"/> if this instance is empty; otherwise, <see langword="false"/>.</value>
    /// <seealso cref="Empty"/>
    public bool IsEmpty
    {
      get { return _x <= 0 && _y <= 0; }
    }


    /// <summary>
    /// Returns the string representation of this <see cref="TextLocation"/>.
    /// </summary>
    /// <returns>The string representation of this <see cref="TextLocation"/></returns>
    public override string ToString()
    {
      return string.Format("(Line {1}, Col {0})", _x, _y);
    }


    /// <summary>
    /// Gets the hash-code.
    /// </summary>
    /// <returns>The hash-code.</returns>
    public override int GetHashCode()
    {
      return unchecked(87 * _x.GetHashCode() ^ _y.GetHashCode());
    }


    /// <summary>
    /// Determines whether two <see cref="Object"/> instances are equal.
    /// </summary>
    /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Object"/>.</param>
    /// <returns><see langword="true"/> if the specified <see cref="Object"/> is equal to the current <see cref="Object"/>; otherwise, <see langword="false"/>.</returns>
    public override bool Equals(object obj)
    {
      if (!(obj is TextLocation)) return false;
      return (TextLocation) obj == this;
    }


    /// <summary>
    /// Determines whether two <see cref="TextLocation"/> instances are equal.
    /// </summary>
    /// <param name="other">The <see cref="TextLocation"/> to compare with the current <see cref="TextLocation"/>.</param>
    /// <returns><see langword="true"/> if the specified <see cref="TextLocation"/> is equal to the current <see cref="TextLocation"/>; otherwise, <see langword="false"/>.</returns>
    public bool Equals(TextLocation other)
    {
      return this == other;
    }


    /// <summary>
    /// Compares two <see cref="TextLocation"/> instances for equality.
    /// </summary>
    /// <param name="a">The first <see cref="TextLocation"/>.</param>
    /// <param name="b">The second <see cref="TextLocation"/>.</param>
    /// <returns><see langword="true"/> if the specified <see cref="TextLocation"/>s are equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator ==(TextLocation a, TextLocation b)
    {
      return (a._x == b._x) && (a._y == b._y);
    }


    /// <summary>
    /// Compares two <see cref="TextLocation"/> instances for inequality.
    /// </summary>
    /// <param name="a">The first <see cref="TextLocation"/>.</param>
    /// <param name="b">The second <see cref="TextLocation"/>.</param>
    /// <returns><see langword="true"/> if the specified <see cref="TextLocation"/>s are not equal; otherwise, <see langword="false"/>.</returns>
    public static bool operator !=(TextLocation a, TextLocation b)
    {
      return (a._x != b._x) || (a._y != b._y);
    }


    /// <summary>
    /// Compares two <see cref="TextLocation"/> instances.
    /// </summary>
    /// <param name="a">The first <see cref="TextLocation"/>.</param>
    /// <param name="b">The second <see cref="TextLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="a"/> lies before <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <(TextLocation a, TextLocation b)
    {
      if (a._y < b._y)
        return true;
      else if (a._y == b._y)
        return (a._x < b._x);
      else
        return false;
    }


    /// <summary>
    /// Compares two <see cref="TextLocation"/> instances.
    /// </summary>
    /// <param name="a">The first <see cref="TextLocation"/>.</param>
    /// <param name="b">The second <see cref="TextLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="a"/> lies after <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >(TextLocation a, TextLocation b)
    {
      if (a._y > b._y)
        return true;
      else if (a._y == b._y)
        return (a._x > b._x);
      else
        return false;
    }


    /// <summary>
    /// Compares two <see cref="TextLocation"/> instances.
    /// </summary>
    /// <param name="a">The first <see cref="TextLocation"/>.</param>
    /// <param name="b">The second <see cref="TextLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="a"/> lies before or at the same location as <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator <=(TextLocation a, TextLocation b)
    {
      return !(a > b);
    }


    /// <summary>
    /// Compares two <see cref="TextLocation"/> instances.
    /// </summary>
    /// <param name="a">The first <see cref="TextLocation"/>.</param>
    /// <param name="b">The second <see cref="TextLocation"/>.</param>
    /// <returns><see langword="true"/> if the <paramref name="a"/> lies after or at the same location as <paramref name="b"/>; otherwise, <see langword="false"/>.</returns>
    public static bool operator >=(TextLocation a, TextLocation b)
    {
      return !(a < b);
    }


    /// <summary>
    /// Compares the current <see cref="TextLocation"/> with another <see cref="TextLocation"/>.
    /// </summary>
    /// <param name="other">The other <see cref="TextLocation"/>.</param>
    /// <returns>
    /// <c>-1</c> if the current <see cref="TextLocation"/> lies before <paramref name="other"/>, 
    /// <c>0</c> if they lie at the same location,
    /// and <c>+1</c> if the current <see cref="TextLocation"/>lies after <paramref name="other"/>.</returns>
    public int CompareTo(TextLocation other)
    {
      if (this == other)
        return 0;
      if (this < other)
        return -1;
      else
        return 1;
    }
  }
}
