using System;
using System.Threading.Tasks;

namespace Command.Core
{
  public interface ICommandInAsync<in TIn>
  {
    Task<IResult> ExecuteAsync(TIn input);
  }

  public abstract class CommandInAsync<TIn> : ICommandInAsync<TIn>
  {
    private readonly IValidator<TIn> inputValidator;

    protected CommandInAsync(IValidator<TIn> inputValidator)
    {
      this.inputValidator = inputValidator;
    }

    public async Task<IResult> ExecuteAsync(TIn input)
    {
      var result = new Result();
      if (inputValidator.Validate(input))
      {
        var task = OnExecuteAsync(input);
        Guard.Requires<NullReferenceException>(task == null, "The task of OnExecute can not be null.");
        result = await task;
        Guard.Requires<NullReferenceException>(
          result == null,
          "The result of task on OnExecuteAsync can not be null.");
      }

      return result;
    }

    protected internal abstract Task<Result> OnExecuteAsync(TIn input);
  }
}
