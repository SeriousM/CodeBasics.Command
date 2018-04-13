namespace Command.Core
{
    using System;
    using System.Threading.Tasks;

    public interface ICommandOutAsync<TOut>
    {
        Task<IResult<TOut>> ExecuteAsync();
    }

    public abstract class CommandOutAsync<TOut> : ICommandOutAsync<TOut>
    {
        private readonly IValidator<TOut> outputValidator;

        protected CommandOutAsync(
            IValidator<TOut> outputValidator)
        {
            this.outputValidator = outputValidator;
        }

        public async Task<IResult<TOut>> ExecuteAsync()
        {
            var task = this.OnExecuteAsync();
            Guard.Requires<NullReferenceException>(task == null, "The task of OnExecute can not be null.");
            var result = await task;
            return CommandOut<TOut>.DefinedResult(this.outputValidator.Validate, result);
        }

        protected internal abstract Task<Result<TOut>> OnExecuteAsync();
    }
}