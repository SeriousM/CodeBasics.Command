using System.Threading.Tasks;
using CodeBasics.Command.Implementation;
using Microsoft.Extensions.Logging;

namespace CodeBasics.Command.Test.Implementation
{
  public class TestCommand : CommandInOutAsyncBase<int, int>
  {
    public TestCommand(ILogger<TestCommand> logger, IInputValidator<int> inputValidator, IOutputValidator<int> outputValidator) : base(logger, inputValidator, outputValidator)
    {
    }

    protected internal override Task<IResult<int>> OnExecuteAsync(int input)
    {
      return Task.FromResult(Result<int>.Success(++input));
    }
  }
}
