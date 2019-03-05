using System.Collections.Generic;

namespace CodeBasics.Command.Implementation
{
  public sealed class Result<TValue> : IResult<TValue>
  {
    private Result()
    {
    }

    public IList<string> Messages { get; } = new List<string>();

    public Status Status { get; private set; }

    public TValue Value { get; private set; }

    public bool WasSuccessful => Status == Status.Success;

    internal static IResult<TValue> PreValidationFail(string message)
    {
      return new Result<TValue> { Messages = { message }, Status = Status.PreValidationFailed };
    }

    internal static IResult<TValue> PostValidationFail(string message)
    {
      return new Result<TValue> { Messages = { message }, Status = Status.PostValidationFalied };
    }

    internal static IResult<TValue> Success(TValue result)
    {
      return new Result<TValue> { Status = Status.Success, Value = result };
    }
  }
}
