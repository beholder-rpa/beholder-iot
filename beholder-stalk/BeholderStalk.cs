namespace beholder_stalk
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using MQTTnet;
    using MQTTnet.Client;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Disconnecting;
    using MQTTnet.Client.Options;
    using Newtonsoft.Json;
    using System;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an implementation of a Beholder Stalk that uses Mqtt as the messaging mechanism.
    /// </summary>
    public sealed class BeholderStalk : IBeholderStalk, IDisposable
    {
        private static readonly Random s_random = new Random();

        private readonly ILogger _logger;
        private readonly string _nexusUrl;
        private readonly string _clientId;
        private readonly IMqttClientOptions _mqttClientOptions;

        public BeholderStalk(IConfigurationRoot configuration, ILogger<BeholderStalk> logger, Keyboard keyboard, Mouse mouse, Joystick joystick, LedMatrix ledMatrix = null)
        {
            _logger = logger;
            Keyboard = keyboard;
            Mouse = mouse;
            Joystick = joystick;
            LedMatrix = ledMatrix;

            Keyboard.KeyboardLedsChanged += Keyboard_KeyboardLedsChanged;
            Mouse.MouseResolutionChanged += Mouse_MouseResolutionChanged;

            // Connect to the Nexus
            _clientId = configuration["beholder_stalk_clientid"];
            _nexusUrl = configuration["beholder_nexus_url"];
            var nexusUsername = configuration["beholder_nexus_username"];
            var nexusPassword = configuration["beholder_nexus_password"];

            // Create a new MQTT client.
            var factory = new MqttFactory();
            var mqttClient = factory.CreateMqttClient();

            _mqttClientOptions = new MqttClientOptionsBuilder()
               .WithClientId(_clientId)
               .WithWebSocketServer(_nexusUrl)
               .WithCredentials(nexusUsername, nexusPassword)
               .WithKeepAlivePeriod(TimeSpan.FromSeconds(10))
               .WithCommunicationTimeout(TimeSpan.FromSeconds(30))
               .WithWillDelayInterval(60 * 1000)
               .WithWillMessage(new MqttApplicationMessage()
               {
                   PayloadFormatIndicator = MQTTnet.Protocol.MqttPayloadFormatIndicator.CharacterData,
                   ContentType = "text/plain",
                   Topic = StalkTopic.Status_BeholderStalk,
                   Payload = Encoding.UTF8.GetBytes("Disconnected"),
                   Retain = true
               })
               .WithCleanSession()
               .Build();

            mqttClient.UseConnectedHandler(OnConnected);
            mqttClient.UseDisconnectedHandler(OnDisconnected);
            mqttClient.UseApplicationMessageReceivedHandler(OnApplicationMessageReceived);
        }

        public IMqttClient MqttClient
        {
            get;
        }

        public Keyboard Keyboard
        {
            get;
            private set;
        }

        public LedMatrix LedMatrix
        {
            get;
        }

        public Mouse Mouse
        {
            get;
            private set;
        }

        public Joystick Joystick
        {
            get;
            private set;
        }

        public void SetAverageKeypressDuration(int min, int max)
        {
            Keyboard.AverageKeypressDuration = new Duration() { Min = min, Max = max };
            _logger.LogInformation($"Set Average Keypress Duation to {min}/{max}...");
        }

        public async Task SendKey(Keypress keypress)
        {
            await Keyboard.SendKey(keypress);
        }

        public async Task SendKeys(string keys)
        {
            _logger.LogInformation($"Sending Keys {keys} to {Keyboard.DevPath}...");
            await Keyboard.SendKeys(keys);
        }

        public void SendKeysRaw(byte[] report)
        {
            _logger.LogInformation($"Sending raw key report [{BitConverter.ToString(report).Replace("-", "")}] to {Keyboard.DevPath}...");
            Keyboard.SendRaw(report);
        }

        public void SendKeysReset()
        {
            Keyboard.SendKeysReset();
        }

        public void SetAverageClickDuration(int min, int max)
        {
            Mouse.AverageClickDuration = new Duration() { Min = min, Max = max };
            _logger.LogInformation($"Set Average Click Duation to {min}/{max}...");
        }

        public async Task SendMouseClick(MouseClick mouseClick)
        {
            await Mouse.SendMouseClick(mouseClick.Button, mouseClick.Direction, mouseClick.Duration);
        }

        public async Task SendMouseActions(string actions)
        {
            _logger.LogInformation($"Sending Mouse Actions {actions} to {Mouse.DevPath}...");
            await Mouse.SendMouseActions(actions);
        }

        public void SendMouseRaw(byte[] report)
        {
            _logger.LogInformation($"Sending raw mouse report [{BitConverter.ToString(report).Replace("-", "")}] to {Mouse.DevPath}...");
            Mouse.SendRaw(report);
        }

        public void SendMouseReset()
        {
            Mouse.SendMouseReset();
        }

        public void SetAveragePressDuration(int min, int max)
        {
            Joystick.AveragePressDuration = new Duration() { Min = min, Max = max };
            _logger.LogInformation($"Set Average Press Duation to {min}/{max}...");
        }

        public async Task SendJoystickActions(string actions)
        {
            _logger.LogInformation($"Sending Joystick Actions {actions} to {Mouse.DevPath}...");
            await Joystick.SendJoystickActions(actions);
        }

        public void SendJoystickRaw(byte[] report)
        {
            _logger.LogInformation($"Sending raw joystick report [{BitConverter.ToString(report).Replace("-", "")}] to {Joystick.DevPath}...");
            Joystick.SendRaw(report);
        }

        public void SendJoystickReset()
        {
            Joystick.SendJoystickReset();
        }

        public void SendLedMatrix(Color[] pixels)
        {
            if (LedMatrix != null)
            {
                _logger.LogInformation($"Sending Led Matrix {pixels.Length} to {LedMatrix.DevPath}...");
                LedMatrix.SetPixels(pixels);
            }
            else
            {
                _logger.LogInformation($"Led Matrix recieved a message, but was not present...");
            }
        }

        private async Task OnConnected(MqttClientConnectedEventArgs e)
        {
            _logger.LogInformation($"Connected to {_nexusUrl}.");

            // Subscribe to all stalk related event topics.
            await MqttClient.SubscribeAsync(
                // Keyboard
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendKey, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendKeys, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendKeysRaw, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendKeysReset, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.KeypressDuration, _clientId)).Build(),

                // Mouse
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendMouseClick, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendMouseActions, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendMouseRaw, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendMouseReset, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.ClickDuration, _clientId)).Build(),

                // Joystick
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendJoystickActions, _clientId)).Build(),
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendJoystickRaw, _clientId)).Build(),

                // Led Matrix
                new MqttTopicFilterBuilder().WithTopic(string.Format(StalkTopic.SendLedMatrix, _clientId)).Build()
                );

            // Report that we've connected.
            await Publish(StalkTopic.Status_BeholderStalk, "Connected", true);
        }

        private void Keyboard_KeyboardLedsChanged(object sender, KeyboardLedsChangedEventArgs e)
        {
            _logger.LogInformation($"Keyboard Leds Changed: {e.KeyboardLeds}");
            _ = Publish(StalkTopic.Status_KeyboardLeds, e.KeyboardLeds);
        }

        private void Mouse_MouseResolutionChanged(object sender, MouseResolutionChangedEventArgs e)
        {
            _logger.LogInformation($"Mouse Resolution Changed: {e.HorizontalResolution} {e.VerticalResolution}");
            _ = Publish(StalkTopic.Status_MouseResolution, new { e.HorizontalResolution, e.VerticalResolution });
        }

        private async Task OnDisconnected(MqttClientDisconnectedEventArgs e)
        {
            _logger.LogInformation($"Reconnecting to {_nexusUrl}.");
            await Task.Delay(TimeSpan.FromSeconds(s_random.Next(2, 12) * 5));

            try
            {
                await MqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
            }
            catch
            {
                _logger.LogInformation($"Reconnection failed.");
            }
        }

        private async Task OnApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            _logger.LogInformation($"Received Application Message for topic {e.ApplicationMessage.Topic}");

            switch (e.ApplicationMessage.Topic)
            {
                // Keyboard 
                case var sendKey when (sendKey == string.Format(StalkTopic.SendKey, _clientId)):
                    var keypress = JsonConvert.DeserializeObject<Keypress>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    await SendKey(keypress);
                    break;
                case var sendKeys when (sendKeys == string.Format(StalkTopic.SendKeys, _clientId)):
                    var keys = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    await SendKeys(keys);
                    break;
                case var sendKeysRaw when (sendKeysRaw == string.Format(StalkTopic.SendKeysRaw, _clientId)):
                    SendKeysRaw(e.ApplicationMessage.Payload);
                    break;
                case var sendKeysReset when (sendKeysReset == string.Format(StalkTopic.SendKeysReset, _clientId)):
                    SendKeysReset();
                    break;
                case var keypressDuration when (keypressDuration == string.Format(StalkTopic.KeypressDuration, _clientId)):
                    if (e.ApplicationMessage.Payload.Length > 0)
                    {
                        var (min, max) = JsonConvert.DeserializeObject<(int min, int max)>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                        SetAverageKeypressDuration(min, max);
                    }

                    await Publish(StalkTopic.Status_ClickDuration, new { Keyboard.AverageKeypressDuration.Min, Keyboard.AverageKeypressDuration.Max });
                    break;

                // Mouse
                case var sendMouseClick when (sendMouseClick == string.Format(StalkTopic.SendMouseClick, _clientId)):
                    var mouseClick = JsonConvert.DeserializeObject<MouseClick>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    await SendMouseClick(mouseClick);
                    break;
                case var sendMouseActions when (sendMouseActions == string.Format(StalkTopic.SendMouseActions, _clientId)):
                    var mouseActions = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    await SendMouseActions(mouseActions);
                    break;
                case var sendMouseRaw when (sendMouseRaw == string.Format(StalkTopic.SendMouseRaw, _clientId)):
                    SendMouseRaw(e.ApplicationMessage.Payload);
                    break;
                case var sendMouseReset when (sendMouseReset == string.Format(StalkTopic.SendMouseReset, _clientId)):
                    SendMouseReset();
                    break;
                case var clickDuration when (clickDuration == string.Format(StalkTopic.ClickDuration, _clientId)):
                    if (e.ApplicationMessage.Payload.Length > 0)
                    {
                        var (min, max) = JsonConvert.DeserializeObject<(int min, int max)>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                        SetAveragePressDuration(min, max);
                    }

                    await Publish(StalkTopic.Status_ClickDuration, new { Mouse.AverageClickDuration.Min, Mouse.AverageClickDuration.Max });
                    break;

                // Joystick
                case var sendJoystickActions when (sendJoystickActions == string.Format(StalkTopic.SendJoystickActions, _clientId)):
                    var joystickActions = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                    await SendJoystickActions(joystickActions);
                    break;
                case var sendJoystickRaw when (sendJoystickRaw == string.Format(StalkTopic.SendJoystickRaw, _clientId)):
                    SendJoystickRaw(e.ApplicationMessage.Payload);
                    break;
                case var sendJoystickReset when (sendJoystickReset == string.Format(StalkTopic.SendJoystickReset, _clientId)):
                    SendJoystickReset();
                    break;

                // Led Matrix
                case var sendLedMatrix when (sendLedMatrix == string.Format(StalkTopic.SendLedMatrix, _clientId)):
                    var pixels = JsonConvert.DeserializeObject<Color[]>(Encoding.UTF8.GetString(e.ApplicationMessage.Payload));
                    SendLedMatrix(pixels);
                    break;
            }
        }

        public async Task Connect()
        {
            _logger.LogInformation($"Connecting to {_nexusUrl}...");
            await MqttClient.ConnectAsync(_mqttClientOptions, CancellationToken.None);
        }

        public async Task Publish<T>(string topic, T payload, bool retain = false)
        {
            var contentType = "application/octet-stream";
            byte[] payloadBytes;
            if (payload is byte[])
            {
                payloadBytes = payload as byte[];
            }
            else if (payload is string)
            {
                contentType = "text/plain";
                payloadBytes = Encoding.UTF8.GetBytes(payload as string);
            }
            else
            {
                contentType = "application/json";
                var payloadString = JsonConvert.SerializeObject(payload);
                payloadBytes = Encoding.UTF8.GetBytes(payloadString);
            }

            await MqttClient.PublishAsync(new MqttApplicationMessage()
            {
                PayloadFormatIndicator = contentType == "application/octet-stream" ? MQTTnet.Protocol.MqttPayloadFormatIndicator.Unspecified : MQTTnet.Protocol.MqttPayloadFormatIndicator.CharacterData,
                ContentType = contentType,
                Topic = string.Format(topic, _clientId),
                Payload = payloadBytes,
                Retain = retain
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
                    if (Keyboard != null)
                    {
                        Keyboard.KeyboardLedsChanged -= Keyboard_KeyboardLedsChanged;
                        Keyboard.Dispose();
                        Keyboard = null;
                    }

                    if (Mouse != null)
                    {
                        Mouse.MouseResolutionChanged -= Mouse_MouseResolutionChanged;
                        Mouse.Dispose();
                        Mouse = null;
                    }

                    if (Joystick != null)
                    {
                        Joystick.Dispose();
                        Joystick = null;
                    }
                }

                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
