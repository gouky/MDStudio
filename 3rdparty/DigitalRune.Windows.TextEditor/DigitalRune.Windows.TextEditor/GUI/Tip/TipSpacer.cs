using System;
using System.Drawing;


namespace DigitalRune.Windows.TextEditor
{
  internal class TipSpacer : TipSection
  {
    SizeF spacerSize;


    public TipSpacer(Graphics graphics, SizeF size)
      : base(graphics)
    {
      spacerSize = size;
    }


    public override void Draw(PointF location)
    {
    }


    protected override void OnMaximumSizeChanged()
    {
      base.OnMaximumSizeChanged();
      SetRequiredSize(new Size((int) Math.Min(MaximumSize.Width, spacerSize.Width), (int) Math.Min(MaximumSize.Height, spacerSize.Height)));
    }
  }
}
