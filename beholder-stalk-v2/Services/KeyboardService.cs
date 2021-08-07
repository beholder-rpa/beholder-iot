namespace beholder_stalk_v2
{
  using beholder_stalk_v2.Protos;
  using Grpc.Core;
  using Microsoft.Extensions.Logging;
  using System.Threading.Tasks;

  public class KeyboardService : Keyboard.KeyboardBase
  {
    private readonly ILogger<KeyboardService> _logger;
    public KeyboardService(ILogger<KeyboardService> logger)
    {
      _logger = logger;
    }

    public override Task<SendKeyReply> SendKey(SendKeyRequest request, ServerCallContext context)
    {
      return base.SendKey(request, context);
    }
  }
}
