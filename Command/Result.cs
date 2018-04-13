namespace Command
{
    public interface IResult<out TValue>
    {
        Status Status { get; }

        TValue Value { get; }
    }

    public sealed class Result<TValue> : IResult<TValue>
    {
        public Status Status { get; set; } = Status.Fail;

        public TValue Value { get; set; }
    }

    public interface IResult
    {
        Status Status { get; }
    }

    public sealed class Result : IResult
    {

        public Status Status { get; set; } = Status.Fail;
    }

    public enum Status
    {
        Fail,

        Success
    }
}