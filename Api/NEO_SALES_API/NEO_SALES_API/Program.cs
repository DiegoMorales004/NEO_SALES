using System.Reflection;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.INFRASTRUCTURE.Extensions;
using NEO_SALES.INFRASTRUCTURE.Filters;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .WriteTo.File(
            "Logs/log-.txt",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
});

builder.Services.Configure<DbConfiguration>(builder.Configuration.GetSection("DBConfiguration"));
builder.Services.Configure<MessageDefaultsConfiguration>(builder.Configuration.GetSection("MessageDefaultsConfiguration"));
builder.Services.Configure<AuthCredentialConfiguration>(builder.Configuration.GetSection("AuthCredential"));
builder.Services.Configure<AuthConfig>(builder.Configuration.GetSection("AuthConfig"));
builder.Services.Configure<BackgroundJobsConfiguration>(builder.Configuration.GetSection("BackgroundJobsConfiguration"));

builder.Services
    .AddInfrastructureServices()
    .AddCoreServices()
    .AddJwtAuthentication(builder.Configuration);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
    options.Filters.Add<RequestResponseLoggingFilter>();
});

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () =>
{
    var version = Assembly
        .GetExecutingAssembly()
        .GetName()
        .Version?
        .ToString();

    return Results.Ok(new
    {
        Status = "OK",
        Version = version,
        Random = Random.Shared.Next()
    });
});

app.MapControllers();

app.Run();
