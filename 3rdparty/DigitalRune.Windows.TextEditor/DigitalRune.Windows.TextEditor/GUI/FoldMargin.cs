using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Highlighting;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// The vertical strip next to the text that indicates the folds (collapsible regions) of the 
  /// text view.
  /// </summary>
  internal class FoldMargin : AbstractMargin
  {
    private int _selectedFoldLine = -1;


    /// <summary>
    /// Gets the size.
    /// </summary>
    /// <value>The size.</value>
    public override Size Size
    {
      get { return new Size(TextArea.TextView.LineHeight - 2, -1); }
    }


    /// <summary>
    /// Gets a value indicating whether this instance is visible.
    /// </summary>
    /// <value>
    /// 	<see langword="true"/> if this instance is visible; otherwise, <see langword="false"/>.
    /// </value>
    public override bool IsVisible
    {
      get { return TextArea.TextEditorProperties.EnableFolding; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="FoldMargin"/> class.
    /// </summary>
    /// <param name="TextArea">The text area.</param>
    public FoldMargin(TextArea TextArea)
      : base(TextArea)
    {
    }


    /// <summary>
    /// Raises the <see cref="AbstractMargin.Paint"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="PaintEventArgs"/> instance containing the event data.</param>
    protected override void OnPaint(PaintEventArgs eventArgs)
    {
      Graphics g = eventArgs.Graphics;
      Rectangle rect = eventArgs.ClipRectangle;

      if (rect.Width <= 0 || rect.Height <= 0)
        return;

      HighlightColor lineNumberPainterColor = TextArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");

      Color backgroundColor = TextArea.Enabled ? lineNumberPainterColor.BackgroundColor : SystemColors.InactiveBorder;
      Brush backgroundBrush = BrushRegistry.GetBrush(backgroundColor);

      int visibleLineDrawingRemainder = TextArea.TextView.RemainderOfFirstVisibleLine;
      int lineHeight = TextArea.TextView.LineHeight;

      for (int y = 0; y < (DrawingPosition.Height + visibleLineDrawingRemainder) / lineHeight + 1; ++y)
      {
        int markerX = DrawingPosition.X;
        int markerY = DrawingPosition.Top + y * lineHeight - visibleLineDrawingRemainder;
        int markerWidth = DrawingPosition.Width;
        int markerHeight = lineHeight;
        Rectangle markerRectangle = new Rectangle(markerX, markerY, markerWidth, markerHeight);

        if (rect.IntersectsWith(markerRectangle))
        {
          g.FillRectangle(backgroundBrush, markerRectangle);

          int currentLine = TextArea.Document.GetFirstLogicalLine(TextArea.TextView.FirstPhysicalLine + y);
          if (currentLine < TextArea.Document.TotalNumberOfLines)
          {
            markerRectangle.Width -= 1;
            PaintFoldMarker(g, currentLine, markerRectangle);
          }
        }
      }
      base.OnPaint(eventArgs);
    }


    bool SelectedFoldFrom(IList<Fold> list)
    {
      if (list != null)
        for (int i = 0; i < list.Count; ++i)
          if (_selectedFoldLine == list[i].StartLine)
            return true;

      return false;
    }


    void PaintFoldMarker(Graphics g, int lineNumber, Rectangle drawingRectangle)
    {
      HighlightColor foldLineColor = TextArea.Document.HighlightingStrategy.GetColorFor("FoldLine");
      HighlightColor selectedFoldLineColor = TextArea.Document.HighlightingStrategy.GetColorFor("SelectedFoldLine");

      List<Fold> foldingsWithStart = TextArea.Document.FoldingManager.GetFoldsWithStartAt(lineNumber);
      List<Fold> foldingsBetween = TextArea.Document.FoldingManager.GetFoldsContainingLine(lineNumber);
      List<Fold> foldingsWithEnd = TextArea.Document.FoldingManager.GetFoldsWithEndAt(lineNumber);

      bool isFoldStart = foldingsWithStart.Count > 0;
      bool isBetween = foldingsBetween.Count > 0;
      bool isFoldEnd = foldingsWithEnd.Count > 0;

      bool isStartSelected = SelectedFoldFrom(foldingsWithStart);
      bool isBetweenSelected = SelectedFoldFrom(foldingsBetween);
      bool isEndSelected = SelectedFoldFrom(foldingsWithEnd);

      int foldMarkerSize = (int) Math.Round(TextArea.TextView.LineHeight * 0.57f);
      foldMarkerSize -= (foldMarkerSize) % 2;
      int foldMarkerYPos = drawingRectangle.Y + (drawingRectangle.Height - foldMarkerSize) / 2;
      int xPos = drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2 + foldMarkerSize / 2;


      if (isFoldStart)
      {
        bool isVisible = true;
        bool moreLinedOpenFold = false;
        foreach (Fold fold in foldingsWithStart)
        {
          if (fold.IsFolded)
            isVisible = false;
          else
            moreLinedOpenFold = fold.EndLine > fold.StartLine;
        }

        bool isFoldEndFromUpperFold = false;
        foreach (Fold fold in foldingsWithEnd)
          if (fold.EndLine > fold.StartLine && !fold.IsFolded)
            isFoldEndFromUpperFold = true;

        RectangleF foldMarkerRectangle = new RectangleF(drawingRectangle.X + (drawingRectangle.Width - foldMarkerSize) / 2,
                                                    foldMarkerYPos,
                                                    foldMarkerSize,
                                                    foldMarkerSize);
        DrawFolds(g, foldMarkerRectangle, isVisible, isStartSelected);

        // draw line above fold marker
        if (isBetween || isFoldEndFromUpperFold)
        {
          Pen pen = BrushRegistry.GetPen(isBetweenSelected ? selectedFoldLineColor.Color : foldLineColor.Color);
          g.DrawLine(pen, xPos, drawingRectangle.Top, xPos, foldMarkerYPos - 1);
        }

        // draw line below fold marker
        if (isBetween || moreLinedOpenFold)
        {
          bool isSelected = isEndSelected || (isStartSelected && isVisible) || isBetweenSelected;
          Color color = (isSelected) ? selectedFoldLineColor.Color : foldLineColor.Color;
          Pen pen = BrushRegistry.GetPen(color);
          g.DrawLine(pen, xPos, foldMarkerYPos + foldMarkerSize + 1, xPos, drawingRectangle.Bottom);
        }
      }
      else if (isFoldEnd)
      {
        int midy = drawingRectangle.Top + drawingRectangle.Height / 2;

        // draw fold end marker
        Color color = isEndSelected ? selectedFoldLineColor.Color : foldLineColor.Color;
        Pen pen = BrushRegistry.GetPen(color);
        g.DrawLine(pen, xPos, midy, xPos + foldMarkerSize / 2, midy);

        // draw line above fold end marker
        // must be drawn after fold marker because it might have a different color than the fold marker
        color = (isBetweenSelected || isEndSelected) ? selectedFoldLineColor.Color : foldLineColor.Color;
        pen = BrushRegistry.GetPen(color);
        g.DrawLine(pen, xPos, drawingRectangle.Top, xPos, midy);

        // draw line below fold end marker
        if (isBetween)
        {
          color = isBetweenSelected ? selectedFoldLineColor.Color : foldLineColor.Color;
          pen = BrushRegistry.GetPen(color);
          g.DrawLine(pen, xPos, midy + 1, xPos, drawingRectangle.Bottom);
        }
      }
      else if (isBetween)
      {
        // just draw the line :)
        Color color = isBetweenSelected ? selectedFoldLineColor.Color : foldLineColor.Color;
        Pen pen = BrushRegistry.GetPen(color);
        g.DrawLine(pen, xPos, drawingRectangle.Top, xPos, drawingRectangle.Bottom);
      }
    }


    /// <summary>
    /// Raises the <see cref="AbstractMargin.MouseMove"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    protected override void OnMouseMove(MouseEventArgs eventArgs)      
    {
      Point mousepos = eventArgs.Location;
      bool showFolding = TextArea.Document.TextEditorProperties.EnableFolding;
      int physicalLine = (mousepos.Y + TextArea.VirtualTop.Y) / TextArea.TextView.LineHeight;
      int realline = TextArea.Document.GetFirstLogicalLine(physicalLine);

      if (!showFolding || realline < 0 || realline + 1 >= TextArea.Document.TotalNumberOfLines)
        return;

      List<Fold> foldMarkers = TextArea.Document.FoldingManager.GetFoldsWithStartAt(realline);
      int oldSelection = _selectedFoldLine;

      if (foldMarkers.Count > 0)
        _selectedFoldLine = realline;
      else
        _selectedFoldLine = -1;

      if (oldSelection != _selectedFoldLine)
        TextArea.Invalidate(DrawingPosition);

      base.OnMouseMove(eventArgs);
    }


    /// <summary>
    /// Raises the <see cref="AbstractMargin.MouseDown"/> event.
    /// </summary>
    /// <param name="eventArgs">The <see cref="System.Windows.Forms.MouseEventArgs"/> instance containing the event data.</param>
    protected override void OnMouseDown(MouseEventArgs eventArgs)
    {
      Point mousepos = eventArgs.Location;
      bool showFolding = TextArea.Document.TextEditorProperties.EnableFolding;
      int physicalLine = ((mousepos.Y + TextArea.VirtualTop.Y) / TextArea.TextView.LineHeight);
      int realline = TextArea.Document.GetFirstLogicalLine(physicalLine);

      // focus the textarea if the user clicks on the line number view
      TextArea.Focus();

      if (!showFolding || realline < 0 || realline + 1 >= TextArea.Document.TotalNumberOfLines)
        return;

      List<Fold> folds = TextArea.Document.FoldingManager.GetFoldsWithStartAt(realline);
      foreach (Fold fold in folds)
        fold.IsFolded = !fold.IsFolded;

      TextArea.Document.FoldingManager.NotifyFoldingChanged(EventArgs.Empty);
      base.OnMouseDown(eventArgs);
    }


    /// <summary>
    /// Raises the <see cref="AbstractMargin.MouseLeave"/> event.
    /// </summary>
    /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
    protected override void OnMouseLeave(EventArgs e)
    {
      if (_selectedFoldLine != -1)
      {
        _selectedFoldLine = -1;
        TextArea.Invalidate(DrawingPosition);
      }
      base.OnMouseLeave(e);
    }


    void DrawFolds(Graphics g, RectangleF rectangle, bool isOpened, bool isSelected)
    {
      HighlightColor foldMarkerColor = TextArea.Document.HighlightingStrategy.GetColorFor("FoldMarker");
      HighlightColor foldLineColor = TextArea.Document.HighlightingStrategy.GetColorFor("FoldLine");
      HighlightColor selectedFoldLine = TextArea.Document.HighlightingStrategy.GetColorFor("SelectedFoldLine");

      Color foregroundColor = isSelected ? selectedFoldLine.Color : foldLineColor.Color;
      Color backgroundColor = foldMarkerColor.BackgroundColor;

      Pen foldLinePen = BrushRegistry.GetPen(foregroundColor);
      Pen foldMarkerPen = BrushRegistry.GetPen(foldMarkerColor.Color);
      Brush backgroundBrush = BrushRegistry.GetBrush(backgroundColor);

      Rectangle intRect = new Rectangle((int) rectangle.X, (int) rectangle.Y, (int) rectangle.Width, (int) rectangle.Height);
      g.FillRectangle(backgroundBrush, intRect);
      g.DrawRectangle(foldLinePen, intRect);

      int space = (int) Math.Round(rectangle.Height / 8.0) + 1;
      int mid = intRect.Height / 2 + intRect.Height % 2;

      // Draw horizontal line (-)
      g.DrawLine(foldMarkerPen,
                 rectangle.X + space,
                 rectangle.Y + mid,
                 rectangle.X + rectangle.Width - space,
                 rectangle.Y + mid);

      if (!isOpened)
      {
        // Draw vertical line (+)
        g.DrawLine(foldMarkerPen, rectangle.X + mid, rectangle.Y + space, rectangle.X + mid, rectangle.Y + rectangle.Height - space);
      }
    }
  }
}
