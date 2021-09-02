namespace beholder_stalk_v2
{
  /// <summary>
  /// Event that is produced when the MqttClient recieves a message.
  /// </summary>
  public class MqttClientMessageReceivedEvent : MqttClientEvent
  {
    public string Topic
    {
      get;
      set;
    }
  }
}