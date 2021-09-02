namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Logging;
  using MQTTnet;
  using System;
  using System.Threading.Tasks;

  [MqttController]
  public class MouseController
  {
    private readonly ILogger<MouseController> _logger;
    private readonly Mouse _mouse;

    public MouseController(Mouse mouse, ILogger<MouseController> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
      //_mouse.MouseResolutionChanged += HandleMouseResolutionChanged;
    }

    private void HandleMouseResolutionChanged(object sender, MouseResolutionChangedEventArgs e)
    {
      //_daprClient.PublishEventAsync(Consts.PubSubName, $"beholder/stalk/{_hostName}/status/mouse/resolution", new MouseResolution() { });
      _logger.LogInformation($"Published mouse resolution changed");
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/click")]
    public Task SendClick(ICloudEvent<SendMouseClickRequest> request)
    {
      throw new NotImplementedException();
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/actions")]
    public Task SendActions(MqttApplicationMessage message)
    {
      throw new NotImplementedException();
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/raw")]
    public Task SendRaw(MqttApplicationMessage message)
    {
      throw new NotImplementedException();
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/reset")]
    public Task SendReset(MqttApplicationMessage message)
    {
      throw new NotImplementedException();
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/click_duration")]
    public Task SetClickDuration(MqttApplicationMessage message)
    {
      throw new NotImplementedException();
    }

    [EventPattern("beholder/stalk/{HOSTNAME}/mouse/move_mouse_to")]
    public Task MoveMouseTo(MqttApplicationMessage message)
    {
      throw new NotImplementedException();
    }
  }
}