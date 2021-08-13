namespace beholder_stalk_v2.Services
{
  using beholder_stalk_v2.HardwareInterfaceDevices;
  using beholder_stalk_v2.Protos;
  using Dapr.AppCallback.Autogen.Grpc.v1;
  using Dapr.Client;
  using Dapr.Client.Autogen.Grpc.v1;
  using Google.Protobuf.WellKnownTypes;
  using Grpc.Core;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Net;
  using System.Threading.Tasks;

  public class MouseService : AppCallback.AppCallbackBase
  {
    private readonly Mouse _mouse;
    private readonly DaprClient _daprClient;
    private readonly ILogger<KeyboardService> _logger;
    private readonly string _hostName;

    public MouseService(Mouse mouse, DaprClient daprClient, ILogger<KeyboardService> logger)
    {
      _mouse = mouse ?? throw new ArgumentNullException(nameof(mouse));
      _mouse.MouseResolutionChanged += HandleMouseResolutionChanged;
      _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _hostName = Environment.GetEnvironmentVariable("BEHOLDER_STALK_NAME") ?? Dns.GetHostName();
    }

    private void HandleMouseResolutionChanged(object sender, MouseResolutionChangedEventArgs e)
    {
      _daprClient.PublishEventAsync(Consts.PubSubName, "beholder/mouseresolutionchanged", new MouseResolution() { });
      _logger.LogInformation($"Published mouse resolution changed");
    }

    /// <summary>
    /// Implement OnInvoke to support sendkey, sendkeys, sendkeysraw, sendkeysreset, and keypressduration invocations
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
    {
      return request.Method switch
      {
        "SendMouseClick" => await Util.InvokeMethodFromInvoke<SendClickRequest, Empty>(request, (input) => SendMouseClick(input)),
        //"SendMouseAction" => await Util.InvokeMethodFromInvoke<SendKeysRequest, Empty>(request, (input) => SendKeys(input)),
        //"SendMouseRaw" => await Util.InvokeMethodFromInvoke<SendKeysRawRequest, Empty>(request, (input) => SendKeysRaw(input)),
        //"SendMouseReset" => await Util.InvokeMethodFromInvoke<Empty, Empty>(request, (input) => SendKeysReset()),
        //"SetAverageClickDuration" => await Util.InvokeMethodFromInvoke<SetAverageKeypressDurationRequest, SetAverageKeypressDurationReply>(request, (input) => SetAverageKeypressDuration(input)),
        _ => throw new NotImplementedException(),
      };
    }

    /// <summary>
    /// Implement ListTopicSubscriptions to register sendkey, sendkeys, sendkeysraw, sendkeysreset, and keypressduration events
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
    {
      var result = new ListTopicSubscriptionsResponse();
      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/mouse/click"
      });

      //result.Subscriptions.Add(new TopicSubscription
      //{
      //  PubsubName = Consts.PubSubName,
      //  Topic = $"beholder/stalk/{_hostName}/sendkeys"
      //});

      //result.Subscriptions.Add(new TopicSubscription
      //{
      //  PubsubName = Consts.PubSubName,
      //  Topic = $"beholder/stalk/{_hostName}/sendkeysraw"
      //});

      //result.Subscriptions.Add(new TopicSubscription
      //{
      //  PubsubName = Consts.PubSubName,
      //  Topic = $"beholder/stalk/{_hostName}/sendkeysreset"
      //});

      //result.Subscriptions.Add(new TopicSubscription
      //{
      //  PubsubName = Consts.PubSubName,
      //  Topic = $"beholder/stalk/{_hostName}/setaveragekeypressduration"
      //});

      return Task.FromResult(result);
    }

    /// <summary>
    /// Implement OnTopicEvent to handle sendkey, sendkeys, sendkeysraw, sendkeysreset, and keypressduration events
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
    {
      if (request.PubsubName != Consts.PubSubName)
      {
        return new TopicEventResponse();
      }

      _logger.LogInformation($"Received Topic Event for {request.Topic}");
      var topic = request.Topic.Replace($"beholder/stalk/{_hostName}/mouse/", "");

      return topic switch
      {
        "click" => await Util.InvokeMethodFromEvent<SendClickRequest, Empty>(_daprClient, request, (input) => SendMouseClick(input)),
        //"sendkeys" => await Util.InvokeMethodFromEvent<SendKeysRequest, Empty>(_daprClient, request, (input) => SendKeys(input)),
        //"sendkeysraw" => await Util.InvokeMethodFromEvent<SendKeysRawRequest, Empty>(_daprClient, request, (input) => SendKeysRaw(input)),
        //"sendkeysreset" => await Util.InvokeMethodFromEvent<Empty, Empty>(_daprClient, request, (input) => SendKeysReset()),
        //"setaveragekeypressduration" => await Util.InvokeMethodFromEvent<SetAverageKeypressDurationRequest, SetAverageKeypressDurationReply>(_daprClient, request, (input) => SetAverageKeypressDuration(input)),
        _ => new TopicEventResponse(),
      };
    }

    public async Task<Empty> SendMouseClick(SendClickRequest request)
    {
      await _mouse.SendMouseClick(request.MouseClick.Button, request.MouseClick.ClickDirection, request.MouseClick.Duration);
      _logger.LogInformation($"Sent Mouse Click {request.MouseClick}");
      return null;
    }
  }
}