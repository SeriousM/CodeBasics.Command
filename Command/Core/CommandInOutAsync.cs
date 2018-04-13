namespace Command.Core
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.Extensions.Logging;

    public interface ICommandInOutAsync<in TIn, TOut>
    {
        Task<IResult<TOut>> ExecuteAsync(TIn input);
    }

    public abstract class CommandInOutAsync<TIn, TOut> : ICommandInOutAsync<TIn, TOut>
    {
        private readonly IValidator<TIn> inputValidator;

        private readonly IValidator<TOut> outputValidator;

        protected CommandInOutAsync(
            IValidator<TIn> inputValidator,
            IValidator<TOut> outputValidator)
        {
            this.inputValidator = inputValidator;
            this.outputValidator = outputValidator;
        }
        
        public async Task<IResult<TOut>> ExecuteAsync(TIn input)
        {
            var result = new Result<TOut>();
            if (this.inputValidator.Validate(input))
            {
                var task = this.OnExecuteAsync(input);
                Guard.Requires<NullReferenceException>(task == null, "The task of OnExecute can not be null.");
                result = await task;
                return CommandOut<TOut>.DefinedResult(this.outputValidator.Validate, result);
            }

            return result;
        }

        protected internal abstract Task<Result<TOut>> OnExecuteAsync(TIn input);
    }
}