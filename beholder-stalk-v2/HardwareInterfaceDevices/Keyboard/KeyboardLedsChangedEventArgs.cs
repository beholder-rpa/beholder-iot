namespace beholder_stalk_v2.HardwareInterfaceDevices
{
    using System;

    public class KeyboardLedsChangedEventArgs : EventArgs
    {
        public KeyboardLedsChangedEventArgs(KeyboardLeds keyboardLeds)
            : base()
        {
            KeyboardLeds = keyboardLeds;
        }

        public KeyboardLeds KeyboardLeds { get; private set; }
    }
}