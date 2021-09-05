namespace beholder_stalk_v2
{
  using MQTTnet.Extensions.ManagedClient;
  using System;
  using System.Threading;
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

    /// <summary>
    /// Publishes a message as a CloudEvent with the specified topic and no data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PublishEventAsync(string topic, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message as a CloudEvent with the specified topic and data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="topic"></param>
    /// <param name="data"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task PublishEventAsync<T>(string topic, T data, CancellationToken cancellationToken = default);
  }
}