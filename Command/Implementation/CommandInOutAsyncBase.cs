using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CodeBasics.Command.Implementation
{
  public abstract class CommandInOutAsyncBase<TIn, TOut> : ICommandInOutAsync<TOut>, ISetSetCommandInput<TIn>, IValidatorSetter<TIn, TOut>, ISetCommandOptions
  {
    private readonly object syncRoot = new object();
    private bool executed;
    private TIn input;

    protected CommandInOutAsyncBase(
      ILogger logger,
      IInputValidator<TIn> inputValidator,
      IOutputValidator<TOut> outputValidator)
    {
      Logger = logger;
      InputValidator = inputValidator;
      OutputValidator = outputValidator;
    }

    protected ILogger Logger { get; }
    protected internal CommandOptions Options { get; private set; }
    protected IInputValidator<TIn> InputValidator { get; private set; }
    protected IOutputValidator<TOut> OutputValidator { get; private set; }

    async Task<IResult<TOut>> ICommandInOutAsync<TOut>.ExecuteAsync()
    {
      lock (syncRoot)
      {
        if (executed)
        {
          var message = $"The command of type '{GetType().FullName}' was already executed.";
          Logger.LogError(message);

          throw new CommandExecutionException(message);
        }

        executed = true;
      }

      using (Logger.BeginScope($"Command Execution: {GetType().FullName}"))
      {
        var executeAsync = preValidation();
        if (!executeAsync.WasSuccessful)
        {
          return executeAsync;
        }

        var commandExecutionResult = await executeCommand();
        if (!commandExecutionResult.WasSuccessful)
        {
          return commandExecutionResult;
        }

        var result = postValidation(commandExecutionResult.Value);
        if (!result.WasSuccessful)
        {
          return result;
        }

        throwStatusAsExceptionIfEnabled(commandExecutionResult);

        return commandExecutionResult;
      }
    }

    private IResult<TOut> preValidation()
    {
      if (InputValidator == null)
      {
        Logger.LogDebug($"Pre-Validation of command '{GetType().FullName}' skipped because of missing input validator.");
        return Result.Success<TOut>(default);
      }

      var preValidationStatus = InputValidator.Validate(input);
      if (!preValidationStatus.IsValid)
      {
        var preValidationFail = Result<TOut>.PreValidationFail("Pre-Validation failed: " + preValidationStatus.Message);

        return preValidationFail;
      }

      Logger.LogDebug($"Pre-Validation of command '{GetType().FullName}' was successful.");

      return Result.Success<TOut>(default);
    }

    private async Task<IResult<TOut>> executeCommand()
    {
      var task = OnExecuteAsync(input) ?? throw new CommandExecutionException($"The resulting task of {nameof(OnExecuteAsync)} can not be null.");

      IResult<TOut> commandExecutionResult;
      try
      {
        commandExecutionResult = await task;
      }
      catch (Exception ex)
      {
        throw new CommandExecutionException($"Execution of command '{GetType().FullName}' resulted in an exception.", ex);
      }

      if (commandExecutionResult == null)
      {
        throw new CommandExecutionException($"The result of {nameof(OnExecuteAsync)} can not be null.");
      }

      if (commandExecutionResult.Status == CommandExecutionStatus.Success)
      {
        Logger.LogDebug($"Execution of command '{GetType().FullName}' was successful.");
      }

      return commandExecutionResult;
    }

    private IResult<TOut> postValidation(TOut commandExecutionResultValue)
    {
      if (OutputValidator == null)
      {
        Logger.LogDebug($"Post-Validation of command '{GetType().FullName}' skipped because of missing output validator.");
        return Result.Success<TOut>(default);
      }

      var postValidationStatus = OutputValidator.Validate(commandExecutionResultValue);
      if (!postValidationStatus.IsValid)
      {
        var postValidationFail = Result<TOut>.PostValidationFail("Post-Validation failed.");

        return postValidationFail;
      }

      Logger.LogDebug($"Post-Validation of command '{GetType().FullName}' was successful.");

      return Result.Success<TOut>(default);
    }

    private void throwStatusAsExceptionIfEnabled(IResult result)
    {
      if (result.Status == CommandExecutionStatus.PreValidationFailed)
      {
        if (Options.ThrowOnPreValidationFail)
        {
          var message = $"PreValidation failed for command '{GetType().FullName}':\n{result.Message}";
          Logger.LogError(message);

          throw new CommandExecutionException(message, result.Exception) { CommandResult = result };
        }
      }

      if (Options.ThrowOnExecutionError)
      {
        var message = $"Execution failed for command '{GetType().FullName}':\n{result.Message}";
        Logger.LogError(message);

        throw new CommandExecutionException(message, result.Exception) { CommandResult = result };
      }

      if (Options.ThrowOnPostValidationFail)
      {
        var message = $"PostValidation failed for command '{GetType().FullName}':\n{result.Message}";
        Logger.LogError(message);

        throw new CommandExecutionException(message, result.Exception) { CommandResult = result };
      }
    }

    void ISetCommandOptions.SetCommandOptions(CommandOptions value) => Options = value;

    void ISetSetCommandInput<TIn>.SetInputParameter(TIn value) => input = value;

    void IValidatorSetter<TIn, TOut>.SetInputValidator(IInputValidator<TIn> validator) => InputValidator = validator;

    void IValidatorSetter<TIn, TOut>.SetOutputValidator(IOutputValidator<TOut> validator) => OutputValidator = validator;

    protected internal abstract Task<IResult<TOut>> OnExecuteAsync(TIn input);
  }
}
