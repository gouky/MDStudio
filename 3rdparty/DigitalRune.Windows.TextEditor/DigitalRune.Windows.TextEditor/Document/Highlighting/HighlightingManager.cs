using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Manages syntax highlighting.
  /// </summary>
  public class HighlightingManager
  {
    private static readonly HighlightingManager _highlightingManager;
    private readonly List<ISyntaxModeFileProvider> _syntaxModeFileProviders = new List<ISyntaxModeFileProvider>();
    private readonly Hashtable _highlightingDefinitions = new Hashtable();
    private readonly Hashtable _extensionsToName = new Hashtable();


    /// <summary>
    /// Occurs when syntax highlighting definitions are reloaded.
    /// </summary>
    public event EventHandler SyntaxHighlightingReloaded;


    /// <summary>
    /// Gets the syntax highlighting definitions.
    /// </summary>
    /// <value>The syntax highlighting definitions.</value>
    /// <remarks>
    /// This is a hash table from extension name to highlighting definition, 
    /// OR from extension name to pair SyntaxMode,ISyntaxModeFileProvider
    /// </remarks>
    public Hashtable HighlightingDefinitions
    {
      get { return _highlightingDefinitions; }
    }


    /// <summary>
    /// Gets the default highlighting strategy.
    /// </summary>
    /// <value>The default highlighting strategy.</value>
    public DefaultHighlightingStrategy DefaultHighlighting
    {
      get { return (DefaultHighlightingStrategy) _highlightingDefinitions["Default"]; }
    }


    /// <summary>
    /// Gets the manager (singleton).
    /// </summary>
    /// <value>The manager.</value>
    public static HighlightingManager Manager
    {
      get { return _highlightingManager; }
    }


    static HighlightingManager()
    {
      _highlightingManager = new HighlightingManager();
      _highlightingManager.AddSyntaxModeFileProvider(new ResourceSyntaxModeProvider());
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="HighlightingManager"/> class.
    /// </summary>
    public HighlightingManager()
    {
      CreateDefaultHighlightingStrategy();
    }


    /// <summary>
    /// Adds the syntax mode provider.
    /// </summary>
    /// <param name="syntaxModeFileProvider">The syntax mode file provider.</param>
    public void AddSyntaxModeFileProvider(ISyntaxModeFileProvider syntaxModeFileProvider)
    {
      foreach (SyntaxMode syntaxMode in syntaxModeFileProvider.SyntaxModes)
      {
        _highlightingDefinitions[syntaxMode.Name] = new DictionaryEntry(syntaxMode, syntaxModeFileProvider);
        foreach (string extension in syntaxMode.Extensions)
          _extensionsToName[extension.ToUpperInvariant()] = syntaxMode.Name;
      }

      if (!_syntaxModeFileProviders.Contains(syntaxModeFileProvider))
        _syntaxModeFileProviders.Add(syntaxModeFileProvider);
    }


    /// <summary>
    /// Adds the highlighting strategy.
    /// </summary>
    /// <param name="highlightingStrategy">The highlighting strategy.</param>
    public void AddHighlightingStrategy(IHighlightingStrategy highlightingStrategy)
    {
      _highlightingDefinitions[highlightingStrategy.Name] = highlightingStrategy;
      foreach (string extension in highlightingStrategy.Extensions)
      {
        _extensionsToName[extension.ToUpperInvariant()] = highlightingStrategy.Name;
      }
    }


    /// <summary>
    /// Reloads the syntax modes.
    /// </summary>
    public void ReloadSyntaxModes()
    {
      _highlightingDefinitions.Clear();
      _extensionsToName.Clear();
      CreateDefaultHighlightingStrategy();
      foreach (ISyntaxModeFileProvider provider in _syntaxModeFileProviders)
      {
        provider.UpdateSyntaxModeList();
        AddSyntaxModeFileProvider(provider);
      }
      OnSyntaxHighlightingReloaded(EventArgs.Empty);
    }


    void CreateDefaultHighlightingStrategy()
    {
      DefaultHighlightingStrategy defaultHighlightingStrategy = new DefaultHighlightingStrategy();
      defaultHighlightingStrategy.Extensions = new string[] { };
      defaultHighlightingStrategy.Rules.Add(new HighlightRuleSet());
      _highlightingDefinitions["Default"] = defaultHighlightingStrategy;
    }


    IHighlightingStrategy LoadDefinition(DictionaryEntry entry)
    {
      SyntaxMode syntaxMode = (SyntaxMode) entry.Key;
      ISyntaxModeFileProvider syntaxModeFileProvider = (ISyntaxModeFileProvider) entry.Value;

			DefaultHighlightingStrategy highlightingStrategy = null;
      try
      {
        XmlTextReader reader = syntaxModeFileProvider.GetSyntaxModeFile(syntaxMode);
        if (reader == null)
          throw new HighlightingDefinitionInvalidException("Could not get syntax mode file for " + syntaxMode.Name);
        highlightingStrategy = HighlightingDefinitionParser.Parse(syntaxMode, reader);
        if (highlightingStrategy.Name != syntaxMode.Name)
        {
          throw new HighlightingDefinitionInvalidException("The name specified in the .xshd '" + highlightingStrategy.Name + "' must be equal the syntax mode name '" + syntaxMode.Name + "'");
        }
      }
      finally
      {
        if (highlightingStrategy == null)
        {
          highlightingStrategy = DefaultHighlighting;
        }
        _highlightingDefinitions[syntaxMode.Name] = highlightingStrategy;
        highlightingStrategy.ResolveReferences();
      }
      return highlightingStrategy;
    }


    internal KeyValuePair<SyntaxMode, ISyntaxModeFileProvider> FindHighlighterEntry(string name)
    {
      foreach (ISyntaxModeFileProvider provider in _syntaxModeFileProviders)
      {
        foreach (SyntaxMode mode in provider.SyntaxModes)
        {
          if (mode.Name == name)
          {
            return new KeyValuePair<SyntaxMode, ISyntaxModeFileProvider>(mode, provider);
          }
        }
      }
      return new KeyValuePair<SyntaxMode, ISyntaxModeFileProvider>(null, null);
    }


    /// <summary>
    /// Finds the syntax highlighting strategy.
    /// </summary>
    /// <param name="name">The name of the syntax highlighting strategy.</param>
    /// <returns>The syntax highlighting strategy.</returns>
    public IHighlightingStrategy FindHighlighter(string name)
    {
      object def = _highlightingDefinitions[name];
      if (def is DictionaryEntry)
        return LoadDefinition((DictionaryEntry) def);

      return def == null ? DefaultHighlighting : (IHighlightingStrategy) def;
    }


    /// <summary>
    /// Finds the syntax highlighting strategy for file.
    /// </summary>
    /// <param name="fileName">Name of the file.</param>
    /// <returns>The syntax highlighting strategy.</returns>
    public IHighlightingStrategy FindHighlighterForFile(string fileName)
    {
      string highlighterName = (string) _extensionsToName[Path.GetExtension(fileName).ToUpperInvariant()];
      if (highlighterName != null)
      {
        object def = _highlightingDefinitions[highlighterName];
        if (def is DictionaryEntry)
          return LoadDefinition((DictionaryEntry) def);

        return def == null ? DefaultHighlighting : (IHighlightingStrategy) def;
      }
      else
      {
        return DefaultHighlighting;
      }
    }


    /// <summary>
    /// Raises the <see cref="SyntaxHighlightingReloaded"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnSyntaxHighlightingReloaded(EventArgs e)
    {
      if (SyntaxHighlightingReloaded != null)
      {
        SyntaxHighlightingReloaded(this, e);
      }
    }
  }
}
