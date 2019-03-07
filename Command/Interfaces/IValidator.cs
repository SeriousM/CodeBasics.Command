using CodeBasics.Command.Implementation;

namespace CodeBasics.Command
{
  public interface IValidator<in T>
  {
    ValidationStatus Validate(T value);
  }
}