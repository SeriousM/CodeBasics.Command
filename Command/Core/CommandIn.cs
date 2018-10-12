namespace Command.Core
{
    using System;

    public interface ICommandIn<in TIn>
    {
        IResult Execute(TIn input);
    }

    public abstract class CommandIn<TIn> : ICommandIn<TIn>
    {
        private readonly IValidator<TIn> inputValidator;

        protected CommandIn(IValidator<TIn> inputValidator)
        {
            this.inputValidator = inputValidator;
        }

        public IResult Execute(TIn input)
        {
            IResult result = this.inputValidator.Validate(input);
            if (result.Status == Status.Fail)
            {
                return result;
            }

            result = this.OnExecute(input);
            Guard.Requires<NullReferenceException>(result == null, "The result of OnExecute can not be null.");

            return result;
        }

        protected internal abstract IResult OnExecute(TIn input);
    }
}