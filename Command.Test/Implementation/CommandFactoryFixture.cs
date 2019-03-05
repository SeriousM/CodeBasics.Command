using System;
using System.Threading.Tasks;
using CodeBasics.Command.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeBasics.Command.Test.Implementation
{
  [TestClass]
  public class CommandFactoryFixture
  {
    private IServiceProvider services;

    public ICommandFactory CommandFactory => services.GetRequiredService<ICommandFactory>();

    [TestInitialize]
    public void TestSetup()
    {
      services = new ServiceCollection()
                .AddCommands()
                .AddSingleton<ICommandInOutAsync<int>, TestCommand>()
                .BuildServiceProvider();
    }

    [TestMethod]
    public async Task Create_should_create_a_working_command()
    {
      // arrange
      var command = CommandFactory.CreateAsync<TestCommand, int, int>(1);

      // act
      var result = await command.ExecuteAsync();

      // assert
      Assert.IsTrue(result.WasSuccessful);
      Assert.AreEqual(2, result.Value);
    }

    [TestMethod]
    public async Task Execute_command_twice_should_throw()
    {
      // arrange
      var command = CommandFactory.CreateAsync<TestCommand, int, int>(1);
      await command.ExecuteAsync();

      // act / assert
      await Assert.ThrowsExceptionAsync<InvalidOperationException>(() => command.ExecuteAsync());
    }

    [TestMethod]
    public async Task InputValidation_fail_should_return_bad_result()
    {
      // arrange
      var command = (TestCommand)CommandFactory.CreateAsync<TestCommand, int, int>(1);
      command.SetInputValidator(new ActionValidator<int>(_ => false));

      // act
      var result = await command.ExecuteAsync();

      // assert
      Assert.IsFalse(result.WasSuccessful);
      Assert.AreEqual(CommandExecutionStatus.PreValidationFailed, result.Status);
    }

    [TestMethod]
    public async Task OutputValidation_fail_should_return_bad_result()
    {
      // arrange
      var command = (TestCommand)CommandFactory.CreateAsync<TestCommand, int, int>(1);
      command.SetOutputValidator(new ActionValidator<int>(_ => false));

      // act
      var result = await command.ExecuteAsync();

      // assert
      Assert.IsFalse(result.WasSuccessful);
      Assert.AreEqual(CommandExecutionStatus.PostValidationFalied, result.Status);
    }
  }
}
