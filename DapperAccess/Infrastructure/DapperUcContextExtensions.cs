using Data.Infrastructure.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DapperAccess.Infrastructure;
public static class DapperUcContextExtensions
{
    public static IServiceCollection AddDapperUcContext<TContext>(this IServiceCollection services,
        Action<DapperUcContextOptions> optionsAction) where TContext : IUcContext
    {
        var options = new DapperUcContextOptions();
        optionsAction(options);

        services.AddScoped(services => new SqlConnection(options.ConnectionString));
        services.TryAddScoped(typeof(IUcContext), typeof(TContext));
        return services;
    }

    public static IdentityBuilder AddDapperUcStores(this IdentityBuilder builder)
    {
        return builder.AddDapperStores();
    }
}
