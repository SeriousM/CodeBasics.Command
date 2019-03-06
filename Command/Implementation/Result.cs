using System;

namespace CodeBasics.Command.Implementation
{
  public sealed class Result<TValue> : IResult<TValue>
  {
    private Result()
    {
    }

    public string Message { get; private set; }
    
    public Exception Exception { get; private set; }

    public CommandExecutionStatus Status { get; private set; }

    public TValue Value { get; private set; }

    public bool WasSuccessful => Status == CommandExecutionStatus.Success;

    internal static IResult<TValue> PreValidationFail(string message)
    {
      return new Result<TValue> { Message = message, Status = CommandExecutionStatus.PreValidationFailed };
    }

    internal static IResult<TValue> PostValidationFail(string message)
    {
      return new Result<TValue> { Message = message, Status = CommandExecutionStatus.PostValidationFalied };
    }

    public static IResult<TValue> ExecutionError(string message, Exception exception = null)
    {
      return new Result<TValue> { Status = CommandExecutionStatus.ExecutionError, Message = message, Exception = exception };
    }

    public static IResult<TValue> Success(TValue result)
    {
      return new Result<TValue> { Status = CommandExecutionStatus.Success, Value = result };
    }
  }
}
