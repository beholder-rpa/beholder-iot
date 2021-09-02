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
      return Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
    }

    public Point[] GetPoints(int quantity)
    {
      var points = new Point[quantity];
      int deltaY = p2.Y - p1.Y, deltaX = p2.X - p1.X;
      double slope = (double)(p2.Y - p1.Y) / (p2.X - p1.X);
      double x, y;

      --quantity;

      for (int i = 0; i < quantity; i++)
      {
        y = slope == 0 ? 0 : deltaY * (i / (double)quantity);
        x = slope == 0 ? deltaX * (i / (double)quantity) : y / slope;
        points[i] = new Point() { X = (int)Math.Round(x) + p1.X, Y = (int)Math.Round(y) + p1.Y };
      }

      points[quantity] = p2;
      return points;
    }
  }
}