namespace beholder_stalk_v2.HardwareInterfaceDevices
{
    using System;

    [Flags]
    public enum KeyboardLeds
    {
        None = 0,
        NumLock = 1,
        CapsLock = 2,
        ScrollLock = 4,
        Compose = 8,
        Kata = 16,
    }
}