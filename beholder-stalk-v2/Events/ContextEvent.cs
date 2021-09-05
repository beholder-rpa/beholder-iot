using beholder_stalk_v2.Models;

namespace beholder_stalk_v2
{
  /// <summary>
  /// Represents an abstract event that is produced by Context Controller.
  /// Use Pattern Matching to handle specific instances of this type.
  /// </summary>
  public abstract record ContextEvent
  {
  }

  /// <summary>
  /// Represents an event that is raised when the pointer position has changed
  /// </summary>
  public record PointerPositionChangedEvent : ContextEvent
  {
    public string Source { get; init;  }

    public Point OldPointerPosition { get; init; }

    public Point NewPointerPosition { get; init; }
  }
}