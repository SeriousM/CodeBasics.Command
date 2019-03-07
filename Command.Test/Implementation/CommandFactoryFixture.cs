using System;
using System.Threading.Tasks;
using CodeBasics.Command.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
                .AddCommand()
                .AddLogging(opts => opts.SetMinimumLevel(LogLevel.Trace).AddConsole())
                .AddScoped<TestCommand>()
                .BuildServiceProvider(true)
                .CreateScope()
                .ServiceProvider;
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
      await Assert.ThrowsExceptionAsync<CommandExecutionException>(() => command.ExecuteAsync());
    }

    [TestMethod]
    public async Task InputValidation_fail_should_return_bad_result()
    {
      // arrange
      var command = CommandFactory.CreateAsync<TestCommand, int, int>(1);
      ((IValidatorSetter<int, int>)command).SetInputValidator(new ActionValidator<int>(_ => false));

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
      var command = CommandFactory.CreateAsync<TestCommand, int, int>(1);
      ((IValidatorSetter<int, int>)command).SetOutputValidator(new ActionValidator<int>(_ => false));

      // act
      var result = await command.ExecuteAsync();

      // assert
      Assert.IsFalse(result.WasSuccessful);
      Assert.AreEqual(CommandExecutionStatus.PostValidationFalied, result.Status);
    }

    [TestMethod]
    public async Task Execution_failed_should_return_bad_result()
    {
      // arrange
      var command = CommandFactory.CreateAsync<TestCommand, int, int>(1);
      ((TestCommand)command).FailExecution = true;

      // act
      var result = await command.ExecuteAsync();

      // assert
      Assert.IsFalse(result.WasSuccessful);
      Assert.AreEqual(CommandExecutionStatus.ExecutionError, result.Status);
    }
  }
}
