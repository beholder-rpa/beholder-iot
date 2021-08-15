namespace beholder_occipital_v1
{
  using Dapr.Client;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Models;
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public class PulseService : IHostedService, IDisposable
  {
    private readonly DaprClient _daprClient;
    private readonly ILogger<PulseService> _logger;
    private readonly Lazy<BeholderServiceInfo> _serviceInfo = new Lazy<BeholderServiceInfo>(() =>
    {
      return new BeholderServiceInfo
      {
        ServiceName = "occipital",
        Version = "v2"
      };
    });
    private Timer _timer;

    public PulseService(DaprClient daprClient, ILogger<PulseService> logger)
    {
      _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task StartAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Occipital Pulse Service running.");

      _timer = new Timer(async state =>
        {
          try
          {
            await _daprClient.PublishEventAsync(Consts.PubSubName, "beholder/ctaf", _serviceInfo.Value, stoppingToken);
            _logger.LogInformation("Occipital Pulsed");
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, $"Error publishing occipital pulse: {ex.Message}");
          }
        }, null, TimeSpan.FromSeconds(5),
          TimeSpan.FromSeconds(5));

      return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
      _logger.LogInformation("Occipital Pulse Service is stopping.");

      _timer?.Change(Timeout.Infinite, 0);

      return Task.CompletedTask;
    }

    public void Dispose()
    {
      _timer?.Dispose();
    }
  }
}