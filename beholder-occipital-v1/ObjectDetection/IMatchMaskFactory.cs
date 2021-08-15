namespace beholder_occipital_v1.ObjectDetection
{
  using OpenCvSharp;
  using System.Collections.Generic;

  public interface IMatchMaskFactory
  {
    float RatioThreshold { get; set; }

    float ScaleIncrement { get; set; }

    int RotationBins { get; set; }

    Mat CreateMatchMask(DMatch[][] matches, KeyPoint[] queryKeyPoints, KeyPoint[] trainKeyPoints, out IList<DMatch> goodMatches);
  }
}