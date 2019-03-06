using System;

namespace CodeBasics.Command.Implementation
{
  public class ActionValidator<T> : IInputValidator<T>, IOutputValidator<T>
  {
    private readonly Func<T, bool> validatorFunc;

    public ActionValidator(Func<T, bool> validatorFunc)
    {
      this.validatorFunc = validatorFunc;
    }

    public bool Validate(T value)
    {
      return validatorFunc.Invoke(value);
    }
  }
}
