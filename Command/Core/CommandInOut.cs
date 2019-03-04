using System;

namespace Command.Core
{
  public abstract class CommandInOut<TIn, TOut> : ICommandInOut<TIn, TOut>
  {
    private readonly IValidator<TIn> inputValidator;

    private readonly IValidator<TOut> outputValidator;

    protected CommandInOut(
      IValidator<TIn> inputValidator,
      IValidator<TOut> outputValidator)
    {
      this.inputValidator = inputValidator;
      this.outputValidator = outputValidator;
    }

    public IResult<TOut> Execute(TIn input)
    {
      var result = new Result<TOut>();
      if (inputValidator.Validate(input))
      {
        result = OnExecute(input);

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

    protected internal abstract Result<TOut> OnExecute(TIn input);
  }
}
