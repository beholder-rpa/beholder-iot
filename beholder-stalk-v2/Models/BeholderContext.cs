using System;

namespace beholder_stalk_v2.Models
{
  public record BeholderContext
  {
    public Point EyeCurrentPointerPosition
    {
      get;
      set;
    }

    public DateTime? LastEyePointerUpdate
    {
      get;
      set;
    }

    public Point PsionixCurrentPointerPosition
    {
      get;
      set;
    }

    public DateTime? LastPsionixPointerUpdate
    {
      get;
      set;
    }

    public SysInfo SysInfo
    {
      get;
      set;
    }
  }
}