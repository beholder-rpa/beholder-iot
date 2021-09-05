namespace beholder_stalk_v2.Models
{
  using System.Text.Json.Serialization;

  public record MouseInfo
  {
    [JsonPropertyName("firstThreshold")]
    public int FirstThreshold
    {
      get;
      init;
    }

    [JsonPropertyName("secondThreshold")]
    public int SecondThreshold
    {
      get;
      init;
    }

    [JsonPropertyName("acceleration")]
    public int Acceleration
    {
      get;
      init;
    }
  }
}