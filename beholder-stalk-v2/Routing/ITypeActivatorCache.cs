namespace beholder_stalk_v2.Routing
{
  using System;

  public interface ITypeActivatorCache
  {
    TInstance CreateInstance<TInstance>(IServiceProvider serviceProvider, Type implementationType);
  }
}