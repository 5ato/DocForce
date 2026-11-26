using DocForce.Application.Interfaces;
using DocForce.Application.Interfaces.Repositories;
using DocForce.Application.Interfaces.Services;
using DocForce.Infrastructure.Common;
using DocForce.Infrastructure.Common.Outbox;
using DocForce.Infrastructure.Context;
using DocForce.Infrastructure.Options;
using DocForce.Infrastructure.Repositories;
using DocForce.Infrastructure.Services;
using DocForce.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DocForce.Infrastructure.Extensions;

public static class IServiceCollectionExtensions
{
    public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddInfrastructureOptions(configuration);

        services.AddDbContext(configuration);
        services.AddRepository();
        services.AddServices();
    }

    internal static void AddInfrastructureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOption>(configuration.GetSection(JwtOption.Section));
    }

    internal static void AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(Constants.DocPostgresConnectionString);

        services.AddDbContext<ApplicationContext>(option =>
        {
            option.UseNpgsql(connectionString);
        });
    }

    internal static void AddRepository(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
    }

    internal static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        services.AddHostedService<PostgresOutboxDispatcher>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
    }
}
