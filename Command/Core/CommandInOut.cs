namespace Command.Core
{
    public interface ICommandInOut<in TIn, out TOut>
    {
        IResult<TOut> Execute(TIn input);
    }

    public abstract class CommandInOut<TIn, TOut> : ICommandInOut<TIn, TOut>
    {
        private readonly IValidator<TIn> inputValidator;

        private readonly IValidator<TOut> outputValidator;

        protected CommandInOut(IValidator<TIn> inputValidator, IValidator<TOut> outputValidator)
        {
            this.inputValidator = inputValidator;
            this.outputValidator = outputValidator;
        }

        public IResult<TOut> Execute(TIn input)
        {
            var result = this.inputValidator.Validate(input);
            result = new Result<TOut>(result);
            if (result.Status == Status.Fail)
            {
                return new Result<TOut>(result);
            }

            result = this.OnExecute(input);
            return CommandOut<TOut>.DefinedResult(this.outputValidator.Validate, (Result<TOut>)result);
        }

        protected internal abstract Result<TOut> OnExecute(TIn input);
    }
}