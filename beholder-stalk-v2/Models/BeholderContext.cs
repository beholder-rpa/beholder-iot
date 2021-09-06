namespace beholder_stalk_v2.Models
{
  using System;

  public record BeholderContext
  {
    public Point EyeCurrentPointerPosition
    {
      get;
      init;
    }

    public DateTime? LastEyePointerUpdate
    {
      get;
      init;
    }

    public Point PsionixCurrentPointerPosition
    {
      get;
      init;
    }

    public DateTime? LastPsionixPointerUpdate
    {
      get;
      init;
    }

    /// <summary>
    /// Gets the position that the most previous movement request started from
    /// </summary>
    public Point LastMovementPosition
    {
      get;
      init;
    }

    public SysInfo SysInfo
    {
      get;
      init;
    }
  }
}