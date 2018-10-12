namespace Command.Core
{
    using System;

    public interface ICommandOut<out TOut>
    {
        IResult<TOut> Execute();
    }

    public abstract class CommandOut<TOut> : ICommandOut<TOut>
    {
        private readonly IValidator<TOut> outputValidator;

        protected CommandOut(IValidator<TOut> outputValidator)
        {
            this.outputValidator = outputValidator;
        }

        public IResult<TOut> Execute()
        {
            var result = this.OnExecute();
            return DefinedResult(this.outputValidator.Validate, result);
        }

        internal static IResult<TOut> DefinedResult(Func<TOut, IResult> validate, Result<TOut> result)
        {
            Guard.Requires<NullReferenceException>(result == null, "The result of OnExecute can not be null.");
            if (result.Status == Status.Success)
            {
                var validated = validate(result.Value);
                result.Status = validated.Status;
                result.Messages.AddRange(result.Messages);
            }

            return result;
        }

        protected internal abstract Result<TOut> OnExecute();
    }
}