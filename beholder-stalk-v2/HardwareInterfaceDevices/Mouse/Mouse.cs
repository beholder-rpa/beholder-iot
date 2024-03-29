namespace beholder_stalk_v2.HardwareInterfaceDevices
{
  using beholder_stalk_v2.Protos;
  using beholder_stalk_v2.Utils;
  using Microsoft.Extensions.Configuration;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Text.RegularExpressions;
  using System.Threading;
  using System.Threading.Tasks;
  using static beholder_stalk_v2.Protos.MouseClick.Types;

  /// <summary>
  /// Represents a HID Mouse
  /// </summary>
  /// <remarks>
  /// See: https://www.microchip.com/forums/m391435.aspx
  /// </remarks>
  public partial class Mouse : HumanInterfaceDevice, IObservable<MouseEvent>
  {
    private static readonly object s_reportLock = new object();
    private static readonly byte[] s_currentReport = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    private static readonly Regex MouseActionsRegex = new Regex(
        @"{(?:(?<Button>(?<ButtonValue>lbutton|lclick|rbutton|rclick|mbutton|mclick|back|xbutton1|forward|xbutton2)(?:\s(?:(?<Repeats>\d+)|(?<Direction>down|up)))?)|(?<Move>(?<XValue>-?\d+),(?<YValue>-?\d+))|(?<Scroll>scroll:(?<ScrollValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7])))|(?<Tilt>tilt:(?<TiltValue>-?(?:[0-9]|[0-9][0-9]|[1][0-2][0-7]))))}"
        , RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

    public event EventHandler<MouseResolutionChangedEventArgs> MouseResolutionChanged;
    private readonly ConcurrentDictionary<IObserver<MouseEvent>, MouseEventUnsubscriber> _observers = new ConcurrentDictionary<IObserver<MouseEvent>, MouseEventUnsubscriber>();
    public int _hres = 800, _vres = 800;

    private readonly ILogger<Mouse> _logger;
    private readonly byte[] sizeBuffer = new byte[1];
    private volatile bool _isMoving = false;
    private readonly object _isMovingLock = new object();

    public Mouse(IConfiguration config, ILogger<Mouse> logger)
        : base(config["hid:mouse:devPath"])
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

      _logger.LogTrace($"Sent Mouse Click: Button {button} Direction: {direction} Duration: {duration}");
    }

    public bool SendMouseMoveTo(MoveMouseToRequest request)
    {
      if (request.CurrentPosition == null || request.TargetPosition == null)
      {
        _logger.LogWarning("CurrentPosition and TargetPosition were not specified, skipping SendMouseMoveTo.");
        return false;
      }

      var movementScaleX = 1.0f;
      if (request.MovementScaleX > 0)
      {
        movementScaleX = request.MovementScaleX;
      }

      var movementScaleY = 1.0f;
      if (request.MovementScaleY > 0)
      {
        movementScaleY = request.MovementScaleY;
      }

      var sourcePoint = new MoveMouseToRequest.Types.Point() { X = (int)Math.Round(request.CurrentPosition.X * movementScaleX), Y = (int)Math.Round(request.CurrentPosition.Y * movementScaleY) };
      var targetPoint = new MoveMouseToRequest.Types.Point() { X = (int)Math.Round(request.TargetPosition.X * movementScaleX), Y = (int)Math.Round(request.TargetPosition.Y * movementScaleY) };

      _logger.LogTrace($"Using source point {sourcePoint.X},{sourcePoint.Y}, which is scaled from {request.CurrentPosition.X} * {movementScaleX},{request.CurrentPosition.Y} * {movementScaleY}");
      _logger.LogTrace($"Using target point {targetPoint.X},{targetPoint.Y}, which is scaled from {request.TargetPosition.X} * {movementScaleX},{request.TargetPosition.Y} * {movementScaleY}");

      var movementSpeed = 1;
      if (request.MovementSpeed > 0)
      {
        movementSpeed = request.MovementSpeed;
      }

      var movementDelayMs = 0;
      if (request.MovementDelayMs > 0)
      {
        movementDelayMs = request.MovementDelayMs;
      }

      var line = new Line(sourcePoint, targetPoint);
      MoveMouseToRequest.Types.Point[] points;
      switch (request.MovementType)
      {
        default:
        case MoveMouseToRequest.Types.MovementType.Linear:
          points = line.GetPoints();
          var length = points.Length;
          points = ArrayUtil.Resize((int)Math.Ceiling((double)length / movementSpeed), points);
          break;
      }

      if (!string.IsNullOrWhiteSpace(request.PointsTopic))
      {
        OnMouseEvent(new MouseMoveToPointsEvent() { Topic = request.PointsTopic, Points = points });
      }

      var movedMouse = false;

      // Use double-check locking to ensure we're not attempting to move when we're already moving
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
              for (int i = 1; i <= points.Length - 1; i++)
              {
                var currentPoint = points[i - 1];
                var nextPoint = points[i];
                var deltaX = (short)(nextPoint.X - currentPoint.X);
                var deltaY = (short)(nextPoint.Y - currentPoint.Y);

                SendMouseMove(deltaX, deltaY);
                _logger.LogTrace($"Move: {nextPoint.X},{nextPoint.Y} ({deltaX},{deltaY})");

                if (movementDelayMs > 0)
                {
                  Thread.Sleep(movementDelayMs);
                }
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

            OnMouseEvent(new MovedMouseEvent()
            {
              From = sourcePoint,
              To = targetPoint,
            });

            movedMouse = true;
          }
        }
      }

      if (movedMouse)
      {
        _logger.LogInformation($"Moved Mouse from ({sourcePoint.X},{sourcePoint.Y}) to ({targetPoint.X},{targetPoint.Y}) in {points.Length} steps using {request.MovementType} behavior, scale of ({movementScaleX},{movementScaleY}) and speed of {movementSpeed}. With Pre-Move Actions {request.PreMoveActions} and Post-Move Actions {request.PostMoveActions}");
      }
      else
      {
        _logger.LogWarning($"Skipped movement - mouse was already moving.");
      }

      return movedMouse;
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

    public IDisposable Subscribe(IObserver<MouseEvent> observer)
    {
      return _observers.GetOrAdd(observer, new MouseEventUnsubscriber(this, observer));
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

      _hres = sizeBuffer[0] << 2;
      _vres = sizeBuffer[0] << 4;

      OnMouseResolutionChanged(new MouseResolutionChangedEventArgs(_hres, _vres));
      OnMouseEvent(new MouseResolutionChangedEvent(_hres, _vres));
      WatchNext();
    }

    /// <summary>
    /// Produces Mouse Events
    /// </summary>
    /// <param name="mouseEvent"></param>
    private void OnMouseEvent(MouseEvent mouseEvent)
    {
      Parallel.ForEach(_observers.Keys, (observer) =>
      {
        try
        {
          observer.OnNext(mouseEvent);
        }
        catch (Exception)
        {
          // Do Nothing.
        }
      });
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
      {
        SendMouseReset();
      }

      base.Dispose(disposing);
    }

    #region Nested Classes
    private sealed class MouseEventUnsubscriber : IDisposable
    {
      private readonly Mouse _parent;
      private readonly IObserver<MouseEvent> _observer;

      public MouseEventUnsubscriber(Mouse parent, IObserver<MouseEvent> observer)
      {
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _observer = observer ?? throw new ArgumentNullException(nameof(observer));
      }

      public void Dispose()
      {
        if (_observer != null && _parent._observers.ContainsKey(_observer))
        {
          _parent._observers.TryRemove(_observer, out _);
        }
      }
    }
    #endregion
  }
}