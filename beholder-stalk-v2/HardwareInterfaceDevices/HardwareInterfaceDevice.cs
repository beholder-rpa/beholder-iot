namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using System;
  using System.IO;

  /// https://www.usb.org/developers
  public abstract class HardwareInterfaceDevice : IDisposable
  {
    protected HardwareInterfaceDevice(string devPath)
    {
      DevPath = devPath;
      Stream = new FileStream(DevPath, FileMode.Open, FileAccess.ReadWrite);
    }

    public string DevPath
    {
      get;
      private set;
    }

    public bool IsDisposed
    {
      get;
      private set;
    }

    public virtual void SendRaw(byte[] report)
    {
      Stream.Write(report);
      Stream.Flush();
    }

    protected Stream Stream
    {
      get;
      private set;
    }

    #region IDisposable Support
    protected virtual void Dispose(bool disposing)
    {
      if (IsDisposed)
      {
        if (disposing)
        {
          if (Stream != null)
          {
            Stream.Dispose();
          }
        }

        IsDisposed = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }
    #endregion
  }
}