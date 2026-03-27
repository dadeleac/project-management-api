using Microsoft.Data.Sqlite;
using ProjectManagement.Api.Common.Constants;

namespace ProjectManagement.Api.Configuration;

public static class SqliteConfigurationHelper
{
    public static void NormalizeConnectionString(
        ConfigurationManager configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(ConfigurationErrors.MissingConnectionString);
        }

        var builder = new SqliteConnectionStringBuilder(connectionString);

        if (string.IsNullOrWhiteSpace(builder.DataSource) ||
            builder.DataSource.Equals(":memory:", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        var fullPath = Path.IsPathRooted(builder.DataSource)
            ? builder.DataSource
            : Path.GetFullPath(Path.Combine(environment.ContentRootPath, builder.DataSource));

        var directory = Path.GetDirectoryName(fullPath);

        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        builder.DataSource = fullPath;

        configuration["ConnectionStrings:DefaultConnection"] = builder.ToString();
    }
}