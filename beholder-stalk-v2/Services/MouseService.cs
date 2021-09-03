namespace beholder_stalk_v2.Services
{
  using beholder_stalk_v2.Protos;
  using Grpc.Core;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;

  public class MouseService : Mouse.MouseBase
  {
    private readonly HardwareInterfaceDevices.Mouse _mouse;
    private readonly IBeholderMqttClient _client;
    private readonly ILogger<MouseService> _logger;

    public MouseService(HardwareInterfaceDevices.Mouse mouse, IBeholderMqttClient client, ILogger<MouseService> logger)
    {
      _mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
      _client = client ?? throw new ArgumentNullException(nameof(client));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<SendMouseClickReply> SendMouseClick(SendMouseClickRequest request, ServerCallContext context)
    {
      _mouse.SendMouseClick(request.MouseClick?.Button, request.MouseClick?.ClickDirection ?? MouseClick.Types.ClickDirection.PressAndRelease, request.MouseClick.Duration);
      _logger.LogInformation($"Sent Mouse Click {request.MouseClick}");
      return Task.FromResult(new SendMouseClickReply());
    }
  }
}