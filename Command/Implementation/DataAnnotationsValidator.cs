using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Command.Extensions;
using Microsoft.Extensions.Logging;

namespace Command.Implementation
{
  public class DataAnnotationsValidator<T> : IInputValidator<T>, IOutputValidator<T>
  {
    public DataAnnotationsValidator(ILogger<DataAnnotationsValidator<T>> logger)
    {
      Logger = logger;
    }

    protected ILogger<DataAnnotationsValidator<T>> Logger { get; }

    public bool Validate(T value)
    {
      var valueType = typeof(T);
      if (TypeExtensions.IsPrimitive(valueType)
       || valueType.GetTypeInfo().IsGenericType
       && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        return true;
      }

      if (value == null)
      {
        Logger.LogError("The value can not be null.");

        return false;
      }

      var validationResults = new List<ValidationResult>();

      // TODO: support for FluidValidation?
      var valid = Validator.TryValidateObject(
        value,
        new ValidationContext(value, null, null),
        validationResults,
        true);

      reportValidationResult(valid, validationResults.ToArray());

      if (!valid)
      {
        return false;
      }

      var onValidateResult = OnValidate(value);
      if (!onValidateResult)
      {
        Logger.LogError($"Validation of '{typeof(T)}' failed in {nameof(OnValidate)} method.");
      }

      return onValidateResult;
    }

    private void reportValidationResult(bool valid, ValidationResult[] validationResults)
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
    }

    protected internal virtual bool OnValidate(T value)
    {
      return true;
    }
  }
}
