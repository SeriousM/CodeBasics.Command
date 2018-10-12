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
        private readonly IResult IS_NOT_VALID = new Result();

        private readonly IResult IS_VALID = new Result { Status = Status.Success };

        [Test]
        public void ReturnFailResultWhenInputIsNotValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_NOT_VALID).Verifiable();

            var command = Mock.CreateInstanceOf<CommandIn<string>>(validationMock.Object);
            var result = command.Execute(It.IsAny<string>());

            result.Status.ShouldBe(Status.Fail);
            validationMock.Verify();
        }

        [Test]
        public void ReturnSuccessResultWhenInputIsValid()
        {
            Mock<IValidator<string>> validationMock = new Mock<IValidator<string>>();
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

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
            validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(this.IS_VALID).Verifiable();

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