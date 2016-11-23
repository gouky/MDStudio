using System;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// Represents a part of the text area.
  /// </summary>
  /// <remarks>
  /// The text area is divided into several vertical strips, such as icon margin, line-number margin,
  /// fold margin, the actual text view, and possible other margins.
  /// </remarks>
  internal abstract class AbstractMargin
  {
    private Cursor _cursor = Cursors.Default;
    private Rectangle _drawingPosition = new Rectangle(0, 0, 0, 0);
    private readonly TextArea _textArea;


    /// <summary>
    /// Gets or sets the drawing position.
    /// </summary>
    /// <value>The drawing position.</value>
    public Rectangle DrawingPosition
    {
      get { return _drawingPosition; }
      set { _drawingPosition = value; }
    }


    /// <summary>
    /// Gets the text area.
    /// </summary>
    /// <value>The text area.</value>
    public TextArea TextArea
    {
      get { return _textArea; }
    }


    /// <summary>
    /// Gets the document.
    /// </summary>
    /// <value>The document.</value>
    public IDocument Document
    {
      get { return _textArea.Document; }
    }


    /// <summary>
    /// Gets the text editor properties.
    /// </summary>
    /// <value>The text editor properties.</value>
    public ITextEditorProperties TextEditorProperties
    {
      get { return _textArea.Document.TextEditorProperties; }
    }


    /// <summary>
    /// Gets or sets the cursor.
    /// </summary>
    /// <value>The cursor.</value>
    public virtual Cursor Cursor
    {
      get { return _cursor; }
      set { _cursor = value; }
    }


    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public virtual Size Size
    {
      get { return new Size(-1, -1); }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is visible; otherwise, <see langword="false"/>.
    /// </value>
    public virtual bool IsVisible
    {
      get { return true; }
    }


    /// <summary>
    /// Occurs when the margin is painted.
    /// </summary>
    public event EventHandler<PaintEventArgs> Paint;


    /// <summary>
    /// Occurs when the mouse button is pressed on the margin.
    /// </summary>
    public event EventHandler<MouseEventArgs> MouseDown;


    /// <summary>
    /// Occurs when the mouse is moved on the margin.
    /// </summary>
    public event EventHandler<MouseEventArgs> MouseMove;


    /// <summary>
    /// Occurs when the mouse leaves the margin.
    /// </summary>
    public event EventHandler MouseLeave;


    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractMargin"/> class.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    protected AbstractMargin(TextArea textArea)
    {
      _textArea = textArea;
    }


    /// <summary>
    /// Handles a mouse down event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    public void HandleMouseDown(MouseEventArgs eventArgs)
    {
      OnMouseDown(eventArgs);
    }


    /// <summary>
    /// Raises the <see cref="MouseDown"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    protected virtual void OnMouseDown(MouseEventArgs eventArgs)
    {
      if (MouseDown != null)
        MouseDown(this, eventArgs);
    }


    /// <summary>
    /// Handles a mouse move.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    public void HandleMouseMove(MouseEventArgs eventArgs)
    {
      OnMouseMove(eventArgs);
    }


    /// <summary>
    /// Raises the <see cref="MouseMove"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    protected virtual void OnMouseMove(MouseEventArgs eventArgs)
    {
      if (MouseMove != null)
        MouseMove(this, eventArgs);
    }


    /// <summary>
    /// Handles a mouse leave event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    public void HandleMouseLeave(EventArgs e)
    {
      OnMouseLeave(e);
    }


    /// <summary>
    /// Raises the <see cref="MouseLeave"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected virtual void OnMouseLeave(EventArgs e)
    {
      if (MouseLeave != null)
        MouseLeave(this, e);
    }


    /// <summary>
    /// Draws the margin.
    /// </summary>
    /// <param name="g">The <see cref="Graphics"/> context.</param>
    /// <param name="rect">The clipping rectangle.</param>
    public void Draw(Graphics g, Rectangle rect)
    {
      OnPaint(new PaintEventArgs(g, rect));
    }


    /// <summary>
    /// Raises the <see cref="Paint"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
    protected virtual void OnPaint(PaintEventArgs eventArgs)
    {
      if (Paint != null)
        Paint(this, eventArgs);
    }
  }
}
