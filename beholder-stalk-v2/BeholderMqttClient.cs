namespace beholder_stalk_v2
{
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Routing;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Options;
  using MQTTnet;
  using MQTTnet.Client;
  using MQTTnet.Client.Connecting;
  using MQTTnet.Client.Disconnecting;
  using MQTTnet.Client.Options;
  using MQTTnet.Extensions.ManagedClient;
  using System;
  using System.Collections.Concurrent;
  using System.Threading.Tasks;

  public class BeholderMqttClient : IBeholderMqttClient
  {
    private readonly StalkOptions _options;
    private readonly MqttApplicationMessageRouter _router;
    private readonly ConcurrentDictionary<IObserver<MqttClientEvent>, MqttClientEventUnsubscriber> _observers = new ConcurrentDictionary<IObserver<MqttClientEvent>, MqttClientEventUnsubscriber>();
    private readonly IManagedMqttClientOptions _mqttClientOptions;
    private readonly ILogger<BeholderMqttClient> _logger;

    public BeholderMqttClient(IOptions<StalkOptions> options, MqttApplicationMessageRouter router, BeholderServiceInfo serviceInfo, ILogger<BeholderMqttClient> logger)
    {
      _options = (options ?? throw new ArgumentNullException(nameof(options))).Value;
      _router = router ?? throw new ArgumentNullException(nameof(router));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      _mqttClientOptions = new ManagedMqttClientOptionsBuilder()
         .WithAutoReconnectDelay(TimeSpan.FromMilliseconds(2500))
         .WithClientOptions(new MqttClientOptionsBuilder()
            .WithClientId($"stalk-v2-{serviceInfo.HostName}")
            .WithTcpServer(_options.MqttBrokerAddress, _options.MqttBrokerPort)
            .WithCredentials(_options.Username, _options.Password)
            .WithKeepAlivePeriod(TimeSpan.FromMilliseconds(_options.KeepAlivePeriodMs ?? 10000))
            .WithCommunicationTimeout(TimeSpan.FromMilliseconds(_options.CommunicationTimeoutMs ?? 15000))
            .WithWillDelayInterval(_options.WillDelayIntervalMs ?? 25000)
            .WithWillMessage(
              new MqttApplicationMessageBuilder()
                  .WithPayloadFormatIndicator(MQTTnet.Protocol.MqttPayloadFormatIndicator.CharacterData)
                  .WithContentType("text/plain")
                  .WithTopic($"beholder/ctas/{_options.HostName}/status")
                  .WithPayload("Disconnected")
                  .WithRetainFlag(true)
                  .Build()
            )
            .WithCleanSession()
            .Build()
            )
         .Build();

      // Create a new MQTT client.
      var factory = new MqttFactory();
      var mqttClient = factory.CreateManagedMqttClient();

      mqttClient.UseConnectedHandler(OnConnected);
      mqttClient.UseDisconnectedHandler(OnDisconnected);
      mqttClient.UseApplicationMessageReceivedHandler(OnApplicationMessageReceived);

      MqttClient = mqttClient;
    }

    public bool IsConnected
    {
      get
      {
        return MqttClient.IsConnected;
      }
    }

    public bool IsDisposed
    {
      get;
      private set;
    }

    public IManagedMqttClient MqttClient
    {
      get;
    }

    public async Task StartAsync()
    {
      _logger.LogInformation($"Attempting to connect to {_options.MqttBrokerAddress}:{_options.MqttBrokerPort}...");
      await MqttClient.StartAsync(_mqttClientOptions);
    }

    public async Task Disconnect()
    {
      await MqttClient.StopAsync();
    }

    #region IObservable<MqttClientEvent>
    IDisposable IObservable<MqttClientEvent>.Subscribe(IObserver<MqttClientEvent> observer)
    {
      return _observers.GetOrAdd(observer, new MqttClientEventUnsubscriber(this, observer));
    }
    #endregion

    private async Task OnConnected(MqttClientConnectedEventArgs e)
    {
      _logger.LogInformation($"Connected to {_options.MqttBrokerAddress}:{_options.MqttBrokerPort}.");

      // Produce the connected Event.
      OnMqttClientEvent(new MqttClientConnectedEvent()
      {
      });

      MqttClient.SubscribeControllers(_router);

      // Report that we've connected.
      await MqttClient.PublishAsync(new MqttApplicationMessageBuilder()
              .WithTopic($"beholder/ctas/{_options.HostName}/status")
              .WithPayload("Connected")
              .WithRetainFlag(true)
              .Build()
            );
    }

    private Task OnDisconnected(MqttClientDisconnectedEventArgs e)
    {
      _logger.LogInformation($"Disconnected From {_options.MqttBrokerAddress}:{_options.MqttBrokerPort}.");
      return Task.CompletedTask;
    }

    /// <summary>
    /// Occurs when we recieve a message on a topic that we've subscribed to.
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
    {
      await _router.InterceptApplicationMessageReceivedAsync(e);

      // Produce the Message Received Event.
      OnMqttClientEvent(new MqttClientMessageReceivedEvent()
      {
        Topic = e.ApplicationMessage?.Topic,
      });
    }

    /// <summary>
    /// Produces MqttClient Events
    /// </summary>
    /// <param name="mqttClientEvent"></param>
    private void OnMqttClientEvent(MqttClientEvent mqttClientEvent)
    {
      Parallel.ForEach(_observers.Keys, (observer) =>
      {
        try
        {
          observer.OnNext(mqttClientEvent);
        }
        catch (Exception)
        {
          // Do Nothing.
        }
      });
    }

    #region IDisposable Support
    private bool _isDisposed = false;

    private void Dispose(bool disposing)
    {
      if (!_isDisposed)
      {
        if (disposing)
        {

        }

        _isDisposed = true;
      }
    }

    public void Dispose()
    {
      Dispose(true);
    }
    #endregion

    #region Nested Classes
    private sealed class MqttClientEventUnsubscriber : IDisposable
    {
      private readonly BeholderMqttClient _parent;
      private readonly IObserver<MqttClientEvent> _observer;

      public MqttClientEventUnsubscriber(BeholderMqttClient parent, IObserver<MqttClientEvent> observer)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _observer = observer ?? throw new ArgumentNullException(nameof(observer));
      }

      public void Dispose()
      {
        if (_observer != null && _parent._observers.ContainsKey(_observer))
        {
          _parent._observers.TryRemove(_observer, out _);
        }
      }
    }
    #endregion
  }
}