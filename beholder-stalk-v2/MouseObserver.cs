﻿namespace beholder_stalk_v2
{
  using beholder_stalk_v2.Protos;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Text;
  using System.Threading.Tasks;

  public class MouseObserver : IObserver<MouseEvent>
  {
    private readonly ILogger<MouseObserver> _logger;
    private readonly IBeholderMqttClient _beholderClient;

    public MouseObserver(IBeholderMqttClient beholderClient, ILogger<MouseObserver> logger)
    {
      _beholderClient = beholderClient ?? throw new ArgumentNullException(nameof(beholderClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnCompleted()
    {
      // Do Nothing
    }

    public void OnError(Exception error)
    {
      // Do Nothing
    }

    public void OnNext(MouseEvent mouseEvent)
    {
      switch (mouseEvent)
      {
        case MouseResolutionChangedEvent mouseResolutionChanged:
          HandleMouseResolutionChanged(mouseResolutionChanged).Forget();
          break;
        case MouseMoveToPointsEvent mouseMoveToPoints:
          HandleMouseMoveToPoints(mouseMoveToPoints).Forget();
          break;
        case MovedMouseEvent movedMouse:
          HandleMouseMoved(movedMouse).Forget();
          break;
        default:
          _logger.LogWarning($"Unhandled or unknown MouseEvent: {mouseEvent}");
          break;
      }
    }

    private async Task HandleMouseResolutionChanged(MouseResolutionChangedEvent mouseResolutionChanged)
    {
      await _beholderClient
        .PublishEventAsync(
          "beholder/stalk/{HOSTNAME}/status/mouse/resolution",
          new MouseResolution()
          {
            HorizontalResolution = (uint)mouseResolutionChanged.HorizontalResolution,
            VerticalResolution = (uint)mouseResolutionChanged.VerticalResolution
          }
        );
      _logger.LogTrace($"Published mouse resolution changed");
    }

    private async Task HandleMouseMoveToPoints(MouseMoveToPointsEvent mouseMoveToPoints)
    {
      var sb = new StringBuilder();
      sb.AppendLine($"x,y,");
      foreach (var point in mouseMoveToPoints.Points)
      {
        sb.AppendLine($"{point.X},{point.Y},");
      }

      await _beholderClient
        .PublishEventAsync(
          mouseMoveToPoints.Topic,
          sb.ToString()
        );
      _logger.LogTrace($"Published mouse move to points");
    }

    private async Task HandleMouseMoved(MovedMouseEvent mouseMoved)
    {
      await _beholderClient
        .PublishEventAsync(
          "beholder/stalk/{HOSTNAME}/status/mouse/moved",
          mouseMoved
        );
      _logger.LogTrace($"Published mouse moved");
    }
  }
}