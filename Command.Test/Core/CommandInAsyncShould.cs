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
    public class CommandInAsyncShould
    {
        private readonly IResult IS_NOT_VALID = new Result();

        private readonly IResult IS_VALID = new Result { Status = Status.Success };

        [Test]
        public async Task ReturnFailResultWhenInputIsNotValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInAsync<string>>(validationMock.Object);
            var result = await command.ExecuteAsync(It.IsAny<string>());

            result.Status.ShouldBe(Status.Fail);
            validationMock.Verify();
        }

        [Test]
        public async Task ReturnSuccessResultWhenInputIsValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInAsync<string>>(
                m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>()))
                      .ReturnsAsync(new Result { Status = Status.Success }),
                validationMock.Object);
            var result = await command.ExecuteAsync(It.IsAny<string>());

            result.Status.ShouldBe(Status.Success);
            validationMock.Verify();
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullResult()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInAsync<string>>(
                m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>())).ReturnsAsync((Result)null),
                validationMock.Object);

            Should.ThrowAsync<NullReferenceException>(() => command.ExecuteAsync(It.IsAny<string>())).Result.Message
                  .ShouldBe("The result of task on OnExecuteAsync can not be null.");
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullTask()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInAsync<string>>(
                m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>())).Returns((Task<Result>)null),
                validationMock.Object);

            Should.ThrowAsync<NullReferenceException>(() => command.ExecuteAsync(It.IsAny<string>())).Result.Message
                  .ShouldBe("The task of OnExecute can not be null.");
        }
    }
}