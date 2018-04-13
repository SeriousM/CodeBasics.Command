namespace Command.Test.Core
{
    using System;

    using Command.Core;

    using Moq;

    using NUnit.Framework;

    using Shouldly;

    using Mock = Test.Mock;

    [TestFixture]
    public class CommandOutShould
    {
        private const bool IS_NOT_VALID = false;

        private const bool IS_VALID = true;

        [Test]
        public void ReturnFailResultWhenOutputIsNotValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate("not valid value")).Returns(IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandOut<string>>(
                m => m.Setup(c => c.OnExecute()).Returns(
                    new Result<string>
                        {
                            Status = Status.Success,
                            Value = "not valid value"
                        }),
                validationMock.Object);
            var result = command.Execute();

            result.Status.ShouldBe(Status.Fail);
            validationMock.Verify();
        }

        [Test]
        public void ReturnSuccessResultWhenInputIsValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate("valid value")).Returns(IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandOut<string>>(
                m => m.Setup(c => c.OnExecute()).Returns(
                    new Result<string>
                        {
                            Status = Status.Success,
                            Value = "valid value"
                        }),
                validationMock.Object);
            var result = command.Execute();

            result.Status.ShouldBe(Status.Success);
            validationMock.Verify();
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteReturnNullResult()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandOut<string>>(
                m => m.Setup(c => c.OnExecute())
                      .Returns((Result<string>)null),
                validationMock.Object);

            Should.Throw<NullReferenceException>(
                      () => command.Execute()).Message
                  .ShouldBe("The result of OnExecute can not be null.");
        }
    }
}