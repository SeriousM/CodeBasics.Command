namespace Command.Test.Core
{
    using System;
    using System.Threading.Tasks;

    using Command.Core;

    using Moq;

    using NUnit.Framework;

    using Shouldly;

    using Mock = Test.Mock;

    [TestFixture]
    public class CommandOutAsyncShould
    {
        private readonly IResult IS_NOT_VALID = new Result();

        private readonly IResult IS_VALID = new Result { Status = Status.Success };

        [Test]
        public async Task ReturnFailResultWhenOutputIsNotValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate("not valid value")).Returns(this.IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
                m => m.Setup(c => c.OnExecuteAsync()).ReturnsAsync(
                    new Result<string>
                        {
                            Status = Status.Success,
                            Value = "not valid value"
                        }),
                validationMock.Object);
            var result = await command.ExecuteAsync();

            result.Status.ShouldBe(Status.Fail);
            validationMock.Verify();
        }

        [Test]
        public async Task ReturnSuccessResultWhenInputIsValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate("valid value")).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
                m => m.Setup(c => c.OnExecuteAsync()).ReturnsAsync(
                    new Result<string>
                        {
                            Status = Status.Success,
                            Value = "valid value"
                        }),
                validationMock.Object);
            var result = await command.ExecuteAsync();

            result.Status.ShouldBe(Status.Success);
            validationMock.Verify();
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullResult()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
                m => m.Setup(c => c.OnExecuteAsync())
                      .ReturnsAsync((Result<string>)null),
                validationMock.Object);

            Should.Throw<NullReferenceException>(
                      () => command.ExecuteAsync()).Message
                  .ShouldBe("The result of OnExecute can not be null.");
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullTask()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
                m => m.Setup(c => c.OnExecuteAsync())
                      .Returns((Task<Result<string>>)null),
                validationMock.Object);

            Should.ThrowAsync<NullReferenceException>(
                      () =>
                          command.ExecuteAsync()).Result.Message
                  .ShouldBe("The task of OnExecute can not be null.");
        }
    }
}