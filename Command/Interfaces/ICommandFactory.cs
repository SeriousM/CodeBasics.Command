namespace CodeBasics.Command
{
  public interface ICommandFactory
  {
    ICommandInOutAsync<TOut> CreateAsync<TCommand, TIn, TOut>(TIn input) where TCommand : ICommandInOutAsync<TOut>;
  }
}
