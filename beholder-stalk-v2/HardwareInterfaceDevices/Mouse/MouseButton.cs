﻿namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using System;

  [Flags]
  public enum MouseButton : int
  {
    None = 0,
    Left = 1,
    Right = 2,
    Middle = 4,
    Back = 8,
    Forward = 16,
  }
}