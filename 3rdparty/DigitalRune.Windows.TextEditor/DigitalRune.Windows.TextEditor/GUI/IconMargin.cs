using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Bookmarks;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// The vertical strip next to the text that contains icons for bookmarks, breakpoints, etc.
  /// </summary>
  internal class IconMargin : AbstractMargin
  {
    private const int _iconMarginWidth = 18;
    private static readonly Size _iconBarSize = new Size(_iconMarginWidth, -1);


    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public override Size Size
    {
      get { return _iconBarSize; }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is visible; otherwise, <see langword="false"/>.
    /// </value>
    public override bool IsVisible
    {
      get { return TextArea.TextEditorProperties.IsIconBarVisible; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="IconMargin"/> class.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public IconMargin(TextArea textArea)
      : base(textArea)
    {
    }


    /// <summary>
    /// Raises the <see cref="AbstractMargin.Paint"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.PaintEventArgs"/> instance containing the event data.</param>
    protected override void OnPaint(PaintEventArgs eventArgs)
    {
      Graphics g = eventArgs.Graphics;
      Rectangle rect = eventArgs.ClipRectangle;

      if (rect.Width <= 0 || rect.Height <= 0)
        return;

      // paint background
      Form parentForm = TextArea.MotherTextEditorControl.ParentForm;
      Color backColor = (parentForm != null) ? parentForm.BackColor : SystemColors.Control;
      Color foreColor = ControlPaint.Dark(backColor);
      using (Brush brush = new SolidBrush(backColor))
        g.FillRectangle(brush, new Rectangle(DrawingPosition.X, rect.Top, DrawingPosition.Width - 1, rect.Height));
      using (Pen pen = new Pen(foreColor))
        g.DrawLine(pen, DrawingPosition.Right - 1, rect.Top, DrawingPosition.Right - 1, rect.Bottom);

      // paint icons
      foreach (Bookmark mark in TextArea.Document.BookmarkManager.Marks)
      {
        int lineNumber = TextArea.Document.GetVisibleLine(mark.LineNumber);
        int lineHeight = TextArea.TextView.LineHeight;
        int yPos = lineNumber * lineHeight - TextArea.VirtualTop.Y;
        if (IsLineInsideRegion(yPos, yPos + lineHeight, rect.Y, rect.Bottom))
        {
          if (lineNumber == TextArea.Document.GetVisibleLine(mark.LineNumber - 1))
          {
            // marker is inside folded region, do not draw it
            continue;
          }
          mark.Draw(g, new Rectangle(0, yPos, _iconMarginWidth, TextArea.TextView.LineHeight));
        }
      }
      base.OnPaint(eventArgs);
    }


    /// <summary>
    /// Raises the <see cref="AbstractMargin.MouseDown"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    protected override void OnMouseDown(MouseEventArgs eventArgs)
    {
      Point mousePos = eventArgs.Location;
      MouseButtons mouseButtons = eventArgs.Button;

      int clickedVisibleLine = (mousePos.Y + TextArea.VirtualTop.Y) / TextArea.TextView.LineHeight;
      int lineNumber = TextArea.Document.GetFirstLogicalLine(clickedVisibleLine);

      if ((mouseButtons & MouseButtons.Right) == MouseButtons.Right)
        if (TextArea.Caret.Line != lineNumber)
          TextArea.Caret.Line = lineNumber;

      IList<Bookmark> marks = TextArea.Document.BookmarkManager.Marks;
      List<Bookmark> marksInLine = new List<Bookmark>();
      int oldCount = marks.Count;
      foreach (Bookmark mark in marks)
        if (mark.LineNumber == lineNumber)
          marksInLine.Add(mark);

      for (int i = marksInLine.Count - 1; i >= 0; i--)
      {
        Bookmark mark = marksInLine[i];
        if (mark.Click(TextArea, eventArgs))
        {
          if (oldCount != marks.Count)
            TextArea.UpdateLine(lineNumber);

          return;
        }
      }
      base.OnMouseDown(eventArgs);
    }


    static bool IsLineInsideRegion(int top, int bottom, int regionTop, int regionBottom)
    {
      if (top >= regionTop && top <= regionBottom)
      {
        // Region overlaps the line's top edge.
        return true;
      }
      else if (regionTop > top && regionTop < bottom)
      {
        // Region's top edge inside line.
        return true;
      }
      return false;
    }
  }
}
