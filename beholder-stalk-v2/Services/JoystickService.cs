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

  public class JoystickService : IHumanInterfaceService
  {
    private readonly Joystick _joystick;
    private readonly DaprClient _daprClient;
    private readonly ILogger<JoystickService> _logger;
    private readonly string _hostName;

    public JoystickService(Joystick joystick, DaprClient daprClient, ILogger<JoystickService> logger)
    {
      _joystick = joystick ?? throw new ArgumentNullException(nameof(joystick));
      _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _hostName = Environment.GetEnvironmentVariable("BEHOLDER_STALK_NAME") ?? Dns.GetHostName();
    }

    public string Name => "joystick";

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
        "SendJoystickButtonClick" => await GrpcUtil.InvokeMethodFromInvoke<SendJoystickButtonPressRequest, Empty>(request, (input) => SendJoystickButtonPress(input)),
        "SendJoystickActions" => await GrpcUtil.InvokeMethodFromInvoke<SendJoystickActionsRequest, Empty>(request, (input) => SendJoystickActions(input)),
        "SendJoystickRaw" => await GrpcUtil.InvokeMethodFromInvoke<SendJoystickRawRequest, Empty>(request, (input) => SendJoystickRaw(input)),
        "SendJoystickReset" => await GrpcUtil.InvokeMethodFromInvoke<Empty, Empty>(request, (input) => SendJoystickReset()),
        "SetAverageJoystickButtonPressDuration" => await GrpcUtil.InvokeMethodFromInvoke<SetAverageJoystickButtonPressDurationRequest, SetAverageJoystickButtonPressDurationReply>(request, (input) => SetAverageJoystickButtonPressDuration(input)),
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
        Topic = $"beholder/stalk/{_hostName}/joystick/button_press"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/joystick/actions"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/joystick/raw"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/joystick/reset"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/joystick/button_press_duration"
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
        "click" => await GrpcUtil.InvokeMethodFromEvent<SendJoystickButtonPressRequest, Empty>(_daprClient, request, (input) => SendJoystickButtonPress(input)),
        "actions" => await GrpcUtil.InvokeMethodFromEvent<SendJoystickActionsRequest, Empty>(_daprClient, request, (input) => SendJoystickActions(input)),
        "raw" => await GrpcUtil.InvokeMethodFromEvent<SendJoystickRawRequest, Empty>(_daprClient, request, (input) => SendJoystickRaw(input)),
        "reset" => await GrpcUtil.InvokeMethodFromEvent<Empty, Empty>(_daprClient, request, (input) => SendJoystickReset()),
        "click_duration" => await GrpcUtil.InvokeMethodFromEvent<SetAverageJoystickButtonPressDurationRequest, SetAverageJoystickButtonPressDurationReply>(_daprClient, request, (input) => SetAverageJoystickButtonPressDuration(input)),
        _ => new TopicEventResponse(),
      };
    }

    public async Task<Empty> SendJoystickButtonPress(SendJoystickButtonPressRequest request)
    {
      await _joystick.SendButton(request.ButtonPress?.Button, request.ButtonPress?.PressDirection ?? JoystickButtonPress.Types.ButtonPressDirection.PressAndRelease, request.ButtonPress?.Duration);
      _logger.LogInformation($"Sent Joystick Click {request.ButtonPress}");
      return null;
    }

    public async Task<Empty> SendJoystickActions(SendJoystickActionsRequest request)
    {
      await _joystick.SendJoystickActions(request.Actions);
      _logger.LogInformation($"Sent Joystick Actions {request.Actions}");
      return null;
    }

    public Task<Empty> SendJoystickRaw(SendJoystickRawRequest request)
    {
      _joystick.SendRaw(request.Report.ToByteArray());
      _logger.LogInformation($"Sent Raw Joystick Actions {request.Report}");
      return Task.FromResult<Empty>(null);
    }

    public Task<Empty> SendJoystickReset()
    {
      _joystick.SendJoystickReset();
      _logger.LogInformation("Reset Joystick");
      return Task.FromResult<Empty>(null);
    }

    public Task<SetAverageJoystickButtonPressDurationReply> SetAverageJoystickButtonPressDuration(SetAverageJoystickButtonPressDurationRequest request)
    {
      if (request.Duration != null)
      {
        _joystick.AveragePressDuration = request.Duration;
      }

      _logger.LogInformation($"Set Average Click Duration to {request.Duration}");

      return Task.FromResult(new SetAverageJoystickButtonPressDurationReply()
      {
        Duration = _joystick.AveragePressDuration
      });
    }

    public async Task OnStatusEvent()
    {
      await _daprClient.PublishEventAsync(Consts.PubSubName, $"beholder/stalk/{_hostName}/mouse/status/mouse/click_duration", new SetAverageJoystickButtonPressDurationReply()
      {
        Duration = _joystick.AveragePressDuration
      });

      _logger.LogInformation($"Published joystick status");
    }
  }
}