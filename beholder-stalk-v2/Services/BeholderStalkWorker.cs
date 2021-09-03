namespace beholder_stalk_v2
{
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Models;
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public class BeholderStalkWorker : BackgroundService
  {
    private readonly BeholderServiceInfo _serviceInfo;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<BeholderStalkWorker> _logger;

    public BeholderStalkWorker(BeholderServiceInfo beholderServiceInfo, IBeholderMqttClient client, ILogger<BeholderStalkWorker> logger)
    {
      _serviceInfo = beholderServiceInfo ?? throw new ArgumentNullException(nameof(beholderServiceInfo));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await _client.StartAsync();

      while (!stoppingToken.IsCancellationRequested)
      {
        // Perform updates on 
        await _client
              .PublishEventAsync(
                "beholder/ctaf",
                _serviceInfo
              );

        await Task.Delay(5000, stoppingToken);
      }
    }

    public override Task StartAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Stalk Worker Service running.");

      return base.StartAsync(cancellationToken);
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
      _logger.LogInformation("Stalk Worker Service is stopping.");

      return base.StopAsync(cancellationToken);
    }
  }
}