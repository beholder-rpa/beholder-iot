namespace beholder_epidermis_v1.Models
{
  using System.Collections.Generic;
  using System.Text.Json.Serialization;

  /// <summary>
  /// Represents Epidermis options used by various areas of the framework.
  /// </summary>
  public record EpidermisOptions
  {
    public EpidermisOptions()
    {
      RedisHost = "beholder-prefrontal";
      RedisPort = 6379;
      RedisAllowAdmin = true;
      RedisRetryDelay = 2500;
    }

    /// <summary>
    /// Gets or sets the redis host
    /// </summary>
    [JsonPropertyName("RedisHost")]
    public string RedisHost
    {
      get;
      set;
    }

    [JsonPropertyName("redisPort")]
    public int RedisPort
    {
      get;
      set;
    }

    [JsonPropertyName("redisAllowAdmin")]
    public bool RedisAllowAdmin
    {
      get;
      set;
    }

    [JsonPropertyName("redisRetryDelay")]
    public int RedisRetryDelay
    {
      get;
      set;
    }

    /// <summary>
    /// Gets or sets any additional properties not previously described.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object> ExtensionData { get; set; }
  }
}
