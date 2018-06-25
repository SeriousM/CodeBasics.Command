namespace Command.Test
{
    using NUnit.Framework;

    using Shouldly;

    [TestFixture]
    public class ResultShould
    {
        [Test]
        public void ReturnFailStatusWhenResultIsConstruct()
        {
            var result = new Result<object>();
            result.Status.ShouldBe(Status.Fail);
        }

        [Test]
        public void ReturnInstanceOfMessagesWhenResultIsConstruct()
        {
            var result = new Result<object>();
            result.Messages.ShouldNotBeNull();
        }
    }
}
