namespace beholder_stalk
{
  using Newtonsoft.Json;
  using System.Collections.Generic;
  using System.Diagnostics.CodeAnalysis;

  public class Duration : IEqualityComparer<Duration>
  {
    [JsonProperty("delay")]
    public int? Delay
    {
      get;
      set;
    }

    [JsonProperty("min")]
    public int? Min
    {
      get;
      set;
    }

    [JsonProperty("max")]
    public int? Max
    {
      get;
      set;
    }

    public static Duration Infinite = new Duration() { Delay = -1, Min = -1, Max = -1 };

    // override object.Equals
    public override bool Equals(object obj)
    {
      //       
      // See the full list of guidelines at
      //   http://go.microsoft.com/fwlink/?LinkID=85237  
      // and also the guidance for operator== at
      //   http://go.microsoft.com/fwlink/?LinkId=85238
      //

      if (obj == null || GetType() != obj.GetType())
      {
        return false;
      }

      return Equals(this, obj as Duration);
    }

    public bool Equals([AllowNull] Duration x, [AllowNull] Duration y)
    {
      if (x != null && y != null)
      {
        return x.Delay == y.Delay && x.Min == y.Min && x.Max == y.Max;
      }

      if (x == null && y == null)
      {
        return true;
      }

      return false;
    }

    public int GetHashCode([DisallowNull] Duration obj)
    {
      return $"{obj.Delay}{obj.Min}{obj.Max}".GetHashCode();
    }

    public override int GetHashCode()
    {
      return GetHashCode(this);
    }
  }
}