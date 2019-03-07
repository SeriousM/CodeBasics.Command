using System;

namespace CodeBasics.Command.Implementation
{
  public class ActionValidator<T> : IValidator<T>, IInputValidator<T>, IOutputValidator<T>
  {
    private readonly Func<T, bool> validatorFunc;

    public ActionValidator(Func<T, bool> validatorFunc)
    {
      this.validatorFunc = validatorFunc;
    }

    public ValidationStatus Validate(T value)
    {
      return new ValidationStatus(validatorFunc.Invoke(value));
    }
  }
}
