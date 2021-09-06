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
  public class MouseController : IObserver<ContextEvent>
  {
    private readonly Mouse _mouse;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<MouseController> _logger;

    private Point _lastMovementPosition = new Point();

    public MouseController(Mouse mouse, IBeholderMqttClient client, ILogger<MouseController> logger)
    {
      _mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Point CurrentPointerPosition
    {
      get;
      private set;
    }

    public SysInfo CurrentSystemInformation
    {
      get;
      private set;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/click")]
    public Task SendMouseClick(ICloudEvent<SendMouseClickRequest> message)
    {
      _mouse.SendMouseClick(message.Data.MouseClick?.Button, message.Data.MouseClick?.ClickDirection ?? MouseClick.Types.ClickDirection.PressAndRelease, message.Data.MouseClick.Duration);
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
      if ((message.Data.CurrentPosition == null || (message.Data.CurrentPosition.X == -1 && message.Data.CurrentPosition.Y == -1)))
      {
        message.Data.CurrentPosition = new MoveMouseToRequest.Types.Point()
        {
          X = CurrentPointerPosition.X,
          Y = CurrentPointerPosition.Y,
        };
        _logger.LogInformation($"Current position was not specified in the message, however is contained in the current context from Psionix reports. Using {message.Data.CurrentPosition.X},{message.Data.CurrentPosition.Y}");
      }

      var lastMovementPosition = _lastMovementPosition;
      if (lastMovementPosition != null &&
          lastMovementPosition.X == message.Data.CurrentPosition.X &&
          lastMovementPosition.Y == message.Data.CurrentPosition.Y)
      {
        _logger.LogWarning($"Previously moved from {lastMovementPosition}. Skipping.");
        return Task.CompletedTask;
      }

      if (message.Data.CurrentPosition.X == message.Data.TargetPosition.X && message.Data.CurrentPosition.Y == message.Data.TargetPosition.Y)
      {
        _logger.LogWarning($"The current and target positions are the same ({message.Data.CurrentPosition.X},{message.Data.CurrentPosition.Y}). Skipping.");
        return Task.CompletedTask;
      }

      var windowsPointerScaleFactor = WindowsMouseUtil.GetPointerScaleFactor(CurrentSystemInformation);
      message.Data.MovementScaleX = message.Data.MovementScaleX * windowsPointerScaleFactor;
      message.Data.MovementScaleY = message.Data.MovementScaleY * windowsPointerScaleFactor;

      if (_mouse.SendMouseMoveTo(message.Data))
      {
        _lastMovementPosition = _lastMovementPosition with
        {
          X = message.Data.CurrentPosition.X,
          Y = message.Data.CurrentPosition.Y,
        };
      }
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

    #region IObserver<ContextEvent>
    public void OnCompleted()
    {
      // Do Nothing
    }

    public void OnError(Exception error)
    {
      // Do Nothing
    }

    public void OnNext(ContextEvent contextEvent)
    {
      switch (contextEvent)
      {
        case PointerPositionChangedEvent pointerPositionChanged:
          CurrentPointerPosition = pointerPositionChanged.NewPointerPosition;
          break;
        case SystemInformationChangedEvent systemInformationChanged:
          CurrentSystemInformation = systemInformationChanged.SysInfo;
          break;
      }
    }
    #endregion
  }
}