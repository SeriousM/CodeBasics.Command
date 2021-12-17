namespace CodeBasics.Command.Implementation
{
  public class ValidationStatus
  {
    public bool IsValid { get; }
    public string Message { get; }

    public string? UserMessage { get; }

    private ValidationStatus(bool isValid, string message = "", string? userMessage = null)
    {
      IsValid = isValid;
      Message = message;
      UserMessage = userMessage;
    }

    public static readonly ValidationStatus Valid = new ValidationStatus(true);

    public static ValidationStatus Invalid(string? message = null, string? userMessage = null) => new ValidationStatus(false, message, userMessage);
  }
}