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

        protected CommandOut(
            IValidator<TOut> outputValidator)
        {
            this.outputValidator = outputValidator;
        }

        internal static IResult<TOut> DefinedResult(Func<TOut, bool> validate, Result<TOut> result)
        {
            Guard.Requires<NullReferenceException>(result == null, "The result of OnExecute can not be null.");
            if (result.Status == Status.Success)
            {
                var valid = validate(result.Value);
                result.Status = valid ? Status.Success : Status.Fail;
            }

            return result;
        }

        public IResult<TOut> Execute()
        {
            var result = this.OnExecute();
            return DefinedResult(this.outputValidator.Validate, result);
        }

        protected internal abstract Result<TOut> OnExecute();
    }
}