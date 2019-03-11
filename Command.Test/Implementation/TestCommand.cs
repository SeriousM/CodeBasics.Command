using System.Threading.Tasks;
using CodeBasics.Command.Implementation;
using Microsoft.Extensions.Logging;

namespace CodeBasics.Command.Test.Implementation
{
  public class TestCommand : CommandInOutAsyncBase<string, int>
  {
    public TestCommand(ILogger<TestCommand> logger, IInputValidator<string> inputValidator, IOutputValidator<int> outputValidator) : base(logger, inputValidator, outputValidator)
    {
    }

    public bool FailExecution { get; set; }

    protected internal override Task<IResult<int>> OnExecuteAsync(string input)
    {
      if (FailExecution)
      {
        return Task.FromResult(Result.ExecutionError<int>("I have to stop this execution."));
      }

      return Task.FromResult(Result.Success(int.Parse(input) + 1));
    }
  }
}
