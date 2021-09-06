namespace beholder_stalk_v2
{
  using beholder_stalk_v2.Models;
  using beholder_stalk_v2.Routing;
  using Microsoft.Extensions.DependencyInjection;
  using System.Collections.Generic;
  using System.Reflection;

  public static class IServiceCollectionExtensions
  {
    public static IServiceCollection AddMqttControllers(this IServiceCollection services, ICollection<Assembly> assemblies = null)
    {
      if (assemblies == null)
      {
        assemblies = new Assembly[] {
          Assembly.GetEntryAssembly()
        };
      }

      var routeTable = MqttRouteTableFactory.Create(assemblies, services, new BeholderServiceInfo());
      services.AddSingleton(routeTable);
      services.AddSingleton<MqttApplicationMessageRouter>();

      services.AddSingleton<IBeholderMqttClient, BeholderMqttClient>();

      return services;
    }
  }
}