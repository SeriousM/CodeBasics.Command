using System;

namespace Command.Core
{
  public interface ICommandOut<out TOut>
  {
    IResult<TOut> Execute();
  }

  public abstract class CommandOut<TOut> : ICommandOut<TOut>
  {
    private readonly IValidator<TOut> outputValidator;

    protected CommandOut(
      IValidator<TOut> outputValidator)
    {
      this.outputValidator = outputValidator;
    }

    public IResult<TOut> Execute()
    {
      var result = OnExecute();

      return DefinedResult(outputValidator.Validate, result);
    }

    internal static IResult<TOut> DefinedResult(Func<TOut, bool> validate, Result<TOut> result)
    {
      Guard.Requires<NullReferenceException>(result == null, "The result of OnExecute can not be null.");
      if (result.Status == Status.Success)
      {
        var valid = validate(result.Value);
        result.Status = valid
          ? Status.Success
          : Status.Fail;
      }

      return result;
    }

    protected internal abstract Result<TOut> OnExecute();
  }
}
