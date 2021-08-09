namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using System;

  [Flags]
  public enum KeyModifier : byte
  {
    None = 0x00,

    LeftCtrl = 0x01,
    RightCtrl = 0x10,

    LeftShift = 0x02,
    RightShift = 0x20,

    LeftAlt = 0x04,
    RightAlt = 0x40,

    LeftMeta = 0x08,
    RightMeta = 0x80,
  }
}