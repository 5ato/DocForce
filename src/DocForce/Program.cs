using DocForce.API.Middlewares;
using DocForce.Application.Extensions;
using DocForce.Infrastructure.Context;
using DocForce.Infrastructure.Extensions;
using DocForce.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);


#region openApi

builder.Services.AddOpenApi();

#endregion

#region asp.net core region

builder.Services.AddControllers();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtOption = configuration.GetSection(JwtOption.Section).Get<JwtOption>()!;

    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidIssuer = jwtOption.Issuer,
        ValidateIssuer = true,
        ValidAudience = jwtOption.Audience,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtOption.Key)),
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

#endregion

#region serilog DI

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Logging.AddSerilog();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Host.UseSerilog();

#endregion

#region healthCheck DI

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationContext>(
        name: "doc_db",
        failureStatus: HealthStatus.Unhealthy,
        tags: ["ready"],
        customTestQuery: null)
    .AddApplicationLifecycleHealthCheck(tags: ["ready"]);

#endregion


var app = builder.Build();

app.UseHsts();
app.UseMiddleware<LoggingMiddleware>();
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#region healthCheck endpoint

// readiness
// TODO: Подумать стоит ли добавлять кэширование, может быть overhead
app.MapHealthChecks("/health/ready", new HealthCheckOptions()
{
    Predicate = registration => registration.Tags.Contains("ready"),
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            totalDurationMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                error = e.Value.Exception?.Message
            })
        };

        await context.Response.WriteAsJsonAsync(response);
    },
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
}).ShortCircuit();

app.MapHealthChecks("/health/live", new HealthCheckOptions()
{
    Predicate = _ => false,
    ResultStatusCodes =
    {
        [HealthStatus.Healthy] = StatusCodes.Status200OK,
        [HealthStatus.Degraded] = StatusCodes.Status200OK,
        [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
    }
}).ShortCircuit();

#endregion

await app.RunAsync();
