namespace CodeBasics.Command
{
  public interface IInputValidator<in T>
  {
    bool Validate(T value);
  }
}
