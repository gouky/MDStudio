using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// The default highlighting strategy.
  /// </summary>
  public class DefaultHighlightingStrategy : IHighlightingStrategyUsingRuleSets
  {
    private string _name;
    private List<HighlightRuleSet> _rules = new List<HighlightRuleSet>();
    private Dictionary<string, HighlightColor> _environmentColors = new Dictionary<string, HighlightColor>();
    private Dictionary<string, string> _properties = new Dictionary<string, string>();
    private string[] _extensions;
    private HighlightColor _digitColor;
    private HighlightRuleSet _defaultRuleSet;
    private HighlightColor _defaultTextColor;

    #region ----- Line state variables -----
    /// <summary>The current line.</summary>
    private LineSegment _currentLine;

    /// <summary>Span stack state variable.</summary>
    private SpanStack _currentSpanStack;
    #endregion

    #region ----- Span state variables -----
    private bool _inSpan;
    private Span _activeSpan;
    private HighlightRuleSet _activeRuleSet;
    #endregion

    #region ----- Line scanning state variables -----
    private int _currentOffset;
    private int _currentLength;
    #endregion


    /// <summary>
    /// Gets or sets the file extensions on which this highlighting strategy gets
    /// used.
    /// </summary>
    /// <value>
    /// The file extensions on which this highlighting strategy gets used.
    /// </value>
    public string[] Extensions
    {
      set { _extensions = value; }
      get { return _extensions; }
    }


    /// <summary>
    /// Gets or sets the color of digits.
    /// </summary>
    /// <value>The color of digits.</value>
    public HighlightColor DigitColor
    {
      get { return _digitColor; }
      set { _digitColor = value; }
    }


    /// <summary>
    /// Gets the environment colors.
    /// </summary>
    /// <value>The environment colors.</value>
    public IEnumerable<KeyValuePair<string, HighlightColor>> EnvironmentColors
    {
      get { return _environmentColors; }
    }


    /// <summary>
    /// Imports the settings from another highlighting strategy.
    /// </summary>
    /// <param name="source">The source.</param>
    protected void ImportSettingsFrom(DefaultHighlightingStrategy source)
    {
      if (source == null)
        throw new ArgumentNullException("source");

      _properties = source._properties;
      _extensions = source._extensions;
      _digitColor = source._digitColor;
      _defaultRuleSet = source._defaultRuleSet;
      _name = source._name;
      _rules = source._rules;
      _environmentColors = source._environmentColors;
      _defaultTextColor = source._defaultTextColor;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHighlightingStrategy"/> class.
    /// </summary>
    public DefaultHighlightingStrategy()
      : this("Default")
    {
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultHighlightingStrategy"/> class.
    /// </summary>
    /// <param name="name">The name.</param>
    public DefaultHighlightingStrategy(string name)
    {
      _name = name;

      _digitColor = new HighlightColor(SystemColors.WindowText, false, false);
      _defaultTextColor = new HighlightColor(SystemColors.WindowText, false, false);

      Color inactiveSelectionForeground = BlendColors(SystemColors.HighlightText, SystemColors.Highlight, 0.2f);
      Color inactiveSelectionBackground = BlendColors(SystemColors.Highlight, SystemColors.Window, 0.4f);

      // set small 'default color environment'
      _environmentColors["Default"] = new HighlightBackground("WindowText", "Window", false, false);
      _environmentColors["Selection"] = new HighlightColor("HighlightText", "Highlight", false, false);
      _environmentColors["SelectionInactive"] = new HighlightColor(inactiveSelectionForeground, inactiveSelectionBackground, false, false);
      _environmentColors["VRuler"] = new HighlightColor("ControlLight", "Window", false, false);
      _environmentColors["InvalidLines"] = new HighlightColor(Color.Red, false, false);
      _environmentColors["CaretMarker"] = new HighlightColor(Color.FromArgb(0xff, 0xff, 0xcc), false, false);
      _environmentColors["CaretLine"] = new HighlightBackground("ControlLight", "Window", false, false);
      _environmentColors["LineNumbers"] = new HighlightBackground("ControlDark", "Window", false, false);
      _environmentColors["FoldLine"] = new HighlightColor("ControlDark", false, false);
      _environmentColors["FoldMarker"] = new HighlightColor("WindowText", "Window", false, false);
      _environmentColors["SelectedFoldLine"] = new HighlightColor("WindowText", false, false);
      _environmentColors["EOLMarkers"] = new HighlightColor("ControlLight", "Window", false, false);
      _environmentColors["SpaceMarkers"] = new HighlightColor("ControlLight", "Window", false, false);
      _environmentColors["TabMarkers"] = new HighlightColor("ControlLight", "Window", false, false);
    }


    private static Color BlendColors(Color src, Color dest, float blendFactor)
    {
      float red = src.R + blendFactor * (dest.R - src.R);
      float green = src.G + blendFactor * (dest.G - src.G);
      float blue = src.B + blendFactor * (dest.B - src.B);
      float alpha = src.A + blendFactor * (dest.A - src.A);
      return Color.FromArgb((int) alpha, (int) red, (int) green, (int) blue);
    }


    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <value>The properties.</value>
    public Dictionary<string, string> Properties
    {
      get { return _properties; }
    }


    /// <summary>
    /// Gets the name of the highlighting strategy (must be unique).
    /// </summary>
    /// <value>The name of the highlighting strategy (must be unique).</value>
    public string Name
    {
      get { return _name; }
    }


    /// <summary>
    /// Gets the syntax highlighting rules.
    /// </summary>
    /// <value>The syntax highlighting rules.</value>
    public List<HighlightRuleSet> Rules
    {
      get { return _rules; }
    }


    /// <summary>
    /// Finds the highlighting rule set.
    /// </summary>
    /// <param name="name">The name of the rule set.</param>
    /// <returns>The rule set.</returns>
    public HighlightRuleSet FindHighlightRuleSet(string name)
    {
      foreach (HighlightRuleSet ruleSet in _rules)
        if (ruleSet.Name == name)
          return ruleSet;

      return null;
    }


    /// <summary>
    /// Adds the rule set.
    /// </summary>
    /// <param name="ruleSet">A rule set.</param>
    public void AddRuleSet(HighlightRuleSet ruleSet)
    {
      HighlightRuleSet existing = FindHighlightRuleSet(ruleSet.Name);
      if (existing != null)
        existing.MergeFrom(ruleSet);
      else
        _rules.Add(ruleSet);
    }


    /// <summary>
    /// Resolves the references.
    /// </summary>
    public void ResolveReferences()
    {
      // Resolve references from Span definitions to RuleSets
      ResolveRuleSetReferences();

      // Resolve references from RuleSet definitions to Highlighters defined in an external mode file
      ResolveExternalReferences();
    }


    void ResolveRuleSetReferences()
    {
      foreach (HighlightRuleSet ruleSet in Rules)
      {
        if (ruleSet.Name == null)
          _defaultRuleSet = ruleSet;

        foreach (Span aSpan in ruleSet.Spans)
        {
          if (aSpan.Rule != null)
          {
            bool found = false;
            foreach (HighlightRuleSet refSet in Rules)
            {
              if (refSet.Name == aSpan.Rule)
              {
                found = true;
                aSpan.RuleSet = refSet;
                break;
              }
            }
            if (!found)
            {
              aSpan.RuleSet = null;
              throw new HighlightingDefinitionInvalidException("The RuleSet " + aSpan.Rule + " could not be found in mode definition " + Name);
            }
          }
          else
          {
            aSpan.RuleSet = null;
          }
        }
      }

      if (_defaultRuleSet == null)
        throw new HighlightingDefinitionInvalidException("No default RuleSet is defined for mode definition " + Name);
    }


    void ResolveExternalReferences()
    {
      foreach (HighlightRuleSet ruleSet in Rules)
      {
        ruleSet.Highlighter = this;
        if (ruleSet.Reference != null)
        {
          IHighlightingStrategy highlighter = HighlightingManager.Manager.FindHighlighter(ruleSet.Reference);

          if (highlighter == null)
            throw new HighlightingDefinitionInvalidException("The mode defintion " + ruleSet.Reference + " which is refered from the " + Name + " mode definition could not be found");
          if (highlighter is IHighlightingStrategyUsingRuleSets)
            ruleSet.Highlighter = (IHighlightingStrategyUsingRuleSets) highlighter;
          else
            throw new HighlightingDefinitionInvalidException("The mode defintion " + ruleSet.Reference + " which is refered from the " + Name + " mode definition does not implement IHighlightingStrategyUsingRuleSets");
        }
      }
    }


    /// <summary>
    /// Gets the default color for text.
    /// </summary>
    /// <value>The default color for text.</value>
    public HighlightColor DefaultTextColor
    {
      get { return _defaultTextColor; }
    }


    /// <summary>
    /// Sets the color for an environment element.
    /// </summary>
    /// <param name="name">The name of the environment element.</param>
    /// <param name="color">The highlighting color.</param>
    public void SetColorFor(string name, HighlightColor color)
    {
      if (name == "Default")
        _defaultTextColor = new HighlightColor(color.Color, color.Bold, color.Italic);

      _environmentColors[name] = color;
    }


    /// <summary>
    /// Gets the color of an environment element.
    /// </summary>
    /// <param name="name">The name of the element.</param>
    /// <returns>The color of an environment element.</returns>
    public HighlightColor GetColorFor(string name)
    {
      HighlightColor color;
      if (_environmentColors.TryGetValue(name, out color))
        return color;
      else
        return _defaultTextColor;
    }


    /// <summary>
    /// Gets the highlighting color for a certain position in the document.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="currentSegment">The current segment.</param>
    /// <param name="currentOffset">The current offset.</param>
    /// <param name="currentLength">Length of the current.</param>
    /// <returns>The highlighting color.</returns>
    public HighlightColor GetColor(IDocument document, LineSegment currentSegment, int currentOffset, int currentLength)
    {
      return GetColor(_defaultRuleSet, document, currentSegment, currentOffset, currentLength);
    }


    /// <summary>
    /// Gets the highlighting color for a certain position in the document.
    /// </summary>
    /// <param name="ruleSet">The rule set.</param>
    /// <param name="document">The document.</param>
    /// <param name="currentSegment">The current segment.</param>
    /// <param name="currentOffset">The current offset.</param>
    /// <param name="currentLength">Length of the current.</param>
    /// <returns></returns>
    protected virtual HighlightColor GetColor(HighlightRuleSet ruleSet, IDocument document, LineSegment currentSegment, int currentOffset, int currentLength)
    {
      if (ruleSet != null)
      {
        if (ruleSet.Reference != null)
          return ruleSet.Highlighter.GetColor(document, currentSegment, currentOffset, currentLength);
        else
          return (HighlightColor) ruleSet.KeyWords[document, currentSegment, currentOffset, currentLength];
      }
      return null;
    }


    /// <summary>
    /// Gets the rule set for a span.
    /// </summary>
    /// <param name="span">The span.</param>
    /// <returns>The rule set.</returns>
    public HighlightRuleSet GetRuleSet(Span span)
    {
      if (span == null)
      {
        return _defaultRuleSet;
      }
      else
      {
        if (span.RuleSet != null)
        {
          if (span.RuleSet.Reference != null)
            return span.RuleSet.Highlighter.GetRuleSet(null);
          else
            return span.RuleSet;
        }
        else
        {
          return null;
        }
      }
    }


    /// <summary>
    /// Marks the tokens. Used internally, do not call.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <remarks>
    /// Used internally, do not call.
    /// </remarks>
    public virtual void MarkTokens(IDocument document)
    {
      if (Rules.Count == 0)
      {
        return;
      }

      int lineNumber = 0;

      while (lineNumber < document.TotalNumberOfLines)
      {
        LineSegment previousLine = (lineNumber > 0 ? document.GetLineSegment(lineNumber - 1) : null);
        if (lineNumber >= document.LineSegmentCollection.Count)
        { // may be, if the last line ends with a delimiter
          break;                                                // then the last line is not in the collection :)
        }

        _currentSpanStack = ((previousLine != null && previousLine.HighlightSpanStack != null) ? previousLine.HighlightSpanStack.Clone() : null);

        if (_currentSpanStack != null)
        {
          while (!_currentSpanStack.IsEmpty && _currentSpanStack.Peek().StopEOL)
          {
            _currentSpanStack.Pop();
          }
          if (_currentSpanStack.IsEmpty) _currentSpanStack = null;
        }

        _currentLine = document.LineSegmentCollection[lineNumber];

        if (_currentLine.Length == -1)
        { // happens when buffer is empty !
          return;
        }

        List<TextWord> words = ParseLine(document);
        // Alex: clear old words
        if (_currentLine.Words != null)
        {
          _currentLine.Words.Clear();
        }
        _currentLine.Words = words;
        _currentLine.HighlightSpanStack = (_currentSpanStack == null || _currentSpanStack.IsEmpty) ? null : _currentSpanStack;

        ++lineNumber;
      }
      document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
      document.CommitUpdate();
      _currentLine = null;
    }


    /// <summary>
    /// Marks the tokens in a line.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="spanChanged"><see langword="true"/> span has changed.</param>
    /// <returns><see langword="true"/> if the line has been parsed successfully and the next line can be parsed.</returns>
    bool MarkTokensInLine(IDocument document, int lineNumber, ref bool spanChanged)
    {
      bool processNextLine = false;
      LineSegment previousLine = (lineNumber > 0 ? document.GetLineSegment(lineNumber - 1) : null);

      _currentSpanStack = ((previousLine != null && previousLine.HighlightSpanStack != null) ? previousLine.HighlightSpanStack.Clone() : null);
      if (_currentSpanStack != null)
      {
        while (!_currentSpanStack.IsEmpty && _currentSpanStack.Peek().StopEOL)
        {
          _currentSpanStack.Pop();
        }
        if (_currentSpanStack.IsEmpty)
        {
          _currentSpanStack = null;
        }
      }

      _currentLine = document.LineSegmentCollection[lineNumber];

      if (_currentLine.Length == -1)
      { // happens when buffer is empty !
        return false;
      }

      List<TextWord> words = ParseLine(document);

      if (_currentSpanStack != null && _currentSpanStack.IsEmpty)
      {
        _currentSpanStack = null;
      }

      // Check if the span state has changed, if so we must re-render the next line
      // This check may seem utterly complicated but I didn't want to introduce any function calls
      // or allocations here for perfomance reasons.
      if (_currentLine.HighlightSpanStack != _currentSpanStack)
      {
        if (_currentLine.HighlightSpanStack == null)
        {
          processNextLine = false;
          foreach (Span sp in _currentSpanStack)
          {
            if (!sp.StopEOL)
            {
              spanChanged = true;
              processNextLine = true;
              break;
            }
          }
        }
        else if (_currentSpanStack == null)
        {
          processNextLine = false;
          foreach (Span sp in _currentLine.HighlightSpanStack)
          {
            if (!sp.StopEOL)
            {
              spanChanged = true;
              processNextLine = true;
              break;
            }
          }
        }
        else
        {
          SpanStack.Enumerator e1 = _currentSpanStack.GetEnumerator();
          SpanStack.Enumerator e2 = _currentLine.HighlightSpanStack.GetEnumerator();
          bool done = false;
          while (!done)
          {
            bool blockSpanIn1 = false;
            while (e1.MoveNext())
            {
              if (!e1.Current.StopEOL)
              {
                blockSpanIn1 = true;
                break;
              }
            }
            bool blockSpanIn2 = false;
            while (e2.MoveNext())
            {
              if (!e2.Current.StopEOL)
              {
                blockSpanIn2 = true;
                break;
              }
            }
            if (blockSpanIn1 || blockSpanIn2)
            {
              if (blockSpanIn1 && blockSpanIn2)
              {
                if (e1.Current != e2.Current)
                {
                  done = true;
                  processNextLine = true;
                  spanChanged = true;
                }
              }
              else
              {
                spanChanged = true;
                done = true;
                processNextLine = true;
              }
            }
            else
            {
              done = true;
              processNextLine = false;
            }
          }
        }
      }
      else
      {
        processNextLine = false;
      }

      // Alex: remove old words
      if (_currentLine.Words != null)
        _currentLine.Words.Clear();
      _currentLine.Words = words;
      _currentLine.HighlightSpanStack = (_currentSpanStack != null && !_currentSpanStack.IsEmpty) ? _currentSpanStack : null;

      return processNextLine;
    }


    /// <summary>
    /// Marks the tokens.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="inputLines">The input lines.</param>
    public virtual void MarkTokens(IDocument document, List<LineSegment> inputLines)
    {
      if (Rules.Count == 0)
        return;

      Dictionary<LineSegment, bool> processedLines = new Dictionary<LineSegment, bool>();

      bool spanChanged = false;
      int documentLineSegmentCount = document.LineSegmentCollection.Count;

      foreach (LineSegment lineToProcess in inputLines)
      {
        if (!processedLines.ContainsKey(lineToProcess))
        {
          int lineNumber = lineToProcess.LineNumber;
          bool processNextLine = true;

          if (lineNumber != -1)
          {
            while (processNextLine && lineNumber < documentLineSegmentCount)
            {
              processNextLine = MarkTokensInLine(document, lineNumber, ref spanChanged);
              processedLines[_currentLine] = true;
              ++lineNumber;
            }
          }
        }
      }

      if (spanChanged || inputLines.Count > 20)
      {
        // if the span was changed (more than inputLines lines had to be reevaluated)
        // or if there are many lines in inputLines, it's faster to update the whole
        // text area instead of many small segments
        document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
      }
      else
      {
        //				document.Caret.ValidateCaretPos();
        //				document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, document.GetLineNumberForOffset(document.Caret.Offset)));
        foreach (LineSegment lineToProcess in inputLines)
        {
          document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.SingleLine, lineToProcess.LineNumber));
        }

      }
      document.CommitUpdate();
      _currentLine = null;
    }


    void UpdateSpanStateVariables()
    {
      _inSpan = (_currentSpanStack != null && !_currentSpanStack.IsEmpty);
      _activeSpan = _inSpan ? _currentSpanStack.Peek() : null;
      _activeRuleSet = GetRuleSet(_activeSpan);
    }


    List<TextWord> ParseLine(IDocument document)
    {
      bool isShaderEffect = (_name == "HLSL" || _name == "Cg");
      List<TextWord> words = new List<TextWord>();
      HighlightColor markNext = null;

      _currentOffset = 0;
      _currentLength = 0;
      UpdateSpanStateVariables();

      int currentLineLength = _currentLine.Length;
      int currentLineOffset = _currentLine.Offset;

      for (int i = 0; i < currentLineLength; ++i)
      {
        char ch = document.GetCharAt(currentLineOffset + i);
        switch (ch)
        {
          case '\n':
          case '\r':
            PushCurWord(document, ref markNext, words);
            ++_currentOffset;
            break;
          case ' ':
            PushCurWord(document, ref markNext, words);
            if (_activeSpan != null && _activeSpan.Color.HasBackground)
            {
              words.Add(new TextWord.SpaceTextWord(_activeSpan.Color));
            }
            else
            {
              words.Add(TextWord.Space);
            }
            ++_currentOffset;
            break;
          case '\t':
            PushCurWord(document, ref markNext, words);
            if (_activeSpan != null && _activeSpan.Color.HasBackground)
            {
              words.Add(new TextWord.TabTextWord(_activeSpan.Color));
            }
            else
            {
              words.Add(TextWord.Tab);
            }
            ++_currentOffset;
            break;
          default:
            {
              // handle escape characters
              char escapeCharacter = '\0';
              if (_activeSpan != null && _activeSpan.EscapeCharacter != '\0')
              {
                escapeCharacter = _activeSpan.EscapeCharacter;
              }
              else if (_activeRuleSet != null)
              {
                escapeCharacter = _activeRuleSet.EscapeCharacter;
              }
              if (escapeCharacter != '\0' && escapeCharacter == ch)
              {
                // we found the escape character
                if (_activeSpan != null && _activeSpan.End != null && _activeSpan.End.Length == 1
                    && escapeCharacter == _activeSpan.End[0])
                {
                  // the escape character is a end-doubling escape character
                  // it may count as escape only when the next character is the escape, too
                  if (i + 1 < currentLineLength)
                  {
                    if (document.GetCharAt(currentLineOffset + i + 1) == escapeCharacter)
                    {
                      _currentLength += 2;
                      PushCurWord(document, ref markNext, words);
                      ++i;
                      continue;
                    }
                  }
                }
                else
                {
                  // this is a normal \-style escape
                  ++_currentLength;
                  if (i + 1 < currentLineLength)
                  {
                    ++_currentLength;
                  }
                  PushCurWord(document, ref markNext, words);
                  ++i;
                  continue;
                }
              }

              // highlight digits
              if ((!_inSpan || (_activeRuleSet != null && _activeRuleSet.HighlightDigits)) && (Char.IsDigit(ch) || (ch == '.' && i + 1 < currentLineLength && Char.IsDigit(document.GetCharAt(currentLineOffset + i + 1)))) && _currentLength == 0)
              {
                bool ishex = false;
                bool isfloatingpoint = false;

                if (ch == '0' && i + 1 < currentLineLength && Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1)) == 'X')
                { // hex digits
                  const string hex = "0123456789ABCDEF";
                  ++_currentLength;
                  ++i; // skip 'x'
                  ++_currentLength;
                  ishex = true;
                  while (i + 1 < currentLineLength && hex.IndexOf(Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1))) != -1)
                  {
                    ++i;
                    ++_currentLength;
                  }
                }
                else
                {
                  ++_currentLength;
                  while (i + 1 < currentLineLength && Char.IsDigit(document.GetCharAt(currentLineOffset + i + 1)))
                  {
                    ++i;
                    ++_currentLength;
                  }
                }
                if (!ishex && i + 1 < currentLineLength && document.GetCharAt(currentLineOffset + i + 1) == '.')
                {
                  isfloatingpoint = true;
                  ++i;
                  ++_currentLength;
                  while (i + 1 < currentLineLength && Char.IsDigit(document.GetCharAt(currentLineOffset + i + 1)))
                  {
                    ++i;
                    ++_currentLength;
                  }
                }

                if (i + 1 < currentLineLength && Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1)) == 'E')
                {
                  isfloatingpoint = true;
                  ++i;
                  ++_currentLength;
                  if (i + 1 < currentLineLength && (document.GetCharAt(currentLineOffset + i + 1) == '+' || document.GetCharAt(_currentLine.Offset + i + 1) == '-'))
                  {
                    ++i;
                    ++_currentLength;
                  }
                  while (i + 1 < _currentLine.Length && Char.IsDigit(document.GetCharAt(currentLineOffset + i + 1)))
                  {
                    ++i;
                    ++_currentLength;
                  }
                }

                if (i + 1 < _currentLine.Length)
                {
                  char nextch = Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1));
                  if (nextch == 'F' || nextch == 'M' || nextch == 'D')
                  {
                    isfloatingpoint = true;
                    ++i;
                    ++_currentLength;
                  }
                  else if (isShaderEffect && (nextch == 'H' || nextch == 'X'))  // Hardcoded path for HLSL suffixes
                  {
                    isfloatingpoint = true;
                    ++i;
                    ++_currentLength;
                  }
                }

                if (!isfloatingpoint)
                {
                  if (!isShaderEffect)
                  {
                    bool isunsigned = false;
                    if (i + 1 < currentLineLength && Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1)) == 'U')
                    {
                      ++i;
                      ++_currentLength;
                      isunsigned = true;
                    }
                    if (i + 1 < currentLineLength && Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1)) == 'L')
                    {
                      ++i;
                      ++_currentLength;
                      if (!isunsigned && i + 1 < currentLineLength && Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1)) == 'U')
                      {
                        ++i;
                        ++_currentLength;
                      }
                    }
                  }
                  else
                  {
                    // Hardcoded path for HLSL number formats
                    if (i + 1 < currentLineLength)
                    {
                      char nextCh = Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1));
                      if (nextCh == 'U')
                      {
                        ++i;
                        ++_currentLength;
                      }
                    }
                    if (i + 1 < currentLineLength)
                    {
                      char nextCh = Char.ToUpper(document.GetCharAt(currentLineOffset + i + 1));
                      if (nextCh == 'I' || nextCh == 'L' || nextCh == 'S' || nextCh == 'T')
                      {
                        ++i;
                        ++_currentLength;
                      }
                    }
                  }
                }

                words.Add(new TextWord(document, _currentLine, _currentOffset, _currentLength, DigitColor, false));
                _currentOffset += _currentLength;
                _currentLength = 0;
                continue;
              }

              // Check for SPAN ENDs
              if (_inSpan)
              {
                if (_activeSpan.End != null && _activeSpan.End.Length > 0)
                {
                  if (MatchExpr(_currentLine, _activeSpan.End, i, document, _activeSpan.IgnoreCase))
                  {
                    PushCurWord(document, ref markNext, words);
                    string regex = GetRegString(_currentLine, _activeSpan.End, i, document);
                    _currentLength += regex.Length;
                    words.Add(new TextWord(document, _currentLine, _currentOffset, _currentLength, _activeSpan.EndColor, false));
                    _currentOffset += _currentLength;
                    _currentLength = 0;
                    i += regex.Length - 1;
                    _currentSpanStack.Pop();
                    UpdateSpanStateVariables();
                    continue;
                  }
                }
              }

              // check for SPAN BEGIN
              if (_activeRuleSet != null)
              {
                foreach (Span span in _activeRuleSet.Spans)
                {
                  if ((!span.IsBeginSingleWord || _currentLength == 0)
                      && (!span.IsBeginStartOfLine.HasValue || span.IsBeginStartOfLine.Value == (_currentLength == 0 && words.TrueForAll(delegate(TextWord textWord) { return textWord.Type != TextWordType.Word; })))
                      && MatchExpr(_currentLine, span.Begin, i, document, _activeRuleSet.IgnoreCase, span.IsBeginSingleWord))
                  {
                    PushCurWord(document, ref markNext, words);
                    string regex = GetRegString(_currentLine, span.Begin, i, document);

                    if (!OverrideSpan(regex, document, words, span, ref i))
                    {
                      _currentLength += regex.Length;
                      words.Add(new TextWord(document, _currentLine, _currentOffset, _currentLength, span.BeginColor, false));
                      _currentOffset += _currentLength;
                      _currentLength = 0;

                      i += regex.Length - 1;
                      if (_currentSpanStack == null)
                      {
                        _currentSpanStack = new SpanStack();
                      }
                      _currentSpanStack.Push(span);
                      span.IgnoreCase = _activeRuleSet.IgnoreCase;

                      UpdateSpanStateVariables();
                    }

                    goto skip;
                  }
                }
              }

              // check if the char is a delimiter
              if (_activeRuleSet != null && ch < 256 && _activeRuleSet.Delimiters[ch])
              {
                PushCurWord(document, ref markNext, words);
                if (_currentOffset + _currentLength + 1 < _currentLine.Length)
                {
                  ++_currentLength;
                  PushCurWord(document, ref markNext, words);
                  goto skip;
                }
              }

              ++_currentLength;
            skip: continue;
            }
        }
      }

      PushCurWord(document, ref markNext, words);
      OnParsedLine(document, _currentLine, words);
      return words;
    }


    /// <summary>
    /// Called when a line has been parsed.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="currentLine">The current line.</param>
    /// <param name="words">The words.</param>
    protected virtual void OnParsedLine(IDocument document, LineSegment currentLine, List<TextWord> words)
    {
    }


    /// <summary>
    /// Override this message to change the parsing of spans.
    /// </summary>
    /// <param name="spanBegin">The span begin.</param>
    /// <param name="document">The document.</param>
    /// <param name="words">The words.</param>
    /// <param name="span">The span.</param>
    /// <param name="lineOffset">The line offset.</param>
    /// <returns>
    /// <see langword="true"/> if this method has parsed the span (customized parsing).
    /// <see langword="false"/> to parse the span as normal.
    /// </returns>
    protected virtual bool OverrideSpan(string spanBegin, IDocument document, List<TextWord> words, Span span, ref int lineOffset)
    {
      return false;
    }


    /// <summary>
    /// pushes the curWord string on the word list, with the
    /// correct color.
    /// </summary>
    void PushCurWord(IDocument document, ref HighlightColor markNext, List<TextWord> words)
    {
      // Svante Lidman : Need to look through the next prev logic.
      if (_currentLength > 0)
      {
        if (words.Count > 0 && _activeRuleSet != null)
        {
          TextWord prevWord;
          int pInd = words.Count - 1;
          while (pInd >= 0)
          {
            if (!words[pInd].IsWhiteSpace)
            {
              prevWord = words[pInd];
              if (prevWord.HasDefaultColor)
              {
                PrevMarker marker = (PrevMarker) _activeRuleSet.PrevMarkers[document, _currentLine, _currentOffset, _currentLength];
                if (marker != null)
                {
                  prevWord.SyntaxColor = marker.Color;
                }
              }
              break;
            }
            pInd--;
          }
        }

        if (_inSpan)
        {
          HighlightColor highlightColor;
          bool hasDefaultColor = true;
          if (_activeSpan.Rule == null)
          {
            highlightColor = _activeSpan.Color;
          }
          else
          {
            highlightColor = GetColor(_activeRuleSet, document, _currentLine, _currentOffset, _currentLength);
            hasDefaultColor = false;
          }

          if (highlightColor == null)
          {
            highlightColor = _activeSpan.Color;
            if (highlightColor.Color == Color.Transparent)
              highlightColor = DefaultTextColor;

            hasDefaultColor = true;
          }
          words.Add(new TextWord(document, _currentLine, _currentOffset, _currentLength, markNext ?? highlightColor, hasDefaultColor));
        }
        else
        {
          HighlightColor c = markNext ?? GetColor(_activeRuleSet, document, _currentLine, _currentOffset, _currentLength);
          if (c == null)
            words.Add(new TextWord(document, _currentLine, _currentOffset, _currentLength, DefaultTextColor, true));
          else
            words.Add(new TextWord(document, _currentLine, _currentOffset, _currentLength, c, false));
        }

        if (_activeRuleSet != null)
        {
          NextMarker nextMarker = (NextMarker) _activeRuleSet.NextMarkers[document, _currentLine, _currentOffset, _currentLength];
          if (nextMarker != null)
          {
            if (nextMarker.MarkMarker && words.Count > 0)
            {
              TextWord prevword = words[words.Count - 1];
              prevword.SyntaxColor = nextMarker.Color;
            }
            markNext = nextMarker.Color;
          }
          else
          {
            markNext = null;
          }
        }
        _currentOffset += _currentLength;
        _currentLength = 0;
      }
    }


    #region Matching
    /// <summary>
    /// get the string, which matches the regular expression expr,
    /// in string s2 at index
    /// </summary>
    static string GetRegString(LineSegment lineSegment, char[] expr, int index, IDocument document)
    {
      int j = 0;
      StringBuilder regexpr = new StringBuilder();

      for (int i = 0; i < expr.Length; ++i, ++j)
      {
        if (index + j >= lineSegment.Length)
          break;

        switch (expr[i])
        {
          case '@': // "special" meaning
            ++i;
            if (i == expr.Length)
              throw new HighlightingDefinitionInvalidException("Unexpected end of @ sequence, use @@ to look for a single @.");
            switch (expr[i])
            {
              case '!': // don't match the following expression
                StringBuilder whatmatch = new StringBuilder();
                ++i;
                while (i < expr.Length && expr[i] != '@')
                {
                  whatmatch.Append(expr[i++]);
                }
                break;
              case '@': // matches @
                regexpr.Append(document.GetCharAt(lineSegment.Offset + index + j));
                break;
            }
            break;
          default:
            if (expr[i] != document.GetCharAt(lineSegment.Offset + index + j))
            {
              return regexpr.ToString();
            }
            regexpr.Append(document.GetCharAt(lineSegment.Offset + index + j));
            break;
        }
      }
      return regexpr.ToString();
    }


    /// <summary>
    /// returns true, if the get the string s2 at index matches the expression expr
    /// </summary>
    static bool MatchExpr(LineSegment lineSegment, char[] expr, int index, IDocument document, bool ignoreCase)
    {
      return MatchExpr(lineSegment, expr, index, document, ignoreCase, false);
    }


    /// <summary>
    /// returns true, if the get the string s2 at index matches the expression expr
    /// </summary>
    static bool MatchExpr(LineSegment lineSegment, char[] expr, int index, IDocument document, bool ignoreCase, bool isSingleWord)
    {
      for (int i = 0, j = 0; i < expr.Length; ++i, ++j)
      {
        switch (expr[i])
        {
          case '@': // "special" meaning
            ++i;
            if (i == expr.Length)
              throw new HighlightingDefinitionInvalidException("Unexpected end of @ sequence, use @@ to look for a single @.");
            switch (expr[i])
            {
              case 'C': // match whitespace or punctuation
                if (index + j == lineSegment.Offset || index + j >= lineSegment.Offset + lineSegment.Length)
                {
                  // nothing (EOL or SOL)
                }
                else
                {
                  char ch = document.GetCharAt(lineSegment.Offset + index + j);
                  if (!Char.IsWhiteSpace(ch) && !Char.IsPunctuation(ch))
                  {
                    return false;
                  }
                }
                break;
              case '!': // don't match the following expression
                {
                  StringBuilder whatmatch = new StringBuilder();
                  ++i;
                  while (i < expr.Length && expr[i] != '@')
                  {
                    whatmatch.Append(expr[i++]);
                  }
                  if (lineSegment.Offset + index + j + whatmatch.Length < document.TextLength)
                  {
                    int k = 0;
                    for (; k < whatmatch.Length; ++k)
                    {
                      char docChar = ignoreCase ? Char.ToUpperInvariant(document.GetCharAt(lineSegment.Offset + index + j + k)) : document.GetCharAt(lineSegment.Offset + index + j + k);
                      char spanChar = ignoreCase ? Char.ToUpperInvariant(whatmatch[k]) : whatmatch[k];
                      if (docChar != spanChar)
                      {
                        break;
                      }
                    }
                    if (k >= whatmatch.Length)
                    {
                      return false;
                    }
                  }
                  //									--j;
                  break;
                }
              case '-': // don't match the  expression before
                {
                  StringBuilder whatmatch = new StringBuilder();
                  ++i;
                  while (i < expr.Length && expr[i] != '@')
                  {
                    whatmatch.Append(expr[i++]);
                  }
                  if (index - whatmatch.Length >= 0)
                  {
                    int k = 0;
                    for (; k < whatmatch.Length; ++k)
                    {
                      char docChar = ignoreCase ? Char.ToUpperInvariant(document.GetCharAt(lineSegment.Offset + index - whatmatch.Length + k)) : document.GetCharAt(lineSegment.Offset + index - whatmatch.Length + k);
                      char spanChar = ignoreCase ? Char.ToUpperInvariant(whatmatch[k]) : whatmatch[k];
                      if (docChar != spanChar)
                        break;
                    }
                    if (k >= whatmatch.Length)
                    {
                      return false;
                    }
                  }
                  //									--j;
                  break;
                }
              case '@': // matches @
                if (index + j >= lineSegment.Length || '@' != document.GetCharAt(lineSegment.Offset + index + j))
                {
                  return false;
                }
                break;
            }
            break;
          default:
            {
              if (index + j >= lineSegment.Length)
              {
                return false;
              }
              char docChar = ignoreCase ? Char.ToUpperInvariant(document.GetCharAt(lineSegment.Offset + index + j)) : document.GetCharAt(lineSegment.Offset + index + j);
              char spanChar = ignoreCase ? Char.ToUpperInvariant(expr[i]) : expr[i];
              if (docChar != spanChar)
              {
                return false;
              }
              break;
            }
        }
      }

      if (isSingleWord)
      {
        int length = expr.Length;
        if (index + length < lineSegment.Length)
        {
          char ch = document.GetCharAt(lineSegment.Offset + index + length);
          if (!Char.IsWhiteSpace(ch) && !Char.IsPunctuation(ch))
          {
            return false;
          }
        }
      }
      return true;
    }
    #endregion
  }
}
