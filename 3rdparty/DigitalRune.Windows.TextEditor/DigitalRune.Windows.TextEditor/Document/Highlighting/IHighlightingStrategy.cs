using System.Collections.Generic;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// A highlighting strategy for a buffer.
  /// </summary>
  public interface IHighlightingStrategy
  {
    /// <summary>
    /// Gets the name of the highlighting strategy (must be unique).
    /// </summary>
    /// <value>The name of the highlighting strategy (must be unique).</value>
    string Name { get; }


    /// <summary>
    /// Gets the file extensions on which this highlighting strategy gets used.
    /// </summary>
    /// <value>
    /// The file extensions on which this highlighting strategy gets used.
    /// </value>
    string[] Extensions { get; }


    /// <summary>
    /// Gets the properties.
    /// </summary>
    /// <value>The properties.</value>
    Dictionary<string, string> Properties { get; }


    /// <summary>
    /// Gets the color of an environment element.
    /// </summary>
    /// <param name="name">The name of the element.</param>
    /// <returns>The color of an environment element.</returns>
    HighlightColor GetColorFor(string name);


    /// <summary>
    /// Marks the tokens. Used internally, do not call.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="lines">The lines.</param>
    void MarkTokens(IDocument document, List<LineSegment> lines);


    /// <summary>
    /// Marks the tokens. Used internally, do not call.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <remarks>
    /// Used internally, do not call.
    /// </remarks>
    void MarkTokens(IDocument document);
  }


  /// <summary>
  /// A highlighting strategy that is based on rule sets.
  /// </summary>
  public interface IHighlightingStrategyUsingRuleSets : IHighlightingStrategy
  {
    /// <summary>
    /// Gets the rule set for the given span.
    /// </summary>
    /// <param name="span">The span.</param>
    /// <returns>The rule set.</returns>
    /// <remarks>
    /// Used internally, do not call
    /// </remarks>
    HighlightRuleSet GetRuleSet(Span span);


    /// <summary>
    /// Gets the highlighting color for the specified text.
    /// </summary>
    /// <param name="document">The document.</param>
    /// <param name="line">The line.</param>
    /// <param name="index">The index.</param>
    /// <param name="length">The length.</param>
    /// <returns>The highlighting color for the specified text.</returns>
    /// <remarks>
    /// Used internally, do not call
    /// </remarks>
    HighlightColor GetColor(IDocument document, LineSegment line, int index, int length);
  }
}
