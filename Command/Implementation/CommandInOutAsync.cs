using System;
using System.Threading.Tasks;

namespace Command.Implementation
{
  public abstract class CommandInOutAsync<TIn, TOut> : ICommandInOutAsync<TIn, TOut>, ICommandSetInput<TIn>
  {
    private readonly IValidator<TIn> inputValidator;
    private readonly IValidator<TOut> outputValidator;
    private TIn input;

    protected CommandInOutAsync(
      IValidator<TIn> inputValidator,
      IValidator<TOut> outputValidator)
    {
      this.inputValidator = inputValidator;
      this.outputValidator = outputValidator;
    }

    public void SetInputParameter(TIn input)
    {
      this.input = input;
    }

    public async Task<IResult<TOut>> ExecuteAsync()
    {
      if (!inputValidator.Validate(input))
      {
        return Result<TOut>.PreValidationFail("Pre-Validation failed.");
      }

      var task = OnExecuteAsync(input);
        
      if (task is null)
      {
        throw new NullReferenceException("The task of OnExecute can not be null.");
      }

      var result = await task;

      if (result is null)
      {
        throw new NullReferenceException("The result of OnExecute can not be null.");
      }

      if (result.Status == Status.Success)
      {
        var valid = outputValidator.Validate(result.Value);
        if (!valid)
        {
          result = Result<TOut>.PostValidationFail("Post-Validation failed.");
        }
      }

      return result;

    }

    protected internal abstract Task<IResult<TOut>> OnExecuteAsync(TIn input);
  }
}
