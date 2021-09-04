namespace beholder_stalk_v2.Models
{
  using System.Text.Json.Serialization;

  public record Size
  {
    [JsonPropertyName("width")]
    public int Width
    {
      get;
      init;
    }

    [JsonPropertyName("height")]
    public int Height
    {
      get;
      init;
    }
  }
}
