namespace beholder_stalk
{
    using Microsoft.Extensions.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a HID Mouse
    /// </summary>
    /// <remarks>
    /// See: https://www.microchip.com/forums/m391435.aspx
    /// </remarks>
    public partial class Mouse : HID
    {
        private static readonly object s_reportLock = new object();
        private static readonly byte[] s_currentReport = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        private static readonly Regex MouseActionsRegex = new Regex(
            @"{(?:(?<Button>(?<ButtonValue>lbutton|lclick|rbutton|rclick|mbutton|mclick|back|xbutton1|forward|xbutton2)(?:\s(?:(?<Repeats>\d+)|(?<Direction>down|up)))?)|(?<Move>(?<XValue>-?\d+),(?<YValue>-?\d+))|(?<Scroll>scroll:(?<ScrollValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7])))|(?<Tilt>tilt:(?<TiltValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7]))))}"
            , RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

        public event EventHandler<MouseResolutionChangedEventArgs> MouseResolutionChanged;

        private readonly byte[] sizeBuffer = new byte[1];

        public Mouse(IConfigurationRoot config)
            : base(config["hid:mouse:devPath"])
        {
            int mouseMin = 40;
            int mouseMax = 141;

            if (int.TryParse(config["beholder_stalk_clickmin"], out int configMouseMin))
            {
                mouseMin = configMouseMin;
            }

            if (int.TryParse(config["beholder_stalk_clickmax"], out int configMouseMax))
            {
                mouseMax = configMouseMax;
            }

            AverageClickDuration = new Duration() { Min = mouseMin, Max = mouseMax };
            WatchNext();
        }

        public Duration AverageClickDuration
        {
            get;
            set;
        }

        private void SendCurrentReport()
        {
            Stream.Write(s_currentReport);
            Stream.Flush();
        }

        protected void OnMouseResolutionChanged(MouseResolutionChangedEventArgs e)
        {
            MouseResolutionChanged?.Invoke(this, e);
        }

        protected void WatchNext()
        {
            Stream.BeginRead(sizeBuffer, 0, 1, new AsyncCallback(ReadCallback), null);
        }

        private void ReadCallback(IAsyncResult ar)
        {
            int bytesRead = Stream.EndRead(ar);
            if (bytesRead == 0)
            {
                return;
            }

            var hres = sizeBuffer[0] << 2;
            var vres = sizeBuffer[0] << 4;
            OnMouseResolutionChanged(new MouseResolutionChangedEventArgs(hres, vres));
            WatchNext();
        }

        public void SendButtonPress(MouseButton button)
        {
            lock (s_reportLock)
            {
                s_currentReport[0] = (byte)(s_currentReport[0] | (byte)button);

                // Reset the reset of the report to 0's.
                for (int i = 1; i < s_currentReport.Length; i++)
                {
                    s_currentReport[i] = 0x00;
                }
                SendCurrentReport();
            }
        }

        public void SendButtonRelease(MouseButton button)
        {
            lock (s_reportLock)
            {
                s_currentReport[0] = (byte)(s_currentReport[0] ^ (byte)button);

                // Reset the reset of the report to 0's.
                for (int i = 1; i < s_currentReport.Length; i++)
                {
                    s_currentReport[i] = 0x00;
                }
                SendCurrentReport();
            }
        }

        public async Task SendMouseClick(string button, ClickDirection direction = ClickDirection.PressAndRelease, Duration duration = null)
        {
            if (string.IsNullOrEmpty(button))
            {
                return;
            }

            if (duration == null)
            {
                duration = AverageClickDuration;
            }

            button = button.ToLowerInvariant();

            if (!Buttons.ContainsKey(button))
            {
                return;
            }

            var buttonValue = Buttons[button];

            switch (direction)
            {
                case ClickDirection.Press:
                    SendButtonPress(buttonValue);
                    break;
                case ClickDirection.Release:
                    SendButtonRelease(buttonValue);
                    break;
                case ClickDirection.PressAndRelease:
                default:
                    SendButtonPress(buttonValue);
                    await Util.Hesitate(duration);
                    SendButtonRelease(buttonValue);
                    break;
            }
        }

        public void SendMouseMove(short x, short y)
        {
            var xBytes = BitConverter.GetBytes(x);
            var yBytes = BitConverter.GetBytes(y);
            lock (s_reportLock)
            {

                s_currentReport[1] = xBytes[0];
                s_currentReport[2] = xBytes[1];
                s_currentReport[3] = yBytes[0];
                s_currentReport[4] = yBytes[1];
                SendCurrentReport();
            }
        }

        public void SendMouseScroll(sbyte scroll)
        {
            lock (s_reportLock)
            {
                s_currentReport[5] = (byte)scroll;
                SendCurrentReport();
            }
        }

        public void SendMouseTilt(sbyte tilt)
        {
            lock (s_reportLock)
            {
                s_currentReport[6] = (byte)tilt;
                SendCurrentReport();
            }
        }

        public override void SendRaw(byte[] report)
        {
            if (report.Length != s_currentReport.Length)
            {
                return;
            }

            lock (s_currentReport)
            {
                for (int i = 0; i < s_currentReport.Length; i++)
                {
                    s_currentReport[i] = report[i];
                }

                SendCurrentReport();
            }
        }

        public void SendMouseReset()
        {
            lock (s_reportLock)
            {
                // Reset the current report to all blanks.
                for (int i = 0; i < s_currentReport.Length; i++)
                {
                    s_currentReport[i] = 0x00;
                }

                SendCurrentReport();
            }
        }

        public async Task SendMouseActions(string actions)
        {
            var mouseActions = new List<IMouseAction>();
            foreach (Match m in MouseActionsRegex.Matches(actions))
            {
                if (m.Groups.ContainsKey("Button") && !string.IsNullOrWhiteSpace(m.Groups["ButtonValue"].Value))
                {
                    var mouseclick = new MouseClick()
                    {
                        Button = m.Groups["ButtonValue"].Value,
                        Direction = ClickDirection.PressAndRelease,
                        Duration = AverageClickDuration,
                    };

                    if (m.Groups.ContainsKey("Direction"))
                    {
                        switch (m.Groups["Direction"].Value.ToLowerInvariant())
                        {
                            case "press":
                            case "down":
                                mouseclick.Direction = ClickDirection.Press;
                                mouseclick.Duration = Duration.Infinite;
                                break;
                            case "release":
                            case "up":
                                mouseclick.Direction = ClickDirection.Release;
                                mouseclick.Duration = Duration.Infinite;
                                break;
                            default:
                                mouseclick.Direction = ClickDirection.PressAndRelease;
                                mouseclick.Duration = AverageClickDuration;
                                break;
                        }
                    }

                    if (m.Groups.ContainsKey("Repeats") && int.TryParse(m.Groups["Repeats"].Value, out int repeats))
                    {
                        for (int i = 0; i < Math.Abs(repeats); i++)
                        {
                            mouseActions.Add(mouseclick);
                        }
                    }
                    else
                    {
                        mouseActions.Add(mouseclick);
                    }
                }
                else if (m.Groups.ContainsKey("Move") && !string.IsNullOrWhiteSpace(m.Groups["XValue"].Value) && !string.IsNullOrWhiteSpace(m.Groups["YValue"].Value))
                {
                    short xVal = 0;
                    short yVal = 0;
                    if (short.TryParse(m.Groups["XValue"].Value, out short x))
                    {
                        xVal = x;
                    }
                    if (short.TryParse(m.Groups["YValue"].Value, out short y))
                    {
                        yVal = y;
                    }
                    var mouseMove = new MouseMove()
                    {
                        X = xVal,
                        Y = yVal
                    };
                    mouseActions.Add(mouseMove);
                }
                else if (m.Groups.ContainsKey("Scroll") && !string.IsNullOrWhiteSpace(m.Groups["ScrollValue"].Value))
                {
                    var mouseScroll = new MouseScroll()
                    {
                        Amount = sbyte.Parse(m.Groups["ScrollValue"].Value),
                    };
                    mouseActions.Add(mouseScroll);
                }
                else if (m.Groups.ContainsKey("Tilt") && !string.IsNullOrWhiteSpace(m.Groups["TiltValue"].Value))
                {
                    var mouseTilt = new MouseTilt()
                    {
                        Amount = sbyte.Parse(m.Groups["TiltValue"].Value),
                    };
                    mouseActions.Add(mouseTilt);
                }
            }

            // Now that we have the mouse actions, press them.
            foreach (var mouseAction in mouseActions)
            {
                switch(mouseAction)
                {
                    case MouseClick mouseClick:
                        await SendMouseClick(mouseClick.Button, mouseClick.Direction, mouseClick.Duration);
                        break;
                    case MouseMove mouseMove:
                        SendMouseMove(mouseMove.X, mouseMove.Y);
                        break;
                    case MouseScroll mouseScroll:
                        SendMouseScroll(mouseScroll.Amount);
                        break;
                    case MouseTilt mouseTilt:
                        SendMouseTilt(mouseTilt.Amount);
                        break;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                SendMouseReset();
            }

            base.Dispose(disposing);
        }
    }
}