using System;

namespace CodeBasics.Command
{
  public class CommandExecutionException : Exception
  {
    internal CommandExecutionException(string message) : base(message)
    {
    }

    internal CommandExecutionException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public IResult CommandResult { get; internal set; }
  }
}
