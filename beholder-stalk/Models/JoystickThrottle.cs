namespace beholder_stalk
{
  using Newtonsoft.Json;

  public class JoystickThrottle : IJoystickAction
  {
    [JsonProperty("amount")]
    public sbyte Amount
    {
      get;
      set;
    }
  }
}