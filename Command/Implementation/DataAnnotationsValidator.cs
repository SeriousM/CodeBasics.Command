using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeBasics.Command.Extensions;
using Microsoft.Extensions.Logging;

namespace CodeBasics.Command.Implementation
{
  public class DataAnnotationsValidator<T> : IValidator<T>, IInputValidator<T>, IOutputValidator<T>
  {
    public DataAnnotationsValidator(ILogger<DataAnnotationsValidator<T>> logger)
    {
      Logger = logger;
    }

    protected ILogger<DataAnnotationsValidator<T>> Logger { get; }

    public ValidationStatus Validate(T value)
    {
      var valueType = typeof(T);

      if (value == null)
      {
        var message = $"The value of type '{typeof(T)}' cannot be null.";
        Logger.LogError(message);

        return new ValidationStatus(false, message);
      }

      if (TypeExtensions.IsPrimitive(valueType)
       || valueType.GetTypeInfo().IsGenericType
       && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        return new ValidationStatus(true, $"Type '{typeof(T)}' is primitive and is considered as valid.");
      }

      var validationResults = new List<ValidationResult>();

      // TODO: support for FluidValidation?
      var valid = Validator.TryValidateObject(
        value,
        new ValidationContext(value, null, null),
        validationResults,
        true);

      var validationReport = reportValidationResult(valid, validationResults.ToArray());

      if (!valid)
      {
        return new ValidationStatus(false, validationReport);
      }

      var onValidateResult = OnValidate(value);
      if (!onValidateResult.IsValid)
      {
        Logger.LogError($"Validation of '{typeof(T)}' failed in {nameof(OnValidate)} method.");
      }

      return onValidateResult;
    }

    private string reportValidationResult(bool valid, ValidationResult[] validationResults)
    {
      var validationReportValid = new List<(string members, string status)>();
      var validationReportInvalid = new List<(string members, string status)>();
      foreach (var validationResult in validationResults)
      {
        if (validationResult.ErrorMessage is null)
        {
          validationReportValid.Add((string.Join(", ", validationResult.MemberNames ?? new[] { "<object>" }), "valid"));
        }
        else
        {
          validationReportInvalid.Add((string.Join(", ", validationResult.MemberNames ?? new[] { "<object>" }), validationResult.ErrorMessage));
        }
      }

      var sb = new StringBuilder();
      sb.AppendLine(valid
        ? $"Validation of '{typeof(T)}' succeeded:"
        : $"Validation of '{typeof(T)}' failed:");

      foreach (var (members, status) in validationReportInvalid.Concat(validationReportValid))
      {
        sb.AppendLine($"{members}: {status}");
      }

      if (valid)
      {
        Logger.LogDebug($"Validation of '{typeof(T)}' succeeded:\n{sb}");
      }
      else
      {
        Logger.LogError($"Validation of '{typeof(T)}' failed:\n{sb}");
      }

      return sb.ToString();
    }

    protected internal virtual ValidationStatus OnValidate(T value)
    {
      return new ValidationStatus(true);
    }
  }
}
