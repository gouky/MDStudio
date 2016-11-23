using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using DigitalRune.Windows.TextEditor.Document;
using DigitalRune.Windows.TextEditor.Highlighting;


namespace DigitalRune.Windows.TextEditor
{
  public partial class TextEditorControl
  {
    private PrintDocument _printDocument;
    private int _currentLine;
    private float _currentTabIndent;
    private StringFormat _printingStringFormat;


    /// <summary>
    /// Gets the print document.
    /// </summary>
    /// <value>The print document.</value>
    [Browsable(false)]
    public PrintDocument PrintDocument
    {
      get
      {
        if (_printDocument == null)
        {
          _printDocument = new PrintDocument();
          _printDocument.BeginPrint += BeginPrint;
          _printDocument.PrintPage += PrintPage;
        }
        return _printDocument;
      }
    }


    void BeginPrint(object sender, PrintEventArgs ev)
    {
      _currentLine = 0;
      _printingStringFormat = (StringFormat) StringFormat.GenericTypographic.Clone();

      // 100 should be enough for everyone ...err ?
      float[] tabStops = new float[100];
      for (int i = 0; i < tabStops.Length; ++i)
        tabStops[i] = TabIndent * _primaryTextArea.TextArea.TextView.ColumnWidth;

      _printingStringFormat.SetTabStops(0, tabStops);
    }


    void Advance(ref float x, ref float y, float maxWidth, float size, float fontHeight)
    {
      if (x + size < maxWidth)
      {
        x += size;
      }
      else
      {
        x = _currentTabIndent;
        y += fontHeight;
      }
    }


    // btw. I hate source code duplication ... but this time I don't care !!!!
    float MeasurePrintingHeight(Graphics g, LineSegment line, float maxWidth)
    {
      float xPos = 0;
      float yPos = 0;
      float fontHeight = Font.GetHeight(g);
      _currentTabIndent = 0;
      FontContainer fontContainer = TextEditorProperties.FontContainer;
      foreach (TextWord word in line.Words)
      {
        switch (word.Type)
        {
          case TextWordType.Space:
            Advance(ref xPos, ref yPos, maxWidth, _primaryTextArea.TextArea.TextView.SpaceWidth, fontHeight);
            break;
          case TextWordType.Tab:
            Advance(ref xPos, ref yPos, maxWidth, TabIndent * _primaryTextArea.TextArea.TextView.ColumnWidth, fontHeight);
            break;
          case TextWordType.Word:
            SizeF drawingSize = g.MeasureString(word.Word, word.GetFont(fontContainer), new SizeF(maxWidth, fontHeight * 100), _printingStringFormat);
            Advance(ref xPos, ref yPos, maxWidth, drawingSize.Width, fontHeight);
            break;
        }
      }
      return yPos + fontHeight;
    }


    void DrawLine(Graphics g, LineSegment line, float yPos, RectangleF margin)
    {
      float xPos = 0;
      float fontHeight = Font.GetHeight(g);
      _currentTabIndent = 0;

      FontContainer fontContainer = TextEditorProperties.FontContainer;
      foreach (TextWord word in line.Words)
      {
        switch (word.Type)
        {
          case TextWordType.Space:
            Advance(ref xPos, ref yPos, margin.Width, _primaryTextArea.TextArea.TextView.SpaceWidth, fontHeight);
            break;
          case TextWordType.Tab:
            Advance(ref xPos, ref yPos, margin.Width, TabIndent * _primaryTextArea.TextArea.TextView.ColumnWidth, fontHeight);
            break;
          case TextWordType.Word:
            g.DrawString(word.Word, word.GetFont(fontContainer), BrushRegistry.GetBrush(word.Color), xPos + margin.X, yPos);
            SizeF drawingSize = g.MeasureString(word.Word, word.GetFont(fontContainer), new SizeF(margin.Width, fontHeight * 100), _printingStringFormat);
            Advance(ref xPos, ref yPos, margin.Width, drawingSize.Width, fontHeight);
            break;
        }
      }
    }


    void PrintPage(object sender, PrintPageEventArgs ev)
    {
      Graphics g = ev.Graphics;
      float yPos = ev.MarginBounds.Top;

      while (_currentLine < Document.TotalNumberOfLines)
      {
        LineSegment curLine = Document.GetLineSegment(_currentLine);
        if (curLine.Words != null)
        {
          float drawingHeight = MeasurePrintingHeight(g, curLine, ev.MarginBounds.Width);
          if (drawingHeight + yPos > ev.MarginBounds.Bottom)
          {
            break;
          }

          DrawLine(g, curLine, yPos, ev.MarginBounds);
          yPos += drawingHeight;
        }
        ++_currentLine;
      }

      // If more lines exist, print another page.
      ev.HasMorePages = _currentLine < Document.TotalNumberOfLines;
    }
  }
}
