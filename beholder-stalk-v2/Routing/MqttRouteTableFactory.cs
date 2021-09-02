namespace beholder_stalk_v2.Routing
{
  using beholder_stalk_v2.Models;
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text.RegularExpressions;

  public static class MqttRouteTableFactory
  {
    private static readonly ConcurrentDictionary<Key, MqttRouteTable> Cache = new ConcurrentDictionary<Key, MqttRouteTable>();

    /// <summary>
    /// Given a list of assemblies, find all instances of MqttControllers and wire up routing for them. Instances of
    /// controllers must inherit from MqttBaseController and be decorated with an MqttRoute attribute.
    /// </summary>
    /// <param name="assembly">Assemblies to scan for routes</param>
    /// <returns></returns>
    internal static MqttRouteTable Create(IEnumerable<Assembly> assemblies, BeholderServiceInfo serviceInfo)
    {
      assemblies ??= new Assembly[] { Assembly.GetCallingAssembly() };

      var key = new Key(assemblies.OrderBy(a => a.FullName).ToArray());

      if (Cache.TryGetValue(key, out var resolvedComponents))
      {
        return resolvedComponents;
      }

      // From the collection of assemblies, get all public instance methods decorated with the EventPatternAttribute within types decorated within types decorated with the MqttControllerAttribute
      var subscriptionCallbackMethods = assemblies.SelectMany(a => a.GetTypes())
          .Where(type => type.GetCustomAttribute<MqttControllerAttribute>(true) != null)
          .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public))
          .Where(m => m.GetCustomAttributes<EventPatternAttribute>(false).Any() && !m.IsSpecialName && !m.GetCustomAttributes<System.Runtime.CompilerServices.CompilerGeneratedAttribute>(true).Any());

      var routeTable = Create(subscriptionCallbackMethods, serviceInfo);

      Cache.TryAdd(key, routeTable);

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

    private readonly struct Key : IEquatable<Key>
    {
      public readonly Assembly[] Assemblies;

      public Key(Assembly[] assemblies)
      {
        Assemblies = assemblies;
      }

      public override bool Equals(object obj)
      {
        return obj is Key other ? base.Equals(other) : false;
      }

      public bool Equals(Key other)
      {
        if (Assemblies == null && other.Assemblies == null)
        {
          return true;
        }
        else if ((Assemblies == null) || (other.Assemblies == null))
        {
          return false;
        }
        else if (Assemblies.Length != other.Assemblies.Length)
        {
          return false;
        }

        for (var i = 0; i < Assemblies.Length; i++)
        {
          if (!Assemblies[i].Equals(other.Assemblies[i]))
          {
            return false;
          }
        }

        return true;
      }

      public override int GetHashCode()
      {
        var hash = new HashCode();

        if (Assemblies != null)
        {
          for (var i = 0; i < Assemblies.Length; i++)
          {
            hash.Add(Assemblies[i]);
          }
        }

        return hash.ToHashCode();
      }
    }
  }
}