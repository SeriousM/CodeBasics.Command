using Command.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Command
{
  public static class CommandServiceExtensions
  {
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
      services.AddSingleton<ICommandFactory, CommandFactory>();

      return services;
    }
  }
}
