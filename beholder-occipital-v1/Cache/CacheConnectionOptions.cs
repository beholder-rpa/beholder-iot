namespace beholder_occipital_v1.Cache
{
  public record CacheConnectionOptions
  {
    public CacheConnectionOptions()
    {
      Host = "beholder-prefrontal";
      Port = 6379;
      ClientName = "beholder-occipital";
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
