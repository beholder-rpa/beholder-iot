namespace beholder_stalk
{
  /// <summary>
  /// Defines the varioustopics in use by the Beholder Stalk
  /// </summary>
  public static class StalkTopic
  {
    // Status Messages (Egress)
    public static string Status_BeholderStalk = "beholder/stalk/{0}/status";

    public static string Status_KeypressDuration = "beholder/stalk/{0}/status/keyboard/keypress_duration";

    public static string Status_KeyboardLeds = "beholder/stalk/{0}/status/keyboard/leds";

    public static string Status_ClickDuration = "beholder/stalk/{0}/mouse/status/mouse/click_duration";

    public static string Status_MouseResolution = "beholder/stalk/{0}/status/mouse/resolution";

    // Keyboard
    public static string SendKey = "beholder/stalk/{0}/keyboard/key";

    public static string SendKeys = "beholder/stalk/{0}/keyboard/keys";

    public static string SendKeysRaw = "beholder/stalk/{0}/keyboard/raw";

    public static string SendKeysReset = "beholder/stalk/{0}/keyboard/reset";

    public static string KeypressDuration = "beholder/stalk/{0}/keyboard/keypress_duration";

    // Mouse
    public static string SendMouseClick = "beholder/stalk/{0}/mouse/click";

    public static string SendMouseActions = "beholder/stalk/{0}/mouse/actions";

    public static string SendMouseRaw = "beholder/stalk/{0}/mouse/raw";

    public static string SendMouseReset = "beholder/stalk/{0}/mouse/reset";

    public static string ClickDuration = "beholder/stalk/{0}/mouse/click_duration";

    // Joystick
    public static string SendJoystickActions = "beholder/stalk/{0}/joystick/actions";

    public static string SendJoystickRaw = "beholder/stalk/{0}/joystick/raw";

    public static string SendJoystickReset = "beholder/stalk/{0}/joystick/reset";

    // Led Matrix
    public static string SendLedMatrix = "beholder/stalk/{0}/ledmatrix/colors";

  }
}