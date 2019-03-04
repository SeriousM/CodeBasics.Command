using System.Collections.Generic;

namespace Command
{
  public interface IResult
  {
    IList<string> Messages { get; }

    Status Status { get; }
  }

  public interface IResult<out TValue> : IResult
  {
    TValue Value { get; }
  }
}