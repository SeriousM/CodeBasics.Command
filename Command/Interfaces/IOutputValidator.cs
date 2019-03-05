namespace Command
{
  public interface IOutputValidator<in T>
  {
    bool Validate(T value);
  }
}
