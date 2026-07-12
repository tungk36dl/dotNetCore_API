using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ProjectCore.Application.Pipeline;

namespace ProjectCore.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register all IRequestHandler<,> implementations in this assembly automatically.
        // No more manual handler registrations — MediatR scans the assembly.
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            // Pipeline behaviors (executed in registration order for each request):
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });

        return services;
    }
}
