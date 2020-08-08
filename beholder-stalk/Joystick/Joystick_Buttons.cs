namespace beholder_stalk
{
    using System.Collections.Generic;

    public partial class Joystick
    {
        public static byte[] ClearReport = new byte[] { 0x00, 0x00, 0x00, 0x04 };

        public static IDictionary<string, (JoystickButton?, HatSwitch?)> Buttons = new Dictionary<string, (JoystickButton?, HatSwitch?)>() {
            { "b1", (JoystickButton.B1, null) },
            { "b2", (JoystickButton.B2, null) },
            { "b3", (JoystickButton.B3, null) },
            { "b4", (JoystickButton.B4, null) },
            { "hat1", (null, HatSwitch.Hat1) },
            { "hat-up", (null, HatSwitch.Hat1) },
            { "hat2", (null, HatSwitch.Hat2) },
            { "hat-right", (null, HatSwitch.Hat2) },
            { "hat3", (null, HatSwitch.Hat3) },
            { "hat-down", (null, HatSwitch.Hat3) },
            { "hat4", (null, HatSwitch.Hat4) },
            { "hat-left", (null, HatSwitch.Hat4) },
        };
    }
}
