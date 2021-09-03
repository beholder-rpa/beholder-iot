namespace beholder_stalk_v2
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using Microsoft.Extensions.Hosting;
  using Microsoft.Extensions.Logging;
  using Models;
  using System;
  using System.Threading;
  using System.Threading.Tasks;

  public class BeholderStalkWorker : BackgroundService
  {
    private readonly Keyboard _keyboard;
    private readonly Mouse _mouse;
    private readonly MouseObserver _mouseObserver;
    private readonly Joystick _joystick;

    private readonly BeholderServiceInfo _serviceInfo;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<BeholderStalkWorker> _logger;

    public BeholderStalkWorker(
      Keyboard keyboard,
      Mouse mouse,
      MouseObserver mouseObserver,
      Joystick joystick,
      BeholderServiceInfo beholderServiceInfo,
      IBeholderMqttClient client,
      ILogger<BeholderStalkWorker> logger
    )
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
      _mouseObserver = mouseObserver ?? throw new ArgumentNullException(nameof(mouseObserver));
      _joystick = joystick ?? throw new ArgumentNullException(nameof(joystick));

      _serviceInfo = beholderServiceInfo ?? throw new ArgumentNullException(nameof(beholderServiceInfo));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
      await _client.StartAsync();
      _mouse.Subscribe(_mouseObserver);

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