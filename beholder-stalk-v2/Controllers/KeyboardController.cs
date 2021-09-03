namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;
  using Keyboard = HardwareInterfaceDevices.Keyboard;

  [MqttController]
  public class KeyboardController
  {
    private readonly Keyboard _keyboard;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<KeyboardController> _logger;

    public KeyboardController(Keyboard keyboard, IBeholderMqttClient client, ILogger<KeyboardController> logger)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _keyboard.KeyboardLedsChanged += new EventHandler<KeyboardLedsChangedEventArgs>(async (sender, e) => await HandleKeyboardLedsChanged(sender, e));
    }

    private async Task HandleKeyboardLedsChanged(object sender, KeyboardLedsChangedEventArgs e)
    {
      await _client
        .PublishEventAsync(
          "beholder/stalk/{HOSTNAME}/status/keyboard/leds",
          e.KeyboardLeds
        );
      _logger.LogInformation($"Published keyboard leds changed");
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/keyboard/key")]
    public async Task SendKey(ICloudEvent<SendKeyRequest> message)
    {
      await _keyboard.SendKey(message.Data.Keypress?.Key, message.Data.Keypress?.KeyDirection ?? Keypress.Types.KeyDirection.PressAndRelease, message.Data.Keypress?.Modifiers, message.Data.Keypress?.Duration);
      _logger.LogInformation($"Sent Keypress {message.Data.Keypress}");
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/keyboard/keys")]
    public async Task SendKeys(ICloudEvent<SendKeysRequest> message)
    {
      await _keyboard.SendKeys(message.Data.Keys);
      _logger.LogInformation($"Sent Keys {message.Data.Keys}");
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/keyboard/raw")]
    public Task SendKeysRaw(ICloudEvent<SendKeysRawRequest> message)
    {
      _keyboard.SendRaw(message.Data.Report.ToByteArray());
      _logger.LogInformation($"Sent Raw Keys {message.Data.Report}");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/keyboard/reset")]
    public Task SendKeyboardReset()
    {
      _keyboard.SendKeysReset();
      _logger.LogInformation("Reset Keys");
      return Task.CompletedTask;
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/keyboard/keypress_duration")]
    public Task SetAverageKeypressDuration(ICloudEvent<SetAverageKeypressDurationRequest> message)
    {
      if (message.Data.Duration != null)
      {
        _keyboard.AverageKeypressDuration = message.Data.Duration;
      }

      _logger.LogInformation($"Set Average Keypress Duration to {message.Data.Duration}");
      return Status();

    }

    [EventPattern("beholder/stalk/{HOSTNAME}/keyboard/status")]
    public async Task Status()
    {
      await _client
        .PublishEventAsync(
          "beholder/stalk/{HOSTNAME}/status/keyboard/keypress_duration",
          new SetAverageKeypressDurationReply()
          {
            Duration = _keyboard.AverageKeypressDuration
          }
        );

      _logger.LogInformation($"Published mouse status");
    }
  }
}