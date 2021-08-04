namespace beholder_stalk
{
  using System;

  public class MouseResolutionChangedEventArgs : EventArgs
  {
    public MouseResolutionChangedEventArgs(int horizontalResolution, int verticalResolution)
        : base()
    {
      HorizontalResolution = horizontalResolution;
      VerticalResolution = verticalResolution;
    }

    public int HorizontalResolution { get; private set; }

    public int VerticalResolution { get; private set; }
  }
}