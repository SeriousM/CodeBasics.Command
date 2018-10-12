namespace Command.Core
{
    using System;
    using System.Threading.Tasks;

    public interface ICommandInAsync<in TIn>
    {
        Task<IResult> ExecuteAsync(TIn input);
    }

    public abstract class CommandInAsync<TIn> : ICommandInAsync<TIn>
    {
        private readonly IValidator<TIn> inputValidator;

        protected CommandInAsync(IValidator<TIn> inputValidator)
        {
            this.inputValidator = inputValidator;
        }

        public async Task<IResult> ExecuteAsync(TIn input)
        {
            var result = this.inputValidator.Validate(input);
            if (result.Status == Status.Fail)
            {
                return result;
            }

            var task = this.OnExecuteAsync(input);
            Guard.Requires<NullReferenceException>(task == null, "The task of OnExecute can not be null.");
            result = await task;
            Guard.Requires<NullReferenceException>(
                result == null,
                "The result of task on OnExecuteAsync can not be null.");

            return result;
        }

        protected internal abstract Task<Result> OnExecuteAsync(TIn input);
    }
}