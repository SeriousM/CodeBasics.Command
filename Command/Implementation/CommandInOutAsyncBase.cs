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
    private CommandOptions options;

    protected CommandInOutAsyncBase(
      ILogger logger,
      IInputValidator<TIn> inputValidator,
      IOutputValidator<TOut> outputValidator)
    {
      Logger = logger;
      InputValidator = inputValidator;
      OutputValidator = outputValidator;
    }

    protected IInputValidator<TIn> InputValidator { get; private set; }
    protected IOutputValidator<TOut> OutputValidator { get; private set; }
    protected ILogger Logger { get; }

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
        if (InputValidator != null)
        {
          var preValidationStatus = InputValidator.Validate(input);
          if (!preValidationStatus.IsValid)
          {
            var preValidationFail = Result<TOut>.PreValidationFail("Pre-Validation failed: " + preValidationStatus.Message);

            if (options.ThrowOnPreValidationFail)
            {
              var message = $"PreValidation failed for command '{GetType().FullName}':\n{preValidationStatus.Message}";
              Logger.LogError(message);

              throw new CommandExecutionException(message, preValidationFail.Exception) { CommandResult = preValidationFail };
            }

            return preValidationFail;
          }
        }

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

        if (commandExecutionResult.Status != CommandExecutionStatus.Success)
        {
          if (options.ThrowOnExecutionError)
          {
            var message = $"Execution failed for command '{GetType().FullName}':\n{commandExecutionResult.Message}";
            Logger.LogError(message);

            throw new CommandExecutionException(message, commandExecutionResult.Exception) { CommandResult = commandExecutionResult };
          }

          return commandExecutionResult;
        }

        {
          var message = $"Execution of command '{GetType().FullName}' was successful.";
          Logger.LogDebug(message);
        }

        if (OutputValidator != null)
        {
          var postValidationStatus = OutputValidator.Validate(commandExecutionResult.Value);
          if (!postValidationStatus.IsValid)
          {
            var postValidationFail = Result<TOut>.PostValidationFail("Post-Validation failed.");

            if (options.ThrowOnPostValidationFail)
            {
              var message = $"PostValidation failed for command '{GetType().FullName}':\n{postValidationStatus.Message}";
              Logger.LogError(message);

              throw new CommandExecutionException(message, postValidationFail.Exception) { CommandResult = postValidationFail };
            }

            return postValidationFail;
          }
        }

        if (commandExecutionResult.WasSuccessful)
        {
        }

        return commandExecutionResult;
      }
    }

    void ISetCommandOptions.SetCommandOptions(CommandOptions value)
    {
      options = value;
    }

    void ISetSetCommandInput<TIn>.SetInputParameter(TIn value)
    {
      input = value;
    }

    void IValidatorSetter<TIn, TOut>.SetInputValidator(IInputValidator<TIn> validator)
    {
      InputValidator = validator;
    }

    void IValidatorSetter<TIn, TOut>.SetOutputValidator(IOutputValidator<TOut> validator)
    {
      OutputValidator = validator;
    }

    protected internal abstract Task<IResult<TOut>> OnExecuteAsync(TIn input);
  }
}
