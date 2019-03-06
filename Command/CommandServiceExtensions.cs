using CodeBasics.Command.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CodeBasics.Command
{
  public static class CommandServiceExtensions
  {
    public static IServiceCollection AddCommand(this IServiceCollection services)
    {
      services.TryAddSingleton<ICommandFactory, CommandFactory>();
      services.TryAddSingleton(typeof(IInputValidator<>), typeof(DataAnnotationsValidator<>));
      services.TryAddSingleton(typeof(IOutputValidator<>), typeof(DataAnnotationsValidator<>));

      services.AddLogging();

      return services;
    }
  }
}
