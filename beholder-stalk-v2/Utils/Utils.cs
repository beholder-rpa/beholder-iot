namespace beholder_stalk_v2.Utils
{
  using System;
  using System.Diagnostics;
  using System.Threading.Tasks;

  public static class Utils
  {
    public static async Task<bool> RetryUntilSuccessOrTimeout(Func<bool> task, TimeSpan timeout, TimeSpan pause = default)
    {
      if (pause == default)
      {
        pause = TimeSpan.FromMilliseconds(1);
      }

      if (pause.TotalMilliseconds < 0)
      {
        throw new ArgumentException("pause must be >= 0 milliseconds");
      }

      var stopwatch = Stopwatch.StartNew();
      do
      {
        if (task()) { return true; }
        await Task.Delay(pause);
      }
      while (stopwatch.Elapsed < timeout);
      return false;
    }
  }
}
