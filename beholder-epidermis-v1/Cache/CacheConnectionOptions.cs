namespace beholder_epidermis_v1.Cache
{
  public record CacheConnectionOptions
  {
    public CacheConnectionOptions()
    {
      Host = "beholder-01.local";
      Port = 6379;
      ClientName = "beholder-daemon";
      RetryDelay = 2500;
      AllowAdmin = true;
    }

    public string Host
    {
      get;
      set;
    }

    public int Port
    {
      get;
      set;
    }

    public string ClientName
    {
      get;
      set;
    }

    public int RetryDelay
    {
      get;
      set;
    }

    public bool AllowAdmin
    {
      get;
      set;
    }
  }
}
