namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using Microsoft.Extensions.Logging;
  using MQTTnet;
  using System;
  using System.Threading.Tasks;

  public class JoystickController
  {
    private readonly ILogger<JoystickController> _logger;
    private readonly Joystick _joystick;

    public JoystickController(Joystick joystick, ILogger<JoystickController> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _joystick = joystick ?? throw new ArgumentNullException(nameof(joystick));
    }
  }
}