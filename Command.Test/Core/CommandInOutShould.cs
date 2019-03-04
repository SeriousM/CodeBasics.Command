using System;
using Command.Core;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Command.Test.Core
{
  [TestFixture]
  public class CommandInOutShould
  {
    private const bool isNotValid = false;

    private const bool isValid = true;

    [Test]
    public void ReturnFailResultWhenInputIsNotValid()
    {
      var inputValidationMock = new Mock<IValidator<string>>();
      inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isNotValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(inputValidationMock.Object, It.IsAny<Validator<string>>());
      var result = command.Execute(It.IsAny<string>());

      result.Status.ShouldBe(Status.Fail);
      inputValidationMock.Verify();
    }

    [Test]
    public void ReturnFailResultWhenOutputIsNotValid()
    {
      var inputValidationMock = new Mock<IValidator<string>>();
      var outputValidationMock = new Mock<IValidator<string>>();
      inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();
      outputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isNotValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(
        m => m.Setup(c => c.OnExecute(It.IsAny<string>())).Returns(
          new Result<string>
          {
            Status = Status.Success
          }),
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
      var inputValidationMock = new Mock<IValidator<string>>();
      var outputValidationMock = new Mock<IValidator<string>>();
      inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();
      outputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(
        m => m.Setup(c => c.OnExecute(It.IsAny<string>())).Returns(
          new Result<string>
          {
            Status = Status.Success
          }),
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
      var inputValidationMock = new Mock<IValidator<string>>();
      var outputValidationMock = new Mock<IValidator<string>>();
      inputValidationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInOut<string, string>>(
        m => m.Setup(c => c.OnExecute(It.IsAny<string>()))
              .Returns((Result<string>)null),
        inputValidationMock.Object,
        outputValidationMock.Object);

      Should.Throw<NullReferenceException>(
               () => command.Execute(It.IsAny<string>())).Message
            .ShouldBe("The result of OnExecute can not be null.");
    }
  }
}
