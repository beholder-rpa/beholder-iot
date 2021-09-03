namespace beholder_stalk_v2.Models
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;

  public record BeholderContext
  {
    public Point CurrentPointerPosition
    {
      get;
      set;
    }
  }
}