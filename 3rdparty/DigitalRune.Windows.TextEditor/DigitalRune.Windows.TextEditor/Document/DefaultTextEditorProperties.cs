using System.Drawing;
using System.Drawing.Text;
using System.Text;
using DigitalRune.Windows.TextEditor.Highlighting;

namespace DigitalRune.Windows.TextEditor.Properties
{
  /// <summary>
  /// The default properties of a text editor.
  /// </summary>
  public class DefaultTextEditorProperties : ITextEditorProperties
  {
    private static Font _defaultFont;
    private int _tabIndent = 4;
    private int _indentationSize = 4;
    private IndentStyle _indentStyle = IndentStyle.Smart;
    private DocumentSelectionMode _documentSelectionMode = DocumentSelectionMode.Normal;
    private Encoding _encoding = System.Text.Encoding.UTF8;
    private BracketMatchingStyle _bracketMatchingStyle = BracketMatchingStyle.After;
    private readonly FontContainer _fontContainer;
    private bool _allowCaretBeyondEOL = false;
    //private bool _caretLine = false;
    private bool _showMatchingBracket = true;
    private bool _showLineNumbers = true;
    private bool _showSpaces = false;
    private bool _showTabs = false;
    private bool _showEOLMarker = false;
    private bool _showInvalidLines = false;
    private bool _isIconBarVisible = false;
    private bool _enableFolding = true;
    private bool _showHorizontalRuler = false;
    private bool _showVerticalRuler = true;
    private bool _convertTabsToSpaces = false;
    private TextRenderingHint _textRenderingHint = TextRenderingHint.SystemDefault;
    private bool _mouseWheelScrollDown = true;
    private bool _mouseWheelTextZoom = true;
    private bool _hideMouseCursor = false;
    private bool _cutCopyWholeLine = true;
    private int _verticalRulerRow = 80;
    private LineViewerStyle _lineViewerStyle = LineViewerStyle.None;
    private string _lineTerminator = "\r\n";
    private bool _autoInsertCurlyBracket = true;
    bool _showScrollBars = true;
    bool _enableCompletion = true;
    bool _enableMethodInsight = true;
    bool _supportsReadOnlySegments = false;


    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTextEditorProperties"/> class.
    /// </summary>
    public DefaultTextEditorProperties()
    {
      if (_defaultFont == null)
        _defaultFont = new Font("Courier New", 10);

      _fontContainer = new FontContainer(_defaultFont);
    }


    /// <summary>
    /// Gets or sets the width of a tab character.
    /// </summary>
    /// <value>The width in spaces of a tab character.</value>
    public int TabIndent
    {
      get { return _tabIndent; }
      set { _tabIndent = value; }
    }


