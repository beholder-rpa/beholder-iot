namespace beholder_occipital_v1.ObjectDetection
{
  using Microsoft.Extensions.Logging;
  using OpenCvSharp;
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public class MatchMaskFactory : IMatchMaskFactory
  {
    private ILogger<MatchMaskFactory> _logger;
    public MatchMaskFactory(ILogger<MatchMaskFactory> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));

      RatioThreshold = 0.76f;
      ScaleIncrement = 1.5f;
      RotationBins = 20;
    }

    public float RatioThreshold { get; set; }
    public float ScaleIncrement { get; set; }
    public int RotationBins { get; set; }
    public bool SkipScaleRotationCulling { get; set; }

    /// <summary>
    /// Returns a mask based on the supplied matches based on various algorithms
    /// </summary>
    /// <param name="matches"></param>
    /// <param name="queryKeypoints"></param>
    /// <param name="trainKeypoints"></param>
    /// <param name="goodMatches"></param>
    /// <returns></returns>
    public Mat CreateMatchMask(DMatch[][] matches, KeyPoint[] queryKeypoints, KeyPoint[] trainKeypoints, out IList<DMatch> goodMatches)
    {
      goodMatches = new List<DMatch>();

      Mat mask = new Mat(matches.Length, 1, MatType.CV_8U);
      mask.SetTo(new Scalar(255));

      int nonZero = Cv2.CountNonZero(mask);
      _logger.LogInformation($"Original Match Count: {nonZero}");
      if (nonZero > 0)
      {
        nonZero = ApplyDistanceRatioMatchCulling(matches, mask, RatioThreshold);
        _logger.LogInformation($"Match Count after distance-ratio match culling: {nonZero}");
      }
      else
      {
        _logger.LogInformation($"Skipped distance-rotation match culling - no matches.");
      }

      if (nonZero > 0)
      {
        if (SkipScaleRotationCulling == false)
        {
          nonZero = ApplyScaleRotationMatchCulling(matches, queryKeypoints, trainKeypoints, mask, ScaleIncrement, RotationBins);
          _logger.LogInformation($"Match Count after scale-rotation match culling: {nonZero}");
        }
        else
        {
          _logger.LogInformation($"Skipped scale-rotation match culling - skip scale-rotation match specified.");
        }
      }
      else
      {
        _logger.LogInformation($"Skipped scale-rotation match culling - no matches.");
      }

      MatIndexer<byte> maskIndexer = mask.GetGenericIndexer<byte>();
      for (int i = 0; i < mask.Rows; i++)
      {
        if (maskIndexer[i] > 0)
        {
          goodMatches.Add(matches[i][0]);
        }
      }

      return mask;
    }

    private static int ApplyScaleRotationMatchCulling(DMatch[][] matches, KeyPoint[] queryKeyPoints, KeyPoint[] trainKeyPoints, Mat mask, float scaleIncrement = 1.5f, int rotationBins = 20)
    {
      int idx = 0;
      int nonZeroCount = 0;
      var maskData = mask.GetGenericIndexer<byte>();

      List<float> logScale = new List<float>();
      List<float> rotations = new List<float>();
      double s, maxS, minS, r;
      maxS = -1.0e-10f; minS = 1.0e10f;

      for (int i = 0; i < mask.Rows; i++)
      {
        if (maskData[i] > 0)
        {
          KeyPoint queryKeyPoint = queryKeyPoints[i];
          KeyPoint trainKeyPoint = trainKeyPoints[matches[i][0].TrainIdx];
          s = Math.Log10(queryKeyPoint.Size / trainKeyPoint.Size);
          logScale.Add((float)s);
          maxS = s > maxS ? s : maxS;
          minS = s < minS ? s : minS;

          r = queryKeyPoint.Angle - trainKeyPoint.Angle;
          r = r < 0.0f ? r + 360.0f : r;
          rotations.Add((float)r);
        }
      }

      if (logScale.Count == 0 || rotations.Count == 0)
      {
        return 0;
      }

      int scaleBinSize = (int)Math.Ceiling((maxS - minS) / Math.Log10(scaleIncrement));
      if (scaleBinSize < 2)
        scaleBinSize = 2;
      float[] scaleRanges = { (float)minS, (float)(minS + scaleBinSize + Math.Log10(scaleIncrement)) };

      using Mat<float> scalesMat = Mat.FromArray(logScale.ToArray());
      using Mat<float> rotationsMat = Mat.FromArray(rotations.ToArray());
      using Mat<float> flagsMat = new Mat<float>(logScale.Count, 1);
      using Mat hist = new Mat();

      flagsMat.SetTo(new Scalar(0.0f));
      float[] flagsMatFloat1 = flagsMat.ToArray();

      int[] histSize = { scaleBinSize, rotationBins };
      float[] rotationRanges = { 0.0f, 360.0f };
      int[] channels = { 0, 1 };
      Rangef[] ranges = { new Rangef(scaleRanges[0], scaleRanges[1]), new Rangef(rotations.Min(), rotations.Max()) };
      double minVal, maxVal;

      Mat[] arrs = { scalesMat, rotationsMat };
      Cv2.CalcHist(arrs, channels, null, hist, 2, histSize, ranges);
      Cv2.MinMaxLoc(hist, out minVal, out maxVal);

      Cv2.Threshold(hist, hist, maxVal * 0.5, 0, ThresholdTypes.Tozero);
      Cv2.CalcBackProject(arrs, channels, hist, flagsMat, ranges);

      MatIndexer<float> flagsMatIndexer = flagsMat.GetIndexer();

      for (int i = 0; i < mask.Rows; i++)
      {
        if (maskData[i] > 0)
        {
          if (flagsMatIndexer[idx++] != 0.0f)
          {
            nonZeroCount++;
          }
          else
            maskData[i] = 0;
        }
      }

      return nonZeroCount;
    }

    /// <summary>
    /// Apply Lowe's Distance-Ratio test - https://link.springer.com/article/10.1023%2FB%3AVISI.0000029664.99615.94
    /// </summary>
    /// <remarks>
    /// This is also known as NNDR Nearest Neighbor Distance Ratio
    /// </remarks>
    /// <param name="matches"></param>
    /// <param name="mask"></param>
    /// <param name="ratio_threshold"></param>
    /// <returns></returns>
    private static int ApplyDistanceRatioMatchCulling(DMatch[][] matches, Mat mask, float ratio_threshold = 0.76f)
    {
      var maskData = mask.GetGenericIndexer<byte>();
      for (int i = 0; i < matches.Length; i++)
      {
        if (matches[i][0].Distance < ratio_threshold * matches[i][1].Distance)
          maskData[i] = 255;
        else
          maskData[i] = 0;
      }
      return Cv2.CountNonZero(mask);
    }
  }
}