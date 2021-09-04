namespace beholder_stalk_v2.Utils
{
  using System;
  using static beholder_stalk_v2.Protos.MoveMouseToRequest.Types;

  public class Line
  {
    public Point p1, p2;

    public Line(Point p1, Point p2)
    {
      this.p1 = p1;
      this.p2 = p2;
    }

    public double GetLength()
    {
      return Math.Sqrt(Math.Pow(p2.Y - p1.Y, 2) + Math.Pow(p2.X - p1.X, 2));
    }

    public Point[] GetPoints()
    {
      if (p1.X == p2.X && p1.Y == p2.Y)
      {
        return Array.Empty<Point>();
      }
      int dx0 = p2.X - p1.X, dy0 = p2.Y - p1.Y;
      int dx1 = Math.Abs(dx0), dy1 = Math.Abs(dy0);
      int sx1 = Math.Sign(dx0), sy1 = Math.Sign(dy0), sx2 = sx1, sy2 = 0;

      if (dx1 <= dy1)
      {
        int tmp = dx1;
        dx1 = dy1;
        dy1 = tmp;
        sy2 = sy1;
        sx2 = 0;
      }

      int err = dx1 >> 1;

      int x = p1.X, y = p1.Y;
      Point[] points = new Point[dx1 + 1];
      for (int i = 0; i <= dx1; i++)
      {
        points[i] = new Point() { X = x, Y = y };
        err += dy1;
        if (err >= dx1)
        {
          err -= dx1;
          x += sx1;
          y += sy1;
        }
        else
        {
          x += sx2;
          y += sy2;
        }
      }

      return points;
    }
  }
}