using System;
using System.Threading.Tasks;

namespace CodeBasics.Command.Implementation
{
  public class ActionValidator<T> : IValidator<T>, IInputValidator<T>, IOutputValidator<T>
  {
    private readonly Func<T, Task<ValidationStatus>> validatorAsyncFunc;

    public ActionValidator(Func<T, Task<bool>> validatorAsyncFunc)
    {
      this.validatorAsyncFunc = async value => await validatorAsyncFunc(value) ? ValidationStatus.Valid : ValidationStatus.Invalid();
    }

    public ActionValidator(Func<T, Task<ValidationStatus>> validatorAsyncFunc)
    {
      this.validatorAsyncFunc = validatorAsyncFunc;
    }

    public ActionValidator(Func<T, bool> validatorFunc)
    {
      validatorAsyncFunc = value => Task.FromResult(validatorFunc(value) ? ValidationStatus.Valid : ValidationStatus.Invalid());
    }

    public ActionValidator(Func<T, ValidationStatus> validatorFunc)
    {
      validatorAsyncFunc = value => Task.FromResult(validatorFunc(value));
    }

    public Task<ValidationStatus> ValidateAsync(T value)
    {
      return validatorAsyncFunc(value);
    }
  }
}
