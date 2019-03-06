using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CodeBasics.Command.Implementation
{
  public abstract class CommandInOutAsyncBase<TIn, TOut> : ICommandInOutAsync<TOut>, ISetSetCommandInput<TIn>, IValidatorSetter<TIn, TOut>, ISetCommandOptions
  {
    private readonly object syncRoot = new object();
    private bool executed;
    private TIn input;
    private CommandOptions options;

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

    void ISetSetCommandInput<TIn>.SetInputParameter(TIn value) => input = value;

    void ISetCommandOptions.SetCommandOptions(CommandOptions value) => options = value;

    void IValidatorSetter<TIn, TOut>.SetInputValidator(IInputValidator<TIn> validator) => InputValidator = validator;

    void IValidatorSetter<TIn, TOut>.SetOutputValidator(IOutputValidator<TOut> validator) => OutputValidator = validator;

    async Task<IResult<TOut>> ICommandInOutAsync<TOut>.ExecuteAsync()
    {
      lock (syncRoot)
      {
        if (executed)
        {
          throw new CommandExecutionException($"The command of type '{GetType().FullName}' was already executed.");
        }

        executed = true;
      }

      using (Logger.BeginScope($"Command Execution: {GetType().FullName}"))
      {
        if (InputValidator != null && !InputValidator.Validate(input))
        {
          var preValidationFail = Result<TOut>.PreValidationFail("Pre-Validation failed.");

          if (options.ThrowOnPreValidationFail)
          {
            throw new CommandExecutionException($"PreValidation failed for command '{GetType().FullName}'") { CommandResult = preValidationFail };
          }

          return preValidationFail;
        }

        var task = OnExecuteAsync(input) ?? throw new CommandExecutionException($"The resulting task of {nameof(OnExecuteAsync)} can not be null.");
        var commandExecutionResult = await task ?? throw new CommandExecutionException($"The result of {nameof(OnExecuteAsync)} can not be null.");

        if (commandExecutionResult.Status != CommandExecutionStatus.Success)
        {
          if (options.ThrowOnExecutionError)
          {
            throw new CommandExecutionException($"Execution failed for command '{GetType().FullName}'") { CommandResult = commandExecutionResult };
          }

          return commandExecutionResult;
        }

        if (OutputValidator != null && !OutputValidator.Validate(commandExecutionResult.Value))
        {
          var postValidationFail = Result<TOut>.PostValidationFail("Post-Validation failed.");

          if (options.ThrowOnPostValidationFail)
          {
            throw new CommandExecutionException($"PostValidation failed for command '{GetType().FullName}'") { CommandResult = postValidationFail };
          }

          return postValidationFail;
        }

        return commandExecutionResult;
      }
    }

    protected internal abstract Task<IResult<TOut>> OnExecuteAsync(TIn input);
  }
}
