namespace beholder_stalk_v2.Models
{
  using System.Text.Json.Serialization;

  public record Point
  {
    [JsonPropertyName("x")]
    public int X
    {
      get;
      set;
    }

    [JsonPropertyName("y")]
    public int Y
    {
      get;
      set;
    }
  }
}