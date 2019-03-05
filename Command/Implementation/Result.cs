using System.Collections.Generic;

namespace CodeBasics.Command.Implementation
{
  public sealed class Result<TValue> : IResult<TValue>
  {
    private Result()
    {
    }

    public IList<string> Messages { get; } = new List<string>();

    public CommandExecutionStatus Status { get; private set; }

    public TValue Value { get; private set; }

    public bool WasSuccessful => Status == CommandExecutionStatus.Success;

    internal static IResult<TValue> PreValidationFail(string message)
    {
      return new Result<TValue> { Messages = { message }, Status = CommandExecutionStatus.PreValidationFailed };
    }

    internal static IResult<TValue> PostValidationFail(string message)
    {
      return new Result<TValue> { Messages = { message }, Status = CommandExecutionStatus.PostValidationFalied };
    }

    internal static IResult<TValue> Success(TValue result)
    {
      return new Result<TValue> { Status = CommandExecutionStatus.Success, Value = result };
    }
  }
}
