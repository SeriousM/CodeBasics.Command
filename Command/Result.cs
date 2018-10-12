namespace Command
{
    using System.Collections.Generic;

    public interface IResult<out TValue> : IResult
    {
        TValue Value { get; }
    }

    public sealed class Result<TValue> : IResult<TValue>
    {
        public Result()
            : this(new Result())
        {
        }

        public Result(IResult result)
        {
            this.Messages.AddRange(result.Messages);
            this.Status = result.Status;
        }

        public List<string> Messages { get; } = new List<string>();

        public Status Status { get; set; } = Status.Fail;

        public TValue Value { get; set; } = default(TValue);
    }

    public interface IResult
    {
        List<string> Messages { get; }

        Status Status { get; }
    }

    public sealed class Result : IResult
    {
        public List<string> Messages { get; } = new List<string>();

        public Status Status { get; set; } = Status.Fail;
    }

    public enum Status
    {
        Fail,

        Success
    }
}