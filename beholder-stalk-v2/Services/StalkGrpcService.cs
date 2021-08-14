namespace beholder_stalk_v2.Services
{
  using Dapr.AppCallback.Autogen.Grpc.v1;
  using Dapr.Client.Autogen.Grpc.v1;
  using Google.Protobuf.WellKnownTypes;
  using Grpc.Core;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Net;
  using System.Threading.Tasks;

  public class StalkGrpcService : AppCallback.AppCallbackBase
  {
    private readonly IDictionary<string, IHumanInterfaceService> _hidServices = new Dictionary<string, IHumanInterfaceService>();

    private readonly ILogger<StalkGrpcService> _logger;
    private readonly string _hostName;

    public StalkGrpcService(IEnumerable<IHumanInterfaceService> hidServices, ILogger<StalkGrpcService> logger)
    {
      if (hidServices == null || !hidServices.Any())
      {
        throw new ArgumentNullException(nameof(hidServices));
      }

      foreach(var hidService in hidServices)
      {
        if (_hidServices.ContainsKey(hidService.Name))
        {
          throw new InvalidOperationException($"Encountered multiple hid services named {hidService.Name}. HID Service names must be unique.");
        }

        _hidServices.Add(hidService.Name, hidService);
      }

      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _hostName = Environment.GetEnvironmentVariable("BEHOLDER_STALK_NAME") ?? Dns.GetHostName();
    }

    public override async Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
    {
      var result = new ListTopicSubscriptionsResponse();
      foreach(var hidService in _hidServices.Values)
      {
        var hidResponse = await hidService.ListTopicSubscriptions(request, context);
        result.Subscriptions.AddRange(hidResponse.Subscriptions);
      }

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/stalk/{_hostName}/status"
      });

      return result;
    }

    public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
    {
      foreach(var hidService in _hidServices.Values.OrderBy(h => h.Name))
      {
        var response = await hidService.OnInvoke(request, context);
        if (response != null)
        {
          return response;
        }
      }

      throw new InvalidOperationException($"Unable to locate service method implementation for {request.Method}");
    }

    public override async Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
    {
      if (request.PubsubName != Consts.PubSubName || !request.Topic.StartsWith($"beholder/stalk/{_hostName}/"))
      {
        return new TopicEventResponse();
      }

      _logger.LogInformation($"Received Topic Event for {request.Topic}");
      var topic = request.Topic.Replace($"beholder/stalk/{_hostName}/", "");
      var topicTarget = topic.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
      if (string.IsNullOrWhiteSpace(topicTarget))
      {
        throw new InvalidOperationException($"Unable to determine topic target from the following topic intended for this host: {request.Topic}");
      }

      if (topicTarget == "status")
      {
        foreach (var hidService in _hidServices.Values.OrderBy(h => h.Name))
        {
          await hidService.OnStatusEvent();
        }
        return new TopicEventResponse();
      }

      if (!_hidServices.ContainsKey(topicTarget))
      {
        throw new InvalidOperationException($"Invalid or unsupported topic target: {topicTarget}");
      }

      return await _hidServices[topicTarget].OnTopicEvent(request, context);
    }
  }
}
