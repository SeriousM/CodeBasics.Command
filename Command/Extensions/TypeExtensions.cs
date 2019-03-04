using System;
using System.Reflection;

namespace Command.Extensions
{
  internal static class TypeExtensions
  {
    public static bool IsPrimitive(this Type that)
    {
      return that.GetTypeInfo().IsPrimitive
          || that == typeof(string)
          || that == typeof(decimal)
          || that == typeof(DateTime)
          || that == typeof(Guid);
    }
  }
}
