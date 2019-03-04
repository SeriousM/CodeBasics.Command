using System;
using System.Threading.Tasks;
using Command.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Command.Test.Implementation
{
  [TestClass]
  public class CommandFactoryShould
  {
    private IServiceProvider services;

    [TestInitialize]
    public void TestSetup()
    {
      services = new ServiceCollection()
                .AddCommands()
                .AddSingleton<ICommandInOutAsync<int, int>, TestCommand>()
                .BuildServiceProvider();
    }

    public ICommandFactory CommandFactory => services.GetRequiredService<ICommandFactory>();

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
  }

  public class NoopValidator<T> : IValidator<T>
  {
    public static IValidator<T> Instance { get; } = new NoopValidator<T>();

    private NoopValidator()
    {
    }

    public bool Validate(T value)
    {
      return true;
    }
  }

  public class TestCommand : CommandInOutAsync<int, int>
  {
    public TestCommand() : base(NoopValidator<int>.Instance, NoopValidator<int>.Instance)
    {
      
    }

    protected internal override async Task<IResult<int>> OnExecuteAsync(int input)
    {
      return Result<int>.Success(++input);
    }

    public TestCommand(IValidator<int> inputValidator, IValidator<int> outputValidator) : base(inputValidator, outputValidator)
    {
    }
  }
}
