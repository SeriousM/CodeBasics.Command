using System;
using Command.Core;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Command.Test.Core
{
  [TestFixture]
  public class CommandInShould
  {
    private const bool isNotValid = false;

    private const bool isValid = true;

    [Test]
    public void ReturnFailResultWhenInputIsNotValid()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isNotValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandIn<string>>(validationMock.Object);
      var result = command.Execute(It.IsAny<string>());

      result.Status.ShouldBe(Status.Fail);
      validationMock.Verify();
    }

    [Test]
    public void ReturnSuccessResultWhenInputIsValid()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

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
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

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
