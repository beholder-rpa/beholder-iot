namespace beholder_stalk
{
    using System;

    [Flags]
    public enum JoystickButton : int
    {
        None = 0,
        B1 = 1,
        B2 = 2,
        B3 = 4,
        B4 = 8,
    }

    public enum HatSwitch : int
    {
        Neutral = 0x04,
        Hat1 = 0x00,
        Hat2 = 0x01,
        Hat3 = 0x02,
        Hat4 = 0x03,
    }
}
