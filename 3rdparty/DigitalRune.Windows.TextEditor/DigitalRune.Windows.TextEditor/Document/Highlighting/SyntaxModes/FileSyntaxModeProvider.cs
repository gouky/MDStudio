using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;


namespace DigitalRune.Windows.TextEditor.Highlighting
{
  /// <summary>
  /// Provides syntax highlighting definitions which are stored in a directory.
  /// </summary>
  /// <remarks>
  /// The directory needs to contain the syntax modes file (<c>"SyntaxModes.xml"</c>) 
  /// and a syntax highlighting definition for each syntax (<c>"*.xshd"</c>).
  /// </remarks>
  public class FileSyntaxModeProvider : ISyntaxModeFileProvider
  {
    private readonly string _directory;
    private List<SyntaxMode> _syntaxModes;


    /// <summary>
    /// Gets the provided syntax highlighting modes.
    /// </summary>
    /// <value>The syntax highlighting modes.</value>
    public ICollection<SyntaxMode> SyntaxModes
    {
      get { return _syntaxModes; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FileSyntaxModeProvider"/> class.
    /// </summary>
    /// <param name="directory">The directory.</param>
    public FileSyntaxModeProvider(string directory)
    {
      _directory = directory;
      UpdateSyntaxModeList();
    }


    /// <summary>
    /// Updates the list of syntax highlighting modes.
    /// </summary>
    public void UpdateSyntaxModeList()
    {
      string syntaxModeFile = Path.Combine(_directory, "SyntaxModes.xml");
      if (File.Exists(syntaxModeFile))
      {
        Stream s = File.OpenRead(syntaxModeFile);
        _syntaxModes = SyntaxMode.GetSyntaxModes(s);
        s.Close();
      }
      else
      {
        _syntaxModes = ScanDirectory(_directory);
      }
    }


    /// <summary>
    /// Gets the syntax highlighting definition for a certain syntax.
    /// </summary>
    /// <param name="syntaxMode">The syntax.</param>
    /// <returns>The syntax highlighting definition.</returns>
    public XmlTextReader GetSyntaxModeFile(SyntaxMode syntaxMode)
    {
      string syntaxModeFile = Path.Combine(_directory, syntaxMode.FileName);
      if (!File.Exists(syntaxModeFile))
        throw new HighlightingDefinitionInvalidException("Can't load highlighting definition " + syntaxModeFile + " (file not found)!");

      return new XmlTextReader(File.OpenRead(syntaxModeFile));
    }


    static List<SyntaxMode> ScanDirectory(string directory)
    {
      string[] files = Directory.GetFiles(directory);
      List<SyntaxMode> modes = new List<SyntaxMode>();
      foreach (string file in files)
      {
        if (Path.GetExtension(file).Equals(".XSHD", StringComparison.OrdinalIgnoreCase))
        {
          XmlTextReader reader = new XmlTextReader(file);
          while (reader.Read())
          {
            if (reader.NodeType == XmlNodeType.Element)
            {
              switch (reader.Name)
              {
                case "SyntaxDefinition":
                  string name = reader.GetAttribute("name");
                  string extensions = reader.GetAttribute("extensions");
                  modes.Add(new SyntaxMode(Path.GetFileName(file), name, extensions));
                  goto bailout;
                default:
                  throw new HighlightingDefinitionInvalidException("Unknown root node in syntax highlighting file :" + reader.Name);
              }
            }
          }
        bailout:
          reader.Close();
        }
      }
      return modes;
    }
  }
}
