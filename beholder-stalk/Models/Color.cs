namespace beholder_stalk
{
  using Newtonsoft.Json;

  public class Color
  {
    public Color(byte red = 0, byte green = 0, byte blue = 0)
    {
      Red = red;
      Green = green;
      Blue = blue;
    }

    [JsonProperty("r")]
    public byte Red { get; }

    [JsonProperty("g")]
    public byte Green { get; }

    [JsonProperty("b")]
    public byte Blue { get; }
  }
}