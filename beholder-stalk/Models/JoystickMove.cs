namespace beholder_stalk
{
  using Newtonsoft.Json;

  public class JoystickMove : IJoystickAction
  {
    [JsonProperty("x")]
    public sbyte X
    {
      get;
      set;
    }

    [JsonProperty("y")]
    public sbyte Y
    {
      get;
      set;
    }
  }
}