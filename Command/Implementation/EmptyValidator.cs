namespace CodeBasics.Command.Implementation
{
  public class EmptyValidator<T> : IValidator<T>, IInputValidator<T>, IOutputValidator<T>
  {
    private EmptyValidator()
    {
    }

    public static EmptyValidator<T> Instance { get; } = new EmptyValidator<T>();

    public ValidationStatus Validate(T value)
    {
      return new ValidationStatus(true);
    }
  }
}
