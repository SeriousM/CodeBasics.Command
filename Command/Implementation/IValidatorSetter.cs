namespace CodeBasics.Command.Implementation
{
  internal interface IValidatorSetter<out TIn, out TOut>
  {
    void SetInputValidator(IInputValidator<TIn> validator);

    void SetOutputValidator(IOutputValidator<TOut> validator);
  }
}