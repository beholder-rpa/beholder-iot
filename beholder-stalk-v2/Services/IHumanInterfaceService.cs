namespace beholder_stalk_v2.Services
{
  using Dapr.AppCallback.Autogen.Grpc.v1;
  using Dapr.Client.Autogen.Grpc.v1;
  using Google.Protobuf.WellKnownTypes;
  using Grpc.Core;
  using System.Threading.Tasks;

  /// <summary>
  /// Represents a service that handles HID 
  /// </summary>
  public interface IHumanInterfaceService
  {
    string Name { get; }

    Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context);

    Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context);

    Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context);

    /// <summary>
    /// Instructs the human interface service to send any status messages that indicate the current state of the HID
    /// </summary>
    /// <returns></returns>
    Task OnStatusEvent();
  }
}
