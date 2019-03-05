using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Command.Test.Implementation
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
  }
}
