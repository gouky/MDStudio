using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Folding;
using DigitalRune.Windows.TextEditor.Highlighting;
using DigitalRune.Windows.TextEditor.Markers;
using DigitalRune.Windows.TextEditor.Selection;
using DigitalRune.Windows.TextEditor.Properties;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// Renders the text part of the text area.
  /// </summary>
  internal class TextView : AbstractMargin, IDisposable
  {
    private int _fontHeight;
    private Highlight _highlight;
    private int _physicalColumn;  // used for calculating physical column during paint
    private int _spaceWidth;
    private int _columnWidth;
    private Font _lastFont;
    private const int _additionalFoldTextSize = 1;


    /// <summary>
    /// Gets or sets a pair of highlighted brackets.
    /// </summary>
    /// <value>The a pair highlighted brackets.</value>
    public Highlight Highlight
    {
      get { return _highlight; }
      set { _highlight = value; }
    }


    /// <summary>
    /// Gets the height of a single line.
    /// </summary>
    /// <value>The height of a single line.</value>
    /// <remarks>
    /// Usually the same as <see cref="FontHeight"/>.
    /// </remarks>
    public int LineHeight
    {
      get { return _fontHeight; }
    }


    /// <summary>
    /// Gets the height of the font.
    /// </summary>
    /// <value>The height of the font.</value>
    public int FontHeight
    {
      get { return _fontHeight; }
    }


    /// <summary>
    /// Gets the width of a space character.
    /// </summary>
    /// <remarks>
    /// This value can be quite small in some fonts - consider using <see cref="ColumnWidth"/> instead.
    /// </remarks>
    public int SpaceWidth
    {
      get { return _spaceWidth; }
    }


    /// <summary>
    /// Gets the width of a column.
    /// </summary>
    /// <remarks>
    /// On mono-spaced fonts, this is the same value as <see cref="SpaceWidth"/>. On variable-spaced
    /// fonts, the <see cref="ColumnWidth"/> can be different to the <see cref="SpaceWidth"/>.
    /// The columns width determines the size of a tab. For example: tab = 4 * column-width.
    /// </remarks>
    public int ColumnWidth
    {
      get { return _columnWidth; }
    }


    /// <summary>
    /// Gets the number of visible lines.
    /// </summary>
    /// <value>The number of visible lines.</value>
    public int NumberOfVisibleLines
    {
      get { return (DrawingPosition.Height / LineHeight) + 1; }
    }


    /// <summary>
    /// Gets the number of visible columns.
    /// </summary>
    /// <value>The number visible columns.</value>
    public int NumberOfVisibleColumns
    {
      get { return (DrawingPosition.Width / ColumnWidth) + 1; }
    }


    /// <summary>
    /// Gets the first visible <b>physical</b> line.
    /// </summary>
    /// <value>The first visible <b>physical</b> line.</value>
    /// <remarks>
    /// <para>
    /// A physical line is line that is rendered in the text editor, whereas a logical line is a 
    /// line in the document. A physical line can contain multiple logical lines, for example when
    /// several files are folded (collapsed) into one.
    /// </para>
    /// <para>
    /// The line numbers match the logical numbers: line number = (logical line + 1)
    /// </para>
    /// </remarks>
    public int FirstPhysicalLine
    {
      get { return TextArea.VirtualTop.Y / LineHeight; }
    }

   
    /// <summary>
    /// Gets or sets the first visible <b>logical</b> line.
    /// </summary>
    /// <value>The first visible line.</value>
    /// <para>
    /// A logical line is a line in the document, whereas a physical line is line that is rendered 
    /// in the text editor. A physical line can contain multiple logical lines, for example when
    /// several files are folded (collapsed) into one.
    /// </para>
    /// <para>
    /// The line numbers match the logical numbers: line number = (logical line + 1)
    /// </para>
    public int FirstLogicalLine
    {
      get { return TextArea.Document.GetFirstLogicalLine(TextArea.VirtualTop.Y / LineHeight); }
      set
      {
        if (FirstLogicalLine != value)
          TextArea.VirtualTop = new Point(TextArea.VirtualTop.X, TextArea.Document.GetVisibleLine(value) * LineHeight);
      }
    }


    /// <summary>
    /// Gets the remainder of the top, visible line that lies outside of the visible area.
    /// </summary>
    /// <value>The remainder of the top, visible line that lies outside of the visible area.</value>
    public int RemainderOfFirstVisibleLine
    {
      get { return TextArea.VirtualTop.Y % LineHeight; }
    }


    /// <summary>
    /// Initializes a new instance of the <see cref="TextView"/> class.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public TextView(TextArea textArea)
      : base(textArea)
    {
      base.Cursor = Cursors.IBeam;
      OptionsChanged();
    }


    /// <summary>
    /// Handles changes of the text editor properties.
    /// </summary>
    public void OptionsChanged()
    {
      _lastFont = TextEditorProperties.FontContainer.RegularFont;
      _fontHeight = GetFontHeight(_lastFont);

      // Calculate width of a space character.
      // Use minimum width - in some fonts, space has no width but kerning is used instead
      // -> DivideByZeroException.
      _spaceWidth = Math.Max(GetWidth(' ', _lastFont), 1);

      // Calculate width of a column.
      // Tabs should have the width of 4 * 'x'.
      _columnWidth = Math.Max(_spaceWidth, GetWidth('x', _lastFont));
    }


    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
      _measureCache.Clear();
    }


    private static int GetFontHeight(Font font)
    {
      int measureHeight = TextRenderer.MeasureText("_", font).Height;
      int definedHeight = (int) Math.Ceiling(font.GetHeight());
      return Math.Max(measureHeight, definedHeight);
    }


    #region Paint functions
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

      // Just to ensure that fontHeight and char widths are always correct...
      if (_lastFont != TextEditorProperties.FontContainer.RegularFont)
      {
        OptionsChanged();
        TextArea.Invalidate();
      }

      int horizontalDelta = TextArea.VirtualTop.X;
      if (horizontalDelta > 0)
        g.SetClip(DrawingPosition);

      for (int y = 0; y < (DrawingPosition.Height + RemainderOfFirstVisibleLine) / LineHeight + 1; ++y)
      {
        Rectangle lineRectangle = new Rectangle(DrawingPosition.X - horizontalDelta,
                                                DrawingPosition.Top + y * LineHeight - RemainderOfFirstVisibleLine,
                                                DrawingPosition.Width + horizontalDelta,
                                                LineHeight);

        if (rect.IntersectsWith(lineRectangle))
        {
          int currentLine = TextArea.Document.GetFirstLogicalLine(TextArea.Document.GetVisibleLine(FirstLogicalLine) + y);
          PaintDocumentLine(g, currentLine, lineRectangle);
        }
      }

      DrawTextMarkers(g);

      if (horizontalDelta > 0)
        g.ResetClip();

      base.OnPaint(eventArgs);
    }


    void PaintDocumentLine(Graphics g, int lineNumber, Rectangle lineRectangle)
    {
      Debug.Assert(lineNumber >= 0);
      Brush bgColorBrush = GetBgColorBrush(lineNumber);
      Brush backgroundBrush = TextArea.Enabled ? bgColorBrush : SystemBrushes.InactiveBorder;

      if (lineNumber >= TextArea.Document.TotalNumberOfLines)
      {
        g.FillRectangle(backgroundBrush, lineRectangle);

        if (TextEditorProperties.ShowInvalidLines)
          DrawInvalidLineMarker(g, lineRectangle.Left, lineRectangle.Top);

        if (TextEditorProperties.ShowVerticalRuler)
          DrawVerticalRuler(g, lineRectangle);

        return;
      }

      int physicalXPos = lineRectangle.X;
      // there can't be a folding which starts in an above line and ends here, because the line is a new one,
      // there must be a return before this line.
      int column = 0;
      _physicalColumn = 0;
      if (TextEditorProperties.EnableFolding)
      {
        while (true)
        {
          List<Fold> starts = TextArea.Document.FoldingManager.GetFoldedFoldsWithStartAfterColumn(lineNumber, column - 1);
          if (starts == null || starts.Count <= 0)
          {
            if (lineNumber < TextArea.Document.TotalNumberOfLines)
            {
              physicalXPos = PaintLinePart(g, lineNumber, column, TextArea.Document.GetLineSegment(lineNumber).Length, lineRectangle, physicalXPos);
            }
            break;
          }

          // search the first starting folding
          Fold firstFolding = starts[0];
          foreach (Fold fold in starts)
          {
            if (fold.StartColumn < firstFolding.StartColumn)
            {
              firstFolding = fold;
            }
          }
          starts.Clear();

          physicalXPos = PaintLinePart(g, lineNumber, column, firstFolding.StartColumn, lineRectangle, physicalXPos);
          column = firstFolding.EndColumn;
          lineNumber = firstFolding.EndLine;
          if (lineNumber >= TextArea.Document.TotalNumberOfLines)
          {
            Debug.Assert(false, "Folding ends after document end");
            break;
          }

          ColumnRange selectionRange2 = TextArea.SelectionManager.GetSelectionAtLine(lineNumber);
          bool drawSelected = ColumnRange.WholeColumn.Equals(selectionRange2) || firstFolding.StartColumn >= selectionRange2.StartColumn && firstFolding.EndColumn <= selectionRange2.EndColumn;

          physicalXPos = PaintFoldingText(g, lineNumber, physicalXPos, lineRectangle, firstFolding.FoldText, drawSelected);
        }
      }
      else
      {
        physicalXPos = PaintLinePart(g, lineNumber, 0, TextArea.Document.GetLineSegment(lineNumber).Length, lineRectangle, physicalXPos);
      }

      if (lineNumber < TextArea.Document.TotalNumberOfLines)
      {
        // Paint things after end of line
        ColumnRange selectionRange = TextArea.SelectionManager.GetSelectionAtLine(lineNumber);
        LineSegment currentLine = TextArea.Document.GetLineSegment(lineNumber);
        HighlightColor selectionColor = TextArea.Focused ? TextArea.Document.HighlightingStrategy.GetColorFor("Selection") : TextArea.Document.HighlightingStrategy.GetColorFor("SelectionInactive");

        bool selectionBeyondEOL = selectionRange.EndColumn > currentLine.Length || ColumnRange.WholeColumn.Equals(selectionRange);

        if (TextEditorProperties.ShowEOLMarker)
        {
          HighlightColor eolMarkerColor = TextArea.Document.HighlightingStrategy.GetColorFor("EOLMarkers");
          physicalXPos += DrawEOLMarker(g, eolMarkerColor.Color, selectionBeyondEOL ? bgColorBrush : backgroundBrush, physicalXPos, lineRectangle.Y);
        }
        else
        {
          if (selectionBeyondEOL)
          {
            g.FillRectangle(BrushRegistry.GetBrush(selectionColor.BackgroundColor), new RectangleF(physicalXPos, lineRectangle.Y, ColumnWidth, lineRectangle.Height));
            physicalXPos += ColumnWidth;
          }
        }

        Brush fillBrush = selectionBeyondEOL && TextEditorProperties.AllowCaretBeyondEOL ? bgColorBrush : backgroundBrush;
        g.FillRectangle(fillBrush, new RectangleF(physicalXPos, lineRectangle.Y, lineRectangle.Width - physicalXPos + lineRectangle.X, lineRectangle.Height));
      }

      if (TextEditorProperties.ShowVerticalRuler)
        DrawVerticalRuler(g, lineRectangle);
    }


    bool DrawLineMarkerAtLine(int lineNumber)
    {
      return lineNumber == TextArea.Caret.Line && TextArea.MotherTextAreaControl.TextEditorProperties.LineViewerStyle == LineViewerStyle.FullRow;
    }


    Brush GetBgColorBrush(int lineNumber)
    {
      if (DrawLineMarkerAtLine(lineNumber))
      {
        HighlightColor caretLine = TextArea.Document.HighlightingStrategy.GetColorFor("CaretMarker");
        return BrushRegistry.GetBrush(caretLine.Color);
      }
      HighlightColor background = TextArea.Document.HighlightingStrategy.GetColorFor("Default");
      Color bgColor = background.BackgroundColor;
      return BrushRegistry.GetBrush(bgColor);
    }


    int PaintFoldingText(Graphics g, int lineNumber, int physicalXPos, Rectangle lineRectangle, string text, bool drawSelected)
    {
      // TODO: get font and color from the highlighting file
      HighlightColor selectionColor = TextArea.Focused ? TextArea.Document.HighlightingStrategy.GetColorFor("Selection") : TextArea.Document.HighlightingStrategy.GetColorFor("SelectionInactive");
      Brush bgColorBrush = drawSelected ? BrushRegistry.GetBrush(selectionColor.BackgroundColor) : GetBgColorBrush(lineNumber);
      Brush backgroundBrush = TextArea.Enabled ? bgColorBrush : SystemBrushes.InactiveBorder;

      Font font = TextArea.TextEditorProperties.FontContainer.RegularFont;

      int wordWidth = MeasureStringWidth(g, text, font) + _additionalFoldTextSize;
      Rectangle rect = new Rectangle(physicalXPos, lineRectangle.Y, wordWidth, lineRectangle.Height - 1);

      g.FillRectangle(backgroundBrush, rect);

      _physicalColumn += text.Length;
      DrawString(g, text, font, drawSelected ? selectionColor.Color : Color.Gray, rect.X + 1, rect.Y);
      g.DrawRectangle(BrushRegistry.GetPen(drawSelected ? Color.DarkGray : Color.Gray), rect.X, rect.Y, rect.Width, rect.Height);

      return physicalXPos + wordWidth + 1;
    }


    private struct MarkerToDraw
    {
      public readonly Marker Marker;
      public readonly RectangleF DrawingRectangle;

      public MarkerToDraw(Marker marker, RectangleF drawingRectangle)
      {
        Marker = marker;
        DrawingRectangle = drawingRectangle;
      }
    }


    private readonly List<MarkerToDraw> _markersToDraw = new List<MarkerToDraw>();


    void DrawTextMarker(Graphics g, Marker marker, RectangleF drawingRect)
    {
      // draw markers later so they can overdraw the following text
      _markersToDraw.Add(new MarkerToDraw(marker, drawingRect));
    }


    void DrawTextMarkers(Graphics g)
    {
      foreach (MarkerToDraw m in _markersToDraw)
      {
        Marker marker = m.Marker;
        RectangleF drawingRect = m.DrawingRectangle;
        float drawYPos = drawingRect.Bottom - 1;
        switch (marker.MarkerType)
        {
          case MarkerType.Underlined:
            g.DrawLine(BrushRegistry.GetPen(marker.Color), drawingRect.X, drawYPos, drawingRect.Right, drawYPos);
            break;
          case MarkerType.WaveLine:
            int reminder = ((int) drawingRect.X) % 4;
            for (float i = (int) drawingRect.X - reminder; i < drawingRect.Right; i += 4)
            {
              g.DrawLine(BrushRegistry.GetPen(marker.Color), i, drawYPos + 3 - 4, i + 2, drawYPos + 1 - 4);
              if (i + 2 < drawingRect.Right)
                g.DrawLine(BrushRegistry.GetPen(marker.Color), i + 2, drawYPos + 1 - 4, i + 4, drawYPos + 3 - 4);
            }
            break;
          case MarkerType.SolidBlock:
            g.FillRectangle(BrushRegistry.GetBrush(marker.Color), drawingRect);
            break;
        }
      }
      _markersToDraw.Clear();
    }


    /// <summary>
    /// Get the marker brush (for solid block markers) at a given position.
    /// </summary>
    /// <param name="offset">The offset.</param>
    /// <param name="length">The length.</param>
    /// <param name="foreColor">The foreground color.</param>
    /// <param name="markers">All markers that have been found.</param>
    /// <returns>
    /// The Brush or null when no marker was found.
    /// </returns>
    Brush GetMarkerBrushAt(int offset, int length, ref Color foreColor, out IList<Marker> markers)
    {
      markers = Document.MarkerStrategy.GetMarkers(offset, length);
      foreach (Marker marker in markers)
      {
        if (marker.MarkerType == MarkerType.SolidBlock)
        {
          if (marker.OverrideTextColor)
            foreColor = marker.TextColor;

          return BrushRegistry.GetBrush(marker.Color);
        }
      }
      return null;
    }


    int PaintLinePart(Graphics g, int lineNumber, int startColumn, int endColumn, Rectangle lineRectangle, int physicalXPos)
    {
      bool drawLineMarker = DrawLineMarkerAtLine(lineNumber);
      Brush backgroundBrush = TextArea.Enabled ? GetBgColorBrush(lineNumber) : SystemBrushes.InactiveBorder;

      HighlightColor selectionColor = TextArea.Focused ? TextArea.Document.HighlightingStrategy.GetColorFor("Selection") : TextArea.Document.HighlightingStrategy.GetColorFor("SelectionInactive");
      ColumnRange selectionRange = TextArea.SelectionManager.GetSelectionAtLine(lineNumber);
      HighlightColor tabMarkerColor = TextArea.Document.HighlightingStrategy.GetColorFor("TabMarkers");
      HighlightColor spaceMarkerColor = TextArea.Document.HighlightingStrategy.GetColorFor("SpaceMarkers");

      LineSegment currentLine = TextArea.Document.GetLineSegment(lineNumber);

      Brush selectionBackgroundBrush = BrushRegistry.GetBrush(selectionColor.BackgroundColor);

      if (currentLine.Words == null)
        return physicalXPos;

      int currentWordOffset = 0; // we cannot use currentWord.Offset because it is not set on space words

      TextWord currentWord;
      TextWord nextCurrentWord = null;
      FontContainer fontContainer = TextEditorProperties.FontContainer;
      for (int wordIdx = 0; wordIdx < currentLine.Words.Count; wordIdx++)
      {
        currentWord = currentLine.Words[wordIdx];
        if (currentWordOffset < startColumn)
        {
          // TODO: maybe we need to split at startColumn when we support fold markers
          // inside words
          currentWordOffset += currentWord.Length;
          continue;
        }
      repeatDrawCurrentWord:
        //physicalXPos += 10; // leave room between drawn words - useful for debugging the drawing code
        if (currentWordOffset >= endColumn || physicalXPos >= lineRectangle.Right)
        {
          break;
        }
        int currentWordEndOffset = currentWordOffset + currentWord.Length - 1;
        TextWordType currentWordType = currentWord.Type;

        IList<Marker> markers;
        Color wordForeColor;
        if (currentWordType == TextWordType.Space)
          wordForeColor = spaceMarkerColor.Color;
        else if (currentWordType == TextWordType.Tab)
          wordForeColor = tabMarkerColor.Color;
        else
          wordForeColor = currentWord.Color;
        Brush wordBackBrush = GetMarkerBrushAt(currentLine.Offset + currentWordOffset, currentWord.Length, ref wordForeColor, out markers);

        // It is possible that we have to split the current word because a marker/the selection begins/ends inside it
        if (currentWord.Length > 1)
        {
          int splitPos = int.MaxValue;
          if (_highlight != null)
          {
            // split both before and after highlight
            if (_highlight.OpeningBrace.Y == lineNumber)
            {
              if (_highlight.OpeningBrace.X >= currentWordOffset && _highlight.OpeningBrace.X <= currentWordEndOffset)
              {
                splitPos = Math.Min(splitPos, _highlight.OpeningBrace.X - currentWordOffset);
              }
            }
            if (_highlight.ClosingBrace.Y == lineNumber)
            {
              if (_highlight.ClosingBrace.X >= currentWordOffset && _highlight.ClosingBrace.X <= currentWordEndOffset)
              {
                splitPos = Math.Min(splitPos, _highlight.ClosingBrace.X - currentWordOffset);
              }
            }
            if (splitPos == 0)
            {
              splitPos = 1; // split after highlight
            }
          }
          if (endColumn < currentWordEndOffset)
          { // split when endColumn is reached
            splitPos = Math.Min(splitPos, endColumn - currentWordOffset);
          }
          if (selectionRange.StartColumn > currentWordOffset && selectionRange.StartColumn <= currentWordEndOffset)
          {
            splitPos = Math.Min(splitPos, selectionRange.StartColumn - currentWordOffset);
          }
          else if (selectionRange.EndColumn > currentWordOffset && selectionRange.EndColumn <= currentWordEndOffset)
          {
            splitPos = Math.Min(splitPos, selectionRange.EndColumn - currentWordOffset);
          }
          foreach (Marker marker in markers)
          {
            int markerColumn = marker.Offset - currentLine.Offset;
            int markerEndColumn = marker.EndOffset - currentLine.Offset + 1; // make end offset exclusive
            if (markerColumn > currentWordOffset && markerColumn <= currentWordEndOffset)
            {
              splitPos = Math.Min(splitPos, markerColumn - currentWordOffset);
            }
            else if (markerEndColumn > currentWordOffset && markerEndColumn <= currentWordEndOffset)
            {
              splitPos = Math.Min(splitPos, markerEndColumn - currentWordOffset);
            }
          }
          if (splitPos != int.MaxValue)
          {
            if (nextCurrentWord != null)
              throw new ApplicationException("split part invalid: first part cannot be splitted further");

            nextCurrentWord = TextWord.Split(ref currentWord, splitPos);
            goto repeatDrawCurrentWord; // get markers for first word part
          }
        }

        // get colors from selection status:
        if (ColumnRange.WholeColumn.Equals(selectionRange) || (selectionRange.StartColumn <= currentWordOffset
                                                               && selectionRange.EndColumn > currentWordEndOffset))
        {
          // word is completely selected
          wordBackBrush = selectionBackgroundBrush;
          if (selectionColor.HasForeground)
          {
            wordForeColor = selectionColor.Color;
          }
        }
        else if (drawLineMarker)
        {
          wordBackBrush = backgroundBrush;
        }

        if (wordBackBrush == null)
        { // use default background if no other background is set
          if (currentWord.SyntaxColor != null && currentWord.SyntaxColor.HasBackground)
            wordBackBrush = BrushRegistry.GetBrush(currentWord.SyntaxColor.BackgroundColor);
          else
            wordBackBrush = backgroundBrush;
        }

        RectangleF wordRectangle;

        if (currentWord.Type == TextWordType.Space)
        {
          ++_physicalColumn;

          wordRectangle = new RectangleF(physicalXPos, lineRectangle.Y, SpaceWidth, lineRectangle.Height);
          g.FillRectangle(wordBackBrush, wordRectangle);

          if (TextEditorProperties.ShowSpaces)
          {
            DrawSpaceMarker(g, wordForeColor, physicalXPos, lineRectangle.Y);
          }
          physicalXPos += SpaceWidth;
        }
        else if (currentWord.Type == TextWordType.Tab)
        {

          _physicalColumn += TextEditorProperties.TabIndent;
          _physicalColumn = (_physicalColumn / TextEditorProperties.TabIndent) * TextEditorProperties.TabIndent;
          // go to next tabstop
          int physicalTabEnd = ((physicalXPos + MinTabWidth - lineRectangle.X)
                                / ColumnWidth / TextEditorProperties.TabIndent)
            * ColumnWidth * TextEditorProperties.TabIndent + lineRectangle.X;
          physicalTabEnd += ColumnWidth * TextEditorProperties.TabIndent;

          wordRectangle = new RectangleF(physicalXPos, lineRectangle.Y, physicalTabEnd - physicalXPos, lineRectangle.Height);
          g.FillRectangle(wordBackBrush, wordRectangle);

          if (TextEditorProperties.ShowTabs)
          {
            DrawTabMarker(g, wordForeColor, physicalXPos, lineRectangle.Y);
          }
          physicalXPos = physicalTabEnd;
        }
        else
        {
          int wordWidth = DrawDocumentWord(g,
                                           currentWord.Word,
                                           new Point(physicalXPos, lineRectangle.Y),
                                           currentWord.GetFont(fontContainer),
                                           wordForeColor,
                                           wordBackBrush);
          wordRectangle = new RectangleF(physicalXPos, lineRectangle.Y, wordWidth, lineRectangle.Height);
          physicalXPos += wordWidth;
        }
        foreach (Marker marker in markers)
        {
          if (marker.MarkerType != MarkerType.SolidBlock)
          {
            DrawTextMarker(g, marker, wordRectangle);
          }
        }

        // draw bracket highlight
        if (_highlight != null)
        {
          if (_highlight.OpeningBrace.Y == lineNumber && _highlight.OpeningBrace.X == currentWordOffset ||
              _highlight.ClosingBrace.Y == lineNumber && _highlight.ClosingBrace.X == currentWordOffset)
          {
            DrawBracketHighlight(g, new Rectangle((int) wordRectangle.X, lineRectangle.Y, (int) wordRectangle.Width - 1, lineRectangle.Height - 1));
          }
        }

        currentWordOffset += currentWord.Length;
        if (nextCurrentWord != null)
        {
          currentWord = nextCurrentWord;
          nextCurrentWord = null;
          goto repeatDrawCurrentWord;
        }
      }
      if (physicalXPos < lineRectangle.Right && endColumn >= currentLine.Length)
      {
        // draw markers at line end
        IList<Marker> markers = Document.MarkerStrategy.GetMarkers(currentLine.Offset + currentLine.Length);
        foreach (Marker marker in markers)
        {
          if (marker.MarkerType != MarkerType.SolidBlock)
          {
            DrawTextMarker(g, marker, new RectangleF(physicalXPos, lineRectangle.Y, ColumnWidth, lineRectangle.Height));
          }
        }
      }
      return physicalXPos;
    }


    int DrawDocumentWord(Graphics g, string word, Point position, Font font, Color foreColor, Brush backBrush)
    {
      if (String.IsNullOrEmpty(word))
        return 0;

      if (word.Length > MaximumWordLength)
      {
        int width = 0;
        for (int i = 0; i < word.Length; i += MaximumWordLength)
        {
          Point pos = position;
          pos.X += width;
          if (i + MaximumWordLength < word.Length)
            width += DrawDocumentWord(g, word.Substring(i, MaximumWordLength), pos, font, foreColor, backBrush);
          else
            width += DrawDocumentWord(g, word.Substring(i, word.Length - i), pos, font, foreColor, backBrush);
        }
        return width;
      }

      int wordWidth = MeasureStringWidth(g, word, font);

      g.FillRectangle(backBrush, new RectangleF(position.X, position.Y, wordWidth + 1, LineHeight));

      DrawString(g, word, font, foreColor, position.X, position.Y);
      return wordWidth;
    }


    private struct WordFontPair
    {
      public readonly string Word;
      public readonly Font Font;

      public WordFontPair(string word, Font font)
      {
        Word = word;
        Font = font;
      }

      public override bool Equals(object obj)
      {
        WordFontPair myWordFontPair = (WordFontPair) obj;
        if (!Word.Equals(myWordFontPair.Word)) return false;
        return Font.Equals(myWordFontPair.Font);
      }

      public override int GetHashCode()
      {
        return Word.GetHashCode() ^ Font.GetHashCode();
      }
    }


    private readonly Dictionary<WordFontPair, int> _measureCache = new Dictionary<WordFontPair, int>();

    // split words after 1000 characters. Fixes GDI+ crash on very longs words, for example
    // a 100 KB Base64-file without any line breaks.
    const int MaximumWordLength = 1000;
    const int MaximumCacheSize = 2000;


    int MeasureStringWidth(Graphics g, string word, Font font)
    {
      int width;

      if (word == null || word.Length == 0)
        return 0;

      if (word.Length > MaximumWordLength)
      {
        width = 0;
        for (int i = 0; i < word.Length; i += MaximumWordLength)
        {
          if (i + MaximumWordLength < word.Length)
            width += MeasureStringWidth(g, word.Substring(i, MaximumWordLength), font);
          else
            width += MeasureStringWidth(g, word.Substring(i, word.Length - i), font);
        }
        return width;
      }

      if (_measureCache.TryGetValue(new WordFontPair(word, font), out width))
        return width;

      if (_measureCache.Count > MaximumCacheSize)
        _measureCache.Clear();

      // This code here provides better results than MeasureString!
      // Example line that is measured wrong:
      // txt.GetPositionFromCharIndex(txt.SelectionStart)
      // (Verdana 10, highlighting makes GetP... bold) -> note the space between 'x' and '('
      // this also fixes "jumping" characters when selecting in non-monospace fonts
      // [...]
      // Replaced GDI+ measurement with GDI measurement: faster and even more exact
      width = TextRenderer.MeasureText(g, word, font, new Size(short.MaxValue, short.MaxValue), textFormatFlags).Width;
      _measureCache.Add(new WordFontPair(word, font), width);
      return width;
    }


    // Important: Some flags combinations work on WinXP, but not on Win2000.
    // Make sure to test changes here on all operating systems.
    const TextFormatFlags textFormatFlags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.PreserveGraphicsClipping;
    #endregion


    #region Conversion Functions
    private readonly Dictionary<Font, Dictionary<char, int>> _fontBoundCharWidth = new Dictionary<Font, Dictionary<char, int>>();


    /// <summary>
    /// Gets the width of a character.
    /// </summary>
    /// <param name="ch">The character.</param>
    /// <param name="font">The font.</param>
    /// <returns>The width in pixel.</returns>
    public int GetWidth(char ch, Font font)
    {
      if (!_fontBoundCharWidth.ContainsKey(font))
      {
        _fontBoundCharWidth.Add(font, new Dictionary<char, int>());
      }
      if (!_fontBoundCharWidth[font].ContainsKey(ch))
      {
        using (Graphics g = TextArea.CreateGraphics())
        {
          return GetWidth(g, ch, font);
        }
      }
      return _fontBoundCharWidth[font][ch];
    }


    /// <summary>
    /// Gets the width of a character.
    /// </summary>
    /// <param name="g">The <see cref="Graphics"/> context.</param>
    /// <param name="ch">The character.</param>
    /// <param name="font">The font.</param>
    /// <returns>The width of the character in pixel.</returns>
    public int GetWidth(Graphics g, char ch, Font font)
    {
      if (!_fontBoundCharWidth.ContainsKey(font))
        _fontBoundCharWidth.Add(font, new Dictionary<char, int>());

      if (!_fontBoundCharWidth[font].ContainsKey(ch))
        _fontBoundCharWidth[font].Add(ch, MeasureStringWidth(g, ch.ToString(), font));

      return _fontBoundCharWidth[font][ch];
    }


    /// <summary>
    /// Counts the visual columns.
    /// </summary>
    /// <param name="logicalLine">The logical line.</param>
    /// <param name="logicalColumn">The logical column.</param>
    /// <returns>The number of visual columns.</returns>
    public int GetVisualColumn(int logicalLine, int logicalColumn)
    {
      int column = 0;
      using (Graphics g = TextArea.CreateGraphics())
      {
        CountColumns(ref column, 0, logicalColumn, logicalLine, g);
      }
      return column;
    }


    /// <summary>
    /// Counts the visual columns (fast version).
    /// </summary>
    /// <param name="line">The line.</param>
    /// <param name="logicalColumn">The logical column.</param>
    /// <returns>The number of visual columns.</returns>
    public int GetVisualColumnFast(LineSegment line, int logicalColumn)
    {
      int lineOffset = line.Offset;
      int tabIndent = Document.TextEditorProperties.TabIndent;
      int guessedColumn = 0;
      for (int i = 0; i < logicalColumn; ++i)
      {
        char ch = (i >= line.Length) ? ' ' : Document.GetCharAt(lineOffset + i);
        switch (ch)
        {
          case '\t':
            guessedColumn += tabIndent;
            guessedColumn = (guessedColumn / tabIndent) * tabIndent;
            break;
          default:
            ++guessedColumn;
            break;
        }
      }
      return guessedColumn;
    }


    /// <summary>
    /// Gets line/column for a visual point.
    /// </summary>
    /// <param name="mousePosition">The point (mouse position).</param>
    /// <returns>The text position (column, line) of the position (x, y).</returns>
    public TextLocation GetLogicalPosition(Point mousePosition)
    {
      Fold dummy;
      return GetLogicalColumn(GetLogicalLine(mousePosition.Y), mousePosition.X, out dummy);
    }


    /// <summary>
    /// Gets line/column for a visual point.
    /// </summary>
    /// <param name="visualPosX">The x position.</param>
    /// <param name="visualPosY">The y position.</param>
    /// <returns>The text position (column, line) of the position (x, y).</returns>
    public TextLocation GetLogicalPosition(int visualPosX, int visualPosY)
    {
      Fold dummy;
      return GetLogicalColumn(GetLogicalLine(visualPosY), visualPosX, out dummy);
    }


    public Fold GetFoldMarkerFromPosition(int visualPosX, int visualPosY)
    {
      Fold fold;
      GetLogicalColumn(GetLogicalLine(visualPosY), visualPosX, out fold);
      return fold;
    }


    /// <summary>
    /// Returns logical line number for a visual point.
    /// </summary>
    /// <param name="visualPosY">The y position.</param>
    /// <returns>The logical line.</returns>
    public int GetLogicalLine(int visualPosY)
    {
      int clickedVisualLine = Math.Max(0, (visualPosY + TextArea.VirtualTop.Y) / LineHeight);
      return Document.GetFirstLogicalLine(clickedVisualLine);
    }


    /// <summary>
    /// Gets the logical column.
    /// </summary>
    /// <param name="lineNumber">The line number.</param>
    /// <param name="visualPosX">The visual pos X.</param>
    /// <param name="inFold">The fold marker that contains the columns (or <see langword="null"/>).</param>
    /// <returns>The logical column.</returns>
    internal TextLocation GetLogicalColumn(int lineNumber, int visualPosX, out Fold inFold)
    {
      visualPosX += TextArea.VirtualTop.X;

      inFold = null;
      if (lineNumber >= Document.TotalNumberOfLines)
      {
        return new TextLocation(visualPosX / ColumnWidth, lineNumber);
      }
      if (visualPosX <= 0)
      {
        return new TextLocation(0, lineNumber);
      }

      int start = 0; // column
      int posX = 0; // visual position

      int result;
      using (Graphics g = TextArea.CreateGraphics())
      {
				// call GetLogicalColumnInternal to skip over text,
				// then skip over fold markers
				// and repeat as necessary.
				// The loop terminates once the correct logical column is reached in
				// GetLogicalColumnInternal or inside a fold marker.
				while (true) 
        {
          LineSegment line = Document.GetLineSegment(lineNumber);
          Fold nextFolding = FindNextFoldedFoldOnLineAfterColumn(lineNumber, start - 1);
          int end = nextFolding != null ? nextFolding.StartColumn : int.MaxValue;
          result = GetLogicalColumnInternal(g, line, start, end, ref posX, visualPosX);

          // break when GetLogicalColumnInternal found the result column
          if (result < end)
            break;

          // reached fold marker
          lineNumber = nextFolding.EndLine;
          start = nextFolding.EndColumn;
          int newPosX = posX + 1 + MeasureStringWidth(g, nextFolding.FoldText, TextEditorProperties.FontContainer.RegularFont);
          if (newPosX >= visualPosX)
          {
            inFold = nextFolding;
            if (IsNearerToAThanB(visualPosX, posX, newPosX))
              return new TextLocation(nextFolding.StartColumn, nextFolding.StartLine);
            else
              return new TextLocation(nextFolding.EndColumn, nextFolding.EndLine);
          }
          posX = newPosX;
        }
      }
      return new TextLocation(result, lineNumber);
    }


    int GetLogicalColumnInternal(Graphics g, LineSegment line, int start, int end, ref int drawingPos, int targetVisualPosX)
    {
      if (start == end)
        return end;
      Debug.Assert(start < end);
      Debug.Assert(drawingPos < targetVisualPosX);

      int tabIndent = Document.TextEditorProperties.TabIndent;

      FontContainer fontContainer = TextEditorProperties.FontContainer;

      List<TextWord> words = line.Words;
      if (words == null) return 0;
      int wordOffset = 0;
      for (int i = 0; i < words.Count; i++)
      {
        TextWord word = words[i];
        if (wordOffset >= end)
        {
          return wordOffset;
        }
        if (wordOffset + word.Length >= start)
        {
          int newDrawingPos;
          switch (word.Type)
          {
            case TextWordType.Space:
              newDrawingPos = drawingPos + _spaceWidth;
              if (newDrawingPos >= targetVisualPosX)
                return IsNearerToAThanB(targetVisualPosX, drawingPos, newDrawingPos) ? wordOffset : wordOffset + 1;
              break;
            case TextWordType.Tab:
              // go to next tab position
              drawingPos = (drawingPos + MinTabWidth) / tabIndent / ColumnWidth * tabIndent * ColumnWidth;
              newDrawingPos = drawingPos + tabIndent * ColumnWidth;
              if (newDrawingPos >= targetVisualPosX)
                return IsNearerToAThanB(targetVisualPosX, drawingPos, newDrawingPos) ? wordOffset : wordOffset + 1;
              break;
            case TextWordType.Word:
              int wordStart = Math.Max(wordOffset, start);
              int wordLength = Math.Min(wordOffset + word.Length, end) - wordStart;
              string text = Document.GetText(line.Offset + wordStart, wordLength);
              Font font = word.GetFont(fontContainer) ?? fontContainer.RegularFont;
              newDrawingPos = drawingPos + MeasureStringWidth(g, text, font);
              if (newDrawingPos >= targetVisualPosX)
              {
                for (int j = 0; j < text.Length; j++)
                {
                  newDrawingPos = drawingPos + MeasureStringWidth(g, text[j].ToString(), font);
                  if (newDrawingPos >= targetVisualPosX)
                  {
                    if (IsNearerToAThanB(targetVisualPosX, drawingPos, newDrawingPos))
                      return wordStart + j;
                    else
                      return wordStart + j + 1;
                  }
                  drawingPos = newDrawingPos;
                }
                return wordStart + text.Length;
              }
              break;
            default:
              throw new NotSupportedException();
          }
          drawingPos = newDrawingPos;
        }
        wordOffset += word.Length;
      }
      return wordOffset;
    }


    static bool IsNearerToAThanB(int num, int a, int b)
    {
      return Math.Abs(a - num) < Math.Abs(b - num);
    }


    Fold FindNextFoldedFoldOnLineAfterColumn(int lineNumber, int column)
    {
      List<Fold> list = Document.FoldingManager.GetFoldedFoldsWithStartAfterColumn(lineNumber, column);
      if (list.Count != 0)
        return list[0];
      else
        return null;
    }


    const int MinTabWidth = 4;

    float CountColumns(ref int column, int start, int end, int logicalLine, Graphics g)
    {
      if (start > end) throw new ArgumentException("start > end");
      if (start == end) return 0;
      float spaceWidth = SpaceWidth;
      float drawingPos = 0;
      int tabIndent = Document.TextEditorProperties.TabIndent;
      LineSegment currentLine = Document.GetLineSegment(logicalLine);
      List<TextWord> words = currentLine.Words;
      if (words == null) return 0;
      int wordCount = words.Count;
      int wordOffset = 0;
      FontContainer fontContainer = TextEditorProperties.FontContainer;
      for (int i = 0; i < wordCount; i++)
      {
        TextWord word = words[i];
        if (wordOffset >= end)
          break;
        if (wordOffset + word.Length >= start)
        {
          switch (word.Type)
          {
            case TextWordType.Space:
              drawingPos += spaceWidth;
              break;
            case TextWordType.Tab:
              // go to next tab position
              drawingPos = (int) ((drawingPos + MinTabWidth) / tabIndent / ColumnWidth) * tabIndent * ColumnWidth;
              drawingPos += tabIndent * ColumnWidth;
              break;
            case TextWordType.Word:
              int wordStart = Math.Max(wordOffset, start);
              int wordLength = Math.Min(wordOffset + word.Length, end) - wordStart;
              string text = Document.GetText(currentLine.Offset + wordStart, wordLength);
              drawingPos += MeasureStringWidth(g, text, word.GetFont(fontContainer) ?? fontContainer.RegularFont);
              break;
          }
        }
        wordOffset += word.Length;
      }
      for (int j = currentLine.Length; j < end; j++)
      {
        drawingPos += ColumnWidth;
      }

      // add one pixel in column calculation to account for floating point calculation errors
      column += (int) ((drawingPos + 1) / ColumnWidth);

      return drawingPos;
    }


    /// <summary>
    /// Gets the x position for a certain text position (column, character).
    /// </summary>
    /// <param name="logicalLine">The logical line.</param>
    /// <param name="logicalColumn">The logical column.</param>
    /// <returns></returns>
    public int GetDrawingXPos(int logicalLine, int logicalColumn)
    {
      IList<Fold> foldings = Document.FoldingManager.GetTopLevelFoldedFoldings();
      int i;
      Fold f = null;
      // search the last folding that's interesting
      for (i = foldings.Count - 1; i >= 0; --i)
      {
        f = foldings[i];
        if (f.StartLine < logicalLine || f.StartLine == logicalLine && f.StartColumn < logicalColumn)
        {
          break;
        }
        Fold f2 = foldings[i / 2];
        if (f2.StartLine > logicalLine || f2.StartLine == logicalLine && f2.StartColumn >= logicalColumn)
        {
          i /= 2;
        }
      }
      int column = 0;
      float drawingPos;
      Graphics g = TextArea.CreateGraphics();
      // if no folding is interesting
      if (f == null || !(f.StartLine < logicalLine || f.StartLine == logicalLine && f.StartColumn < logicalColumn))
      {
        drawingPos = CountColumns(ref column, 0, logicalColumn, logicalLine, g);
        return (int) (drawingPos - TextArea.VirtualTop.X);
      }

      // if logicalLine/logicalColumn is in folding
      if (f.EndLine > logicalLine || f.EndLine == logicalLine && f.EndColumn > logicalColumn)
      {
        logicalColumn = f.StartColumn;
        logicalLine = f.StartLine;
        --i;
      }
      int lastFolding = i;

      // search backwards until a new visible line is reached
      for (; i >= 0; --i)
      {
        f = foldings[i];
        if (f.EndLine < logicalLine)
        { 
          // reached the begin of a new visible line
          break;
        }
      }
      int firstFolding = i + 1;

      if (lastFolding < firstFolding)
      {
        drawingPos = CountColumns(ref column, 0, logicalColumn, logicalLine, g);
        return (int) (drawingPos - TextArea.VirtualTop.X);
      }

      int foldEnd = 0;
      drawingPos = 0;
      for (i = firstFolding; i <= lastFolding; ++i)
      {
        f = foldings[i];
        drawingPos += CountColumns(ref column, foldEnd, f.StartColumn, f.StartLine, g);
        foldEnd = f.EndColumn;
        column += f.FoldText.Length;
        drawingPos += _additionalFoldTextSize;
        drawingPos += MeasureStringWidth(g, f.FoldText, TextEditorProperties.FontContainer.RegularFont);
      }
      drawingPos += CountColumns(ref column, foldEnd, logicalColumn, logicalLine, g);
      g.Dispose();
      return (int) (drawingPos - TextArea.VirtualTop.X);
    }
    #endregion


    #region DrawHelper functions
    private static void DrawBracketHighlight(Graphics g, Rectangle rect)
    {
      g.FillRectangle(BrushRegistry.GetBrush(Color.FromArgb(50, 0, 0, 255)), rect);
      g.DrawRectangle(Pens.Blue, rect);
    }


    private static void DrawString(Graphics g, string text, Font font, Color color, int x, int y)
    {
      TextRenderer.DrawText(g, text, font, new Point(x, y), color, textFormatFlags);
    }


    private void DrawInvalidLineMarker(Graphics g, int x, int y)
    {
      HighlightColor invalidLinesColor = TextArea.Document.HighlightingStrategy.GetColorFor("InvalidLines");
      DrawString(g, "~", invalidLinesColor.GetFont(TextEditorProperties.FontContainer), invalidLinesColor.Color, x, y);
    }


    private void DrawSpaceMarker(Graphics g, Color color, int x, int y)
    {
      HighlightColor spaceMarkerColor = TextArea.Document.HighlightingStrategy.GetColorFor("SpaceMarkers");
      DrawString(g, "\u00B7", spaceMarkerColor.GetFont(TextEditorProperties.FontContainer), color, x, y);
    }


    private void DrawTabMarker(Graphics g, Color color, int x, int y)
    {
      HighlightColor tabMarkerColor = TextArea.Document.HighlightingStrategy.GetColorFor("TabMarkers");
      DrawString(g, "\u00BB", tabMarkerColor.GetFont(TextEditorProperties.FontContainer), color, x, y);
    }


    private int DrawEOLMarker(Graphics g, Color color, Brush backBrush, int x, int y)
    {
      HighlightColor eolMarkerColor = TextArea.Document.HighlightingStrategy.GetColorFor("EOLMarkers");

      int width = GetWidth('\u00B6', eolMarkerColor.GetFont(TextEditorProperties.FontContainer));
      g.FillRectangle(backBrush,
                      new RectangleF(x, y, width, LineHeight));

      DrawString(g, "\u00B6", eolMarkerColor.GetFont(TextEditorProperties.FontContainer), color, x, y);
      return width;
    }


    private void DrawVerticalRuler(Graphics g, Rectangle lineRectangle)
    {
      int xpos = ColumnWidth * TextEditorProperties.VerticalRulerColumn - TextArea.VirtualTop.X;
      if (xpos <= 0)
        return;

      HighlightColor vRulerColor = TextArea.Document.HighlightingStrategy.GetColorFor("VRuler");

      g.DrawLine(BrushRegistry.GetPen(vRulerColor.Color),
                 DrawingPosition.Left + xpos,
                 lineRectangle.Top,
                 DrawingPosition.Left + xpos,
                 lineRectangle.Bottom);
    }
    #endregion
  }
}
