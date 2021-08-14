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

  public class MouseService : IHumanInterfaceService
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

    public string Name => "mouse";

    private void HandleMouseResolutionChanged(object sender, MouseResolutionChangedEventArgs e)
    {
      _daprClient.PublishEventAsync(Consts.PubSubName, $"beholder/stalk/{_hostName}/status/mouse/resolution", new MouseResolution() { });
      _logger.LogInformation($"Published mouse resolution changed");
    }

    /// <summary>
    /// Implement OnInvoke to support sendmouseclick, sendmouseacctions, sendmouseraw, sendmousereset and setaverageclickduration invocations
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
    {
      return request.Method switch
      {
        "SendMouseClick" => await Util.InvokeMethodFromInvoke<SendMouseClickRequest, Empty>(request, (input) => SendMouseClick(input)),
        "SendMouseActions" => await Util.InvokeMethodFromInvoke<SendMouseActionsRequest, Empty>(request, (input) => SendMouseActions(input)),
        "SendMouseRaw" => await Util.InvokeMethodFromInvoke<SendMouseRawRequest, Empty>(request, (input) => SendMouseRaw(input)),
        "SendMouseReset" => await Util.InvokeMethodFromInvoke<Empty, Empty>(request, (input) => SendMouseReset()),
        "SetAverageClickDuration" => await Util.InvokeMethodFromInvoke<SetAverageMouseClickDurationRequest, SetAverageMouseClickDurationReply>(request, (input) => SetAverageMouseClickDuration(input)),
        _ => null,
      };
    }

    /// <summary>
    /// Implement ListTopicSubscriptions to register sendmouseclick, sendmouseacctions, sendmouseraw, sendmousereset and setaverageclickduration events
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
    {
      var result = new ListTopicSubscriptionsResponse();
      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/mouse/click"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/mouse/actions"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/mouse/raw"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/mouse/reset"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/mouse/click_duration"
      });

      return Task.FromResult(result);
    }

    /// <summary>
    /// Implement OnTopicEvent to handle sendmouseclick, sendmouseacctions, sendmouseraw, sendmousereset and setaverageclickduration events
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
    {
      var topic = request.Topic.Replace($"beholder/stalk/{_hostName}/mouse/", "");
      return topic switch
      {
        "click" => await Util.InvokeMethodFromEvent<SendMouseClickRequest, Empty>(_daprClient, request, (input) => SendMouseClick(input)),
        "actions" => await Util.InvokeMethodFromEvent<SendMouseActionsRequest, Empty>(_daprClient, request, (input) => SendMouseActions(input)),
        "raw" => await Util.InvokeMethodFromEvent<SendMouseRawRequest, Empty>(_daprClient, request, (input) => SendMouseRaw(input)),
        "reset" => await Util.InvokeMethodFromEvent<Empty, Empty>(_daprClient, request, (input) => SendMouseReset()),
        "click_duration" => await Util.InvokeMethodFromEvent<SetAverageMouseClickDurationRequest, SetAverageMouseClickDurationReply>(_daprClient, request, (input) => SetAverageMouseClickDuration(input)),
        _ => new TopicEventResponse(),
      };
    }

    public async Task<Empty> SendMouseClick(SendMouseClickRequest request)
    {
      await _mouse.SendMouseClick(request.MouseClick.Button, request.MouseClick.ClickDirection, request.MouseClick.Duration);
      _logger.LogInformation($"Sent Mouse Click {request.MouseClick}");
      return null;
    }

    public async Task<Empty> SendMouseActions(SendMouseActionsRequest request)
    {
      await _mouse.SendMouseActions(request.Actions);
      _logger.LogInformation($"Sent Mouse Actions {request.Actions}");
      return null;
    }

    public Task<Empty> SendMouseRaw(SendMouseRawRequest request)
    {
      _mouse.SendRaw(request.Report.ToByteArray());
      _logger.LogInformation($"Sent Raw Mouse Actions {request.Report}");
      return null;
    }

    public Task<Empty> SendMouseReset()
    {
      _mouse.SendMouseReset();
      _logger.LogInformation("Reset Mouse");
      return null;
    }

    public Task<SetAverageMouseClickDurationReply> SetAverageMouseClickDuration(SetAverageMouseClickDurationRequest request)
    {
      if (request.Duration != null)
      {
        _mouse.AverageClickDuration = request.Duration;
      }

      _logger.LogInformation($"Set Average Click Duration to {request.Duration}");

      return Task.FromResult(new SetAverageMouseClickDurationReply()
      {
        Duration = _mouse.AverageClickDuration
      });
    }

    public async Task OnStatusEvent()
    {
      await _daprClient.PublishEventAsync(Consts.PubSubName, $"beholder/stalk/{_hostName}/mouse/status/mouse/click_duration", new SetAverageMouseClickDurationReply()
      {
        Duration = _mouse.AverageClickDuration
      });

      _logger.LogInformation($"Published mouse status");
    }
  }
}