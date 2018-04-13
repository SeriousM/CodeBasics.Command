namespace Command.Test.Core
{
    using System;

    using Command.Core;

    using Moq;

    using NUnit.Framework;

    using Shouldly;

    using Mock = Test.Mock;

    [TestFixture]
    public class CommandInShould
    {
        private const bool IS_NOT_VALID = false;

        private const bool IS_VALID = true;

        [Test]
        public void ReturnFailResultWhenInputIsNotValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandIn<string>>(validationMock.Object);
            var result = command.Execute(It.IsAny<string>());

            result.Status.ShouldBe(Status.Fail);
            validationMock.Verify();
        }

        [Test]
        public void ReturnSuccessResultWhenInputIsValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandIn<string>>(
                m => m.Setup(c => c.OnExecute(It.IsAny<string>())).Returns(
                    new Result
                        {
                            Status = Status.Success
                        }),
                validationMock.Object);
            var result = command.Execute(It.IsAny<string>());

            result.Status.ShouldBe(Status.Success);
            validationMock.Verify();
        }

        [Test]
        public void ThrownNullReferenceExceptionWhenOnExecuteReturnNullResult()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(IS_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandIn<string>>(
                m => m.Setup(c => c.OnExecute(It.IsAny<string>()))
                      .Returns((Result)null),
                validationMock.Object);

            Should.Throw<NullReferenceException>(
                      () => command.Execute(It.IsAny<string>())).Message
                  .ShouldBe("The result of OnExecute can not be null.");
        }
    }
}