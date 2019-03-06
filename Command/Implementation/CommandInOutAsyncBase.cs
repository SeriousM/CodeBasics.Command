using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CodeBasics.Command.Implementation
{
  public abstract class CommandInOutAsyncBase<TIn, TOut> : ICommandInOutAsync<TOut>, ICommandSetInput<TIn>, IValidatorSetter<TIn, TOut>
  {
    private readonly object syncRoot = new object();
    private bool executed;
    private TIn input;

    protected IInputValidator<TIn> InputValidator { get; private set; }
    protected IOutputValidator<TOut> OutputValidator { get; private set; }
    protected ILogger Logger { get; }

    protected CommandInOutAsyncBase(
      ILogger logger,
      IInputValidator<TIn> inputValidator,
      IOutputValidator<TOut> outputValidator)
    {
      Logger = logger;
      InputValidator = inputValidator;
      OutputValidator = outputValidator;
    }

    void IValidatorSetter<TIn, TOut>.SetInputValidator(IInputValidator<TIn> validator)
    {
      InputValidator = validator;
    }

    void IValidatorSetter<TIn, TOut>.SetOutputValidator(IOutputValidator<TOut> validator)
    {
      OutputValidator = validator;
    }

    async Task<IResult<TOut>> ICommandInOutAsync<TOut>.ExecuteAsync()
    {
      lock (syncRoot)
      {
        if (executed)
        {
          throw new InvalidOperationException($"The command of type '{GetType().FullName}' was already executed.");
        }

        executed = true;
      }

      using (Logger.BeginScope($"Command Execution: {GetType().FullName}"))
      {
        if (InputValidator != null && !InputValidator.Validate(input))
        {
          return Result<TOut>.PreValidationFail("Pre-Validation failed.");
        }

        var task = OnExecuteAsync(input) ?? throw new NullReferenceException("The task of OnExecute can not be null.");
        var result = await task ?? throw new NullReferenceException("The result of OnExecute can not be null.");

        if (result.Status == CommandExecutionStatus.Success)
        {
          if (OutputValidator != null && !OutputValidator.Validate(result.Value))
          {
            result = Result<TOut>.PostValidationFail("Post-Validation failed.");
          }
        }

        return result;
      }
    }

    void ICommandSetInput<TIn>.SetInputParameter(TIn value)
    {
      input = value;
    }

    protected internal abstract Task<IResult<TOut>> OnExecuteAsync(TIn input);
  }
}
