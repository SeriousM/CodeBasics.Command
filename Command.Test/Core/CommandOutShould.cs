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
        private readonly IResult IS_NOT_VALID = new Result();

        private readonly IResult IS_VALID = new Result { Status = Status.Success };

        [Test]
        public void ReturnFailResultWhenOutputIsNotValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate("not valid value")).Returns(this.IS_NOT_VALID).Verifiable();

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
            validationMock.Setup(v => v.Validate("valid value")).Returns(this.IS_VALID).Verifiable();

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
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

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