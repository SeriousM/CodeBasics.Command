using System.Threading.Tasks;

namespace CodeBasics.Command
{
  public interface ICommandInOutAsync<TOut>
  {
    /// <summary>
    /// Executes the command.
    /// Input and output will be validated regarding to the implementation.
    /// Depending on the <see cref="CommandOptions"/> the validation and execution may throw an exception.
    /// </summary>
    Task<IResult<TOut>> ExecuteAsync();

    /// <summary>
    /// Offers a way to check if a command may be executable.
    /// This is helpful to have check-up code in a single place.
    /// </summary>
    Task<bool> CouldExecuteAsync();
  }
}
