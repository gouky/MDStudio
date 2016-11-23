using System.Drawing;
using System.Drawing.Text;
using System.Text;
using DigitalRune.Windows.TextEditor.Highlighting;

namespace DigitalRune.Windows.TextEditor.Properties
{
  /// <summary>
  /// Describes the caret marker
  /// </summary>
  public enum LineViewerStyle
  {
    /// <summary>No line viewer will be displayed.</summary>
    None,
    /// <summary>The row in which the caret is will be marked.</summary>
    FullRow
  }


  /// <summary>
  /// Describes the indent style
  /// </summary>
  public enum IndentStyle
  {
    /// <summary>No indentation occurs.</summary>
    None,
    /// <summary>The indentation from the line above will be taken to indent the current line. </summary>
    Auto,
    /// <summary>Intelligent, context sensitive indentation will occur.</summary>
    Smart
  }


  /// <summary>
  /// Describes the selection mode of the text area
  /// </summary>
  public enum DocumentSelectionMode
  {
    /// <summary>The 'normal' selection mode.</summary>
    Normal,
    /// <summary>
    /// Selections will be added to the current selection or new  ones will 
    /// be created (multi-select mode)
    /// </summary>
    Additive
  }

  
  /// <summary>
  /// The bracket matching style.
  /// </summary>
  public enum BracketMatchingStyle
  {
    /// <summary>Cursor on bracket: Highlight brackets on cursor position.</summary>
    OnBracket,
    /// <summary> Cursor after bracket: Highlight brackets before cursor position.</summary>
    After
  }


  /// <summary>
  /// Stores the properties of a text editor.
  /// </summary>
  public interface ITextEditorProperties
  {
    //bool CaretLine
    //{
    //  get;
    //  set;
    //}


    /// <summary>
    /// Gets or sets a value indicating whether to automatically insert curly 
    /// brackets.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if automatically inserting curly brackets; otherwise, <see langword="false"/>.
    /// </value>
    bool AutoInsertCurlyBracket
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to hide the mouse cursor while typing.
    /// </summary>
    /// <value><see langword="true"/> to hide the mouse cursor; otherwise, <see langword="false"/>.</value>
    bool HideMouseCursor
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether the icon bar visible.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if the icon bar visible; otherwise, <see langword="false"/>.
    /// </value>
    bool IsIconBarVisible
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether allow placing a caret beyond
    /// the end of the line (often called virtual space).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if placing the caret beyond the end of line is allowed; 
    /// otherwise, <see langword="false"/>.
    /// </value>
    bool AllowCaretBeyondEOL
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to highlight matching brackets.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if matching brackets are highlighted; otherwise, <see langword="false"/>.
    /// </value>
    bool ShowMatchingBracket
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to cut/copy the whole line when
    /// nothing is selected.
    /// </summary>
    /// <value><see langword="true"/> to cut/copy the whole line; otherwise, <see langword="false"/>.</value>
    bool CutCopyWholeLine
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the <see cref="System.Drawing.Text.TextRenderingHint"/> for rendering the text.
    /// </summary>
    /// <value>The <see cref="System.Drawing.Text.TextRenderingHint"/>.</value>
    TextRenderingHint TextRenderingHint
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to scroll the text down or up
    /// when rotating the mouse wheel (default is 'down').
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> to scroll down; <see langword="false"/> false to scroll up.
    /// </value>
    bool MouseWheelScrollDown
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to zoom the text with the mouse
    /// wheel.
    /// </summary>
    /// <value><see langword="true"/> to zoom the text with the mouse wheel; otherwise, <see langword="false"/>.</value>
    bool MouseWheelTextZoom
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the line terminator.
    /// </summary>
    /// <value>The line terminator.</value>
    string LineTerminator
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the style the current line is highlighted.
    /// </summary>
    /// <value>The line viewer style.</value>
    LineViewerStyle LineViewerStyle
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show invalid lines.
    /// </summary>
    /// <value><see langword="true"/> to show invalid lines; otherwise, <see langword="false"/>.</value>
    bool ShowInvalidLines
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the column of the vertical ruler.
    /// </summary>
    /// <value>The column of vertical ruler.</value>
    int VerticalRulerColumn
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display spaces in the text area.
    /// </summary>
    /// <value><see langword="true"/> to display spaces in the text area; otherwise, <see langword="false"/>.</value>
    bool ShowSpaces
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display tabs.
    /// </summary>
    /// <value><see langword="true"/> to display tabs; otherwise, <see langword="false"/>.</value>
    bool ShowTabs
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether display the end-of-line marker.
    /// </summary>
    /// <value><see langword="true"/> to display the end-of-line marker; otherwise, <see langword="false"/>.</value>
    bool ShowEOLMarker
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to convert tabs to spaces.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> to convert tabs to spaces; otherwise, <see langword="false"/>.
    /// </value>
    bool ConvertTabsToSpaces
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show horizontal ruler (columns).
    /// </summary>
    /// <value><see langword="true"/> to show the horizontal ruler; otherwise, <see langword="false"/>.</value>
    bool ShowHorizontalRuler
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show the vertical ruler (vertical guide).
    /// </summary>
    /// <value><see langword="true"/> to show the vertical ruler; otherwise, <see langword="false"/>.</value>
    bool ShowVerticalRuler
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the encoding.
    /// </summary>
    /// <value>The encoding.</value>
    Encoding Encoding
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable folding.
    /// </summary>
    /// <value><see langword="true"/> if folding is enabled; otherwise, <see langword="false"/>.</value>
    bool EnableFolding
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    /// <value><see langword="true"/> if line numbers are shown; otherwise, <see langword="false"/>.</value>
    bool ShowLineNumbers
    { 
      get;
      set;
    }


    /// <summary>
    /// The width of a tab.
    /// </summary>
    int TabIndent
    {
      get;
      set;
    }


    /// <summary>
    /// The amount of spaces a tab is converted to if <see cref="ConvertTabsToSpaces"/> is true.
    /// </summary>
    int IndentationSize
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets the indent style.
    /// </summary>
    /// <value>The indent style.</value>
    IndentStyle IndentStyle
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the selection mode.
    /// </summary>
    /// <value>The selection mode.</value>
    DocumentSelectionMode DocumentSelectionMode
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets the font.
    /// </summary>
    /// <value>The font.</value>
    Font Font
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets the font container.
    /// </summary>
    /// <value>The font container.</value>
    FontContainer FontContainer
    {
      get;
    }


    /// <summary>
    /// Gets or sets the bracket matching style.
    /// </summary>
    /// <value>The bracket matching style.</value>
    BracketMatchingStyle BracketMatchingStyle
    { 
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether this document supports read-only segments.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if the document supports read-only segments; otherwise, <see langword="false"/>.
    /// </value>
    bool SupportsReadOnlySegments
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show scroll bars.
    /// </summary>
    /// <value><see langword="true"/> if to show scroll bars; otherwise, <see langword="false"/>.</value>
    bool ShowScrollBars
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable code completion.
    /// </summary>
    /// <value><see langword="true"/> if code completion is enabled; otherwise, <see langword="false"/>.</value>
    bool EnableCompletion
    {
      get;
      set;
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable method insight.
    /// </summary>
    /// <value><see langword="true"/> if method insight is enabled; otherwise, <see langword="false"/>.</value>
    bool EnableInsight
    {
      get;
      set;
    }
  }
}
