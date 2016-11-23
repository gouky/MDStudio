using System;
using System.Drawing;
using DigitalRune.Windows.TextEditor.Document;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// Event arguments for a tool tip request.
  /// </summary>
  public class ToolTipRequestEventArgs : EventArgs
  {
    private readonly Point _mousePosition;
    private readonly TextLocation _logicalPosition;
    private readonly bool _inDocument;
    private string _toolTipText;


    /// <summary>
    /// Gets the mouse position.
    /// </summary>
    /// <value>The mouse position.</value>
    public Point MousePosition
    {
      get { return _mousePosition; }
    }


    /// <summary>
    /// Gets the logical position.
    /// </summary>
    /// <value>The logical position.</value>
    public TextLocation LogicalPosition
    {
      get { return _logicalPosition; }
    }


    /// <summary>
    /// Gets a value indicating whether the mouse cursor is in the document.
    /// </summary>
    /// <value><see langword="true"/> if the mouse cursor is over document; otherwise, <see langword="false"/>.</value>
    public bool InDocument
    {
      get { return _inDocument; }
    }


    /// <summary>
    /// Gets if some client handling the event has already shown a tool tip.
    /// </summary>
    public bool ToolTipShown
    {
      get { return _toolTipText != null; }
    }


    internal string ToolTipText
    {
      get { return _toolTipText; }
    }


    /// <summary>
    /// Shows the tool tip.
    /// </summary>
    /// <param name="text">The text.</param>
    public void ShowToolTip(string text)
    {
      _toolTipText = text;
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="ToolTipRequestEventArgs"/> class.
    /// </summary>
    /// <param name="mousePosition">The mouse position.</param>
    /// <param name="logicalPosition">The logical position.</param>
    /// <param name="inDocument">if set to <see langword="true"/> mouse cursor is over the document.</param>
    public ToolTipRequestEventArgs(Point mousePosition, TextLocation logicalPosition, bool inDocument)
    {
      _mousePosition = mousePosition;
      _logicalPosition = logicalPosition;
      _inDocument = inDocument;
    }
  }
}
