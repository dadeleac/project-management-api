using ProjectManagement.Infrastructure;

namespace ProjectManagement.Api.Extensions
{
    public static class MigrationExtensions
    {
        public static async Task ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();

            await initializer.InitializeAsync();
        }
    }
}
