namespace Command
{
  public interface IValidator<in T>
  {
    bool Validate(T value);
  }
}