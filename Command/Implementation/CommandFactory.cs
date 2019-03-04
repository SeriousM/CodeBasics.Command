using System;
using Microsoft.Extensions.DependencyInjection;

namespace Command.Implementation
{
  class CommandFactory : ICommandFactory
  {
    private readonly IServiceProvider services;

    public CommandFactory(IServiceProvider services)
    {
      this.services = services;
    }

    public ICommandInOutAsync<TIn, TOut> CreateAsync<TCommand, TIn, TOut>(TIn input) where TCommand : ICommandInOutAsync<TIn, TOut>
    {
      var command = services.GetRequiredService<ICommandInOutAsync<TIn, TOut>>();
      ((ICommandSetInput<TIn>)command).SetInputParameter(input);

      return command;
    }
  }
}