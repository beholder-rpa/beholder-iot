namespace beholder_stalk_v2
{
  using System;
  using System.Linq;
  using System.Reflection;

  public static class TypeExtensions
  {
    public static bool IsSubclassOfRawGeneric(this Type generic, Type toCheck)
    {
      while (toCheck != null && toCheck != typeof(object))
      {
        var typeInfo = toCheck.GetTypeInfo();
        var cur = typeInfo.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
        if (generic == cur)
        {
          return true;
        }
        toCheck = typeInfo.BaseType;
      }
      return false;
    }

    public static bool ImplementsRawGeneric(this Type generic, Type genericInterfaceType)
    {
      return generic.GetInterfaces()
        .Where(i => i.GetTypeInfo().IsGenericType)
        .Any(i => i.GetTypeInfo().GetGenericTypeDefinition() == genericInterfaceType);
    }
  }
}