namespace Command
{
    using System.Collections.Generic;

    public interface IResult<out TValue> : IResult
    {
        TValue Value { get; }
    }

    public sealed class Result<TValue> : IResult<TValue>
    {
        public IList<string> Messages { get; } = new List<string>();

        public Status Status { get; set; } = Status.Fail;

        public TValue Value { get; set; } = default(TValue);
    }

    public interface IResult
    {
        IList<string> Messages { get; }

        Status Status { get; }
    }

    public sealed class Result : IResult
    {
        public IList<string> Messages { get; } = new List<string>();

        public Status Status { get; set; } = Status.Fail;
    }

    public enum Status
    {
        Fail,

        Success
    }
}