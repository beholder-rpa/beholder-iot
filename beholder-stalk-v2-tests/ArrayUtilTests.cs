namespace beholder_stalk_v2_tests
{
  using beholder_stalk_v2.Utils;
  using Xunit;

  public class ArrayUtilTests
  {
    [Fact]
    public void Resize_Scales_Bigger()
    {
      var myArray = new int[5] { 0, 1, 2, 3, 4 };
      var newArray = ArrayUtil.Resize(10, myArray);
      Assert.Equal(new int[10] { 0, 0, 1, 1, 2, 2, 3, 3, 4, 4 }, newArray);
    }

    [Fact]
    public void Resize_Scales_Bigger_2()
    {
      var myArray = new int[7] { 0, 1, 2, 3, 4, 5, 6 };
      var newArray = ArrayUtil.Resize(11, myArray);
      Assert.Equal(new int[11] { 0, 0, 1, 1, 2, 3, 3, 4, 5, 5, 6 }, newArray);
    }

    [Fact]
    public void Resize_Samples_Smaller()
    {
      var myArray = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      var newArray = ArrayUtil.Resize(5, myArray);
      Assert.Equal(new int[5] { 0, 2, 4, 6, 8 }, newArray);
    }

    [Fact]
    public void Resize_Samples_Smaller_2()
    {
      var myArray = new int[13] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };
      var newArray = ArrayUtil.Resize(7, myArray);
      Assert.Equal(new int[7] { 0, 1, 3, 5, 7, 9, 11 }, newArray);
    }

    [Fact]
    public void Resize_Returns_Empty_When_0()
    {
      var myArray = new int[10] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
      var newArray = ArrayUtil.Resize(0, myArray);
      Assert.Empty(newArray);
    }
  }
}
