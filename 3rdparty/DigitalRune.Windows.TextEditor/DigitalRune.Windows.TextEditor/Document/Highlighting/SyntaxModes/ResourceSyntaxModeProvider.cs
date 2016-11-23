using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Provides that syntax highlighting defintions which are stored as resources
  /// in this assembly.
  /// </summary>
  public class ResourceSyntaxModeProvider : ISyntaxModeFileProvider
  {
    private readonly List<SyntaxMode> _syntaxModes;


    /// <summary>
    /// Gets the provided syntax highlighting modes.
    /// </summary>
    /// <value>The syntax highlighting modes.</value>
    public ICollection<SyntaxMode> SyntaxModes
    {
      get { return _syntaxModes; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ResourceSyntaxModeProvider"/> class.
    /// </summary>
    public ResourceSyntaxModeProvider()
    {
      Assembly assembly = typeof(SyntaxMode).Assembly;
      Stream syntaxModeStream = assembly.GetManifestResourceStream("DigitalRune.Windows.TextEditor.Resources.SyntaxModes.xml");
      _syntaxModes = (syntaxModeStream != null) ? SyntaxMode.GetSyntaxModes(syntaxModeStream) : new List<SyntaxMode>();
    }


    /// <summary>
    /// Gets the syntax highlighting definition for a certain syntax.
    /// </summary>
    /// <param name="syntaxMode">The syntax.</param>
    /// <returns>The syntax highlighting definition.</returns>
    public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
    {
      Assembly assembly = typeof(SyntaxMode).Assembly;
      // ReSharper disable AssignNullToNotNullAttribute
      return new XmlTextReader(assembly.GetManifestResourceStream("DigitalRune.Windows.TextEditor.Resources." + syntaxMode.FileName));
      // ReSharper restore AssignNullToNotNullAttribute
    }


    /// <summary>
    /// Updates the list of syntax highlighting modes.
    /// </summary>
    /// <remarks>
    /// Has no effect in this case, because the resources cannot change during
    /// runtime.
    /// </remarks>
    public void UpdateSyntaxModeList()
    {
      // resources don't change during runtime
    }
  }
}
