namespace Command.Core
{
  public interface ICommandInOut<in TIn, out TOut>
  {
    IResult<TOut> Execute(TIn input);
  }

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

        return CommandOut<TOut>.DefinedResult(outputValidator.Validate, result);
      }

      return result;
    }

    protected internal abstract Result<TOut> OnExecute(TIn input);
  }
}
