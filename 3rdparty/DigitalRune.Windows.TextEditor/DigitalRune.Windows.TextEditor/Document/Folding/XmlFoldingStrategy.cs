using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Folding
{
  /// <summary>
  /// Holds information about the start of a fold in an xml string.
  /// </summary>
  internal class XmlFoldStart
  {
    private readonly int _line;
    private readonly int _column;
    private readonly string _prefix = String.Empty;
    private readonly string _name = String.Empty;
    private string _foldText = String.Empty;


    /// <summary>
    /// Initializes a new instance of the <see cref="XmlFoldStart"/> class.
    /// </summary>
    /// <param name="prefix">The prefix.</param>
    /// <param name="name">The name.</param>
    /// <param name="line">The line.</param>
    /// <param name="column">The column.</param>
    public XmlFoldStart(string prefix, string name, int line, int column)
    {
      _line = line;
      _column = column;
      _prefix = prefix;
      _name = name;
    }

    /// <summary>
    /// The line where the fold should start.  Lines start from 0.
    /// </summary>
    public int Line
    {
      get { return _line; }
    }

    /// <summary>
    /// The column where the fold should start.  Columns start from 0.
    /// </summary>
    public int Column
    {
      get { return _column; }
    }

    /// <summary>
    /// The name of the xml item with its prefix if it has one.
    /// </summary>
    public string Name
    {
      get
      {
        if (_prefix.Length > 0)
          return String.Concat(_prefix, ":", _name);
        else
          return _name;
      }
    }

    /// <summary>
    /// The text to be displayed when the item is folded.
    /// </summary>
    public string FoldText
    {
      get { return _foldText; }
      set { _foldText = value; }
    }
  }


  /// <summary>
  /// Determines folds for an xml string in the editor.
  /// </summary>
  public class XmlFoldingStrategy : IFoldingStrategy
  {
    private bool _showAttributesWhenFolded;


    /// <summary>
    /// Gets or sets a value indicating whether attributes should be displayed on folded elements.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if attributes should be displayed on folded elements; otherwise, <see langword="false"/>.
    /// </value>
    public bool ShowAttributesWhenFolded
    {
      get { return _showAttributesWhenFolded; }
      set { _showAttributesWhenFolded = value; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="XmlFoldingStrategy"/> class.
    /// </summary>
    public XmlFoldingStrategy()
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="XmlFoldingStrategy"/> class.
    /// </summary>
    /// <param name="showAttributesWhenFolded">if set to <see langword="true"/> attributes will be displayed on folded elements.</param>
    public XmlFoldingStrategy(bool showAttributesWhenFolded)
    {
      _showAttributesWhenFolded = showAttributesWhenFolded;
    }


    /// <summary>
    /// Adds folds to the text editor around each start-end element pair.
    /// </summary>
    /// <remarks>
    /// <para>If the xml is not well formed then no folds are created.</para> 
    /// <para>Note that the xml text reader lines and positions start 
    /// from 1 and the SharpDevelop text editor line information starts
    /// from 0.</para>
    /// </remarks>
    public List<Fold> GenerateFolds(IDocument document, string fileName, object parseInformation)
    {
      List<Fold> foldMarkers = new List<Fold>();
      Stack stack = new Stack();

      try
      {
        string xml = document.TextContent;
        XmlTextReader reader = new XmlTextReader(new StringReader(xml));
        
        // Do not parse external references such as a DTD.
        reader.XmlResolver = null;  

        while (reader.Read())
        {
          switch (reader.NodeType)
          {
            case XmlNodeType.Element:
              if (!reader.IsEmptyElement)
              {
                XmlFoldStart newFoldStart = CreateElementFoldStart(reader);
                stack.Push(newFoldStart);
              }
              break;

            case XmlNodeType.EndElement:
              XmlFoldStart foldStart = (XmlFoldStart) stack.Pop();
              CreateElementFold(document, foldMarkers, reader, foldStart);
              break;

            case XmlNodeType.Comment:
              CreateCommentFold(document, foldMarkers, reader);
              break;
          }
        }
      }
      catch (Exception)
      {
        // If the xml is not well formed keep the foldings 
        // that already exist in the document.
        return new List<Fold>(document.FoldingManager.Folds);
      }

      return foldMarkers;
    }


    /// <summary>
    /// Creates a comment fold if the comment spans more than one line.
    /// </summary>
    /// <remarks>The text displayed when the comment is folded is the first 
    /// line of the comment.</remarks>
    private static void CreateCommentFold(IDocument document, List<Fold> foldMarkers, XmlTextReader reader)
    {
      if (reader.Value != null)
      {
        string comment = reader.Value.Replace("\r\n", "\n");
        string[] lines = comment.Split('\n');
        if (lines.Length > 1)
        {

          // Take off 5 chars to get the actual comment start (takes
          // into account the <!-- chars.

          int startCol = reader.LinePosition - 5;
          int startLine = reader.LineNumber - 1;

          // Add 3 to the end col value to take into account the '-->'
          int endCol = lines[lines.Length - 1].Length + startCol + 3;
          int endLine = startLine + lines.Length - 1;
          string foldText = String.Concat("<!--", lines[0], "-->");
          Fold fold = new Fold(document, startLine, startCol, endLine, endCol, foldText);
          foldMarkers.Add(fold);
        }
      }
    }

    /// <summary>
    /// Creates an XmlFoldStart for the start tag of an element.
    /// </summary>
    XmlFoldStart CreateElementFoldStart(XmlTextReader reader)
    {
      // Take off 2 from the line position returned 
      // from the xml since it points to the start
      // of the element name and not the beginning 
      // tag.
      XmlFoldStart newFoldStart = new XmlFoldStart(reader.Prefix, reader.LocalName, reader.LineNumber - 1, reader.LinePosition - 2);

      if (_showAttributesWhenFolded && reader.HasAttributes)
        newFoldStart.FoldText = String.Concat("<", newFoldStart.Name, " ", GetAttributeFoldText(reader), ">");
      else
        newFoldStart.FoldText = String.Concat("<", newFoldStart.Name, ">");

      return newFoldStart;
    }

    /// <summary>
    /// Create an element fold if the start and end tag are on 
    /// different lines.
    /// </summary>
    private static void CreateElementFold(IDocument document, List<Fold> foldMarkers, XmlTextReader reader, XmlFoldStart foldStart)
    {
      int endLine = reader.LineNumber - 1;
      if (endLine > foldStart.Line)
      {
        int endCol = reader.LinePosition + foldStart.Name.Length;
        Fold fold = new Fold(document, foldStart.Line, foldStart.Column, endLine, endCol, foldStart.FoldText);
        foldMarkers.Add(fold);
      }
    }

    /// <summary>
    /// Gets the element's attributes as a string on one line that will
    /// be displayed when the element is folded.
    /// </summary>
    /// <remarks>
    /// Currently this puts all attributes from an element on the same
    /// line of the start tag.  It does not cater for elements where attributes
    /// are not on the same line as the start tag.
    /// </remarks>
    private static string GetAttributeFoldText(XmlTextReader reader)
    {
      StringBuilder text = new StringBuilder();

      for (int i = 0; i < reader.AttributeCount; ++i)
      {
        reader.MoveToAttribute(i);

        text.Append(reader.Name);
        text.Append("=");
        text.Append(reader.QuoteChar.ToString());
        text.Append(XmlEncodeAttributeValue(reader.Value, reader.QuoteChar));
        text.Append(reader.QuoteChar.ToString());

        // Append a space if this is not the
        // last attribute.
        if (i < reader.AttributeCount - 1)
          text.Append(" ");
      }

      return text.ToString();
    }

    /// <summary>
    /// Xml encode the attribute string since the string returned from
    /// the XmlTextReader is the plain unencoded string and .NET
    /// does not provide us with an xml encode method.
    /// </summary>
    static string XmlEncodeAttributeValue(string attributeValue, char quoteChar)
    {
      StringBuilder encodedValue = new StringBuilder(attributeValue);

      encodedValue.Replace("&", "&amp;");
      encodedValue.Replace("<", "&lt;");
      encodedValue.Replace(">", "&gt;");

      if (quoteChar == '"')
        encodedValue.Replace("\"", "&quot;");
      else
        encodedValue.Replace("'", "&apos;");

      return encodedValue.ToString();
    }
  }
}
