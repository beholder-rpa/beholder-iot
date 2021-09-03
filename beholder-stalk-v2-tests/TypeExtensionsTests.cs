namespace beholder_stalk_v2.Tests
{
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2;
  using Xunit;

  public class TypeExtensionsTests
  {
    [Fact]
    public void Test1()
    {
      var someType = typeof(ICloudEvent<string>);
      var result = someType.IsGenericType && someType.GetGenericTypeDefinition() == typeof(ICloudEvent<>);
      Assert.True(result);
    }
  }
}
