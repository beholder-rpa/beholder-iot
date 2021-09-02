namespace beholder_stalk_v2
{
  /// <summary>
  /// Event that is produced when the MqttClient is disconnected
  /// </summary>
  public class MqttClientDisconnectedEvent : MqttClientEvent
  {
    public string Reason
    {
      get;
      set;
    }
  }
}