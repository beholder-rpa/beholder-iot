namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;

  [MqttController]
  public class JoystickController
  {
    private readonly Joystick _joystick;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<JoystickController> _logger;

    public JoystickController(Joystick joystick, IBeholderMqttClient client, ILogger<JoystickController> logger)
    {
      _joystick = joystick ?? throw new ArgumentNullException(nameof(joystick));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/joystick/button_press")]
    public async Task SendButton(ICloudEvent<SendJoystickButtonPressRequest> message)
    {
      await _joystick.SendButton(message.Data.ButtonPress?.Button, message.Data.ButtonPress?.PressDirection ?? JoystickButtonPress.Types.ButtonPressDirection.PressAndRelease, message.Data.ButtonPress?.Duration);
      _logger.LogInformation($"Sent Joystick Button Press {message.Data.ButtonPress}");
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/joystick/actions")]
    public async Task SendJoystickActions(ICloudEvent<SendJoystickActionsRequest> message)
    {
      await _joystick.SendJoystickActions(message.Data.Actions);
      _logger.LogInformation($"Sent Joystick Actions {message.Data.Actions}");
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/joystick/raw")]
    public Task SendJoystickRaw(ICloudEvent<SendJoystickRawRequest> message)
    {
      _joystick.SendRaw(message.Data.Report.ToByteArray());
      _logger.LogInformation($"Sent Raw Joystick Report {message.Data.Report}");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/joystick/reset")]
    public Task SendJoystickReset()
    {
      _joystick.SendJoystickReset();
      _logger.LogInformation("Reset Joystick");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/joystick/button_press_duration")]
    public Task SetJoystickButtonPressDuration(ICloudEvent<SetAverageJoystickButtonPressDurationRequest> message)
    {
      if (message.Data.Duration != null)
      {
        _joystick.AveragePressDuration = message.Data.Duration;
      }

      _logger.LogInformation($"Set Average Click Duration to {message.Data.Duration}");
      return Status();
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/joystick/status")]
    public async Task Status()
    {
      await _client
        .PublishEventAsync(
          "beholder/stalk/{HOSTNAME}/status/joystick/button_press_duration",
          new SetAverageJoystickButtonPressDurationReply()
          {
            Duration = _joystick.AveragePressDuration
          }
        );

      _logger.LogInformation($"Published joystick status");
    }
  }
}