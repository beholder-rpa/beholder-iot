namespace beholder_stalk_v2
{
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Routing;
  using Microsoft.Extensions.Caching.Memory;
  using Microsoft.Extensions.DependencyInjection;
  using System.Collections.Generic;
  using System.Reflection;

  public static class IServiceCollectionExtensions
  {
    public static IServiceCollection AddMqttControllers(this IServiceCollection services, ICollection<Assembly> assemblies = null)
    {
      services.AddSingleton<BeholderServiceInfo>();

      if (assemblies == null)
      {
        assemblies = new Assembly[] {
          Assembly.GetEntryAssembly()
        };
      }

      services.AddSingleton<IMemoryCache, MemoryCache>();
      services.AddSingleton(sp => MqttRouteTableFactory.Create(assemblies, sp.GetRequiredService<BeholderServiceInfo>()));
      services.AddSingleton<ITypeActivatorCache>(new TypeActivatorCache());
      services.AddSingleton<MqttApplicationMessageRouter>();

      return services;
    }
  }
}