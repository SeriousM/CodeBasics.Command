namespace Command
{
  public interface ICommandFactory
  {
    ICommandInOutAsync<TIn, TOut> CreateAsync<TCommand, TIn, TOut>(TIn input) where TCommand : ICommandInOutAsync<TIn, TOut>;
  }
}
