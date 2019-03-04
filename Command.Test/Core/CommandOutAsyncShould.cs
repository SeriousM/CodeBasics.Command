using System;
using System.Threading.Tasks;
using Command.Core;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Command.Test.Core
{
  [TestFixture]
  public class CommandOutAsyncShould
  {
    private const bool isNotValid = false;

    private const bool isValid = true;

    [Test]
    public async Task ReturnFailResultWhenOutputIsNotValid()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate("not valid value")).Returns(isNotValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
        m => m.Setup(c => c.OnExecuteAsync()).ReturnsAsync(
          new Result<string>
          {
            Status = Status.Success,
            Value = "not valid value"
          }),
        validationMock.Object);
      var result = await command.ExecuteAsync();

      result.Status.ShouldBe(Status.Fail);
      validationMock.Verify();
    }

    [Test]
    public async Task ReturnSuccessResultWhenInputIsValid()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate("valid value")).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
        m => m.Setup(c => c.OnExecuteAsync()).ReturnsAsync(
          new Result<string>
          {
            Status = Status.Success,
            Value = "valid value"
          }),
        validationMock.Object);
      var result = await command.ExecuteAsync();

      result.Status.ShouldBe(Status.Success);
      validationMock.Verify();
    }

    [Test]
    public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullResult()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
        m => m.Setup(c => c.OnExecuteAsync())
              .ReturnsAsync((Result<string>)null),
        validationMock.Object);

      Should.Throw<NullReferenceException>(
               () => command.ExecuteAsync()).Message
            .ShouldBe("The result of OnExecute can not be null.");
    }

    [Test]
    public void ThrownNullReferenceExceptionWhenOnExecuteAsyncReturnNullTask()
    {
      var validationMock = new Mock<IValidator<string>>();
      validationMock.Setup(v => v.Validate(It.IsAny<string>())).Returns(isValid).Verifiable();

      var command = Mock.CreateInstanceOf<CommandOutAsync<string>>(
        m => m.Setup(c => c.OnExecuteAsync())
              .Returns((Task<Result<string>>)null),
        validationMock.Object);

      Should.ThrowAsync<NullReferenceException>(
               () =>
                 command.ExecuteAsync()).Result.Message
            .ShouldBe("The task of OnExecute can not be null.");
    }
  }
}
