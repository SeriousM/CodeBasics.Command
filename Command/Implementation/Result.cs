using System.Collections.Generic;

namespace Command.Implementation
{
  public sealed class Result : IResult
  {
    private Result()
    {
    }

    internal static IResult PreValidationFail(string message)
    {
      return new Result { Messages = { message }, Status = Status.Fail };
    }

    internal static IResult PostValidationFail(string message)
    {
      return new Result { Messages = { message }, Status = Status.Fail };
    }

    internal static IResult ExecutionFail(string message)
    {
      return new Result { Messages = { message }, Status = Status.Fail };
    }
    
    internal static IResult Success()
    {
      return new Result { Status = Status.Success };
    }

    public IList<string> Messages { get; } = new List<string>();

    public Status Status { get; private set; } = Status.Fail;

    public bool WasSuccessful => Status == Status.Success;
  }

  public sealed class Result<TValue> : IResult<TValue>
  {
    private Result()
    {
    }

    internal static IResult<TValue> PreValidationFail(string message)
    {
      return new Result<TValue> { Messages = { message }, Status = Status.Fail };
    }

    internal static IResult<TValue> PostValidationFail(string message)
    {
      return new Result<TValue> { Messages = { message }, Status = Status.Fail };
    }

    internal static IResult<TValue> ExecutionFail(string message)
    {
      return new Result<TValue> { Messages = { message }, Status = Status.Fail };
    }
    
    internal static IResult<TValue> Success(TValue result)
    {
      return new Result<TValue> { Status = Status.Success, Value = result };
    }

    public IList<string> Messages { get; } = new List<string>();

    public Status Status { get; private set; } = Status.Fail;

    public TValue Value { get; private set; }

    public bool WasSuccessful => Status == Status.Success;
  }
}
