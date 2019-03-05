using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CodeBasics.Command.Implementation
{
  public abstract class CommandInOutAsyncBase<TIn, TOut> : ICommandInOutAsync<TOut>, ICommandSetInput<TIn>
  {
    private readonly ILogger logger;
    private IInputValidator<TIn> inputValidator;
    private IOutputValidator<TOut> outputValidator;
    private readonly object syncRoot = new object();
    private bool executed;
    private TIn input;

    protected CommandInOutAsyncBase(
      ILogger logger,
      IInputValidator<TIn> inputValidator,
      IOutputValidator<TOut> outputValidator)
    {
      this.logger = logger;
      this.inputValidator = inputValidator;
      this.outputValidator = outputValidator;
    }

    internal void SetInputValidator(IInputValidator<TIn> validator)
    {
      inputValidator = validator;
    }

    internal void SetOutputValidator(IOutputValidator<TOut> validator)
    {
      outputValidator = validator;
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

      using (logger.BeginScope($"Command Execution: {GetType().FullName}"))
      {
        if (inputValidator != null && !inputValidator.Validate(input))
        {
          return Result<TOut>.PreValidationFail("Pre-Validation failed.");
        }

        var task = OnExecuteAsync(input) ?? throw new NullReferenceException("The task of OnExecute can not be null.");
        var result = await task ?? throw new NullReferenceException("The result of OnExecute can not be null.");

        if (result.Status == CommandExecutionStatus.Success)
        {
          if (outputValidator != null && !outputValidator.Validate(result.Value))
          {
            result = Result<TOut>.PostValidationFail("Post-Validation failed.");
          }
        }

        return result;
      }
    }

    public void SetInputParameter(TIn value)
    {
      input = value;
    }

    protected internal abstract Task<IResult<TOut>> OnExecuteAsync(TIn input);
  }
}
