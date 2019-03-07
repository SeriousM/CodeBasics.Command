using CodeBasics.Command.Implementation;

namespace CodeBasics.Command
{
  public interface IInputValidator<in T>
  {
    ValidationStatus Validate(T value);
  }
}
