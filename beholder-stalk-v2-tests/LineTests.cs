namespace beholder_stalk_v2_tests
{
  using beholder_stalk_v2.Utils;
  using System;
  using Xunit;
  using static beholder_stalk_v2.Protos.MoveMouseToRequest.Types;

  public class LineTests
  {
    [Fact]
    public void Test1()
    {
      var myLine = new Line(new Point() { X = 0, Y = 0 }, new Point() { X = 100, Y = 100 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test2()
    {
      var myLine = new Line(new Point() { X = 100, Y = 100 }, new Point() { X = 0, Y = 0 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test3()
    {
      var myLine = new Line(new Point() { X = 0, Y = 100 }, new Point() { X = 100, Y = 0 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test4()
    {
      var myLine = new Line(new Point() { X = 100, Y = 0 }, new Point() { X = 0, Y = 100 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test5()
    {
      var myLine = new Line(new Point() { X = 0, Y = 0 }, new Point() { X = 100, Y = 0 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test6()
    {
      var myLine = new Line(new Point() { X = 100, Y = 0 }, new Point() { X = 0, Y = 0 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test7()
    {
      var myLine = new Line(new Point() { X = 0, Y = 0 }, new Point() { X = 0, Y = 100 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test8()
    {
      var myLine = new Line(new Point() { X = 0, Y = 0 }, new Point() { X = 0, Y = 100 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test9()
    {
      var myLine = new Line(new Point() { X = 0, Y = 25 }, new Point() { X = 100, Y = 75 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test10()
    {
      var myLine = new Line(new Point() { X = 25, Y = 0 }, new Point() { X = 75, Y = 100 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test11()
    {
      var myLine = new Line(new Point() { X = 100, Y = 75 }, new Point() { X = 0, Y = 25 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test12()
    {
      var myLine = new Line(new Point() { X = 75, Y = 100 }, new Point() { X = 25, Y = 0 });
      var points = myLine.GetPoints();
      Assert.Equal(101, points.Length);
      Assert.Equal(myLine.p2.X, points[100].X);
      Assert.Equal(myLine.p2.Y, points[100].Y);
    }

    [Fact]
    public void Test13()
    {
      var myLine = new Line(new Point() { X = 0, Y = 0 }, new Point() { X = 0, Y = 0 });
      var points = myLine.GetPoints();
      Assert.Empty(points);
    }

    [Fact]
    public void Test14()
    {
      var myLine = new Line(new Point() { X = 0, Y = 0 }, new Point() { X = 100, Y = 100 });
      var length = myLine.GetLength();
      Assert.Equal(142, Math.Ceiling(length));
    }

    [Fact]
    public void Test15()
    {
      var myLine = new Line(new Point() { X = 999, Y = 1000 }, new Point() { X = 1000, Y = 1000 });
      var points = myLine.GetPoints();
      Assert.Equal(2, points.Length);
      Assert.Equal(myLine.p2.X, points[1].X);
      Assert.Equal(myLine.p2.Y, points[1].Y);
    }
  }
}
