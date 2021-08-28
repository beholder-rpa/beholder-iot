namespace beholder_epidermis_v1.Controllers
{
  using beholder_epidermis_v1.Mvc;
  using Dapr.Client;
  using Microsoft.AspNetCore.Mvc;
  using Microsoft.Extensions.Logging;
  using SixLabors.ImageSharp;
  using SixLabors.ImageSharp.Drawing.Processing;
  using SixLabors.ImageSharp.PixelFormats;
  using SixLabors.ImageSharp.Processing;
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;

  [Route("api/epidermis/[controller]")]
  [ApiController]
  public class MpegController : ControllerBase
  {
    private readonly ILogger<MpegController> _logger;
    private readonly DaprClient _daprClient;
    
    private readonly Lazy<byte[]> _blankImage = new Lazy<byte[]>(() =>
    {
      using var image = new Image<Rgb24>(400, 225);
      image.Mutate(img => img.Fill(Brushes.Solid(Color.DarkGrey)));

      using var memStream = new MemoryStream();
      image.SaveAsJpeg(memStream);
      return memStream.ToArray();
    });

    public MpegController(ILogger<MpegController> logger, DaprClient daprClient)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
    }

    // GET /api/epidermis/mjpeg/SOMEKEYHERE
    [HttpGet("{*key}")]
    public IActionResult Get(string key, CancellationToken cancellationToken = default)
    {
      var first = true;
      return new MjpegStream(async cancellationToken => {
        if (first)
          first = false; // no delay for first image
        else
          await Task.Delay(TimeSpan.FromMilliseconds(33.3), cancellationToken); // Don't exceed 30fps

        var img = await _daprClient.GetStateAsync<byte[]>(Consts.StateStoreName, key, ConsistencyMode.Eventual, cancellationToken: cancellationToken);

        if (img != null)
        {
          return img;
        }

        return _blankImage.Value;
      }, null);
    }
  }
}
