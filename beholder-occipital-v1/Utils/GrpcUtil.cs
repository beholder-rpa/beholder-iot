namespace beholder_occipital_v1
{
  using Dapr.AppCallback.Autogen.Grpc.v1;
  using Dapr.Client;
  using Dapr.Client.Autogen.Grpc.v1;
  using Google.Protobuf;
  using Google.Protobuf.WellKnownTypes;
  using System;
  using System.Collections.Generic;
  using System.Net.Mime;
  using System.Text.Json;
  using System.Threading;
  using System.Threading.Tasks;

  public static class GrpcUtil
  {
    private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy() };

    /// <summary>
    /// Invokes the specified method using the payload contained in a TopicEventRequest. If pubsubName and topicName are specified and the method return value is not empty, publishes the return value as an event.
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TReply"></typeparam>
    /// <param name="client"></param>
    /// <param name="request"></param>
    /// <param name="method"></param>
    /// <param name="pubsubName"></param>
    /// <param name="topicName"></param>
    /// <param name="metadata"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<TopicEventResponse> InvokeMethodFromEvent<TRequest, TReply>(DaprClient client, TopicEventRequest request, Func<TRequest, Task<TReply>> method, string pubsubName = null, string topicName = null, Dictionary<string, string> metadata = null, CancellationToken cancellationToken = default)
      where TRequest : IMessage, new()
    {
      var response = new TopicEventResponse();
      switch (request.DataContentType)
      {
        case MediaTypeNames.Application.Json:
          var input = JsonSerializer.Deserialize<TRequest>(request.Data.ToStringUtf8(), JsonOptions);
          var reply = await method(input);
          response.Status = TopicEventResponse.Types.TopicEventResponseStatus.Success;
          if (reply is Empty || reply == null || string.IsNullOrWhiteSpace(pubsubName) || string.IsNullOrWhiteSpace(topicName))
          {
            return response;
          }

          await client.PublishEventAsync(pubsubName, topicName, reply, metadata, cancellationToken);
          break;
        default:
          throw new NotImplementedException($"TopicEventRequests of {request.Topic} of data content type {request.DataContentType} have not been implemented.");
      }
      return response;
    }

    public static async Task<InvokeResponse> InvokeMethodFromInvoke<TRequest, TReply>(InvokeRequest request, Func<TRequest, Task<TReply>> method)
      where TRequest : IMessage, new()
      where TReply : IMessage
    {
      var response = new InvokeResponse()
      {
        ContentType = request.ContentType
      };

      TRequest input;
      switch (request.ContentType)
      {
        case MediaTypeNames.Application.Json:
          {
            Console.WriteLine(request.Data.Value.ToStringUtf8());
            input = JsonSerializer.Deserialize<TRequest>(request.Data.Value.ToStringUtf8(), JsonOptions);
            var output = await method(input);
            response.Data = new Any() { Value = ByteString.CopyFromUtf8(JsonSerializer.Serialize(output, JsonOptions)) };
          }
          break;
        default:
          {
            input = request.Data.Unpack<TRequest>();
            var output = await method(input);
            response.Data = Any.Pack(output);
          }
          break;
      }

      return response;
    }
  }
}