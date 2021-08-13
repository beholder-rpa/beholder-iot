namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using System.Collections.Generic;

  public partial class Mouse
  {
    public static byte[] ClearReport = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    public static IDictionary<string, MouseButton> Buttons = new Dictionary<string, MouseButton>() {
            { "lbutton", MouseButton.Left },
            { "lclick", MouseButton.Left },
            { "rbutton", MouseButton.Right },
            { "rclick", MouseButton.Right },
            { "mbutton", MouseButton.Middle },
            { "mclick", MouseButton.Middle },
            { "back", MouseButton.Back },
            { "xbutton1", MouseButton.Back },
            { "forward", MouseButton.Forward },
            { "xbutton2", MouseButton.Forward },
        };
  }
}