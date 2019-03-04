namespace Command.Core
{
  public interface ICommandInOut<in TIn, out TOut>
  {
    IResult<TOut> Execute(TIn input);
  }
}