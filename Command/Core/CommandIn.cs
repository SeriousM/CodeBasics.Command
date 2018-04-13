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
            IResult result = new Result();
            if (this.inputValidator.Validate(input))
            {
                result = this.OnExecute(input);
                Guard.Requires<NullReferenceException>(result == null, "The result of OnExecute can not be null.");
            }

            return result;
        }

        protected internal abstract IResult OnExecute(TIn input);
    }
}