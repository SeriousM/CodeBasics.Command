using System;
using System.Threading.Tasks;

namespace CodeBasics.Command.Implementation
{
  public abstract class CommandInOutAsyncBase<TIn, TOut> : ICommandInOutAsync<TOut>, ICommandSetInput<TIn>
  {
    private readonly IInputValidator<TIn> inputValidator;
    private readonly IOutputValidator<TOut> outputValidator;
    private readonly object syncRoot = new object();
    private bool executed;
    private TIn input;

    protected CommandInOutAsyncBase(
      IInputValidator<TIn> inputValidator,
      IOutputValidator<TOut> outputValidator)
    {
      this.inputValidator = inputValidator;
      this.outputValidator = outputValidator;
    }

    public async Task<IResult<TOut>> ExecuteAsync()
    {
      lock (syncRoot)
      {
        if (executed)
        {
          throw new InvalidOperationException($"The command of type '{GetType().FullName}' was already executed.");
        }

        executed = true;
      }

      if (inputValidator != null && !inputValidator.Validate(input))
      {
        return Result<TOut>.PreValidationFail("Pre-Validation failed.");
      }

      var task = OnExecuteAsync(input) ?? throw new NullReferenceException("The task of OnExecute can not be null.");
      var result = await task ?? throw new NullReferenceException("The result of OnExecute can not be null.");

      if (result.Status == Status.Success)
      {
        if (outputValidator != null && !outputValidator.Validate(result.Value))
        {
          result = Result<TOut>.PostValidationFail("Post-Validation failed.");
        }
      }

      return result;
    }

    public void SetInputParameter(TIn value)
    {
      input = value;
    }

    protected internal abstract Task<IResult<TOut>> OnExecuteAsync(TIn input);
  }
}
