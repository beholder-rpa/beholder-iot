namespace beholder_stalk_v2.Controllers
{
  using beholder_stalk_v2.Models;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Threading.Tasks;

  [MqttController]
  public class ContextController
  {
    private readonly BeholderContext _context;
    private readonly ILogger<ContextController> _logger;

    public ContextController(BeholderContext context, ILogger<ContextController> logger)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [EventPattern("beholder/eye/+/pointer_position")]
    public Task UpdatePointerPositionFromEye(ICloudEvent<Point> pointerPosition)
    {
      if (pointerPosition.Data != null &&
          (_context.LastEyePointerUpdate.HasValue == false ||
            _context.LastEyePointerUpdate.Value < pointerPosition.Time
          )
         )
      {
        _context.LastEyePointerUpdate = pointerPosition.Time;
        _context.EyeCurrentPointerPosition = pointerPosition.Data;
      }
      return Task.CompletedTask;
    }

    [EventPattern("beholder/psionix/+/pointer_position")]
    public Task UpdatePointerPositionFromPsionix(ICloudEvent<Point> pointerPosition)
    {
      if (pointerPosition.Data != null &&
          (_context.LastPsionixPointerUpdate.HasValue == false ||
            _context.LastPsionixPointerUpdate.Value < pointerPosition.Time
          )
         )
      {
        _context.LastPsionixPointerUpdate = pointerPosition.Time;
        _context.PsionixCurrentPointerPosition = pointerPosition.Data;
      }
      return Task.CompletedTask;
    }

    [EventPattern("beholder/psionix/+/system_information")]
    public Task UpdateSystemInformationFromPsionix(ICloudEvent<SysInfo> sysInfo)
    {
      if (sysInfo.Data != null)
      {
        _context.SysInfo = sysInfo.Data;
      }
      return Task.CompletedTask;
    }
  }
}