using System.Threading.Tasks;

namespace Command
{
  public interface ICommandInOutAsync<TOut>
  {
    Task<IResult<TOut>> ExecuteAsync();
  }

  internal interface ICommandSetInput<in TIn>
  {
    void SetInputParameter(TIn input);
  }
}