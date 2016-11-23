using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Selection;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// The vertical strip next to the text that shows the line numbers.
  /// </summary>
  internal class LineNumberMargin : AbstractMargin
  {
    private static readonly Cursor _rightLeftCursor;


    /// <summary>
    /// Gets or sets the cursor.
    /// </summary>
    /// <value>The cursor.</value>
    public override Cursor Cursor
    {
      get { return _rightLeftCursor; }
    }


    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public override Size Size
    {
      get { return new Size(TextArea.TextView.ColumnWidth * Math.Max(3, (int) Math.Log10(TextArea.Document.TotalNumberOfLines) + 1) + 3, -1); }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is visible; otherwise, <see langword="false"/>.
    /// </value>
    public override bool IsVisible
    {
      get { return TextArea.TextEditorProperties.ShowLineNumbers; }
    }


    static LineNumberMargin()
    {
      Stream cursorStream = Assembly.GetCallingAssembly().GetManifestResourceStream("DigitalRune.Windows.TextEditor.Resources.RightArrow.cur");
      if (cursorStream == null) 
        throw new Exception("Could not find cursor resource.");
      _rightLeftCursor = new Cursor(cursorStream);
      cursorStream.Close();
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="LineNumberMargin"/> class.
    /// </summary>
    /// <param name="TextArea">The text area.</param>
    public LineNumberMargin(TextArea TextArea)
      : base(TextArea)
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

      int lineHeight = TextArea.TextView.LineHeight;
      HighlightColor lineNumberPainterColor = TextArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
      Font font = lineNumberPainterColor.GetFont(TextEditorProperties.FontContainer);
      Color foregroundColor = lineNumberPainterColor.Color;
      Color backgroundColor = TextArea.Enabled ? lineNumberPainterColor.BackgroundColor : SystemColors.InactiveBorder;
      Brush backgroundBrush = BrushRegistry.GetBrush(backgroundColor);
      Pen dottedPen = BrushRegistry.GetDotPen(foregroundColor);
      const TextFormatFlags textFormatFlags = TextFormatFlags.Right | TextFormatFlags.NoPadding;

      // Draw background
      Rectangle backgroundRectangle = DrawingPosition;
      backgroundRectangle.Intersect(rect);
      g.FillRectangle(backgroundBrush, backgroundRectangle);
      g.DrawLine(dottedPen, backgroundRectangle.Right - 2, backgroundRectangle.Top, backgroundRectangle.Right - 2, backgroundRectangle.Bottom);

      // Draw line numbers
      int visibleLineDrawingRemainder = TextArea.TextView.RemainderOfFirstVisibleLine;
      for (int y = 0; y < (DrawingPosition.Height + visibleLineDrawingRemainder) / lineHeight + 1; ++y)
      {
        int ypos = DrawingPosition.Y + lineHeight * y - visibleLineDrawingRemainder;
        Rectangle textRectangle = new Rectangle(DrawingPosition.X, ypos, DrawingPosition.Width - 3, lineHeight);
        if (rect.IntersectsWith(textRectangle))
        {
          int curLine = TextArea.Document.GetFirstLogicalLine(TextArea.Document.GetVisibleLine(TextArea.TextView.FirstLogicalLine) + y);

          if (curLine < TextArea.Document.TotalNumberOfLines)
            TextRenderer.DrawText(g, (curLine + 1).ToString(), font, textRectangle, foregroundColor, backgroundColor, textFormatFlags);
        }
      }
      base.OnPaint(eventArgs);
    }


    /// <summary>
    /// Raises the <see cref="AbstractMargin.MouseDown"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    protected override void OnMouseDown(MouseEventArgs eventArgs)
    {
      Point mousepos = eventArgs.Location;
      MouseButtons mouseButtons = eventArgs.Button;
      TextLocation selectionStartPos;

      TextArea.SelectionManager.SelectFrom.Where = WhereFrom.Gutter;
      int realline = TextArea.TextView.GetLogicalLine(mousepos.Y);
      if (realline >= 0 && realline < TextArea.Document.TotalNumberOfLines)
      {
        // shift-select
        if ((Control.ModifierKeys & Keys.Shift) != 0)
        {
          if (!TextArea.SelectionManager.HasSomethingSelected && realline != TextArea.Caret.Position.Y)
          {
            if (realline >= TextArea.Caret.Position.Y)
            { 
              // at or below starting selection, place the cursor on the next line
              // nothing is selected so make a new selection from cursor
              selectionStartPos = TextArea.Caret.Position;
              // whole line selection - start of line to start of next line
              if (realline < TextArea.Document.TotalNumberOfLines - 1)
              {
                TextArea.SelectionManager.SetSelection(new DefaultSelection(TextArea.Document, selectionStartPos, new TextLocation(0, realline + 1)));
                TextArea.Caret.Position = new TextLocation(0, realline + 1);
              }
              else
              {
                TextArea.SelectionManager.SetSelection(new DefaultSelection(TextArea.Document, selectionStartPos, new TextLocation(TextArea.Document.GetLineSegment(realline).Length + 1, realline)));
                TextArea.Caret.Position = new TextLocation(TextArea.Document.GetLineSegment(realline).Length + 1, realline);
              }
            }
            else
            { 
              // prior lines to starting selection, place the cursor on the same line as the new selection
              // nothing is selected so make a new selection from cursor
              selectionStartPos = TextArea.Caret.Position;
              // whole line selection - start of line to start of next line
              TextArea.SelectionManager.SetSelection(new DefaultSelection(TextArea.Document, selectionStartPos, new TextLocation(selectionStartPos.X, selectionStartPos.Y)));
              TextArea.SelectionManager.ExtendSelection(new TextLocation(selectionStartPos.X, selectionStartPos.Y), new TextLocation(0, realline));
              TextArea.Caret.Position = new TextLocation(0, realline);
            }
          }
          else
          {
            // let MouseMove handle a shift-click in a gutter
            MouseEventArgs e = new MouseEventArgs(mouseButtons, 1, mousepos.X, mousepos.Y, 0);
            TextArea.RaiseMouseMove(e);
          }
        }
        else
        { 
          // this is a new selection with no shift-key
          // sync the textareamousehandler mouse location
          // (fixes problem with clicking out into a menu then back to the gutter whilst
          // there is a selection)
          TextArea.MousePositionInternal = mousepos;

          selectionStartPos = new TextLocation(0, realline);
          TextArea.SelectionManager.ClearSelection();
          // whole line selection - start of line to start of next line
          if (realline < TextArea.Document.TotalNumberOfLines - 1)
          {
            TextArea.SelectionManager.SetSelection(new DefaultSelection(TextArea.Document, selectionStartPos, new TextLocation(selectionStartPos.X, selectionStartPos.Y + 1)));
            TextArea.Caret.Position = new TextLocation(selectionStartPos.X, selectionStartPos.Y + 1);
          }
          else
          {
            TextArea.SelectionManager.SetSelection(new DefaultSelection(TextArea.Document, new TextLocation(0, realline), new TextLocation(TextArea.Document.GetLineSegment(realline).Length + 1, selectionStartPos.Y)));
            TextArea.Caret.Position = new TextLocation(TextArea.Document.GetLineSegment(realline).Length + 1, selectionStartPos.Y);
          }
        }
      }
      base.OnMouseDown(eventArgs);
    }
  }
}
