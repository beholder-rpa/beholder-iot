namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Configuration;
  using System;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using System.Threading.Tasks;
  using static beholder_stalk_v2.Protos.JoystickButtonPress.Types;

  /// <summary>
  /// Represents a HID Joystick
  /// </summary>
  public partial class Joystick : HumanInterfaceDevice
  {
    private static readonly object s_reportLock = new object();
    private static readonly byte[] s_currentReport = new byte[] { 0x00, 0x00, 0x00, 0x04 };

    private static readonly Regex JoystickActionsRegex = new Regex(
        @"{(?:(?<Button>(?<ButtonValue>b1|b2|b3|b4|hat1|hat2|hat3|hat4|hat0)(?:\s(?:(?<Repeats>\d+)|(?<Direction>down|up)))?)|(?<Move>(?<XValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7])),(?<YValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7])))|(?<Throttle>throttle:(?<ThrottleValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7]))))}"
        , RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public Joystick(IConfiguration config)
        : base(config["hid:joystick:devPath"])
    {
      uint pressMin = 40;
      uint pressMax = 141;

      if (uint.TryParse(config["beholder_stalk_pressmin"], out uint configPressMin))
      {
        pressMin = configPressMin;
      }

      if (uint.TryParse(config["beholder_stalk_pressmax"], out uint configPressMax))
      {
        pressMax = configPressMax;
      }

      AveragePressDuration = new JoystickButtonPressDuration() { Min = pressMin, Max = pressMax };
    }

    public JoystickButtonPressDuration AveragePressDuration
    {
      get;
      set;
    }

    private void SendCurrentReport()
    {
      Stream.Write(s_currentReport);
      Stream.Flush();
    }

    public void SendJoystickThrottle(sbyte amount)
    {
      lock (s_reportLock)
      {
        s_currentReport[0] = (byte)amount;
        SendCurrentReport();
      }
    }

    public void SendJoystickMove(sbyte x, sbyte y)
    {
      lock (s_reportLock)
      {
        s_currentReport[1] = (byte)x;
        s_currentReport[2] = (byte)y;
        SendCurrentReport();
      }
    }

    public void SendButtonPress(JoystickButton? button, HatSwitch? hatSwitch)
    {
      lock (s_reportLock)
      {
        var currentButton = s_currentReport[3] >> 4;
        var currentHatSwitch = s_currentReport[3] % 8;
        if (button.HasValue)
        {
          currentButton |= (byte)button;
        }

        if (hatSwitch.HasValue)
        {
          currentHatSwitch = (byte)hatSwitch;
        }

        s_currentReport[3] = (byte)((currentButton << 4) + currentHatSwitch);
        SendCurrentReport();
      }
    }

    public void SendButtonRelease(JoystickButton? button, HatSwitch? hatSwitch)
    {
      lock (s_reportLock)
      {
        var currentButton = s_currentReport[3] >> 4;
        var currentHatSwitch = s_currentReport[3] % 8;

        if (button.HasValue)
        {
          currentButton ^= (byte)button;
        }

        if (hatSwitch.HasValue)
        {
          currentHatSwitch = 0x04;
        }

        s_currentReport[3] = (byte)((currentButton << 4) + currentHatSwitch);
        SendCurrentReport();
      }
    }

    public async Task SendButton(string button, ButtonPressDirection direction = ButtonPressDirection.PressAndRelease, JoystickButtonPressDuration duration = null)
    {
      if (string.IsNullOrEmpty(button))
      {
        return;
      }

      if (duration == null)
      {
        duration = AveragePressDuration;
      }

      button = button.ToLowerInvariant();

      if (!Buttons.ContainsKey(button))
      {
        return;
      }

      var buttonValue = Buttons[button];

      switch (direction)
      {
        case ButtonPressDirection.Press:
          SendButtonPress(buttonValue.Item1, buttonValue.Item2);
          break;
        case ButtonPressDirection.Release:
          SendButtonRelease(buttonValue.Item1, buttonValue.Item2);
          break;
        case ButtonPressDirection.PressAndRelease:
        default:
          SendButtonPress(buttonValue.Item1, buttonValue.Item2);
          await DelayUtil.Think(duration);
          SendButtonRelease(buttonValue.Item1, buttonValue.Item2);
          break;
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

    public void SendJoystickReset()
    {
      lock (s_reportLock)
      {
        // Reset the current report to all blanks.
        for (int i = 0; i < s_currentReport.Length; i++)
        {
          s_currentReport[i] = ClearReport[i];
        }

        SendCurrentReport();
      }
    }

    public async Task SendJoystickActions(string actions)
    {
      var joystickActions = new List<IJoystickAction>();
      foreach (Match m in JoystickActionsRegex.Matches(actions))
      {
        if (m.Groups.ContainsKey("Button") && !string.IsNullOrWhiteSpace(m.Groups["ButtonValue"].Value))
        {
          var buttonPress = new JoystickButtonPress()
          {
            Button = m.Groups["ButtonValue"].Value,
            PressDirection = ButtonPressDirection.PressAndRelease,
            Duration = AveragePressDuration,
          };

          if (m.Groups.ContainsKey("Direction"))
          {
            switch (m.Groups["Direction"].Value.ToLowerInvariant())
            {
              case "press":
              case "down":
                buttonPress.PressDirection = ButtonPressDirection.Press;
                buttonPress.Duration = JoystickButtonPressDuration.Infinite;
                break;
              case "release":
              case "up":
                buttonPress.PressDirection = ButtonPressDirection.Release;
                buttonPress.Duration = JoystickButtonPressDuration.Infinite;
                break;
              default:
                buttonPress.PressDirection = ButtonPressDirection.PressAndRelease;
                buttonPress.Duration = AveragePressDuration;
                break;
            }
          }

          if (m.Groups.ContainsKey("Repeats") && int.TryParse(m.Groups["Repeats"].Value, out int repeats))
          {
            for (int i = 0; i < Math.Abs(repeats); i++)
            {
              joystickActions.Add(buttonPress);
            }
          }
          else
          {
            joystickActions.Add(buttonPress);
          }
        }
        else if (m.Groups.ContainsKey("Move") && !string.IsNullOrWhiteSpace(m.Groups["XValue"].Value) && !string.IsNullOrWhiteSpace(m.Groups["YValue"].Value))
        {
          sbyte xVal = 0;
          sbyte yVal = 0;
          if (sbyte.TryParse(m.Groups["XValue"].Value, out sbyte x))
          {
            xVal = x;
          }
          if (sbyte.TryParse(m.Groups["YValue"].Value, out sbyte y))
          {
            yVal = y;
          }
          var joystickMove = new JoystickMove()
          {
            X = xVal,
            Y = yVal
          };
          joystickActions.Add(joystickMove);
        }
        else if (m.Groups.ContainsKey("Throttle") && !string.IsNullOrWhiteSpace(m.Groups["ThrottleValue"].Value))
        {
          var joystickThrottle = new JoystickThrottle()
          {
            Amount = sbyte.Parse(m.Groups["ThrottleValue"].Value),
          };
          joystickActions.Add(joystickThrottle);
        }
      }

      // Now that we have the joystick actions, press them.
      foreach (var joystickAction in joystickActions)
      {
        switch (joystickAction)
        {
          case JoystickButtonPress buttonPress:
            await SendButton(buttonPress.Button, buttonPress.PressDirection, buttonPress.Duration);
            break;
          case JoystickMove joystickMove:
            SendJoystickMove((sbyte)joystickMove.X, (sbyte)joystickMove.Y);
            break;
          case JoystickThrottle joystickThrottle:
            SendJoystickThrottle((sbyte)joystickThrottle.Amount);
            break;
        }
      }
    }
  }
}