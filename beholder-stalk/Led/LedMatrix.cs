namespace beholder_stalk
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;

  public class LedMatrix
  {
    private const string SENSE_HAT_FRAMEBUFFER_NAME = "RPi-Sense FB";

    private const int Rows = 8;
    private const int Columns = 8;

    public LedMatrix(string devPath)
    {
      DevPath = devPath;
    }

    public string DevPath
    {
      get;
      private set;
    }

    public void SetPixels(IList<Color> pixels)
    {
      if (pixels.Count() != Rows * Columns)
      {
        throw new InvalidOperationException($"{nameof(pixels)} argument length must be {Rows * Columns}");
      }

      static byte[] Pack(Color color)
      {
        var r = (color.Red >> 3) & 0x1F;
        var g = (color.Green >> 2) & 0x3F;
        var b = (color.Blue >> 3) & 0x1F;
        var bits16 = (ushort)((r << 11) + (g << 5) + b);
        return BitConverter.GetBytes(bits16);
      }

      var content = new List<byte>();
      foreach (var pixel in pixels)
      {
        content.AddRange(Pack(pixel));
      }

      File.WriteAllBytes(DevPath, content.ToArray());
    }


    public static string GetFrameBufferDevicePath()
    {
      static bool IsSenseFrameBuffer(string file)
      {
        var nameFile = Path.Combine(file, "name");
        if (!File.Exists(nameFile))
        {
          return false;
        }

        var name = File.ReadAllText(nameFile).Trim();
        return name == SENSE_HAT_FRAMEBUFFER_NAME;
      }

      static string GetDevice(string file)
      {
        var frameBufferDevice = Path.Combine("/dev", Path.GetFileName(file));
        return File.Exists(frameBufferDevice)
            ? frameBufferDevice
            : null;
      }

      try
      {
        return Directory
            .EnumerateFileSystemEntries("/sys/class/graphics/", "fb*")
            .Where(IsSenseFrameBuffer)
            .Select(GetDevice)
            .FirstOrDefault(p => p != null);
      }
      catch (DirectoryNotFoundException)
      {
        return null;
      }
    }
  }
}