namespace beholder_stalk_v2.Models
{
  using System.Text.Json.Serialization;

  public record SysInfo
  {
    [JsonPropertyName("cursorSize")]
    public Size CursorSize
    {
      get;
      init;
    }

    [JsonPropertyName("dpiX")]
    public float DpiX
    {
      get;
      set;
    }

    [JsonPropertyName("dpiY")]
    public float DpiY
    {
      get;
      set;
    }

    [JsonPropertyName("doubleClickSize")]
    public Size DoubleClickSize
    {
      get;
      init;
    }

    [JsonPropertyName("doubleClickTime")]
    public int DoubleClickTime
    {
      get;
      init;
    }

    [JsonPropertyName("keyboardDelay")]
    public int KeyboardDelay
    {
      get;
      init;
    }

    [JsonPropertyName("keyboardSpeed")]
    public int KeyboardSpeed
    {
      get;
      init;
    }

    [JsonPropertyName("monitorCount")]
    public int MonitorCount
    {
      get;
      init;
    }

    [JsonPropertyName("mouseSpeed")]
    public int MouseSpeed
    {
      get;
      init;
    }

    [JsonPropertyName("mouseInfo")]
    public MouseInfo MouseInfo
    {
      get;
      init;
    }

    [JsonPropertyName("mouseUpdateRate")]
    public int MouseUpdateRate
    {
      get;
      init;
    }

    [JsonPropertyName("workingArea")]
    public Rectangle WorkingArea
    {
      get;
      init;
    }
  }
}
