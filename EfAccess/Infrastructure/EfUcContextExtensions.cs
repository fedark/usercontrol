using Data.Infrastructure.Abstractions;
using Data.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EfAccess.Infrastructure;
public static class EfUcContextExtensions
{
    public static IServiceCollection AddEfUcContext<TContext>(this IServiceCollection services,
        Action<EfUcContextOptions> optionsAction) where TContext : IUcContext
    {
        var efOptions = new EfUcContextOptions();
        optionsAction(efOptions);

        services.Configure<IdentitySeedOptions>(options =>
        {
            options.AdminName = efOptions.SeedOptions.AdminName;
            options.OwnerName = efOptions.SeedOptions.OwnerName;
            options.OwnerPassword = efOptions.SeedOptions.OwnerPassword;
        });
        services.TryAddScoped<UserProfileProvider>();

        var provider = efOptions.DbProvider;
        var connectionString = efOptions.ConnectionString;

        services.AddDbContext<EfDbContext>(options =>
        {
            _ = provider switch
            {
                EfDbProvider.SqlServer => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Ef.SqlServerMigrations")),
                EfDbProvider.PostgreSql => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Ef.PostgreSqlMigrations")),
                EfDbProvider.Sqlite => options.UseSqlite(connectionString, b => b.MigrationsAssembly("Ef.SqliteMigrations")),
                EfDbProvider.ContainerSqlServer => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("Ef.SqlServerMigrations")),
                EfDbProvider.ContainerPostgreSql => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Ef.PostgreSqlMigrations")),

                _ => throw new Exception($"Database provider '{provider}' is not supported")
            };
        });

        services.TryAddScoped(typeof(IUcContext), typeof(TContext));

        return services;
    }

    public static IdentityBuilder AddEfUcStores(this IdentityBuilder builder)
    {
        return builder.AddEntityFrameworkStores<EfDbContext>();
    }
}
