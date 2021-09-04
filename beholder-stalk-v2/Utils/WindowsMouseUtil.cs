namespace beholder_stalk_v2.Utils
{
  using beholder_stalk_v2.Models;
  using System;

  public class WindowsMouseUtil
  {
    /// <summary>
    /// From https://www.esreality.com/index.php?a=post&id=1945096
    /// </summary>
    /// <remarks>
    /// The system applies two tests to the specified relative mouse motion when applying acceleration.
    /// If the specified distance along either the x or y axis is greater than the first mouse threshold value, and the mouse acceleration level is not zero, the operating system doubles the distance.
    /// If the specified distance along either the x- or y-axis is greater than the second mouse threshold value, and the mouse acceleration level is equal to two, the operating system doubles the distance that resulted from applying the first threshold test.
    /// It is thus possible for the operating system to multiply relatively-specified mouse motion along the x- or y-axis by up to four times.
    /// </remarks>
    public static float GetPointerScaleFactor(SysInfo sysInfo)
    {
      var isAccelerated = false;
      var mouseSpeed = 10; // AKA Cursor Speed. System Default is 10

      if (sysInfo?.MouseSpeed > 0)
      {
        mouseSpeed = sysInfo.MouseSpeed;
      }

      if (sysInfo?.MouseInfo?.Acceleration > 0)
      {
        isAccelerated = true;
      }

      float scaleFactor;
      if (isAccelerated == false)
      {
        scaleFactor = mouseSpeed switch
        {
          1 =>  33.3333f,
          2 =>  16.1290f,
          3 =>  8.0645f,
          4 =>  4.0160f,
          5 =>  2.6737f,
          6 =>  2.0004f,
          7 =>  1.6025f,
          8 =>  1.3351f,
          9 =>  1.1442f,
          10 => 1.0f,
          11 => 0.8006f,
          12 => 0.6688f,
          13 => 0.5730f,
          14 => 0.5012f,
          15 => 0.4454f,
          16 => 0.4010f,
          17 => 0.3640f,
          18 => 0.3340f,
          19 => 0.3086f,
          20 => 0.2867f,
          _ => throw new ArgumentOutOfRangeException($"Mouse speed is out of the range of known windows values of 1-20. {mouseSpeed}"),
        };

        return scaleFactor;
      }

      // This gets more difficult as with acceleration on, the scale factor has a component of # of reports
      scaleFactor = mouseSpeed switch
      {
        1 => 33.3333f,
        2 => 16.1290f,
        3 => 8.0645f,
        4 => 4.0160f,
        5 => 2.6737f,
        6 => 2.0004f,
        7 => 1.6025f,
        8 => 1.3351f,
        9 => 1.1442f,
        10 => 1.0f,
        11 => 0.8006f,
        12 => 0.6688f,
        13 => 0.5730f,
        14 => 0.5012f,
        15 => 0.4454f,
        16 => 0.4010f,
        17 => 0.3640f,
        18 => 0.3340f,
        19 => 0.3086f,
        20 => 0.2867f,
        _ => throw new ArgumentOutOfRangeException($"Mouse speed is out of the range of known windows values of 1-20. {mouseSpeed}"),
      };

      return scaleFactor;
    }
  }
}
