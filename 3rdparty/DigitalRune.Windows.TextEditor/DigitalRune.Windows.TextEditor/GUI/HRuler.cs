using System.Drawing;
using System.Windows.Forms;
using DigitalRune.Windows.TextEditor.Highlighting;


namespace DigitalRune.Windows.TextEditor
{
  /// <summary>
  /// Horizontal ruler - text column measuring ruler at the top of the text area.
  /// </summary>
  internal class HRuler : Control
  {
    private readonly TextArea _textArea;


    /// <summary>
    /// Initializes a new instance of the <see cref="HRuler"/> class.
    /// </summary>
    /// <param name="textArea">The text area.</param>
    public HRuler(TextArea textArea)
    {
      _textArea = textArea;
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
      SetStyle(ControlStyles.ResizeRedraw, true);
      SetStyle(ControlStyles.FixedHeight, true);

      HighlightColor lineNumberPainterColor = textArea.Document.HighlightingStrategy.GetColorFor("LineNumbers");
      Font lineNumberFont = lineNumberPainterColor.GetFont(textArea.TextEditorProperties.FontContainer);
      Font = new Font(lineNumberFont.FontFamily, lineNumberFont.Size * 3.0f / 4.0f, FontStyle.Regular);
      Size textSize = TextRenderer.MeasureText("0", Font);
      Height = textSize.Height * 5 / 4;
    }


    /// <summary>
    /// Raises the <see cref="Control.Paint"></see> event.
    /// </summary>
    /// <param name="e">A <see cref="PaintEventArgs"></see> that contains the event data.</param>
    protected override void OnPaint(PaintEventArgs e)
    {
      Graphics g = e.Graphics;
      int column = 0;
      float columnWidth = _textArea.TextView.ColumnWidth;
      const TextFormatFlags textFormat = TextFormatFlags.Top | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPadding;
      for (float x = _textArea.TextView.DrawingPosition.Left; x < _textArea.TextView.DrawingPosition.Right; x += columnWidth, ++column)
      {
        int lineHeight = (column % 5 == 0) ? Height / 3 : Height / 5;

        if (column % 10 == 0)
        {
          string columnNumber = column.ToString();
          Size textSize = TextRenderer.MeasureText(columnNumber, Font);
          Rectangle textRectangle = new Rectangle((int) x - textSize.Width / 2, 0, textSize.Width, textSize.Height);
          TextRenderer.DrawText(g, columnNumber, Font, textRectangle, Color.Black, textFormat);
        }
        g.DrawLine(Pens.Black, (int) x, Bottom - 1, (int) x, Bottom - lineHeight);
      }
    }


    /// <summary>
    /// Paints the background of the control. 
    /// </summary>
    /// <param name="e">
    /// The <see cref="System.Windows.Forms.PaintEventArgs"/> that contains 
    /// information about the control to paint.
    /// </param>
    protected override void OnPaintBackground(PaintEventArgs e)
    {
      // paint background
      Form parentForm = _textArea.MotherTextEditorControl.ParentForm;
      Color backColor = (parentForm != null) ? parentForm.BackColor : SystemColors.Control;

      using (SolidBrush brush = new SolidBrush(backColor))
        e.Graphics.FillRectangle(brush, ClientRectangle);

      using (Pen pen = new Pen(ControlPaint.Dark(backColor)))
        e.Graphics.DrawLine(pen, ClientRectangle.Left, ClientRectangle.Bottom - 1, ClientRectangle.Right, ClientRectangle.Bottom -1);
    }
  }
}
