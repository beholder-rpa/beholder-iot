namespace beholder_stalk_v2.Models
{
  using System;
  using System.Linq;
  using System.Net;
  using System.Text.Json.Serialization;

  public record BeholderServiceInfo
  {
    public BeholderServiceInfo()
    {
      ServiceName = "stalk";
      Version = "v2";
      HostName = Environment.GetEnvironmentVariable("BEHOLDER_STALK_NAME") ?? Dns.GetHostName();
      IpAddresses = string.Join(", ", Dns.GetHostAddresses(Dns.GetHostName()).Select(ip => ip.ToString()));
    }

    [JsonPropertyName("hostName")]
    public string HostName
    {
      get;
    }

    [JsonPropertyName("ipAddresses")]
    public string IpAddresses
    {
      get;
    }

    [JsonPropertyName("serviceName")]
    public string ServiceName
    {
      get;
      set;
    }

    [JsonPropertyName("version")]
    public string Version
    {
      get;
      set;
    }
  }
}