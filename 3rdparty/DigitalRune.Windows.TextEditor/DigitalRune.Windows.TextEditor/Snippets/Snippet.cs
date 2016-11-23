using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor.Completion
{
  /// <summary>
  /// Defines a snippet (text template) which can be inserted in the 
  /// text editor by typing or selecting a shortcut.
  /// </summary>
  /// <remarks>
  /// </remarks>
  public class Snippet
  {
    //--------------------------------------------------------------
    #region Fields
    //--------------------------------------------------------------
    #endregion


    //--------------------------------------------------------------
    #region Properties
    //--------------------------------------------------------------

    /// <summary>
    /// Gets or sets the shortcut.
    /// </summary>
    /// <value>The shortcut.</value>
    public string Shortcut { get; set; }


    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>The description.</value>
    public string Description { get; set; }


    /// <summary>
    /// Gets or sets the text that is inserted in the text editor
    /// if the snippet is selected.
    /// </summary>
    /// <value>The text.</value>
    /// <remarks>
    /// As a special character snippets can contain the character '|'. This character marks the 
    /// position where the caret will be placed after the snippet is expanded.
    /// </remarks>
    /// <example>
    /// Here is an example snippet that defines a while-loop. Note how the '|' is placed inside the
    /// parenthesis. When the user expands the snippet, the caret will be placed where the '|' is.
    /// <code>
    /// Snippet whileLoop = new Snippet("while", "while (|)\n{\n}", "while loop")
    /// </code>
    /// </example>
    public string Text { get; set; }
    #endregion


    //--------------------------------------------------------------
    #region Creation and Cleanup
    //--------------------------------------------------------------

    /// <summary>
    /// Initializes a new instance of the <see cref="Snippet"/> class.
    /// </summary>
    /// <param name="shortcut">The shortcut.</param>
    /// <param name="text">The template text.</param>
    public Snippet(string shortcut, string text)
    {
      Shortcut = shortcut;
      Text = text;
      Description = String.Empty;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="Snippet"/> class.
    /// </summary>
    /// <param name="shortcut">The shortcut.</param>
    /// <param name="text">The text.</param>
    /// <param name="description">The description.</param>
    public Snippet(string shortcut, string text, string description)
    {
      Shortcut = shortcut;
      Text = text;
      Description = description;
    }
    #endregion


    //--------------------------------------------------------------
    #region Methods
    //--------------------------------------------------------------

    /// <summary>
    /// Inserts the snippet into the text editor at the current caret position.
    /// </summary>
    /// <param name="textEditorControl">The text editor control.</param>
    public void Insert(TextEditorControl textEditorControl)
    {
      TextArea textArea = textEditorControl.ActiveTextAreaControl.TextArea;
      Insert(textArea);      
    }


    /// <summary>
    /// Inserts the snippet into the text editor at the current caret position.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public void Insert(TextArea textArea)
    {
      TextEditorControl textEditorControl = textArea.MotherTextEditorControl;
      IDocument document = textEditorControl.Document;

      document.UndoStack.StartUndoGroup();

      //string selectedText = String.Empty;
      //if (textArea.SelectionManager.HasSomethingSelected)
      //{
      //  selectedText = textArea.SelectionManager.SelectedText;
      //  textArea.Caret.Position = textArea.SelectionManager.Selections[0].StartPosition;
      //  textArea.SelectionManager.RemoveSelectedText();
      //}

      // -----
      // SharpDevelop only:
      // The following line searches for the variable "$(Selection)" in the template text.
      // The variable is then replaced by the text that was previously selected in the editor.
      //   string snippetText = StringParser.Parse(Text, new string[,] { { "Selection", selectedText } });

      // We simply ignore the line above and just copy the text:
      string snippetText = Text;
      // -----

      int finalCaretOffset = snippetText.IndexOf('|');
      if (finalCaretOffset >= 0)
      {
        snippetText = snippetText.Remove(finalCaretOffset, 1);
      }
      else
      {
        finalCaretOffset = snippetText.Length;
      }
      int caretOffset = textArea.Caret.Offset;

      textEditorControl.BeginUpdate();
      int beginLine = textArea.Caret.Line;
      document.Insert(caretOffset, snippetText);

      textArea.Caret.Position = document.OffsetToPosition(caretOffset + finalCaretOffset);
      int endLine = document.OffsetToPosition(caretOffset + snippetText.Length).Y;

      // Save old property settings
      IndentStyle save1 = textEditorControl.TextEditorProperties.IndentStyle;
      textEditorControl.TextEditorProperties.IndentStyle = IndentStyle.Smart;

      document.FormattingStrategy.IndentLines(textArea, beginLine, endLine);
      document.UndoStack.EndUndoGroup();
      textEditorControl.EndUpdate();
      document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
      document.CommitUpdate();

      // Restore old property settings
      textEditorControl.TextEditorProperties.IndentStyle = save1;
    }


    /// <summary>
    /// Shows the template completion window.
    /// </summary>
    /// <param name="textEditorControl">The text editor control.</param>
    /// <param name="snippets">The snippets (text template).</param>
    /// <param name="imageList">The image list to use in the completion window.</param>
    /// <param name="snippetImageIndex">Index of the snippet image in the <paramref name="imageList"/>.</param>
    /// <param name="ch">The character that is going to be inserted. <c>'\0'</c> if no character.</param>
    public static void ShowTemplateCompletionWindow(TextEditorControl textEditorControl, IEnumerable<Snippet> snippets, ImageList imageList, int snippetImageIndex, char ch)
    {
      SnippetCompletionDataProvider snippetCompletionDataProvider = new SnippetCompletionDataProvider(snippets, imageList, snippetImageIndex);
      textEditorControl.ShowCompletionWindow(snippetCompletionDataProvider, ch, false);
    }
    #endregion    
  }
}
