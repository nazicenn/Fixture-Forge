using FixtureForge.Application.Connections;
using FixtureForge.Application.Generation;
using FixtureForge.Application.Projects;
using FixtureForge.Application.Schema;
using FixtureForge.Infrastructure.Connections;
using FixtureForge.Infrastructure.Generation;
using FixtureForge.Infrastructure.Projects;
using FixtureForge.Infrastructure.Schema;
using FixtureForge.Infrastructure.Security;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using FixtureForge.Application.Queries;
using FixtureForge.Infrastructure.Queries;

namespace FixtureForge.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<ITableService, TableService>();
        services.AddScoped<IRelationshipService, RelationshipService>();
        services.AddScoped<IGenerationService, GenerationService>();
        services.AddScoped<IFixtureGenerator, FixtureGenerator>();
        services.AddScoped<IConnectionService, ConnectionService>();
        services.AddSingleton<ConnectionStringProtector>();
        services.AddValidatorsFromAssembly(typeof(IProjectService).Assembly);
        services.AddScoped<IQueryService, QueryService>();

        return services;
    }
}