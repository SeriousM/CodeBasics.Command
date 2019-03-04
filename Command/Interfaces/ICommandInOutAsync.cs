using System.Threading.Tasks;

namespace Command
{
  public interface ICommandInOutAsync<in TIn, TOut>
  {
    Task<IResult<TOut>> ExecuteAsync();
  }

  internal interface ICommandSetInput<in TIn>
  {
    void SetInputParameter(TIn input);
  }
}