namespace beholder_occipital_v1.ObjectDetection
{
  using OpenCvSharp;
  using OpenCvSharp.Features2D;
  using OpenCvSharp.Flann;

  public class SiftFlannMatchProcessor : IMatchProcessor
  {
    /// <summary>
    /// Detect feature descriptors in the query and target images using a feature detector
    /// </summary>
    /// <param name="queryImage"></param>
    /// <param name="trainImage"></param>
    /// <param name="queryKeyPoints"></param>
    /// <param name="trainKeyPoints"></param>
    /// <returns></returns>
    public DMatch[][] ProcessAndObtainMatches(Mat queryImage, Mat trainImage, out KeyPoint[] queryKeyPoints, out KeyPoint[] trainKeyPoints)
    {
      // Obtain the feature descriptors using the feature detector;
      using var queryDescriptors = new Mat();
      using var trainDescriptors = new Mat();

      // SIFT is considered the most accurate algorithm - https://ieeexplore.ieee.org/document/8346440
      // At least among those available in OpenCV - https://arxiv.org/pdf/1808.02267.pdf
      using Feature2D detector = SIFT.Create();
      {
        detector.DetectAndCompute(queryImage, null, out queryKeyPoints, queryDescriptors);
        detector.DetectAndCompute(trainImage, null, out trainKeyPoints, trainDescriptors);
      }

      // Pass the descriptors to a matcher to obtain the matches
      return ObtainMatches(queryDescriptors, trainDescriptors);
    }

    /// <summary>
    /// Obtain the matching feature descriptors using the descriptor matcher
    /// </summary>
    /// <remarks>
    /// Flann provides a faster algorithm the brute-force matching method - https://stackoverflow.com/questions/10610966/difference-between-bfmatcher-and-flannbasedmatcher/16141103
    /// </remarks>
    /// <param name="queryDescriptors"></param>
    /// <param name="trainDescriptors"></param>
    /// <returns></returns>
    private static DMatch[][] ObtainMatches(Mat queryDescriptors, Mat trainDescriptors)
    {
      using var indexParams = new IndexParams();
      indexParams.SetAlgorithm(0);
      indexParams.SetInt("trees", 5);

      using var searchParams = new SearchParams();
      searchParams.SetInt("checks", 50);

      using DescriptorMatcher matcher = new FlannBasedMatcher(indexParams, searchParams);
      return matcher.KnnMatch(queryDescriptors, trainDescriptors, 2);
    }
  }
}