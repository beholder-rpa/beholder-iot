namespace beholder_stalk_v2.Protos
{
  using beholder_stalk_v2.Metadata;
  using System.ComponentModel.DataAnnotations;

  [MetadataType(typeof(KeypressMetadata))]
  public partial class Keypress
  {
  }

  partial class KeypressDuration : IDuration
  {
    internal static KeypressDuration Infinite = new KeypressDuration() { Delay = uint.MaxValue, Min = uint.MaxValue, Max = uint.MaxValue };
  }
}