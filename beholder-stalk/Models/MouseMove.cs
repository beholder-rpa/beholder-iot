namespace beholder_stalk
{
  using Newtonsoft.Json;

  public class MouseMove : IMouseAction
  {
    [JsonProperty("x")]
    public short X
    {
      get;
      set;
    }

    [JsonProperty("y")]
    public short Y
    {
      get;
      set;
    }
  }
}