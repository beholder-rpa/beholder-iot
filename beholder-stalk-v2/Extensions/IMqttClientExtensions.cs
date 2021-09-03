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
  }
}