    /// <summary>
    /// The amount of spaces a tab is converted to if <see cref="ConvertTabsToSpaces"/> is true.
    /// </summary>
    /// <value></value>
    public int IndentationSize
    {
      get { return _indentationSize; }
      set { _indentationSize = value; }
    }

    
    /// <summary>
    /// Gets or sets the indent style.
    /// </summary>
    /// <value>The indent style.</value>
    public IndentStyle IndentStyle
    {
      get { return _indentStyle; }
      set { _indentStyle = value; }
    }


    
    //public bool CaretLine
    //{
    //  get { return _caretLine; }
    //  set { _caretLine = value; }
    //}


    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    /// <value>The selection mode.</value>
    public DocumentSelectionMode DocumentSelectionMode
    {
      get { return _documentSelectionMode; }
      set { _documentSelectionMode = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether allow placing a caret beyond
    /// the end of the line (often called virtual space).
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if placing the caret beyond the end of line is allowed;
    /// otherwise, <see langword="false"/>.
    /// </value>
    public bool AllowCaretBeyondEOL
    {
      get { return _allowCaretBeyondEOL; }
      set { _allowCaretBeyondEOL = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to highlight matching brackets.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if matching brackets are highlighted; otherwise, <see langword="false"/>.
    /// </value>
    public bool ShowMatchingBracket
    {
      get { return _showMatchingBracket; }
      set { _showMatchingBracket = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    /// <value><see langword="true"/> if line numbers are shown; otherwise, <see langword="false"/>.</value>
    public bool ShowLineNumbers
    {
      get { return _showLineNumbers; }
      set { _showLineNumbers = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display spaces in the text area.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> to display spaces in the text area; otherwise, <see langword="false"/>.
    /// </value>
    public bool ShowSpaces
    {
      get { return _showSpaces; }
      set { _showSpaces = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display tabs.
    /// </summary>
    /// <value><see langword="true"/> to display tabs; otherwise, <see langword="false"/>.</value>
    public bool ShowTabs
    {
      get { return _showTabs; }
      set { _showTabs = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether display the end-of-line marker.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> to display the end-of-line marker; otherwise, <see langword="false"/>.
    /// </value>
    public bool ShowEOLMarker
    {
      get { return _showEOLMarker; }
      set { _showEOLMarker = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show invalid lines.
    /// </summary>
    /// <value><see langword="true"/> to show invalid lines; otherwise, <see langword="false"/>.</value>
    public bool ShowInvalidLines
    {
      get { return _showInvalidLines; }
      set { _showInvalidLines = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the icon bar visible.
    /// </summary>
    /// <value><see langword="true"/> if the icon bar visible; otherwise, <see langword="false"/>.</value>
    public bool IsIconBarVisible
    {
      get { return _isIconBarVisible; }
      set { _isIconBarVisible = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable folding.
    /// </summary>
    /// <value><see langword="true"/> if folding is enabled; otherwise, <see langword="false"/>.</value>
    public bool EnableFolding
    {
      get { return _enableFolding; }
      set { _enableFolding = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show horizontal ruler (columns).
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> to show the horizontal ruler; otherwise, <see langword="false"/>.
    /// </value>
    public bool ShowHorizontalRuler
    {
      get { return _showHorizontalRuler; }
      set { _showHorizontalRuler = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show the vertical ruler (vertical guide).
    /// </summary>
    /// <value><see langword="true"/> to show the vertical ruler; otherwise, <see langword="false"/>.</value>
    public bool ShowVerticalRuler
    {
      get { return _showVerticalRuler; }
      set { _showVerticalRuler = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to convert tabs to spaces.
    /// </summary>
    /// <value><see langword="true"/> to convert tabs to spaces; otherwise, <see langword="false"/>.</value>
    public bool ConvertTabsToSpaces
    {
      get { return _convertTabsToSpaces; }
      set { _convertTabsToSpaces = value; }
    }


    /// <summary>
    /// Gets or sets the <see cref="System.Drawing.Text.TextRenderingHint"/> for rendering the text.
    /// </summary>
    /// <value>The <see cref="System.Drawing.Text.TextRenderingHint"/>.</value>
    public TextRenderingHint TextRenderingHint
    {
      get { return _textRenderingHint; }
      set { _textRenderingHint = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to scroll the text down or up
    /// when rotating the mouse wheel (default is 'down').
    /// </summary>
    /// <value><see langword="true"/> to scroll down; <see langword="false"/> false to scroll up.</value>
    public bool MouseWheelScrollDown
    {
      get { return _mouseWheelScrollDown; }
      set { _mouseWheelScrollDown = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to zoom the text with the mouse
    /// wheel.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> to zoom the text with the mouse wheel; otherwise, <see langword="false"/>.
    /// </value>
    public bool MouseWheelTextZoom
    {
      get { return _mouseWheelTextZoom; }
      set { _mouseWheelTextZoom = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to hide the mouse cursor while typing.
    /// </summary>
    /// <value><see langword="true"/> to hide the mouse cursor; otherwise, <see langword="false"/>.</value>
    public bool HideMouseCursor
    {
      get { return _hideMouseCursor; }
      set { _hideMouseCursor = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to cut/copy the whole line when
    /// nothing is selected.
    /// </summary>
    /// <value><see langword="true"/> to cut/copy the whole line; otherwise, <see langword="false"/>.</value>
    public bool CutCopyWholeLine
    {
      get { return _cutCopyWholeLine; }
      set { _cutCopyWholeLine = value; }
    }


    /// <summary>
    /// Gets or sets the encoding.
    /// </summary>
    /// <value>The encoding.</value>
    public Encoding Encoding
    {
      get { return _encoding; }
      set { _encoding = value; }
    }


    /// <summary>
    /// Gets or sets the column of the vertical ruler.
    /// </summary>
    /// <value>The column of vertical ruler.</value>
    public int VerticalRulerColumn
    {
      get { return _verticalRulerRow; }
      set { _verticalRulerRow = value; }
    }


    /// <summary>
    /// Gets or sets the style the current line is highlighted.
    /// </summary>
    /// <value>The line viewer style.</value>
    public LineViewerStyle LineViewerStyle
    {
      get { return _lineViewerStyle; }
      set { _lineViewerStyle = value; }
    }


    /// <summary>
    /// Gets or sets the line terminator.
    /// </summary>
    /// <value>The line terminator.</value>
    public string LineTerminator
    {
      get { return _lineTerminator; }
      set { _lineTerminator = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to automatically insert curly
    /// brackets.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if automatically inserting curly brackets; otherwise, <see langword="false"/>.
    /// </value>
    public bool AutoInsertCurlyBracket
    {
      get { return _autoInsertCurlyBracket; }
      set { _autoInsertCurlyBracket = value; }
    }


    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>The font.</value>
    public Font Font
    {
      get { return _fontContainer.DefaultFont; }
      set { _fontContainer.DefaultFont = value; }
    }


    /// <summary>
    /// Gets the font container.
    /// </summary>
    /// <value>The font container.</value>
    public FontContainer FontContainer
    {
      get { return _fontContainer; }
    }


    /// <summary>
    /// Gets or sets the bracket matching style.
    /// </summary>
    /// <value>The bracket matching style.</value>
    public BracketMatchingStyle BracketMatchingStyle
    {
      get { return _bracketMatchingStyle; }
      set { _bracketMatchingStyle = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether this document supports read-only segments.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if the document supports read-only segments; otherwise, <see langword="false"/>.
    /// </value>
    public bool SupportsReadOnlySegments
    {
      get { return _supportsReadOnlySegments; }
      set { _supportsReadOnlySegments = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show scroll bars.
    /// </summary>
    /// <value><see langword="true"/> if to show scroll bars; otherwise, <see langword="false"/>.</value>
    public bool ShowScrollBars
    {
      get { return _showScrollBars; }
      set { _showScrollBars = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable code completion.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if code completion is enabled; otherwise, <see langword="false"/>.
    /// </value>
    public bool EnableCompletion
    {
      get { return _enableCompletion; }
      set { _enableCompletion = value; }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable method insight.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if method insight is enabled; otherwise, <see langword="false"/>.
    /// </value>
    public bool EnableInsight
    {
      get { return _enableMethodInsight; }
      set { _enableMethodInsight = value; }
    }
  }
}
