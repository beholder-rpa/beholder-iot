namespace beholder_stalk_v2
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

  public class KeyboardService : AppCallback.AppCallbackBase
  {
    private readonly Keyboard _keyboard;
    private readonly DaprClient _daprClient;
    private readonly ILogger<KeyboardService> _logger;
    private readonly string _hostName;

    public KeyboardService(Keyboard keyboard, DaprClient daprClient, ILogger<KeyboardService> logger)
    {
      _keyboard = keyboard ?? throw new ArgumentNullException(nameof(keyboard));
      _keyboard.KeyboardLedsChanged += HandleKeyboardLedsChanged;
      _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _hostName = Environment.GetEnvironmentVariable("BEHOLDER_STALK_NAME") ?? Dns.GetHostName();
    }

    private void HandleKeyboardLedsChanged(object sender, KeyboardLedsChangedEventArgs e)
    {
      _daprClient.PublishEventAsync(Consts.PubSubName, "keyboardledschanged", e.KeyboardLeds);
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
        "SendKey" => await Util.InvokeMethodFromInvoke<SendKeyRequest, Empty>(request, (input) => SendKey(input)),
        "SendKeys" => await Util.InvokeMethodFromInvoke<SendKeysRequest, Empty>(request, (input) => SendKeys(input)),
        "SendKeysRaw" => await Util.InvokeMethodFromInvoke<SendKeysRawRequest, Empty>(request, (input) => SendKeysRaw(input)),
        "SendKeysReset" => await Util.InvokeMethodFromInvoke<Empty, Empty>(request, (input) => SendKeysReset()),
        "SetAverageKeypressDuration" => await Util.InvokeMethodFromInvoke<SetAverageKeypressDurationRequest, SetAverageKeypressDurationReply>(request, (input) => SetAverageKeypressDuration(input)),
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
        Topic = $"beholder/stalk/{_hostName}/sendkey"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/sendkeys"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/sendkeysraw"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/sendkeysreset"
      });

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/setaveragekeypressduration"
      });

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
      var topic = request.Topic.Replace($"beholder/stalk/{_hostName}/", "");

      return topic switch
      {
        "sendkey" => await Util.InvokeMethodFromEvent<SendKeyRequest, Empty>(_daprClient, request, (input) => SendKey(input)),
        "sendkeys" => await Util.InvokeMethodFromEvent<SendKeysRequest, Empty>(_daprClient, request, (input) => SendKeys(input)),
        "sendkeysraw" => await Util.InvokeMethodFromEvent<SendKeysRawRequest, Empty>(_daprClient, request, (input) => SendKeysRaw(input)),
        "sendkeysreset" => await Util.InvokeMethodFromEvent<Empty, Empty>(_daprClient, request, (input) => SendKeysReset()),
        "setaveragekeypressduration" => await Util.InvokeMethodFromEvent<SetAverageKeypressDurationRequest, SetAverageKeypressDurationReply>(_daprClient, request, (input) => SetAverageKeypressDuration(input)),
        _ => new TopicEventResponse(),
      };
    }

    public async Task<Empty> SendKey(SendKeyRequest request)
    {
      await _keyboard.SendKey(request.Keypress);
      _logger.LogInformation($"Sent Keypress {request.Keypress}");
      return null;
    }

    public async Task<Empty> SendKeys(SendKeysRequest request)
    {
      await _keyboard.SendKeys(request.Keys);
      _logger.LogInformation($"Sent Keys {request.Keys}");
      return null;
    }

    public Task<Empty> SendKeysRaw(SendKeysRawRequest request)
    {
      _keyboard.SendRaw(request.Report.ToByteArray());
      _logger.LogInformation($"Sent Raw Keys {request.Report}");
      return null;
    }

    public Task<Empty> SendKeysReset()
    {
      _keyboard.SendKeysReset();
      _logger.LogInformation("Reset Keys");
      return null;
    }

    public Task<SetAverageKeypressDurationReply> SetAverageKeypressDuration(SetAverageKeypressDurationRequest request)
    {
      if (request.Duration != null)
      {
        _keyboard.AverageKeypressDuration = request.Duration;
      }

      _logger.LogInformation($"Set Average Keypress Duration to {request.Duration}");

      return Task.FromResult(new SetAverageKeypressDurationReply()
      {
        Duration = _keyboard.AverageKeypressDuration
      });
    }
  }
}