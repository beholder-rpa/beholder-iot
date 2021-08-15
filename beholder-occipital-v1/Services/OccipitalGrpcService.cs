namespace beholder_occipital_v1.Services
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

  public class OccipitalGrpcService : AppCallback.AppCallbackBase
  {
    private readonly IDictionary<string, IObjectDetectionService> _objectDetectionServices = new Dictionary<string, IObjectDetectionService>();

    private readonly ILogger<OccipitalGrpcService> _logger;
    private readonly string _hostName;

    public OccipitalGrpcService(IEnumerable<IObjectDetectionService> objectDetectionServices, ILogger<OccipitalGrpcService> logger)
    {
      if (objectDetectionServices == null || !objectDetectionServices.Any())
      {
        throw new ArgumentNullException(nameof(objectDetectionServices));
      }

      foreach (var objectDetectionService in objectDetectionServices)
      {
        if (_objectDetectionServices.ContainsKey(objectDetectionService.Name))
        {
          throw new InvalidOperationException($"Encountered multiple object detection services named {objectDetectionService.Name}. Object Detection Service names must be unique.");
        }

        _objectDetectionServices.Add(objectDetectionService.Name, objectDetectionService);
      }

      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _hostName = Environment.GetEnvironmentVariable("BEHOLDER_OCCIPITAL_NAME") ?? Dns.GetHostName();
    }

    public override async Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
    {
      var result = new ListTopicSubscriptionsResponse();
      foreach (var hidService in _objectDetectionServices.Values)
      {
        var objectDetectionResponse = await hidService.ListTopicSubscriptions(request, context);
        result.Subscriptions.AddRange(objectDetectionResponse.Subscriptions);
      }

      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/occipital/{_hostName}/status"
      });

      return result;
    }

    public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
    {
      foreach (var objectDetectionService in _objectDetectionServices.Values.OrderBy(h => h.Name))
      {
        var response = await objectDetectionService.OnInvoke(request, context);
        if (response != null)
        {
          return response;
        }
      }

      throw new InvalidOperationException($"Unable to locate service method implementation for {request.Method}");
    }

    public override async Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
    {
      if (request.PubsubName != Consts.PubSubName || !request.Topic.StartsWith($"beholder/occipital/{_hostName}/"))
      {
        return new TopicEventResponse();
      }

      _logger.LogInformation($"Received Topic Event for {request.Topic}");
      var topic = request.Topic.Replace($"beholder/occipital/{_hostName}/", "");
      var topicTarget = topic.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).FirstOrDefault();
      if (string.IsNullOrWhiteSpace(topicTarget))
      {
        throw new InvalidOperationException($"Unable to determine topic target from the following topic intended for this host: {request.Topic}");
      }

      if (topicTarget == "status")
      {
        foreach (var hidService in _objectDetectionServices.Values.OrderBy(h => h.Name))
        {
          await hidService.OnStatusEvent();
        }
        return new TopicEventResponse();
      }

      if (!_objectDetectionServices.ContainsKey(topicTarget))
      {
        throw new InvalidOperationException($"Invalid or unsupported topic target: {topicTarget}");
      }

      return await _objectDetectionServices[topicTarget].OnTopicEvent(request, context);
    }
  }
}