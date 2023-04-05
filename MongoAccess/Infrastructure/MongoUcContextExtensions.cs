using AspNetCore.Identity.Mongo;
using Data.Infrastructure.Abstractions;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MongoDB.Driver;

namespace MongoAccess.Infrastructure;
public static class MongoUcContextExtensions
{
    public static IServiceCollection AddMongoUcContext<TContext>(this IServiceCollection services,
        Action<MongoUcContextOptions> optionsAction) where TContext : IUcContext
    {
        var options = new MongoUcContextOptions();
        optionsAction(options);

        services.AddSingleton(new MongoClient(options.ConnectionString).GetDatabase(options.DatabaseName));
        services.TryAddScoped(typeof(IUcContext), typeof(TContext));
        return services;
    }

    //public static IdentityBuilder AddMongoUcStores(this IdentityBuilder builder)
    //{
    //    return builder.AddMongoDbStores<User, Role, string>();
    //}
}
