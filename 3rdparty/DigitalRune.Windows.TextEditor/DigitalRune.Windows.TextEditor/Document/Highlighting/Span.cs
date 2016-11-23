using System;
using System.Xml;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Defines a span (for example: strings, line comments, block comments, etc.).
  /// </summary>
  public sealed class Span
  {
    private readonly bool stopEOL;
    private readonly HighlightColor color;
    private readonly HighlightColor beginColor;
    private readonly HighlightColor endColor;
    private readonly char[] begin;
    private readonly char[] end;
    private readonly string name;
    private readonly string rule;
    private HighlightRuleSet ruleSet;
    private readonly char escapeCharacter;
    private bool ignoreCase;
    private readonly bool isBeginSingleWord;
    private readonly bool? isBeginStartOfLine;
    private readonly bool isEndSingleWord;


    /// <summary>
    /// Gets the expression that defines the begin of the span.
    /// </summary>
    /// <value>The expression that defines the begin of the span.</value>
    public char[] Begin
    {
      get { return begin; }
    }


    /// <summary>
    /// Gets the expression that defines the end of the span.
    /// </summary>
    /// <value>The expression that defines the end of the span.</value>
    public char[] End
    {
      get { return end; }
    }


    /// <summary>
    /// Gets or sets the rule set.
    /// </summary>
    /// <value>The rule set.</value>
    internal HighlightRuleSet RuleSet
    {
      get { return ruleSet; }
      set { ruleSet = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to ignore the case.
    /// </summary>
    /// <value><see langword="true"/> to ignore the case; otherwise, <see langword="false"/>.</value>
    public bool IgnoreCase
    {
      get { return ignoreCase; }
      set { ignoreCase = value; }
    }


    /// <summary>
    /// Gets a value indicating whether stop the span at the end of the line.
    /// </summary>
    /// <value><see langword="true"/> if the span stops at the end of the line; otherwise, <see langword="false"/>.</value>
    public bool StopEOL
    {
      get { return stopEOL; }
    }


    /// <summary>
    /// Gets a value indicating whether this span starts with the begin of a line.
    /// </summary>
    /// <value>A value indicating whether this span starts with the begin of a line.</value>
    public bool? IsBeginStartOfLine
    {
      get { return isBeginStartOfLine; }
    }


    /// <summary>
    /// Gets a value indicating whether this span begins with a single word.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this span begins with a single word; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsBeginSingleWord
    {
      get { return isBeginSingleWord; }
    }


    /// <summary>
    /// Gets a value indicating whether this span ends with a single word.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this span ends with a single word; otherwise, <see langword="false"/>.
    /// </value>
    public bool IsEndSingleWord
    {
      get { return isEndSingleWord; }
    }


    /// <summary>
    /// Gets the color.
    /// </summary>
    /// <value>The color.</value>
    public HighlightColor Color
    {
      get { return color; }
    }


    /// <summary>
    /// Gets the color of the begin of the span.
    /// </summary>
    /// <value>The color of the begin of the span.</value>
    public HighlightColor BeginColor
    {
      get
      {
        if (beginColor != null)
          return beginColor;
        else
          return color;
      }
    }


    /// <summary>
    /// Gets the end color of the span.
    /// </summary>
    /// <value>The end color of the span.</value>
    public HighlightColor EndColor
    {
      get { return endColor ?? color; }
    }


    /// <summary>
    /// Gets the name.
    /// </summary>
    /// <value>The name.</value>
    public string Name
    {
      get { return name; }
    }


    /// <summary>
    /// Gets the rule applied to this span.
    /// </summary>
    /// <value>The rule applied to this span.</value>
    public string Rule
    {
      get { return rule; }
    }


    /// <summary>
    /// Gets the escape character of the span. The escape character is a character that can be used in front
    /// of the span end to make it not end the span. The escape character followed by another escape character
    /// means the escape character was escaped like in @"a "" b" literals in C#.
    /// The default value '\0' means no escape character is allowed.
    /// </summary>
    public char EscapeCharacter
    {
      get { return escapeCharacter; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Span"/> class.
    /// </summary>
    /// <param name="span">The XML element that describes the span.</param>
    public Span(XmlElement span)
    {
      color = new HighlightColor(span);

      if (span.HasAttribute("rule"))
        rule = span.GetAttribute("rule");

      if (span.HasAttribute("escapecharacter"))
        escapeCharacter = span.GetAttribute("escapecharacter")[0];

      name = span.GetAttribute("name");
      if (span.HasAttribute("stopateol"))
        stopEOL = Boolean.Parse(span.GetAttribute("stopateol"));

      begin = span["Begin"].InnerText.ToCharArray();
      beginColor = new HighlightColor(span["Begin"], color);

      if (span["Begin"].HasAttribute("singleword"))
        isBeginSingleWord = Boolean.Parse(span["Begin"].GetAttribute("singleword"));

      if (span["Begin"].HasAttribute("startofline"))
        isBeginStartOfLine = Boolean.Parse(span["Begin"].GetAttribute("startofline"));

      if (span["End"] != null)
      {
        end = span["End"].InnerText.ToCharArray();
        endColor = new HighlightColor(span["End"], color);
        if (span["End"].HasAttribute("singleword"))
          isEndSingleWord = Boolean.Parse(span["End"].GetAttribute("singleword"));
      }
    }
  }
}
