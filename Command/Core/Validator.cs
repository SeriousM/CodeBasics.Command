namespace Command.Core
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Reflection;

    using Extensions;

    using Microsoft.Extensions.Logging;

    public interface IValidator<in T>
    {
        bool Validate(T value);
    }

    public class Validator<T> : IValidator<T>
    {
        public Validator(ILogger<Validator<T>> logger)
        {
            this.Logger = logger;
        }

        protected ILogger<Validator<T>> Logger { get; }

        public bool Validate(T value)
        {
            Type valueType = typeof(T);
            bool isNull = value == null;
            if (valueType.IsPrimitive()
                || (isNull && valueType.GetTypeInfo().IsGenericType
                           && valueType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return true;
            }

            if (isNull)
            {
                this.Logger.LogError("The value can not be null.");
                return false;
            }

            var validationResults = new List<ValidationResult>();
            bool valid = Validator.TryValidateObject(
                value,
                new ValidationContext(value, null, null),
                validationResults,
                true);
            validationResults.ForEach(validationResult => this.Logger.LogInformation(validationResult.ToString()));
            return valid && this.OnValidate(value);
        }

        protected internal virtual bool OnValidate(T value)
        {
            return true;
        }
    }
}