using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagement.Application.Common.Interfaces.Commands;
using ProjectManagement.Application.Common.Interfaces.Queries;
using ProjectManagement.Infrastructure.Common.Persistence;
using ProjectManagement.Infrastructure.Repositories.Commands;
using ProjectManagement.Infrastructure.Repositories.Queries;

namespace ProjectManagement.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IProjectQueryService, ProjectQueryService>();
        services.AddScoped<ITaskQueryService, TaskQueryService>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();

        return services;
    }
}