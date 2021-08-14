namespace beholder_stalk_v2.Protos
{
  public partial class MouseClick : IMouseAction
  {
  }

  public partial class MouseMove : IMouseAction
  {
  }

  public partial class MouseScroll : IMouseAction
  {
  }

  public partial class MouseTilt : IMouseAction
  {
  }

  partial class MouseClickDuration : IDuration
  {
    internal static MouseClickDuration Infinite = new MouseClickDuration() { Delay = uint.MaxValue, Min = uint.MaxValue, Max = uint.MaxValue };
  }
}