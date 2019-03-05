using System;
using Microsoft.Extensions.DependencyInjection;

namespace CodeBasics.Command.Implementation
{
  internal class CommandFactory : ICommandFactory
  {
    private readonly IServiceProvider services;

    public CommandFactory(IServiceProvider services)
    {
      this.services = services;
    }

    public ICommandInOutAsync<TOut> CreateAsync<TCommand, TIn, TOut>(TIn input) where TCommand : ICommandInOutAsync<TOut>
    {
      var command = services.GetRequiredService<ICommandInOutAsync<TOut>>();
      if (command is ICommandSetInput<TIn> cmd)
      {
        cmd.SetInputParameter(input);
      }
      else
      {
        throw new ArgumentException($"Command '{typeof(TCommand).FullName}' does not implement '{typeof(ICommandSetInput<TIn>).Name}'. Maybe the wrong type was implemented?");
      }

      return command;
    }
  }
}
