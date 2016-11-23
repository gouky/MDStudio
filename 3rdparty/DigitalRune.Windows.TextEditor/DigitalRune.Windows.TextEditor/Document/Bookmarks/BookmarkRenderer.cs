using System;
using System.Drawing;
using System.Drawing.Drawing2D;


namespace DigitalRune.Windows.TextEditor.Bookmarks
{
  /// <summary>
  /// Renders graphics for bookmarks.
  /// </summary>
  public static class BookmarkRenderer
  {
    /// <summary>
    /// Draws a breakpoint symbol.
    /// </summary>
    /// <param name="g">The <see cref="Graphics"/> context.</param>
    /// <param name="rectangle">The bounding rectangle.</param>
    /// <param name="enabled"><see langword="true"/> if enabled..</param>
    /// <param name="willBeHit"><see langword="true"/> if it will be hit.</param>
    public static void DrawBreakpoint(Graphics g, Rectangle rectangle, bool enabled, bool willBeHit)
    {
      int diameter = Math.Min(rectangle.Width - 4, rectangle.Height);
      Rectangle rect = new Rectangle(2, rectangle.Y + (rectangle.Height - diameter) / 2, diameter, diameter);

      using (GraphicsPath path = new GraphicsPath())
      {
        path.AddEllipse(rect);
        using (PathGradientBrush pthGrBrush = new PathGradientBrush(path))
        {
          pthGrBrush.CenterPoint = new PointF(rect.Left + rect.Width / 3, rect.Top + rect.Height / 3);
          pthGrBrush.CenterColor = Color.MistyRose;
          Color[] colors = { willBeHit ? Color.Firebrick : Color.Olive };
          pthGrBrush.SurroundColors = colors;

          if (enabled)
          {
            g.FillEllipse(pthGrBrush, rect);
          }
          else
          {
            g.FillEllipse(SystemBrushes.Control, rect);
            using (Pen pen = new Pen(pthGrBrush))
            {
              g.DrawEllipse(pen, new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, rect.Height - 2));
            }
          }
        }
      }
    }


    /// <summary>
    /// Draws a bookmark symbol.
    /// </summary>
    /// <param name="g">The <see cref="Graphics"/> context.</param>
    /// <param name="rectangle">The bounding rectangle.</param>
    /// <param name="enabled"><see langword="true"/> if enabled.</param>
    public static void DrawBookmark(Graphics g, Rectangle rectangle, bool enabled)
    {
      int delta = rectangle.Height / 8;
      Rectangle rect = new Rectangle(1, rectangle.Y + delta, rectangle.Width - 4, rectangle.Height - delta * 2);

      Point gradientStart = new Point(rect.Left, rect.Top);
      Point gradientEnd = new Point(rect.Right, rect.Bottom);
      if (enabled)
      {
        using (Brush brush = new LinearGradientBrush(gradientStart, gradientEnd, Color.SkyBlue, Color.White))
        {
          FillRoundRect(g, brush, rect);
        }
      }
      else
      {
        FillRoundRect(g, Brushes.White, rect);
      }
      using (Brush brush = new LinearGradientBrush(gradientStart, gradientEnd, Color.SkyBlue, Color.Blue))
      {
        using (Pen pen = new Pen(brush))
        {
          DrawRoundRect(g, pen, rect);
        }
      }
    }


    /// <summary>
    /// Draws an arrow symbol.
    /// </summary>
    /// <param name="g">The <see cref="Graphics"/> context.</param>
    /// <param name="rectangle">The bounding rectangle.</param>
    public static void DrawArrow(Graphics g, Rectangle rectangle)
    {
      int delta = rectangle.Height / 8;
      Rectangle rect = new Rectangle(1, rectangle.Y + delta, rectangle.Width - 4, rectangle.Height - delta * 2);
      using (Brush brush = new LinearGradientBrush(new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Bottom), Color.LightYellow, Color.Yellow))
      {
        FillArrow(g, brush, rect);
      }

      using (Brush brush = new LinearGradientBrush(new Point(rect.Left, rect.Top), new Point(rect.Right, rect.Bottom), Color.Yellow, Color.Brown))
      {
        using (Pen pen = new Pen(brush))
        {
          DrawArrow(g, pen, rect);
        }
      }
    }


    static GraphicsPath CreateArrowGraphicsPath(Rectangle r)
    {
      GraphicsPath gp = new GraphicsPath();
      int halfX = r.Width / 2;
      int halfY = r.Height / 2;
      gp.AddLine(r.X, r.Y + halfY / 2, r.X + halfX, r.Y + halfY / 2);
      gp.AddLine(r.X + halfX, r.Y + halfY / 2, r.X + halfX, r.Y);
      gp.AddLine(r.X + halfX, r.Y, r.Right, r.Y + halfY);
      gp.AddLine(r.Right, r.Y + halfY, r.X + halfX, r.Bottom);
      gp.AddLine(r.X + halfX, r.Bottom, r.X + halfX, r.Bottom - halfY / 2);
      gp.AddLine(r.X + halfX, r.Bottom - halfY / 2, r.X, r.Bottom - halfY / 2);
      gp.AddLine(r.X, r.Bottom - halfY / 2, r.X, r.Y + halfY / 2);
      gp.CloseFigure();
      return gp;
    }


    static GraphicsPath CreateRoundRectGraphicsPath(Rectangle r)
    {
      GraphicsPath gp = new GraphicsPath();
      int radius = r.Width / 2;
      gp.AddLine(r.X + radius, r.Y, r.Right - radius, r.Y);
      gp.AddArc(r.Right - radius, r.Y, radius, radius, 270, 90);

      gp.AddLine(r.Right, r.Y + radius, r.Right, r.Bottom - radius);
      gp.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0, 90);

      gp.AddLine(r.Right - radius, r.Bottom, r.X + radius, r.Bottom);
      gp.AddArc(r.X, r.Bottom - radius, radius, radius, 90, 90);

      gp.AddLine(r.X, r.Bottom - radius, r.X, r.Y + radius);
      gp.AddArc(r.X, r.Y, radius, radius, 180, 90);

      gp.CloseFigure();
      return gp;
    }


    static void DrawRoundRect(Graphics g, Pen p, Rectangle r)
    {
      using (GraphicsPath gp = CreateRoundRectGraphicsPath(r))
      {
        g.DrawPath(p, gp);
      }
    }


    static void FillRoundRect(Graphics g, Brush b, Rectangle r)
    {
      using (GraphicsPath gp = CreateRoundRectGraphicsPath(r))
      {
        g.FillPath(b, gp);
      }
    }


    static void DrawArrow(Graphics g, Pen p, Rectangle r)
    {
      using (GraphicsPath gp = CreateArrowGraphicsPath(r))
      {
        g.DrawPath(p, gp);
      }
    }


    static void FillArrow(Graphics g, Brush b, Rectangle r)
    {
      using (GraphicsPath gp = CreateArrowGraphicsPath(r))
      {
        g.FillPath(b, gp);
      }
    }
  }
}
