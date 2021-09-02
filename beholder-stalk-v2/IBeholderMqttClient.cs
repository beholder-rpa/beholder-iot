namespace beholder_stalk_v2
{
  using MQTTnet.Extensions.ManagedClient;
  using System;
  using System.Threading.Tasks;

  public interface IBeholderMqttClient : IObservable<MqttClientEvent>
  {
    /// <summary>
    /// Gets a value that indicates if the current instance is connected to MQTT
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// Gets a value that indicates if the current instance has been disposed
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Gets the underlying MqttClient implementation.
    /// </summary>
    IManagedMqttClient MqttClient { get; }

    /// <summary>
    /// Start receiving messages from MQTT
    /// </summary>
    Task StartAsync();

    /// <summary>
    /// Stops receiving messages from MQTT
    /// </summary>
    Task Disconnect();
  }
}