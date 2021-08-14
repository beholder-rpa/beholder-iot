namespace beholder_stalk_v2
{
  using System;
  using System.Threading.Tasks;

  public static class DelayUtil
  {
    private static readonly Random Random = new Random();

    public static uint GetDelay(IDuration duration = null)
    {
      if (duration != null)
      {
        if (duration.Delay != default)
        {
          return duration.Delay;
        }

        if (duration.Max != default && duration.Min != default)
        {
          return (uint)Random.NextLong(duration.Min, duration.Max);
        }
      }

      return 0;
    }

    public static Task Think(IDuration duration = null)
    {
      return Task.Delay((int)GetDelay(duration));
    }
  }
}