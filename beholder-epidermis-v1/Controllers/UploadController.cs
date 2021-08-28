namespace beholder_epidermis_v1.Controllers
{
  using Dapr.Client;
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
    private readonly DaprClient _daprClient;

    public UploadController(ILogger<UploadController> logger, DaprClient daprClient)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    }

    // POST /api/epidermis/upload/SOMEKEYHERE
    [HttpGet("{*key}")]
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
        await _daprClient.SaveStateAsync(Consts.StateStoreName, key, fileBytes);
      }

      return Ok(new { count = files.Count, size });
    }
  }
}
