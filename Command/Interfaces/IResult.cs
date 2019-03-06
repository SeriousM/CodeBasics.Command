using System;

namespace CodeBasics.Command
{
  public interface IResult
  {
    string Message { get; }
    
    Exception Exception { get; }

    CommandExecutionStatus Status { get; }

    bool WasSuccessful { get; }
  }

  public interface IResult<out TValue> : IResult
  {
    TValue Value { get; }
  }
}
