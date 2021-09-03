namespace beholder_stalk_v2.Services
{
  using beholder_stalk_v2.Protos;
  using Grpc.Core;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;

  public class KeyboardService : Keyboard.KeyboardBase
  {
    private readonly HardwareInterfaceDevices.Keyboard _keyboard;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<KeyboardService> _logger;

    public KeyboardService(HardwareInterfaceDevices.Keyboard keyboard, IBeholderMqttClient client, ILogger<KeyboardService> logger)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<SendKeyReply> SendKey(SendKeyRequest request, ServerCallContext context)
    {
      await _keyboard.SendKey(request.Keypress?.Key, request.Keypress?.KeyDirection ?? Keypress.Types.KeyDirection.PressAndRelease, request.Keypress?.Modifiers, request.Keypress?.Duration);
      _logger.LogInformation($"Sent Keypress {request.Keypress}");
      return new SendKeyReply();
    }
  }
}
