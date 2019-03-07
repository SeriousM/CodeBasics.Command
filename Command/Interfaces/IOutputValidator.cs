using CodeBasics.Command.Implementation;

namespace CodeBasics.Command
{
  public interface IOutputValidator<in T>
  {
    ValidationStatus Validate(T value);
  }
}
