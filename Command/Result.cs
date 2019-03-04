using System.Collections.Generic;

namespace Command
{
  public sealed class Result : IResult
  {
    public IList<string> Messages { get; } = new List<string>();

    public Status Status { get; set; } = Status.Fail;
  }

  public sealed class Result<TValue> : IResult<TValue>
  {
    public IList<string> Messages { get; } = new List<string>();

    public Status Status { get; set; } = Status.Fail;

    public TValue Value { get; set; } = default(TValue);
  }
}
