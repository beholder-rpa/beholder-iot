namespace beholder_epidermis_v1.Controllers
{
  using beholder_epidermis_v1.Cache;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Net.Mime;
  using System.Threading;
  using System.Threading.Tasks;

  [Route("api/epidermis/[controller]")]
  [ApiController]
  public class DownloadController : ControllerBase
  {
    private readonly ILogger<DownloadController> _logger;
    private readonly ICacheClient _cacheClient;

    public DownloadController(ILogger<DownloadController> logger, ICacheClient cacheClient)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _cacheClient = cacheClient ?? throw new ArgumentNullException(nameof(cacheClient));
    }

    // POST /api/epidermis/download/SOMEKEYHERE
    [HttpGet("{*key}")]
    public async Task<IActionResult> DownloadAsync(
      string key,
      [FromQuery(Name = "d")] bool download = false,
      [FromQuery(Name = "ct")] string contentType = null,
      [FromQuery(Name = "fn")] string fileName = null,
      CancellationToken cancellationToken = default
    )
    {
      var item = await _cacheClient.Base64ByteArrayGet(key);

      if (string.IsNullOrWhiteSpace(contentType))
      {
        contentType = MediaTypeNames.Application.Octet;
      }

      if (download)
      {
        return File(item, contentType, fileName);
      }

      return File(item, contentType);
    }
  }
}
