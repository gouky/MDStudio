using System;
using System.Drawing;
using System.Windows.Forms;


namespace DigitalRune.Windows.TextEditor
{
  internal class TipText : TipSection
  {
    protected Color tipColor;
    protected Font tipFont;
    protected TextFormatFlags textFormatFlags;
    protected string tipText;


    public TipText(Graphics graphics, Font font, string text)
      : base(graphics)
    {
      if (text != null && text.Length > short.MaxValue)
        throw new ArgumentException("TipText: text too long (max. is " + short.MaxValue + " characters)", "text");

      tipFont = font; tipText = text;
      Color = SystemColors.InfoText;
      textFormatFlags = TextFormatFlags.Default;
    }


    public override void Draw(PointF location)
    {
      if (IsTextVisible())
      {
        Rectangle rectangle = new Rectangle((int) (location.X + 1), (int) location.Y, (int) AllocatedSize.Width, (int) AllocatedSize.Height);
        TextRenderer.DrawText(Graphics, tipText, tipFont, rectangle, Color, textFormatFlags);
      }
    }


    protected override void OnMaximumSizeChanged()
    {
      base.OnMaximumSizeChanged();

      if (IsTextVisible())
      {
        Size tipSize = TextRenderer.MeasureText(tipText, tipFont);
        SetRequiredSize(tipSize);
      }
      else
      {
        SetRequiredSize(Size.Empty);
      }
    }


    protected bool IsTextVisible()
    {
      return tipText != null && tipText.Length > 0;
    }

    public Color Color
    {
      get { return tipColor; }
      set { tipColor = value; }
    }
  }


  internal class CountTipText : TipText
  {
    readonly float triHeight = 10;
    readonly float triWidth = 10;

    public CountTipText(Graphics graphics, Font font, string text)
      : base(graphics, font, text)
    {
    }

    void DrawTriangle(float x, float y, bool flipped)
    {
      Brush brush = BrushRegistry.GetBrush(Color.FromArgb(192, 192, 192));
      Graphics.FillRectangle(brush, new RectangleF(x, y, triHeight, triHeight));
      float triHeight2 = triHeight / 2;
      float triHeight4 = triHeight / 4;
      brush = Brushes.Black;
      if (flipped)
      {
        Graphics.FillPolygon(brush, new PointF[] {
					new PointF(x,                y + triHeight2 - triHeight4),
					new PointF(x + triWidth / 2, y + triHeight2 + triHeight4),
					new PointF(x + triWidth,     y + triHeight2 - triHeight4),
				});
      }
      else
      {
        Graphics.FillPolygon(brush, new PointF[] {
					new PointF(x,                y +  triHeight2 + triHeight4),
					new PointF(x + triWidth / 2, y +  triHeight2 - triHeight4),
					new PointF(x + triWidth,     y +  triHeight2 + triHeight4),
				});
      }
    }


    public Rectangle DrawingRectangle1;
    public Rectangle DrawingRectangle2;


    public override void Draw(PointF location)
    {
      if (tipText != null && tipText.Length > 0)
      {
        base.Draw(new PointF(location.X + triWidth + 4, location.Y));
        DrawingRectangle1 = new Rectangle((int) location.X + 2,
                                         (int) location.Y + 2,
                                         (int) (triWidth),
                                         (int) (triHeight));
        DrawingRectangle2 = new Rectangle((int) (location.X + AllocatedSize.Width - triWidth - 2),
                                         (int) location.Y + 2,
                                         (int) (triWidth),
                                         (int) (triHeight));
        DrawTriangle(location.X + 2, location.Y + 2, false);
        DrawTriangle(location.X + AllocatedSize.Width - triWidth - 2, location.Y + 2, true);
      }
    }


    protected override void OnMaximumSizeChanged()
    {
      if (IsTextVisible())
      {
        Size tipSize = TextRenderer.MeasureText(tipText, tipFont);
        tipSize.Width += (int) (triWidth * 2 + 8);
        SetRequiredSize(tipSize);
      }
      else
      {
        SetRequiredSize(Size.Empty);
      }
    }
  }
}
