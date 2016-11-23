namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Factory for creating syntax highlighting strategies.
  /// </summary>
  public class HighlightingStrategyFactory
  {
    /// <summary>
    /// Creates the default syntax highlighting strategy.
    /// </summary>
    /// <returns>The default syntax highlighting strategy</returns>
    public static IHighlightingStrategy CreateHighlightingStrategy()
    {
      return (IHighlightingStrategy) HighlightingManager.Manager.HighlightingDefinitions["Default"];
    }


    /// <summary>
    /// Creates the syntax highlighting strategy.
    /// </summary>
    /// <param name="name">The name of the syntax highlighting strategy.</param>
    /// <returns>The syntax highlighting strategy</returns>
    public static IHighlightingStrategy CreateHighlightingStrategy(string name)
    {
      IHighlightingStrategy highlightingStrategy = HighlightingManager.Manager.FindHighlighter(name);

      if (highlightingStrategy == null)
        return CreateHighlightingStrategy();

      return highlightingStrategy;
    }


    /// <summary>
    /// Creates the syntax highlighting strategy for a file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>The syntax highlighting strategy.</returns>
    public static IHighlightingStrategy CreateHighlightingStrategyForFile(string fileName)
    {
      IHighlightingStrategy highlightingStrategy = HighlightingManager.Manager.FindHighlighterForFile(fileName);
      if (highlightingStrategy == null)
        return CreateHighlightingStrategy();
      
      return highlightingStrategy;
    }
  }
}
