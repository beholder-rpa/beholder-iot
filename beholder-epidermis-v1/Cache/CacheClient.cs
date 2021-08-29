namespace beholder_epidermis_v1.Cache
{
  using beholder_epidermis_v1.Models;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using StackExchange.Redis;
  using System;
  using System.Text.Json;
  using System.Threading.Tasks;

  public class CacheClient : ICacheClient
  {
    private readonly ConfigurationOptions configuration = null;
    private readonly Lazy<IConnectionMultiplexer> _connection = null;
    private readonly ILogger<CacheClient> _logger;

    public CacheClient(IOptions<EpidermisOptions> options, ILogger<CacheClient> logger)
    {
      if (options == null || options.Value == null)
      {
        throw new ArgumentNullException(nameof(options));
      }
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      var connectionOptions = options.Value;

      configuration = new ConfigurationOptions()
      {
        EndPoints = { { connectionOptions.RedisHost, connectionOptions.RedisPort }, },
        AllowAdmin = connectionOptions.RedisAllowAdmin,
        ClientName = $"beholder-daemon-{Environment.GetEnvironmentVariable("BEHOLDER_EPIDERMIS_NAME")}",
        ReconnectRetryPolicy = new LinearRetry(connectionOptions.RedisRetryDelay),
        AbortOnConnectFail = false,
      };

      _connection = new Lazy<IConnectionMultiplexer>(() =>
      {
        var redis = ConnectionMultiplexer.Connect(configuration);
        redis.ErrorMessage += Connection_ErrorMessage;
        //redis.InternalError += _Connection_InternalError;
        //redis.ConnectionFailed += _Connection_ConnectionFailed;
        //redis.ConnectionRestored += _Connection_ConnectionRestored;
        return redis;
      });
    }

    public IConnectionMultiplexer Connection { get { return _connection.Value; } }

    //for the default database
    public IDatabase Database => Connection.GetDatabase();

    public async Task<T> JsonGet<T>(string key)
    {
      var redisValue = await Database.StringGetAsync(key, CommandFlags.None);
      if (!redisValue.HasValue)
        return default;
      return JsonSerializer.Deserialize<T>(redisValue);
    }

    public async Task<bool> JsonSet<T>(string key, T value, TimeSpan? expiry = null)
    {
      if (value == null) return false;
      return await Database.StringSetAsync(key, JsonSerializer.Serialize(value), expiry, When.Always, CommandFlags.None);
    }

    public async Task<byte[]> Base64ByteArrayGet(string key)
    {
      var redisValue = await Database.StringGetAsync(key, CommandFlags.None);
      if (!redisValue.HasValue)
        return default;

      return Convert.FromBase64String(redisValue);
    }

    public async Task<bool> Base64ByteArraySet(string key, byte[] value, TimeSpan? expiry = null)
    {
      if (value == null) return false;
      return await Database.StringSetAsync(key, Convert.ToBase64String(value), expiry, When.Always, CommandFlags.None);
    }

    private void Connection_ErrorMessage(object sender, RedisErrorEventArgs e)
    {
      _logger.LogError($"An error occurred: {e.Message}", e);
    }
  }
}
