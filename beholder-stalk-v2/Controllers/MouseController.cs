namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;

  [MqttController]
  public class MouseController
  {
    private readonly Mouse _mouse;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<MouseController> _logger;

    public MouseController(Mouse mouse, IBeholderMqttClient client, ILogger<MouseController> logger)
    {
      _mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      
      _mouse.MouseResolutionChanged += new EventHandler<MouseResolutionChangedEventArgs>(async (sender, e) => await HandleMouseResolutionChanged(sender, e)); ;
    }

    private async Task HandleMouseResolutionChanged(object sender, MouseResolutionChangedEventArgs e)
    {
      await _client
        .PublishEventAsync(
          "beholder/stalk/{HOSTNAME}/status/mouse/resolution",
          new MouseResolution() {
            HorizontalResolution = (uint)e.HorizontalResolution,
            VerticalResolution = (uint)e.VerticalResolution
          }
        );
      _logger.LogInformation($"Published mouse resolution changed");
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/click")]
    public Task SendMouseClick(ICloudEvent<SendMouseClickRequest> message)
    {
      _mouse.SendMouseClick(message.Data.MouseClick?.Button, message.Data.MouseClick?.ClickDirection ?? MouseClick.Types.ClickDirection.PressAndRelease, message.Data.MouseClick.Duration);
      _logger.LogInformation($"Sent Mouse Click {message.Data.MouseClick}");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/actions")]
    public Task SendMouseActions(ICloudEvent<SendMouseActionsRequest> message)
    {
      _mouse.SendMouseActions(message.Data.Actions);
      _logger.LogInformation($"Sent Mouse Actions {message.Data.Actions}");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/raw")]
    public Task SendMouseRaw(ICloudEvent<SendMouseRawRequest> message)
    {
      _mouse.SendRaw(message.Data.Report.ToByteArray());
      _logger.LogInformation($"Sent Raw Mouse Report {message.Data.Report}");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/reset")]
    public Task SendMouseReset()
    {
      _mouse.SendMouseReset();
      _logger.LogInformation("Reset Mouse");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/click_duration")]
    public Task AverageClickDuration(ICloudEvent<SetAverageMouseClickDurationRequest> message)
    {
      if (message.Data.Duration != null)
      {
        _mouse.AverageClickDuration = message.Data.Duration;
      }

      _logger.LogInformation($"Set Average Click Duration to {message.Data.Duration}");
      return Status();
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/move_mouse_to")]
    public Task MoveMouseTo(ICloudEvent<MoveMouseToRequest> message)
    {
      _mouse.SendMouseMoveTo(message.Data);
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/status")]
    public async Task Status()
    {
      await _client
        .PublishEventAsync(
          "beholder/stalk/{HOSTNAME}/status/mouse/click_duration",
          new SetAverageMouseClickDurationReply()
          {
            Duration = _mouse.AverageClickDuration
          }
        );

      _logger.LogInformation($"Published mouse status");
    }
  }
}