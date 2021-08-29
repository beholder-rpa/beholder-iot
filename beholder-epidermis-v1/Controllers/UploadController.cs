namespace beholder_epidermis_v1.Controllers
{
  using beholder_epidermis_v1.Cache;
  using Microsoft.AspNetCore.Http;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Logging;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Threading.Tasks;

  [Route("api/epidermis/[controller]")]
  [ApiController]
  public class UploadController : ControllerBase
  {
    private readonly ILogger<UploadController> _logger;
    private readonly ICacheClient _cacheClient;

    public UploadController(ILogger<UploadController> logger, ICacheClient cacheClient)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _cacheClient = cacheClient ?? throw new ArgumentNullException(nameof(cacheClient));
    }

    // POST /api/epidermis/upload/SOMEKEYHERE
    [HttpPost("{*key}")]
    public async Task<IActionResult> UploadAsync(string key, List<IFormFile> files)
    {
      long size = files.Sum(f => f.Length);

      foreach (var formFile in files)
      {
        if (formFile.Length <= 0)
        {
          continue;
        }

        using var ms = new MemoryStream();
        formFile.CopyTo(ms);
        var fileBytes = ms.ToArray();
        await _cacheClient.Base64ByteArraySet(key, fileBytes);
      }

      return Ok(new { count = files.Count, size });
    }
  }
}
