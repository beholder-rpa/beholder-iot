namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Protos;
  using beholder_stalk_v2.Utils;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;
  using Mouse = HardwareInterfaceDevices.Mouse;

  [MqttController]
  public class MouseController
  {
    private readonly Mouse _mouse;
    private readonly BeholderContext _context;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<MouseController> _logger;

    public MouseController(Mouse mouse, IBeholderMqttClient client, BeholderContext context, ILogger<MouseController> logger)
    {
      _mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
      if ((message.Data.CurrentPosition == null || (message.Data.CurrentPosition.X == -1 && message.Data.CurrentPosition.Y == -1)) && _context.PsionixCurrentPointerPosition != null)
      {
        message.Data.CurrentPosition = new MoveMouseToRequest.Types.Point()
        {
          X = _context.PsionixCurrentPointerPosition.X,
          Y = _context.PsionixCurrentPointerPosition.Y,
        };
        _logger.LogInformation($"Current position was not specified in the message, however is contained in the current context from Psionix reports. Using {message.Data.CurrentPosition.X},{message.Data.CurrentPosition.Y}");
      }

      if ((message.Data.CurrentPosition == null || (message.Data.CurrentPosition.X == -1 && message.Data.CurrentPosition.Y == -1)) && _context.EyeCurrentPointerPosition != null)
      {
        message.Data.CurrentPosition = new MoveMouseToRequest.Types.Point()
        {
          X = _context.EyeCurrentPointerPosition.X,
          Y = _context.EyeCurrentPointerPosition.Y,
        };
        _logger.LogInformation($"Current position was not specified in the message, however is contained in the current context from Eye reports. Using {message.Data.CurrentPosition.X},{message.Data.CurrentPosition.Y}");
      }

      var windowsPointerScaleFactor = WindowsMouseUtil.GetPointerScaleFactor(_context.SysInfo);
      message.Data.MovementScaleX = message.Data.MovementScaleX * windowsPointerScaleFactor;
      message.Data.MovementScaleY = message.Data.MovementScaleY * windowsPointerScaleFactor;

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