using System.Collections.Generic;
using System.Xml;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Provides syntax highlighting definitions.
  /// </summary>
  public interface ISyntaxModeFileProvider
  {
    /// <summary>
    /// Gets the provided syntax highlighting modes.
    /// </summary>
    /// <value>The syntax highlighting modes.</value>
    ICollection<SyntaxMode> SyntaxModes { get; }


    /// <summary>
    /// Gets the syntax highlighting definition for a certain syntax.
    /// </summary>
    /// <param name="syntaxMode">The syntax.</param>
    /// <returns>The syntax highlighting definition.</returns>
    XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode);


    /// <summary>
    /// Updates the list of syntax highlighting modes.
    /// </summary>
    void UpdateSyntaxModeList();
  }
}
