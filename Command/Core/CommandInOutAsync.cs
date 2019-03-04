using System;
using System.Threading.Tasks;

namespace Command.Core
{
  public abstract class CommandInOutAsync<TIn, TOut> : ICommandInOutAsync<TIn, TOut>
  {
    private readonly IValidator<TIn> inputValidator;

    private readonly IValidator<TOut> outputValidator;

    protected CommandInOutAsync(
      IValidator<TIn> inputValidator,
      IValidator<TOut> outputValidator)
    {
      this.inputValidator = inputValidator;
      this.outputValidator = outputValidator;
    }

    public async Task<IResult<TOut>> ExecuteAsync(TIn input)
    {
      var result = new Result<TOut>();
      if (inputValidator.Validate(input))
      {
        var task = OnExecuteAsync(input);
        
        if (task is null)
        {
          throw new NullReferenceException("The task of OnExecute can not be null.");
        }

        result = await task;

        if (result is null)
        {
          throw new NullReferenceException("The result of OnExecute can not be null.");
        }

        if (result.Status == Status.Success)
        {
          var valid = outputValidator.Validate(result.Value);
          result.Status = valid
            ? Status.Success
            : Status.Fail;
        }

        return result;
      }

      return result;
    }

    protected internal abstract Task<Result<TOut>> OnExecuteAsync(TIn input);
  }
}
