namespace beholder_occipital_v1.ObjectDetection
{
  using OpenCvSharp;

  public interface IMatchProcessor
  {
    DMatch[][] ProcessAndObtainMatches(Mat queryImage, Mat trainImage, out KeyPoint[] queryKeyPoints, out KeyPoint[] trainKeyPoints);
  }
}