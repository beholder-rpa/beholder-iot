namespace beholder_stalk_v2.Utils
{
  using System;
  using System.Threading;

  public static class Utils
  {
    public static bool RetryUntilSuccessOrTimeout(Func<bool> task, TimeSpan timeout, TimeSpan pause = default)
    {
      if (pause == default)
      {
        pause = TimeSpan.FromMilliseconds(1);
      }

      if (pause.TotalMilliseconds < 0)
      {
        throw new ArgumentException("pause must be >= 0 milliseconds");
      }

      return SpinWait.SpinUntil(task, timeout);
    }
  }
}