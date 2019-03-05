using System.Threading.Tasks;
using Command.Implementation;

namespace Command.Test.Implementation
{
  public class TestCommand : CommandInOutAsyncBase<int, int>
  {
    public TestCommand(IInputValidator<int> inputValidator, IOutputValidator<int> outputValidator) : base(inputValidator, outputValidator)
    {
    }

    protected internal override Task<IResult<int>> OnExecuteAsync(int input)
    {
      return Task.FromResult(Result<int>.Success(++input));
    }
  }
}
