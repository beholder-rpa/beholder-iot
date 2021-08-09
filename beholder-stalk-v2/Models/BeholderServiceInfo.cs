namespace beholder_stalk_v2.Models
{
  using System;
  using System.Linq;
  using System.Net;

  public record BeholderServiceInfo
  {
    public BeholderServiceInfo()
    {
      HostName = Environment.GetEnvironmentVariable("BEHOLDER_STALK_NAME") ?? Dns.GetHostName();
      IpAddresses = string.Join(", ", Dns.GetHostAddresses(Dns.GetHostName()).Select(ip => ip.ToString()));
    }

    public string HostName
    {
      get;
    }

    public string IpAddresses
    {
      get;
    }

    public string ServiceName
    {
      get;
      set;
    }

    public string Version
    {
      get;
      set;
    }
  }
}