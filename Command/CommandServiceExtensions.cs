using Command.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace Command
{
  public static class CommandServiceExtensions
  {
    public static IServiceCollection AddCommands(this IServiceCollection services)
    {
      services.AddSingleton<ICommandFactory, CommandFactory>();
      services.AddSingleton(typeof(IInputValidator<>), typeof(DataAnnotationsValidator<>));
      services.AddSingleton(typeof(IOutputValidator<>), typeof(DataAnnotationsValidator<>));

      services.AddLogging();

      return services;
    }
  }
}
