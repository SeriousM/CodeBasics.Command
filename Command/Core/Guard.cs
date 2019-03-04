using System;
using System.Diagnostics.CodeAnalysis;

namespace Command.Core
{
  [ExcludeFromCodeCoverage]
  internal static class Guard
  {
    public static void Requires<TException>(bool condition, string message)
      where TException : Exception, new()
    {
      if (!condition)
      {
        return;
      }

      var exception = (TException)Activator.CreateInstance(typeof(TException), message);

      throw exception;
    }
  }
}
