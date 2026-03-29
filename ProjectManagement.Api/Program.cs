using ProjectManagement.Api.Configuration;
using ProjectManagement.Api.Extensions;
using ProjectManagement.Api.Middleware;
using ProjectManagement.Application;
using ProjectManagement.Infrastructure;
using Serilog;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());


// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters
            .Add(new JsonStringEnumConverter(allowIntegerValues: false));
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();

SqliteConfigurationHelper.NormalizeConnectionString(builder.Configuration, builder.Environment);
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

await app.ApplyMigrations();


app.UseHttpsRedirection();

app.UseAuthorization();

app.UseSerilogRequestLogging();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();



app.Run();
