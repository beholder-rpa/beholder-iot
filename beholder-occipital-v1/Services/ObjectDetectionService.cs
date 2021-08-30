namespace beholder_occipital_v1.Services
{
  using beholder_occipital_v1.Cache;
  using beholder_occipital_v1.ObjectDetection;
  using beholder_occipital_v1.Protos;
  using Dapr.AppCallback.Autogen.Grpc.v1;
  using Dapr.Client;
  using Dapr.Client.Autogen.Grpc.v1;
  using Google.Protobuf.WellKnownTypes;
  using Grpc.Core;
  using Microsoft.Extensions.Logging;
  using OpenCvSharp;
  using System;
  using System.Collections.Generic;
  using System.Net;
  using System.Threading.Tasks;

  public class ObjectDetectionService : IObjectDetectionService
  {
    private readonly IMatchMaskFactory _matchMaskFactory;
    private readonly IMatchProcessor _matchProcessor;
    private readonly DaprClient _daprClient;
    private readonly ICacheClient _cacheClient;
    private readonly ILogger<ObjectDetectionService> _logger;
    private readonly string _hostName;

    public ObjectDetectionService(IMatchMaskFactory matchMaskFactory, IMatchProcessor matchProcessor, DaprClient daprClient, ICacheClient cacheClient, ILogger<ObjectDetectionService> logger)
    {
      _matchMaskFactory = matchMaskFactory ?? throw new ArgumentNullException(nameof(matchMaskFactory));
      _matchProcessor = matchProcessor ?? throw new ArgumentNullException(nameof(matchProcessor));

      _daprClient = daprClient ?? throw new ArgumentNullException(nameof(daprClient));
      _cacheClient = cacheClient ?? throw new ArgumentNullException(nameof(cacheClient));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _hostName = Environment.GetEnvironmentVariable("BEHOLDER_OCCIPITAL_NAME") ?? Dns.GetHostName();
    }

    public string Name => "object_detection";

    /// <summary>
    /// Implement OnInvoke to support detect invocations
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
    {
      return request.Method switch
      {
        "Detect" => await GrpcUtil.InvokeMethodFromInvoke<ObjectDetectionRequest, ObjectDetectionReply>(request, (input) => Detect(input)),
        _ => null,
      };
    }

    /// <summary>
    /// Implement ListTopicSubscriptions to register detect events
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
    {
      var result = new ListTopicSubscriptionsResponse();
      result.Subscriptions.Add(new TopicSubscription
      {
        PubsubName = Consts.PubSubName,
        Topic = $"beholder/occipital/{_hostName}/object_detection/detect"
      });

      return Task.FromResult(result);
    }

    /// <summary>
    /// Implement OnTopicEvent to handle detect events
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
    {
      var topic = request.Topic.Replace($"beholder/occipital/{_hostName}/object_detection/", "");
      _logger.LogInformation($"Invoking method for event topic {topic}");
      return topic switch
      {
        "detect" => await GrpcUtil.InvokeMethodFromEvent<ObjectDetectionRequest, ObjectDetectionReply>(_daprClient, request, (input) => Detect(input)),
        _ => null
      };
    }

    public async Task<ObjectDetectionReply> Detect(ObjectDetectionRequest request)
    {
      if (string.IsNullOrWhiteSpace(request.QueryImagePrefrontalKey))
      {
        throw new InvalidOperationException("QueryImagePrefrontalKey must be specified");
      }

      if (string.IsNullOrWhiteSpace(request.TargetImagePrefrontalKey))
      {
        throw new InvalidOperationException("QueryImagePrefrontalKey must be specified");
      }

      _logger.LogInformation($"Performing Object Detection using {request.QueryImagePrefrontalKey}, {request.TargetImagePrefrontalKey}");

      var queryImageBytes = await _cacheClient.Base64ByteArrayGet(request.QueryImagePrefrontalKey);
      if (queryImageBytes == default)
      {
        throw new InvalidOperationException($"Query Image from Cache using key {request.QueryImagePrefrontalKey} was empty");
      }
      var targetImageBytes = await _cacheClient.Base64ByteArrayGet(request.TargetImagePrefrontalKey);
      if (targetImageBytes == default)
      {
        throw new InvalidOperationException($"Target Image from Cache using key {request.TargetImagePrefrontalKey} was empty");
      }

      using var queryImage = Cv2.ImDecode(queryImageBytes, ImreadModes.Color);
      using var trainImage = Cv2.ImDecode(targetImageBytes, ImreadModes.Color);

      if (queryImage.Empty())
      {
        throw new InvalidOperationException("The query image is empty.");
      }

      if (trainImage.Empty())
      {
        throw new InvalidOperationException("The train image is empty.");
      }

      var locationsResult = new List<IEnumerable<Point>>();

      DMatch[][] knn_matches = _matchProcessor.ProcessAndObtainMatches(queryImage, trainImage, out KeyPoint[] queryKeyPoints, out KeyPoint[] trainKeyPoints);

      if (request.MatchMaskSettings == null)
      {
        _matchMaskFactory.RatioThreshold = 0.76f;
        _matchMaskFactory.ScaleIncrement = 2.0f;
        _matchMaskFactory.RotationBins = 20;
      }
      else
      {
        _matchMaskFactory.RatioThreshold = request.MatchMaskSettings.RatioThreshold;
        _matchMaskFactory.ScaleIncrement = request.MatchMaskSettings.ScaleIncrement;
        _matchMaskFactory.RotationBins = request.MatchMaskSettings.RotationBins;
      }

      using var mask = _matchMaskFactory.CreateMatchMask(knn_matches, queryKeyPoints, trainKeyPoints, out var allGoodMatches);
      var goodMatches = new List<DMatch>(allGoodMatches);
      while (goodMatches.Count > 4)
      {
        // Use Homeography to obtain a perspective-corrected rectangle of the target in the query image.
        var sourcePoints = new Point2f[goodMatches.Count];
        var destinationPoints = new Point2f[goodMatches.Count];
        for (int i = 0; i < goodMatches.Count; i++)
        {
          DMatch match = goodMatches[i];
          sourcePoints[i] = queryKeyPoints[match.QueryIdx].Pt;
          destinationPoints[i] = trainKeyPoints[match.TrainIdx].Pt;
        }

        Point[] targetPoints = null;
        using var homography = Cv2.FindHomography(InputArray.Create(sourcePoints), InputArray.Create(destinationPoints), HomographyMethods.Ransac, 5.0);
        {
          if (homography.Rows > 0)
          {
            Point2f[] queryCorners = {
              new Point2f(0, 0),
              new Point2f(queryImage.Cols, 0),
              new Point2f(queryImage.Cols, queryImage.Rows),
              new Point2f(0, queryImage.Rows)
            };

            Point2f[] dest = Cv2.PerspectiveTransform(queryCorners, homography);
            targetPoints = new Point[dest.Length];
            for (int i = 0; i < dest.Length; i++)
            {
              targetPoints[i] = dest[i].ToPoint();
            }
          }
        }

        var matchesToRemove = new List<DMatch>();

        if (targetPoints != null)
        {
          locationsResult.Add(targetPoints);

          // Remove matches within bounding rectangle
          for (int i = 0; i < goodMatches.Count; i++)
          {
            DMatch match = goodMatches[i];
            var pt = trainKeyPoints[match.TrainIdx].Pt;
            var inPoly = Cv2.PointPolygonTest(targetPoints, pt, false);
            if (inPoly == 1)
            {
              matchesToRemove.Add(match);
            }
          }
        }

        // If we're no longer doing meaningful work, break out of the loop
        if (matchesToRemove.Count == 0)
        {
          break;
        }

        foreach (var match in matchesToRemove)
        {
          goodMatches.Remove(match);
        }
      }

      // If an output image prefrontal key is specified, generate an output image and store it in prefrontal state
      if (!string.IsNullOrWhiteSpace(request.OutputImagePrefrontalKey))
      {
        byte[] maskBytes = new byte[mask.Rows * mask.Cols];
        Cv2.Polylines(trainImage, locationsResult, true, new Scalar(255, 0, 0), 3, LineTypes.AntiAlias);
        using var outImg = new Mat();
        Cv2.DrawMatches(queryImage, queryKeyPoints, trainImage, trainKeyPoints, allGoodMatches, outImg, new Scalar(0, 255, 0), flags: DrawMatchesFlags.NotDrawSinglePoints);
        var outImageBytes = outImg.ImEncode();
        await _cacheClient.Base64ByteArraySet(request.OutputImagePrefrontalKey, outImageBytes);
      }

      // Create and return the result
      var result = new ObjectDetectionReply();
      foreach (var locationsResultPoly in locationsResult)
      {
        var poly = new ObjectPoly();
        foreach (var locationResultPolyPoint in locationsResultPoly)
        {
          poly.Points.Add(new ObjectPoly.Types.Point()
          {
            X = locationResultPolyPoint.X,
            Y = locationResultPolyPoint.Y,
          });
        }
        result.Locations.Add(poly);
      }

      _logger.LogInformation($"Located {result.Locations.Count} polys: {result.Locations}");
      return result;
    }

    public Task OnStatusEvent()
    {
      // Do Nothing
      return Task.CompletedTask;
    }
  }
}