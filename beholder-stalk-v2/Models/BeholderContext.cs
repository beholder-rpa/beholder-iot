namespace beholder_stalk_v2.Models
{
  public record BeholderContext
  {
    public Point EyeCurrentPointerPosition
    {
      get;
      set;
    }

    public Point PsionixCurrentPointerPosition
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