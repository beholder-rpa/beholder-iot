namespace beholder_stalk_v2.Protos
{
  public partial class Keypress
  {
    partial class Types
    {
      partial class KeypressDuration : IDuration
      {
        internal static KeypressDuration Infinite = new KeypressDuration() { Delay = uint.MaxValue, Min = uint.MaxValue, Max = uint.MaxValue };
      }
    }
  }
}
