using System.Threading.Tasks;

namespace Command.Core
{
  public interface ICommandInOutAsync<in TIn, TOut>
  {
    Task<IResult<TOut>> ExecuteAsync(TIn input);
  }
}