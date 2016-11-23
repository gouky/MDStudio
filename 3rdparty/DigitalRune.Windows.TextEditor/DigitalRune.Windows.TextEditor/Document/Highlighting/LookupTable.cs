using System;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// This class implements a keyword map. It implements a digital search tree (tries) to find
  /// a word.
  /// </summary>
  public class LookupTable
  {
    private readonly Node _root = new Node(null, null);
    private readonly bool _caseSensitive;
    private int _length;


    /// <summary>
    /// Gets the number of elements in the table.
    /// </summary>
    /// <value>The number of elements in the table.</value>
    public int Count
    {
      get { return _length; }
    }


    /// <summary>
    /// Get the object that was inserted under the keyword.
    /// </summary>
    /// <value>The object that was inserted under the keyword.</value>
    /// <remarks>
    /// <para>
    /// The keyword is taken from an <see cref="IDocument"/> at a given location
    /// (line, offset in line, length).
    /// </para>
    /// <para>
    /// Returns <see langword="null"/>, if no such keyword exists.
    /// </para>
    /// </remarks>
    public object this[IDocument document, LineSegment line, int offsetInLine, int lengthOfWord]
    {
      get
      {
        if (lengthOfWord == 0)
          return null;

        Node next = _root;

        int wordOffset = line.Offset + offsetInLine;
        if (_caseSensitive)
        {
          for (int i = 0; i < lengthOfWord; ++i)
          {
            int index = document.GetCharAt(wordOffset + i) % 256;
            next = next[index];

            if (next == null)
              return null;

            if (next.Color != null && TextHelper.CompareSegment(document, wordOffset, lengthOfWord, next.Word))
              return next.Color;
          }
        }
        else
        {
          for (int i = 0; i < lengthOfWord; ++i)
          {
            int index = Char.ToUpper(document.GetCharAt(wordOffset + i)) % 256;

            next = next[index];

            if (next == null)
              return null;

            if (next.Color != null && TextHelper.CompareSegment(document, wordOffset, lengthOfWord, next.Word, _caseSensitive))
              return next.Color;
          }
        }
        return null;
      }
    }


    /// <summary>
    /// Gets or sets an object in the tree under the keyword.
    /// </summary>
    public object this[string keyword]
    {
      get
      {
        if (String.IsNullOrEmpty(keyword))
          return null;

        Node next = _root;
        int length = keyword.Length;

        if (_caseSensitive)
        {
          for (int i = 0; i < length; ++i)
          {
            int index = keyword[i] % 256;
            next = next[index];

            if (next == null)
              return null;

            if (next.Color != null && String.Compare(keyword, next.Word, false) == 0)
              return next.Color;
          }
        }
        else
        {
          for (int i = 0; i < length; ++i)
          {
            int index = Char.ToUpper(keyword[i]) % 256;

            next = next[index];

            if (next == null)
              return null;

            if (next.Color != null && String.Compare(keyword, next.Word, true) == 0)
              return next.Color;
          }
        }
        return null;
      }

      set
      {
        Node node = _root;
        Node next = _root;
        if (!_caseSensitive)
        {
          keyword = keyword.ToUpper();
        }
        ++_length;

        // insert word into the tree
        for (int i = 0; i < keyword.Length; ++i)
        {
          int index = keyword[i] % 256; // index of current char

          next = next[index];             // get node to this index

          if (next == null)
          { // no node created -> insert word here
            node[index] = new Node(value, keyword);
            break;
          }

          if (next.Word != null && next.Word.Length != i)
          { // node there, take node content and insert them again
            string tmpword = next.Word;                  // this word will be inserted 1 level deeper (better, don't need too much 
            object tmpcolor = next.Color;                 // string comparisons for finding.)
            next.Color = next.Word = null;
            this[tmpword] = tmpcolor;
          }

          if (i == keyword.Length - 1)
          { // end of keyword reached, insert node there, if a node was here it was
            next.Word = keyword;       // reinserted, if it has the same length (keyword EQUALS this word) it will be overwritten
            next.Color = value;
            break;
          }

          node = next;
        }
      }
    }


    /// <summary>
    /// Creates a new instance of <see cref="LookupTable"/>
    /// </summary>
    public LookupTable(bool casesensitive)
    {
      _caseSensitive = casesensitive;
    }


    private class Node
    {
      public string Word;
      public object Color;
      private Node[] _children;

      // Lazily initialize children array. Saves 200 KB of memory for the C# highlighting
      // because we don't have to store the array for leaf nodes.
      public Node this[int index]
      {
        get
        {
          if (_children != null)
            return _children[index];

          return null;
        }
        set
        {
          if (_children == null)
            _children = new Node[256];
          _children[index] = value;
        }
      }
			

      public Node(object color, string word)
      {
        Word = word;
        Color = color;
      }
    }
  }
}
