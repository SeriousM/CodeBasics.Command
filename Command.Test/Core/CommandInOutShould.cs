namespace Command.Test.Core
{
    using System;

    using Command.Core;

    using Moq;

    using NUnit.Framework;

    using Shouldly;

    using Mock = Test.Mock;

    [TestFixture]
    public class CommandInOutShould
    {
        private readonly IResult IS_NOT_VALID = new Result();

        private readonly IResult IS_VALID = new Result { Status = Status.Success };

        [Test]
        public void ReturnFailResultWhenInputIsNotValid()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(
                inputValidationMock.Object,
                It.IsAny<Validator<string>>());
            var result = command.Execute(It.IsAny<string>());

            result.Status.ShouldBe(Status.Fail);
            inputValidationMock.Verify();
        }

        [Test]
        public void ReturnFailResultWhenOutputIsNotValid()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            Mock<IValidator<string>> outputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();
            outputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(
                m => m.Setup(c => c.OnExecute(It.IsAny<string>())).Returns(
                    new Result<string> { Status = Status.Success }),
                inputValidationMock.Object,
                outputValidationMock.Object);
            var result = command.Execute(It.IsAny<string>());

            result.Status.ShouldBe(Status.Fail);
            inputValidationMock.Verify();
            outputValidationMock.Verify();
        }

        [Test]
        public void ReturnSuccessResultWhenInputAndOutputIsValid()
        {
            Mock<IValidator<string>> inputValidationMock = new Mock<IValidator<string>>();
            Mock<IValidator<string>> outputValidationMock = new Mock<IValidator<string>>();
            inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();
            outputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(
                m => m.Setup(c => c.OnExecute(It.IsAny<string>())).Returns(
                    new Result<string> { Status = Status.Success }),
                inputValidationMock.Object,
                outputValidationMock.Object);
            var result = command.Execute(It.IsAny<string>());

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

            var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(
                m => m.Setup(c => c.OnExecute(It.IsAny<string>())).Returns((Result<string>)null),
                inputValidationMock.Object,
                outputValidationMock.Object);

            Should.Throw<NullReferenceException>(() => command.Execute(It.IsAny<string>())).Message
                  .ShouldBe("The result of OnExecute can not be null.");
        }
    }
}