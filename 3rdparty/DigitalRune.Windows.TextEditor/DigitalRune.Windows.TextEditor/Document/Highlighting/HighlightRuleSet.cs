using System;
using System.Xml;
using System.Collections.Generic;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Defines a set of syntax highlighting rules.
  /// </summary>
  public class HighlightRuleSet
  {
    private readonly LookupTable keyWords;
    private List<Span> spans = new List<Span>();
    private readonly LookupTable prevMarkers;
    private readonly LookupTable nextMarkers;
    private IHighlightingStrategyUsingRuleSets highlighter;
    private readonly char escapeCharacter;
    private readonly bool ignoreCase;
    private readonly bool highlightDigits;
    private string name;
    private readonly bool[] delimiters = new bool[256];
    private readonly string reference;


    /// <summary>
    /// Gets an array that defines for each character whether it is an delimiter.
    /// </summary>
    /// <value>
    /// An array that defines for each character whether it is an delimiter.
    /// </value>
    public bool[] Delimiters
    {
      get { return delimiters; }
    }


    /// <summary>
    /// Gets the spans.
    /// </summary>
    /// <value>The spans.</value>
    public List<Span> Spans
    {
      get { return spans; }
    }


    /// <summary>
    /// Gets or sets the highlighting strategy.
    /// </summary>
    /// <value>The highlighting strategy.</value>
    internal IHighlightingStrategyUsingRuleSets Highlighter
    {
      get { return highlighter; }
      set { highlighter = value; }
    }

    /// <summary>
    /// Gets the lookup table for keywords.
    /// </summary>
    /// <value>The lookup table for keywords.</value>
    public LookupTable KeyWords
    {
      get { return keyWords; }
    }


    /// <summary>
    /// Gets the lookup table for strings that mark the previous token.
    /// </summary>
    /// <value>The lookup table for strings that mark the previous token.</value>
    public LookupTable PrevMarkers
    {
      get { return prevMarkers; }
    }


    /// <summary>
    /// Gets the lookup table for strings that mark the next token.
    /// </summary>
    /// <value>The lookup table for strings that mark the next token.</value>
    public LookupTable NextMarkers
    {
      get { return nextMarkers; }
    }


    /// <summary>
    /// Gets the escape character.
    /// </summary>
    /// <value>The escape character.</value>
    public char EscapeCharacter
    {
      get { return escapeCharacter; }
    }


    /// <summary>
    /// Gets a value indicating whether this rule is case-sensitive or not.
    /// </summary>
    /// <value><see langword="true"/> if case-insensitive; <see langword="false"/> if case-sensitive.</value>
    public bool IgnoreCase
    {
      get { return ignoreCase; }
    }


    /// <summary>
    /// Gets a value indicating whether digits should be highlighted.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if digits in the span are highlighted; otherwise, <see langword="false"/>.
    /// </value>
    public bool HighlightDigits
    {
      get { return highlightDigits; }
    }


    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return name; }
      set { name = value; }
    }


    /// <summary>
    /// Gets the reference rule set.
    /// </summary>
    /// <value>The reference rule set.</value>
    public string Reference
    {
      get { return reference; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightRuleSet"/> class.
    /// </summary>
    public HighlightRuleSet()
    {
      keyWords = new LookupTable(false);
      prevMarkers = new LookupTable(false);
      nextMarkers = new LookupTable(false);
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightRuleSet"/> class.
    /// </summary>
    /// <param name="el">The XML element that describes the rule set.</param>
    public HighlightRuleSet(XmlElement el)
    {
      if (el.Attributes["name"] != null)
        Name = el.Attributes["name"].InnerText;

      if (el.HasAttribute("escapecharacter"))
        escapeCharacter = el.GetAttribute("escapecharacter")[0];

      if (el.Attributes["reference"] != null)
        reference = el.Attributes["reference"].InnerText;

      if (el.Attributes["ignorecase"] != null)
        ignoreCase = Boolean.Parse(el.Attributes["ignorecase"].InnerText);

      if (el.Attributes["highlightDigits"] != null)
        highlightDigits = Boolean.Parse(el.Attributes["highlightDigits"].InnerText);

      for (int i = 0; i < Delimiters.Length; ++i)
        delimiters[i] = false;

      if (el["Delimiters"] != null)
      {
        string delimiterString = el["Delimiters"].InnerText;
        foreach (char ch in delimiterString)
          delimiters[ch] = true;
      }

      keyWords = new LookupTable(!IgnoreCase);
      prevMarkers = new LookupTable(!IgnoreCase);
      nextMarkers = new LookupTable(!IgnoreCase);

      XmlNodeList nodes = el.GetElementsByTagName("KeyWords");
      foreach (XmlElement el2 in nodes)
      {
        HighlightColor color = new HighlightColor(el2);

        XmlNodeList keys = el2.GetElementsByTagName("Key");
        foreach (XmlElement node in keys)
        {
          keyWords[node.Attributes["word"].InnerText] = color;
        }
      }

      nodes = el.GetElementsByTagName("Span");
      foreach (XmlElement el2 in nodes)
        Spans.Add(new Span(el2));

      nodes = el.GetElementsByTagName("MarkPrevious");
      foreach (XmlElement el2 in nodes)
      {
        PrevMarker prev = new PrevMarker(el2);
        prevMarkers[prev.What] = prev;
      }

      nodes = el.GetElementsByTagName("MarkFollowing");
      foreach (XmlElement el2 in nodes)
      {
        NextMarker next = new NextMarker(el2);
        nextMarkers[next.What] = next;
      }
    }


    /// <summary>
    /// Merges spans etc. from the other rule set into this rule set.
    /// </summary>
    /// <param name="ruleSet">The rule set.</param>
    public void MergeFrom(HighlightRuleSet ruleSet)
    {
      for (int i = 0; i < delimiters.Length; i++)
      {
        delimiters[i] |= ruleSet.delimiters[i];
      }
      // insert merged spans in front of old spans
      List<Span> oldSpans = spans;
      spans = new List<Span>(ruleSet.spans);
      spans.AddRange(oldSpans);
      //keyWords.MergeFrom(ruleSet.keyWords);
      //prevMarkers.MergeFrom(ruleSet.prevMarkers);
      //nextMarkers.MergeFrom(ruleSet.nextMarkers);
    }
  }
}
