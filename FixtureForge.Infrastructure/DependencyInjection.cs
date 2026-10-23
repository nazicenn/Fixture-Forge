using FixtureForge.Application.Projects;
using FixtureForge.Infrastructure.Projects;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FixtureForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddValidatorsFromAssembly(typeof(IProjectService).Assembly);
        return services;
    }
}