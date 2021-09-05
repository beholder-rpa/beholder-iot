using System;

namespace beholder_stalk_v2.Models
{
  public class BeholderContext
  {
    public BeholderContext()
    {
      Data = new BeholderContextData();
    }

    public BeholderContextData Data
    {
      get;
      set;
    }
  }

  public record BeholderContextData
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