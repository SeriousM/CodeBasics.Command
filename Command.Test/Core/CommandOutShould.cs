using System;
using Command.Core;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Command.Test.Core
{
  [TestFixture]
  public class CommandOutShould
  {
    private const bool isNotValid = false;

    private const bool isValid = true;

    [Test]
    public void ReturnFailResultWhenOutputIsNotValid()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate("not valid value")).Returns(isNotValid).Verifiable();

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
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate("valid value")).Returns(isValid).Verifiable();

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
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

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
