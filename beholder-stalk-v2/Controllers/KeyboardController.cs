namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using Microsoft.Extensions.Logging;
  using MQTTnet;
  using System;
  using System.Threading.Tasks;

  public class KeyboardController
  {
    private readonly ILogger<KeyboardController> _logger;
    private readonly Keyboard _keyboard;

    public KeyboardController(Keyboard keyboard, ILogger<KeyboardController> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
    }
  }
}