namespace Command.Core
{
    using System;

    public interface ICommandInOut<in TIn, out TOut>
    {
        IResult<TOut> Execute(TIn input);
    }

    public abstract class CommandInOut<TIn, TOut> : ICommandInOut<TIn, TOut>
    {
        private readonly IValidator<TIn> inputValidator;

        private readonly IValidator<TOut> outputValidator;

        protected CommandInOut(
            IValidator<TIn> inputValidator,
            IValidator<TOut> outputValidator)
        {
            this.inputValidator = inputValidator;
            this.outputValidator = outputValidator;
        }

        public IResult<TOut> Execute(TIn input)
        {
            var result = new Result<TOut>();
            if (this.inputValidator.Validate(input))
            {
                result = this.OnExecute(input);
                return CommandOut<TOut>.DefinedResult(this.outputValidator.Validate, result);
            }

            return result;
        }

        protected internal abstract Result<TOut> OnExecute(TIn input);
    }
}