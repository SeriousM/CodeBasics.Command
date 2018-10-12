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
        IResult Validate(T value);
    }

    public class Validator<T> : IValidator<T>
    {
        public Validator(ILogger<Validator<T>> logger)
        {
            this.Logger = logger;
        }

        protected ILogger<Validator<T>> Logger { get; }

        public IResult Validate(T value)
        {
            var result = new Result { Status = Status.Fail };
            Type valueType = typeof(T);
            bool isNull = value == null;
            if (valueType.IsPrimitive() || (isNull && valueType.GetTypeInfo().IsGenericType
                                                   && valueType.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                result.Status = Status.Success;
                return result;
            }

            if (isNull)
            {
                var message = "The value can not be null.";
                this.Logger.LogError(message);
                result.Messages.Add(message);
                return result;
            }

            var validationResults = new List<ValidationResult>();
            bool valid = Validator.TryValidateObject(
                value,
                new ValidationContext(value, null, null),
                validationResults,
                true);

            if (!valid)
            {
                validationResults.ForEach(
                    validationResult =>
                        {
                            var message = validationResult.ToString();
                            result.Messages.Add(message);
                            this.Logger.LogInformation(message);
                        });
                result.Status = Status.Fail;
                return result;
            }

            return this.OnValidate(value);
        }

        protected internal virtual IResult OnValidate(T value)
        {
            return new Result { Status = Status.Success };
        }
    }
}