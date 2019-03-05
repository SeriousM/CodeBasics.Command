using System.Collections.Generic;

namespace CodeBasics.Command
{
  public interface IResult
  {
    IList<string> Messages { get; }

    CommandExecutionStatus Status { get; }

    bool WasSuccessful { get; }
  }

  public interface IResult<out TValue> : IResult
  {
    TValue Value { get; }
  }
}
