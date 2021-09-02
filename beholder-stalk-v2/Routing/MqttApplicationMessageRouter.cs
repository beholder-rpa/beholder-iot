#nullable enable

namespace beholder_stalk_v2.Routing
{
  using beholder_stalk_v2.Models;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;
  using MQTTnet;
  using MQTTnet.Server;
  using System;
  using System.Reflection;
  using System.Text;
  using System.Text.Json;
  using System.Threading.Tasks;

  public sealed class MqttApplicationMessageRouter : IMqttServerApplicationMessageInterceptor
  {
    private readonly ILogger<MqttApplicationMessageRouter> logger;
    private readonly IServiceProvider applicationServices;
    private readonly ITypeActivatorCache typeActivator;

    public MqttApplicationMessageRouter(IServiceProvider applicationServices, ILogger<MqttApplicationMessageRouter> logger, MqttRouteTable routeTable, ITypeActivatorCache typeActivator)
    {
      this.applicationServices = applicationServices;
      this.logger = logger;
      this.typeActivator = typeActivator;
      RouteTable = routeTable;
    }

    public MqttRouteTable RouteTable
    {
      get;
    }

    public async Task InterceptApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
      var subscriptionMethods = RouteTable.GetTopicSubscriptions(e.ApplicationMessage);
      if (subscriptionMethods == null || subscriptionMethods.Length == 0)
      {
        // Route not found
        logger.LogDebug($"Skipping message because '{e.ApplicationMessage.Topic}' did not match any known routes.");
        return;
      }

      using var scope = applicationServices.CreateScope();
      foreach (var subscriptionMethod in subscriptionMethods)
      {
        Type? declaringType = subscriptionMethod.DeclaringType;

        if (declaringType == null)
        {
          throw new InvalidOperationException($"{subscriptionMethod} must have a declaring type.");
        }

        var classInstance = typeActivator.CreateInstance<object>(scope.ServiceProvider, declaringType);

        ParameterInfo[] parameters = subscriptionMethod.GetParameters();

        if (parameters.Length == 0)
        {
          await HandlerInvoker(subscriptionMethod, classInstance, null).ConfigureAwait(false);
        }
        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(MqttApplicationMessage))
        {
          try
          {
            await HandlerInvoker(subscriptionMethod, classInstance, new object[] { e.ApplicationMessage }).ConfigureAwait(false);
          }
          catch (ArgumentException ex)
          {
            logger.LogError(ex, $"Unable to match route parameters to all arguments. See inner exception for details.");
            throw;
          }
          catch (TargetInvocationException ex)
          {
            logger.LogError(ex.InnerException, $"Unhandled MQTT action exception. See inner exception for details.");
            throw;
          }
          catch (Exception ex)
          {
            logger.LogError(ex, "Unable to invoke Mqtt Action.  See inner exception for details.");
            throw;
          }
        }
      }
    }

    public async Task InterceptApplicationMessagePublishAsync(MqttApplicationMessageInterceptorContext context)
    {
      // Don't process messages sent from the server itself. This avoids footguns like a server failing to publish
      // a message because a route isn't found on a controller.
      if (context.ClientId == null)
      {
        return;
      }

      var subscriptionMethods = RouteTable.GetTopicSubscriptions(context.ApplicationMessage);
      if (subscriptionMethods == null || subscriptionMethods.Length == 0)
      {
        // Route not found
        logger.LogDebug($"Rejecting message publish because '{context.ApplicationMessage.Topic}' did not match any known routes.");
        return;
      }

      using var scope = applicationServices.CreateScope();
      foreach (var subscriptionMethod in subscriptionMethods)
      {
        Type? declaringType = subscriptionMethod.DeclaringType;

        if (declaringType == null)
        {
          throw new InvalidOperationException($"{subscriptionMethod} must have a declaring type.");
        }

        var classInstance = typeActivator.CreateInstance<object>(scope.ServiceProvider, declaringType);

        ParameterInfo[] parameters = subscriptionMethod.GetParameters();

        context.AcceptPublish = true;

        if (parameters.Length == 0)
        {
          await HandlerInvoker(subscriptionMethod, classInstance, null).ConfigureAwait(false);
        }
        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(MqttApplicationMessage))
        {
          try
          {
            await HandlerInvoker(subscriptionMethod, classInstance, new object[] { context.ApplicationMessage }).ConfigureAwait(false);
          }
          catch (ArgumentException ex)
          {
            logger.LogError(ex, $"Unable to match route parameters to all arguments. See inner exception for details.");
            throw;
          }
          catch (TargetInvocationException ex)
          {
            logger.LogError(ex.InnerException, $"Unhandled MQTT action exception. See inner exception for details.");
            throw;
          }
          catch (Exception ex)
          {
            logger.LogError(ex, "Unable to invoke Mqtt Action.  See inner exception for details.");
            throw;
          }
        }
        else if (parameters.Length == 1 && parameters[0].ParameterType == typeof(ICloudEvent<>))
        {
          object? cloudEvent = null;
          string requestJson = string.Empty;
          try
          {
            requestJson = Encoding.UTF8.GetString(context.ApplicationMessage.Payload, 0, context.ApplicationMessage.Payload.Length);
            cloudEvent = JsonSerializer.Deserialize(requestJson, parameters[0].ParameterType);
          }
          catch (Exception ex)
          {
            logger.LogError(ex, $"Unable to deserialize cloud event - {ex.Message}: {requestJson}");
            throw;
          }

          try
          {
            await HandlerInvoker(subscriptionMethod, classInstance, new object?[] { cloudEvent }).ConfigureAwait(false);
          }
          catch (ArgumentException ex)
          {
            logger.LogError(ex, $"Unable to match route parameters to all arguments. See inner exception for details.");
            throw;
          }
          catch (TargetInvocationException ex)
          {
            logger.LogError(ex.InnerException, $"Unhandled MQTT action exception. See inner exception for details.");
            throw;
          }
          catch (Exception ex)
          {
            logger.LogError(ex, "Unable to invoke Mqtt Action.  See inner exception for details.");
            throw;
          }
        }
      }
    }

    private static Task HandlerInvoker(MethodInfo method, object instance, object?[]? parameters)
    {
      if (method.ReturnType == typeof(void))
      {
        method.Invoke(instance, parameters);

        return Task.CompletedTask;
      }
      else if (method.ReturnType == typeof(Task))
      {
        var result = (Task?)method.Invoke(instance, parameters);

        if (result == null)
        {
          throw new NullReferenceException($"{method?.DeclaringType?.FullName}.{method?.Name} returned null instead of Task");
        }

        return result;
      }

      throw new InvalidOperationException($"Unsupported Action return type \"{method.ReturnType}\" on method {method?.DeclaringType?.FullName}.{method?.Name}. Only void and {nameof(Task)} are allowed.");
    }
  }
}