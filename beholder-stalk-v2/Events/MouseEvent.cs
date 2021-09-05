namespace beholder_stalk_v2
{
  using System.Text.Json.Serialization;
  using static beholder_stalk_v2.Protos.MoveMouseToRequest.Types;

  /// <summary>
  /// Represents an abstract event that is produced by Mouse HID implementations.
  /// Use Pattern Matching to handle specific instances of this type.
  /// </summary>
  public abstract record MouseEvent
  {
  }

  /// <summary>
  /// Represents an event that is raised with the mouse reports that the mouse resolution has changed
  /// </summary>
  public record MouseResolutionChangedEvent : MouseEvent
  {
    public MouseResolutionChangedEvent(int horizontalResolution, int verticalResolution)
    {
      HorizontalResolution = horizontalResolution;
      VerticalResolution = verticalResolution;
    }

    [JsonPropertyName("horizontalResolution")]
    public int HorizontalResolution { get; init; }

    [JsonPropertyName("nerticalResolution")]
    public int VerticalResolution { get; init; }
  }

  /// <summary>
  /// Represents an event that is raised when a SendMouseMoveTo message indicates a topic to publish the movement mouse points to
  /// </summary>
  public record MouseMoveToPointsEvent : MouseEvent
  {
    [JsonPropertyName("topic")]
    public string Topic { get; init; }

    [JsonPropertyName("points")]
    public Point[] Points { get; init; }
  }

  public record MovedMouseEvent : MouseEvent
  {
    [JsonPropertyName("from")]
    public Point From
    {
      get;
      set;
    }

    [JsonPropertyName("to")]
    public Point To
    {
      get;
      set;
    }
  }
}