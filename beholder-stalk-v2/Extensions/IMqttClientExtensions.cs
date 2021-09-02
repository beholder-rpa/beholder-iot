namespace beholder_stalk_v2
{
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Routing;
  using MQTTnet;
  using MQTTnet.Extensions.ManagedClient;
  using System.Collections.Generic;
  using System.Text.Json;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;

  public static class IMqttClientExtensions
  {
    public static void SubscribeControllers(this IManagedMqttClient client, MqttApplicationMessageRouter router)
    {
      var filters = new List<MqttTopicFilter>();
      foreach (var pattern in router.RouteTable.Keys)
      {
        var patternFilter = new MqttTopicFilterBuilder()
                    .WithTopic(pattern)
                    .Build();
        filters.Add(patternFilter);
      }

      client.SubscribeAsync(filters.ToArray());
    }

    public static async Task PublishEventAsync<T>(this IManagedMqttClient client, string pubSubName, string topic, T data, BeholderServiceInfo serviceInfo = null, CancellationToken cancellationToken = default)
    {
      if (serviceInfo == null)
      {
        serviceInfo = new BeholderServiceInfo();
      }

      // Replace tokens within the pattern
      var pattern = Regex.Replace(topic, @"{\s*?hostname\s*?}", serviceInfo.HostName, RegexOptions.IgnoreCase | RegexOptions.Compiled);

      var cloudEvent = new CloudEvent<T>()
      {
        Data = data,
        Source = "stalk",
        Type = "com.dapr.event.sent",
        ExtensionAttributes = new Dictionary<string, object>()
        {
          { "pubsubname", pubSubName },
          { "topic", pattern },
        }
      };

      await client.PublishAsync(
                new MqttApplicationMessageBuilder()
                    .WithTopic(pattern)
                    .WithPayload(JsonSerializer.Serialize(cloudEvent))
                    .Build(),
                cancellationToken
            );
    }
  }
}