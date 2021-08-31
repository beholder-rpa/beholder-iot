namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Configuration;
  using System;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using System.Threading;
  using static beholder_stalk_v2.Protos.MouseClick.Types;

  /// <summary>
  /// Represents a HID Mouse
  /// </summary>
  /// <remarks>
  /// See: https://www.microchip.com/forums/m391435.aspx
  /// </remarks>
  public partial class Mouse : HumanInterfaceDevice
  {
    private static readonly object s_reportLock = new object();
    private static readonly byte[] s_currentReport = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    private static readonly Regex MouseActionsRegex = new Regex(
        @"{(?:(?<Button>(?<ButtonValue>lbutton|lclick|rbutton|rclick|mbutton|mclick|back|xbutton1|forward|xbutton2)(?:\s(?:(?<Repeats>\d+)|(?<Direction>down|up)))?)|(?<Move>(?<XValue>-?\d+),(?<YValue>-?\d+))|(?<Scroll>scroll:(?<ScrollValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7])))|(?<Tilt>tilt:(?<TiltValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7]))))}"
        , RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public event EventHandler<MouseResolutionChangedEventArgs> MouseResolutionChanged;

    private readonly byte[] sizeBuffer = new byte[1];
    private volatile bool _isMoving = false;
    private readonly object _isMovingLock = new object();

    public Mouse(IConfiguration config)
        : base(config["hid:mouse:devPath"])
    {
      uint mouseMin = 40;
      uint mouseMax = 141;

      if (uint.TryParse(config["beholder_stalk_clickmin"], out uint configMouseMin))
      {
        mouseMin = configMouseMin;
      }

      if (uint.TryParse(config["beholder_stalk_clickmax"], out uint configMouseMax))
      {
        mouseMax = configMouseMax;
      }

      AverageClickDuration = new MouseClickDuration() { Min = mouseMin, Max = mouseMax };
      WatchNext();
    }

    public MouseClickDuration AverageClickDuration
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

    public void SendMouseClick(string button, ClickDirection direction = ClickDirection.PressAndRelease, MouseClickDuration duration = null)
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
          DelayUtil.Think(duration).GetAwaiter().GetResult();
          SendButtonRelease(buttonValue);
          break;
      }
    }

    public void SendMouseMoveTo(MoveMouseToRequest request)
    {
      if (!_isMoving)
      {
        lock (_isMovingLock)
        {
          if (!_isMoving)
          {
            try
            {
              _isMoving = true;
              if (!string.IsNullOrWhiteSpace(request.PreMoveActions))
              {
                SendMouseActions(request.PreMoveActions);
              }

              var momementSpeed = 5;
              if (request.MovementSpeed > 0)
              {
                momementSpeed = request.MovementSpeed;
              }

              if (request.CurrentPosition == null)
              {
                request.CurrentPosition = new MoveMouseToRequest.Types.Point() { X = 0, Y = 0 };
              }

              if (request.TargetPosition == null)
              {
                request.TargetPosition = new MoveMouseToRequest.Types.Point() { X = 0, Y = 0 };
              }

              var estimatedPosition = new MoveMouseToRequest.Types.Point()
              {
                X = request.CurrentPosition.X,
                Y = request.CurrentPosition.Y,
              };

              while (estimatedPosition.X != request.TargetPosition.X && estimatedPosition.Y != request.TargetPosition.Y)
              {
                short xAmount, yAmount;
                if (estimatedPosition.X < request.TargetPosition.X)
                {
                  xAmount = (short)momementSpeed;
                }
                else if (estimatedPosition.X > request.TargetPosition.X)
                {
                  xAmount = (short)(momementSpeed * -1);
                }
                else
                {
                  xAmount = 0;
                }

                if (estimatedPosition.Y < request.TargetPosition.Y)
                {
                  yAmount = (short)momementSpeed;
                }
                else if (estimatedPosition.Y > request.TargetPosition.Y)
                {
                  yAmount = (short)(momementSpeed * -1);
                }
                else
                {
                  yAmount = 0;
                }

                SendMouseMove(xAmount, yAmount);
                estimatedPosition.X += xAmount;
                estimatedPosition.Y += yAmount;
              }

              if (!string.IsNullOrWhiteSpace(request.PostMoveActions))
              {
                SendMouseActions(request.PostMoveActions);
              }
            }
            finally
            {
              _isMoving = false;
            }
          }
        }
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

    public void SendMouseActions(string actions)
    {
      var mouseActions = new List<IMouseAction>();
      foreach (Match m in MouseActionsRegex.Matches(actions))
      {
        if (m.Groups.ContainsKey("Button") && !string.IsNullOrWhiteSpace(m.Groups["ButtonValue"].Value))
        {
          var mouseclick = new MouseClick()
          {
            Button = m.Groups["ButtonValue"].Value,
            ClickDirection = ClickDirection.PressAndRelease,
            Duration = AverageClickDuration,
          };

          if (m.Groups.ContainsKey("Direction"))
          {
            switch (m.Groups["Direction"].Value.ToLowerInvariant())
            {
              case "press":
              case "down":
                mouseclick.ClickDirection = ClickDirection.Press;
                mouseclick.Duration = MouseClickDuration.Infinite;
                break;
              case "release":
              case "up":
                mouseclick.ClickDirection = ClickDirection.Release;
                mouseclick.Duration = MouseClickDuration.Infinite;
                break;
              default:
                mouseclick.ClickDirection = ClickDirection.PressAndRelease;
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
        switch (mouseAction)
        {
          case MouseClick mouseClick:
            SendMouseClick(mouseClick.Button, mouseClick.ClickDirection, mouseClick.Duration);
            break;
          case MouseMove mouseMove:
            SendMouseMove((short)mouseMove.X, (short)mouseMove.Y);
            break;
          case MouseScroll mouseScroll:
            SendMouseScroll((sbyte)mouseScroll.Amount);
            break;
          case MouseTilt mouseTilt:
            SendMouseTilt((sbyte)mouseTilt.Amount);
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