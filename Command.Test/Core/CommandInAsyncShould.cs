using System;
using System.Threading.Tasks;
using Command.Core;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Command.Test.Core
{
  [TestFixture]
  public class CommandInAsyncShould
  {
    private const bool isNotValid = false;

    private const bool isValid = true;

    [Test]
    public async Task ReturnFailResultWhenInputIsNotValid()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isNotValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInAsync<string>>(validationMock.Object);
      var result = await command.ExecuteAsync(It.IsAny<string>());

      result.Status.ShouldBe(Status.Fail);
      validationMock.Verify();
    }

    [Test]
    public async Task ReturnSuccessResultWhenInputIsValid()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInAsync<string>>(
        m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>()))
              .ReturnsAsync(
                 new Result
                 {
                   Status = Status.Success
                 }),
        validationMock.Object);
      var result = await command.ExecuteAsync(It.IsAny<string>());

      result.Status.ShouldBe(Status.Success);
      validationMock.Verify();
    }

    [Test]
    public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullResult()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInAsync<string>>(
        m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>()))
              .ReturnsAsync((Result)null),
        validationMock.Object);

      Should.ThrowAsync<NullReferenceException>(
               () => command.ExecuteAsync(It.IsAny<string>())).Result.Message
            .ShouldBe("The result of task on OnExecuteAsync can not be null.");
    }

    [Test]
    public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullTask()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandInAsync<string>>(
        m => m.Setup(c => c.OnExecuteAsync(It.IsAny<string>()))
              .Returns((Task<Result>)null),
        validationMock.Object);

      Should.ThrowAsync<NullReferenceException>(
               () =>
                 command.ExecuteAsync(It.IsAny<string>())).Result.Message
            .ShouldBe("The task of OnExecute can not be null.");
    }
  }
}
