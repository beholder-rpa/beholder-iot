namespace beholder_stalk_v2.Routing
{
  using beholder_stalk_v2.Models;
  using Microsoft.Extensions.DependencyInjection;
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text.RegularExpressions;

  public static class MqttRouteTableFactory
  {
    /// <summary>
    /// Given a list of assemblies, find all instances of MqttControllers and wire up routing for them. Instances of
    /// controllers must be decorated with an MqttController attribute.
    /// </summary>
    /// <param name="assembly">Assemblies to scan for routes</param>
    /// <returns></returns>
    internal static MqttRouteTable Create(IEnumerable<Assembly> assemblies, IServiceCollection serviceCollection, BeholderServiceInfo serviceInfo)
    {
      assemblies ??= new Assembly[] { Assembly.GetCallingAssembly() };

      // From the collection of assemblies, get all public instance methods decorated with the EventPatternAttribute within types decorated within types decorated with the MqttControllerAttribute
      var subscriptionCallbackMethods = assemblies.SelectMany(a => a.GetTypes())
          .Where(type => type.GetCustomAttribute<MqttControllerAttribute>(true) != null)
          .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
          .Where(m => m.GetCustomAttributes<EventPatternAttribute>(false).Any() && !m.IsSpecialName && !m.GetCustomAttributes<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(true).Any());

      var routeTable = Create(subscriptionCallbackMethods, serviceInfo);

      var addedTypes = new List<Type>();
      foreach (var route in routeTable)
      {
        foreach (var methodInfo in route.Value)
        {
          if (!addedTypes.Contains(methodInfo.DeclaringType))
          {
            serviceCollection.AddSingleton(methodInfo.DeclaringType);
            addedTypes.Add(methodInfo.DeclaringType);
          }
        }
      }

      return routeTable;
    }

    internal static MqttRouteTable Create(IEnumerable<MethodInfo> subscriptionCallbackMethods, BeholderServiceInfo serviceInfo)
    {
      var routeTable = new MqttRouteTable();

      foreach (var callbackMethod in subscriptionCallbackMethods)
      {
        var callbackMethodPatterns = callbackMethod.GetCustomAttributes<EventPatternAttribute>(inherit: false)
            .Select(a => a.Pattern)
            .ToArray();

        foreach (var patternWithTokens in callbackMethodPatterns)
        {
          // Replace tokens within the pattern
          var pattern = Regex.Replace(patternWithTokens, @"{\s*?hostname\s*?}", serviceInfo.HostName, RegexOptions.IgnoreCase | RegexOptions.Compiled);

          if (routeTable.ContainsKey(pattern))
          {
            routeTable[pattern].Add(callbackMethod);
          }
          else
          {
            routeTable.Add(pattern, new List<MethodInfo> { callbackMethod });
          }
        }
      }

      return routeTable;
    }
  }
}