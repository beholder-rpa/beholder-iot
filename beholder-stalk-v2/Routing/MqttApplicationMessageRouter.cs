#nullable enable

namespace beholder_stalk_v2.Routing
{
  using beholder_stalk_v2.Models;
  using Microsoft.Extensions.DependencyInjection;
  using Microsoft.Extensions.Logging;
  using MQTTnet;
  using System;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Text.Json;
  using System.Threading.Tasks;

  public sealed class MqttApplicationMessageRouter
  {
    private readonly IServiceProvider _applicationServices;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ILogger<MqttApplicationMessageRouter> _logger;
    private readonly ITypeActivatorCache _typeActivator;

    public MqttApplicationMessageRouter(IServiceProvider applicationServices, JsonSerializerOptions serializerOptions, ILogger<MqttApplicationMessageRouter> logger, MqttRouteTable routeTable, ITypeActivatorCache typeActivator)
    {
      _applicationServices = applicationServices ?? throw new ArgumentNullException(nameof(applicationServices));
      _serializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _typeActivator = typeActivator ?? throw new ArgumentNullException(nameof(typeActivator));
      RouteTable = routeTable ?? throw new ArgumentNullException(nameof(routeTable));
    }

    public MqttRouteTable RouteTable
    {
      get;
    }

    public async Task InterceptApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs e)
    {
      _logger.LogTrace($"Processing message with topic '{e.ApplicationMessage.Topic}'.");

      var subscriptionMethods = RouteTable.GetTopicSubscriptions(e.ApplicationMessage);
      if (subscriptionMethods == null || subscriptionMethods.Length == 0)
      {
        // Route not found
        _logger.LogDebug($"Skipping message because '{e.ApplicationMessage.Topic}' did not match any known routes.");
        return;
      }

      using var scope = _applicationServices.CreateScope();
      foreach (var subscriptionMethod in subscriptionMethods)
      {
        Type? declaringType = subscriptionMethod.DeclaringType;

        if (declaringType == null)
        {
          throw new InvalidOperationException($"{subscriptionMethod} must have a declaring type.");
        }

        var classInstance = _typeActivator.CreateInstance<object>(scope.ServiceProvider, declaringType);

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
            _logger.LogError(ex, $"Unable to match route parameters to all arguments. See inner exception for details.");
            throw;
          }
          catch (TargetInvocationException ex)
          {
            _logger.LogError(ex.InnerException, $"Unhandled MQTT action exception. See inner exception for details.");
            throw;
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, "Unable to invoke Mqtt Action.  See inner exception for details.");
            throw;
          }
        }
        else if (parameters.Length == 1 && typeof(ICloudEvent).IsAssignableFrom(parameters[0].ParameterType))
        {
          object? cloudEvent = null;
          string requestJson = string.Empty;

          var cloudEventType = typeof(CloudEvent);
          if (parameters[0].ParameterType.IsGenericType && parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(ICloudEvent<>))
          {
            var cloudEventDataType = parameters[0].ParameterType.GetGenericArguments().First();
            cloudEventType = typeof(CloudEvent<>).MakeGenericType(cloudEventDataType);
          }

          try
          {
            requestJson = Encoding.UTF8.GetString(e.ApplicationMessage.Payload, 0, e.ApplicationMessage.Payload.Length);
            cloudEvent = JsonSerializer.Deserialize(requestJson, cloudEventType, _serializerOptions);
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, $"Unable to deserialize cloud event - {ex.Message}: {requestJson}");
            throw;
          }

          try
          {
            _logger.LogTrace($"Invoking method '{subscriptionMethod}' on type {declaringType} with ICloudEvent<T> - {requestJson}'.");
            await HandlerInvoker(subscriptionMethod, classInstance, new object?[] { cloudEvent }).ConfigureAwait(false);
          }
          catch (ArgumentException ex)
          {
            _logger.LogError(ex, $"Unable to match route parameters to all arguClass.csments. See inner exception for details.");
            throw;
          }
          catch (TargetInvocationException ex)
          {
            _logger.LogError(ex.InnerException, $"Unhandled MQTT action exception. See inner exception for details.");
            throw;
          }
          catch (Exception ex)
          {
            _logger.LogError(ex, "Unable to invoke Mqtt Action.  See inner exception for details.");
            throw;
          }
        }
        else
        {
          _logger.LogWarning($"Unable to invoke Handler Function on type {declaringType}. Parameter Count: {parameters.Length} Parameter Types: {string.Join(",", parameters.Select(p => p.ParameterType.ToString()))}.");
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