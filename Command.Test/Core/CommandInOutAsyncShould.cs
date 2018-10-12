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
    public class CommandInOutAsyncShould
    {
        private readonly IResult IS_NOT_VALID = new Result();

        private readonly IResult IS_VALID = new Result { Status = Status.Success };

        [Test]
        public async Task ReturnFailResultWhenInputIsNotValid()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOutAsync<string, string>>(
                inputValidationMock.Object,
                It.IsAny<Validator<string>>());
            var result = await command.ExecuteAsync(It.IsAny<string>());

            result.Status.ShouldBe(Status.Fail);
            inputValidationMock.Verify();
        }

        [Test]
        public async Task ReturnFailResultWhenOutputIsNotValid()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            Mock<IValidator<string>> outputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();
            outputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOutAsync<string, string>>(
                m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>())).ReturnsAsync(
                    new Result<string> { Status = Status.Success }),
                inputValidationMock.Object,
                outputValidationMock.Object);
            var result = await command.ExecuteAsync(It.IsAny<string>());

            result.Status.ShouldBe(Status.Fail);
            inputValidationMock.Verify();
            outputValidationMock.Verify();
        }

        [Test]
        public async Task ReturnSuccessResultWhenInputAndOutputIsValid()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            Mock<IValidator<string>> outputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();
            outputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOutAsync<string, string>>(
                m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>())).ReturnsAsync(
                    new Result<string> { Status = Status.Success }),
                inputValidationMock.Object,
                outputValidationMock.Object);
            var result = await command.ExecuteAsync(It.IsAny<string>());

            result.Status.ShouldBe(Status.Success);
            inputValidationMock.Verify();
            outputValidationMock.Verify();
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteReturnNullResult()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            Mock<IValidator<string>> outputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOutAsync<string, string>>(
                m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>())).ReturnsAsync((Result<string>)null),
                inputValidationMock.Object,
                outputValidationMock.Object);

            Should.Throw<NullReferenceException>(() => command.ExecuteAsync(It.IsAny<string>())).Message
                  .ShouldBe("The result of OnExecute can not be null.");
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteReturnNullTask()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            Mock<IValidator<string>> outputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOutAsync<string, string>>(
                m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>())).Returns((Task<Result<string>>)null),
                inputValidationMock.Object,
                outputValidationMock.Object);

            Should.Throw<NullReferenceException>(() => command.ExecuteAsync(It.IsAny<string>())).Message
                  .ShouldBe("The task of OnExecute can not be null.");
        }
    }
}