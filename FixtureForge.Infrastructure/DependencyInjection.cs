using FixtureForge.Application.Generation;
using FixtureForge.Application.Projects;
using FixtureForge.Application.Schema;
using FixtureForge.Infrastructure.Generation;
using FixtureForge.Infrastructure.Projects;
using FixtureForge.Infrastructure.Schema;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FixtureForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<IRelationshipService, RelationshipService>();
        services.AddScoped<IGenerationService, GenerationService>();
        services.AddValidatorsFromAssembly(typeof(IProjectService).Assembly);
        services.AddScoped<IFixtureGenerator, FixtureGenerator>();
        return services;
    }
}