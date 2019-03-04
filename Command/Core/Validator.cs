using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Command.Extensions;
using Microsoft.Extensions.Logging;

namespace Command.Core
{
  public class Validator<T> : IValidator<T>
  {
    public Validator(ILogger<Validator<T>> logger)
    {
      Logger = logger;
    }

    protected ILogger<Validator<T>> Logger { get; }

    public bool Validate(T value)
    {
      var valueType = typeof(T);
      var isNull = value == null;
      if (TypeExtensions.IsPrimitive(valueType)
       || isNull
       && valueType.GetTypeInfo().IsGenericType
       && valueType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        return true;
      }

      if (isNull)
      {
        Logger.LogError("The value can not be null.");

        return false;
      }

      var validationResults = new List<ValidationResult>();
      var valid = Validator.TryValidateObject(
        value,
        new ValidationContext(value, null, null),
        validationResults,
        true);
      validationResults.ForEach(validationResult => Logger.LogInformation(validationResult.ToString()));

      return valid && OnValidate(value);
    }

    protected internal virtual bool OnValidate(T value)
    {
      return true;
    }
  }
}
