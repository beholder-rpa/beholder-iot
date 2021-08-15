namespace beholder_occipital_v1.Services
{
  using Dapr.AppCallback.Autogen.Grpc.v1;
  using Dapr.Client.Autogen.Grpc.v1;
  using Google.Protobuf.WellKnownTypes;
  using Grpc.Core;
  using System.Threading.Tasks;

  public interface IObjectDetectionService
  {
    string Name { get; }

    Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context);

    Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context);

    Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context);

    /// <summary>
    /// Instructs the object detection service to send any status messages that indicate the current state of the occipital
    /// </summary>
    /// <returns></returns>
    Task OnStatusEvent();
  }
}