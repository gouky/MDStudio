using System.ComponentModel;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor
{
  public partial class TextEditorControl
  {
    /// <summary>
    /// Gets or sets the text rendering hint.
    /// </summary>
    /// <value>
    /// Specifies the quality of text rendering (whether to use hinting and/or anti-aliasing).
    /// </value>
    [Category("Appearance")]
    [DefaultValue(TextRenderingHint.SystemDefault)]
    [Description("Specifies the quality of text rendering (whether to use hinting and/or anti-aliasing).")]
    public TextRenderingHint TextRenderingHint
    {
      get
      {
        return Document.TextEditorProperties.TextRenderingHint;
      }
      set
      {
        Document.TextEditorProperties.TextRenderingHint = value;
        OptionsChanged();
      }
    }

    
    /// <summary>
    /// Gets or sets a value indicating whether to display spaces in the text area.
    /// </summary>
    /// <value><see langword="true"/> to display spaces in the text area; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("If 'true' spaces are displayed.")]
    public bool ShowSpaces
    {
      get { return Document.TextEditorProperties.ShowSpaces; }
      set
      {
        Document.TextEditorProperties.ShowSpaces = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to display tabs.
    /// </summary>
    /// <value><see langword="true"/> to display tabs; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("If 'true' tabs are displayed.")]
    public bool ShowTabs
    {
      get { return Document.TextEditorProperties.ShowTabs; }
      set
      {
        Document.TextEditorProperties.ShowTabs = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether display the end-of-line marker.
    /// </summary>
    /// <value><see langword="true"/> to display the end-of-line marker; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("If 'true' end-of-line (EOL) markers are shown.")]
    public bool ShowEOLMarkers
    {
      get { return Document.TextEditorProperties.ShowEOLMarker; }
      set
      {
        Document.TextEditorProperties.ShowEOLMarker = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show horizontal ruler (columns).
    /// </summary>
    /// <value><see langword="true"/> to show the horizontal ruler; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("If 'true' the horizontal ruler (column numbers) are shown.")]
    public bool ShowHRuler
    {
      get { return Document.TextEditorProperties.ShowHorizontalRuler; }
      set
      {
        Document.TextEditorProperties.ShowHorizontalRuler = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show the vertical ruler (vertical guide).
    /// </summary>
    /// <value><see langword="true"/> to show the vertical ruler; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(true)]
    [Description("If 'true' the vertical ruler (vertical guide) is shown in the text area.")]
    public bool ShowVRuler
    {
      get { return Document.TextEditorProperties.ShowVerticalRuler; }
      set
      {
        Document.TextEditorProperties.ShowVerticalRuler = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the column of the vertical ruler.
    /// </summary>
    /// <value>The column of vertical ruler.</value>
    [Category("Appearance")]
    [DefaultValue(80)]
    [Description("The column in which the vertical ruler is displayed.")]
    public int VRulerRow
    {
      get { return Document.TextEditorProperties.VerticalRulerColumn; }
      set
      {
        Document.TextEditorProperties.VerticalRulerColumn = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show line numbers.
    /// </summary>
    /// <value><see langword="true"/> if line numbers are shown; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(true)]
    [Description("If 'true' line numbers are shown.")]
    public bool ShowLineNumbers
    {
      get { return Document.TextEditorProperties.ShowLineNumbers; }
      set
      {
        Document.TextEditorProperties.ShowLineNumbers = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show invalid lines.
    /// </summary>
    /// <value><see langword="true"/> to show invalid lines; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("If 'true' invalid lines are marked in the text area.")]
    public bool ShowInvalidLines
    {
      get { return Document.TextEditorProperties.ShowInvalidLines; }
      set
      {
        Document.TextEditorProperties.ShowInvalidLines = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable folding.
    /// </summary>
    /// <value><see langword="true"/> if folding is enabled; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(true)]
    [Description("If 'true' folding/outlining is enabled in the text area.")]
    public bool EnableFolding
    {
      get { return Document.TextEditorProperties.EnableFolding; }
      set
      {
        Document.TextEditorProperties.EnableFolding = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to highlight matching brackets.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if matching brackets are highlighted; otherwise, <see langword="false"/>.
    /// </value>
    [Category("Appearance")]
    [DefaultValue(true)]
    [Description("If 'true' matching brackets are highlighted.")]
    public bool ShowMatchingBracket
    {
      get { return Document.TextEditorProperties.ShowMatchingBracket; }
      set
      {
        Document.TextEditorProperties.ShowMatchingBracket = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether the icon bar visible.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if the icon bar visible; otherwise, <see langword="false"/>.
    /// </value>
    [Category("Appearance")]
    [DefaultValue(false)]
    [Description("If 'true' the icon bar is displayed.")]
    public bool IsIconBarVisible
    {
      get { return Document.TextEditorProperties.IsIconBarVisible; }
      set
      {
        Document.TextEditorProperties.IsIconBarVisible = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the width of a tab character.
    /// </summary>
    /// <value>The width in spaces of a tab character.</value>
    [Category("Appearance")]
    [DefaultValue(4)]
    [Description("The width in spaces of a tab character.")]
    public int TabIndent
    {
      get { return Document.TextEditorProperties.TabIndent; }
      set
      {
        Document.TextEditorProperties.TabIndent = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the width of the indentation.
    /// </summary>
    /// <value>The width in spaces of the indentation.</value>
    [Category("Appearance")]
    [DefaultValue(4)]
    [Description("The width in spaces of a indentation.")]
    public int IndentationSize
    {
      get { return Document.TextEditorProperties.IndentationSize; }
      set
      {
        Document.TextEditorProperties.IndentationSize = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the style the current line is highlighted.
    /// </summary>
    /// <value>The line viewer style.</value>
    [Category("Appearance")]
    [DefaultValue(LineViewerStyle.None)]
    [Description("Determines whether the current line is highlighted.")]
    public LineViewerStyle LineViewerStyle
    {
      get { return Document.TextEditorProperties.LineViewerStyle; }
      set
      {
        Document.TextEditorProperties.LineViewerStyle = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the indent style.
    /// </summary>
    /// <value>The indent style</value>
    [Category("Behavior")]
    [DefaultValue(IndentStyle.Smart)]
    [Description("The indent style.")]
    public IndentStyle IndentStyle
    {
      get { return Document.TextEditorProperties.IndentStyle; }
      set
      {
        Document.TextEditorProperties.IndentStyle = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to convert tabs to spaces.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> to convert tabs to spaces; otherwise, <see langword="false"/>.
    /// </value>
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Converts tabs to spaces while typing if set to 'true'.")]
    public bool ConvertTabsToSpaces
    {
      get { return Document.TextEditorProperties.ConvertTabsToSpaces; }
      set
      {
        Document.TextEditorProperties.ConvertTabsToSpaces = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to hide the mouse cursor while typing.
    /// </summary>
    /// <value><see langword="true"/> to hide the mouse cursor; otherwise, <see langword="false"/>.</value>
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Hide the mouse cursor while typing.")]
    public bool HideMouseCursor
    {
      get { return Document.TextEditorProperties.HideMouseCursor; }
      set
      {
        Document.TextEditorProperties.HideMouseCursor = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether allow placing a caret beyond
    /// the end of the line (often called virtual space).
    /// </summary>
    /// <value>
    /// <see langword="true"/> if placing the caret beyond the end of line is allowed; 
    /// otherwise, <see langword="false"/>.
    /// </value>
    [Category("Behavior")]
    [DefaultValue(false)]
    [Description("Allows the caret to be placed beyond the end of line.")]
    public bool AllowCaretBeyondEOL
    {
      get { return Document.TextEditorProperties.AllowCaretBeyondEOL; }
      set
      {
        Document.TextEditorProperties.AllowCaretBeyondEOL = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the bracket matching style.
    /// </summary>
    /// <value>The bracket matching style.</value>
    [Category("Behavior")]
    [DefaultValue(BracketMatchingStyle.After)]
    [Description("Specifies if the bracket matching should match the bracket before or after the caret.")]
    public BracketMatchingStyle BracketMatchingStyle
    {
      get { return Document.TextEditorProperties.BracketMatchingStyle; }
      set
      {
        Document.TextEditorProperties.BracketMatchingStyle = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the font of the text area.
    /// </summary>
    /// <value>
    /// The base font of the text area. No bold or italic fonts
    /// can be used because bold/italic is reserved for highlighting
    /// purposes.
    /// </value>
    /// <returns>The <see cref="Font"></see> to apply to the text displayed by the control. The default is the value of the <see cref="Control.DefaultFont"></see> property.</returns>
    /// <PermissionSet>
    /// <IPermission class="System.Security.Permissions.EnvironmentPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="UnmanagedCode, ControlEvidence"/>
    /// <IPermission class="System.Diagnostics.PerformanceCounterPermission, System, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Unrestricted="true"/>
    /// </PermissionSet>
    [Browsable(true)]
    [Description("The base font of the text area. No bold or italic fonts can be used because bold/italic is reserved for highlighting purposes.")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override Font Font
    {
      get
      {
        return Document.TextEditorProperties.Font;
      }
      set
      {
        Document.TextEditorProperties.Font = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets the document selection mode.
    /// </summary>
    /// <value>The document selection mode.</value>
    [Category("Behavior")]
    [DefaultValue(DocumentSelectionMode.Normal)]
    [Description("Specifies the selection mode.")]
    public DocumentSelectionMode DocumentSelectionMode
    {
      get { return Document.TextEditorProperties.DocumentSelectionMode; }
      set
      {
        Document.TextEditorProperties.DocumentSelectionMode = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to show scroll bars.
    /// </summary>
    /// <value><see langword="true"/> to display scroll bars; otherwise, <see langword="false"/>.</value>
    [Category("Appearance")]
    [DefaultValue(true)]
    [Description("If 'true' scroll bars are displayed.")]
    public bool ShowScrollBars
    {
      get { return Document.TextEditorProperties.ShowScrollBars; }
      set
      {
        Document.TextEditorProperties.ShowScrollBars = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable code completion.
    /// </summary>
    /// <value><see langword="true"/> if code completion is enabled; otherwise, <see langword="false"/>.</value>
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Specifies whether code completion is enabled")]
    public bool EnableCompletion
    {
      get { return Document.TextEditorProperties.EnableCompletion; }
      set
      {
        Document.TextEditorProperties.EnableCompletion = value;
        OptionsChanged();
      }
    }


    /// <summary>
    /// Gets or sets a value indicating whether to enable method insight.
    /// </summary>
    /// <value><see langword="true"/> if method insight is enabled; otherwise, <see langword="false"/>.</value>
    [Category("Behavior")]
    [DefaultValue(true)]
    [Description("Specifies whether code completion is enabled")]
    public bool EnableMethodInsight
    {
      get { return Document.TextEditorProperties.EnableInsight; }
      set
      {
        Document.TextEditorProperties.EnableInsight = value;
        OptionsChanged();
      }
    }
  }
}
