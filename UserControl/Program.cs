using System.Globalization;
using Data.Infrastructure.Services;
using Data.Models;
using EfAccess.Impl;
using EfAccess.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserControl.Access;
using UserControl.Localization;
using UserControl.Services;
using UserControl.ViewModels.MappingProfiles;

var builder = WebApplication.CreateBuilder(args);

ConfigureDataContext(builder.Services, builder.Configuration);
ConfigureIdentity(builder.Services, builder.Configuration);
ConfigureMappings(builder.Services);

builder.Services.AddControllersWithViews()
    .AddMvcLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var localizationOptions = builder.Configuration.GetRequiredSection("LocalizationOptions");
    var defaultCulture = localizationOptions["DefaultCulture"] ?? throw new Exception("Default culture is not configured.");

    var supportedCultures = localizationOptions.GetRequiredSection("SupportedCultures").Get<string[]>()
        .Select(c => new CultureInfo(c)).ToList();

    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

ConfigurePolicies(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRequestLocalization(app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value);

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapDefaultControllerRoute();

app.Run();

static void ConfigureDataContext(IServiceCollection services, IConfiguration configuration)
{
    if (!Enum.TryParse<EfDbProvider>(configuration.GetValue("DbProvider", nameof(EfDbProvider.SqlServer)), out var provider))
    {
        provider = EfDbProvider.SqlServer;
    }

    var connectionString = configuration.GetConnectionString("Default") ?? provider switch
    {
        EfDbProvider.SqlServer => configuration.GetConnectionString("UserControlLocalDB"),
        EfDbProvider.PostgreSql => configuration.GetConnectionString("UserControlPostgreSqlDB"),
        EfDbProvider.Sqlite => configuration.GetConnectionString("UserControlSqliteDB"),
        EfDbProvider.ContainerSqlServer => configuration.GetConnectionString("UserControlContainerSqlServerDB"),
        EfDbProvider.ContainerPostgreSql => configuration.GetConnectionString("UserControlContainerPostgreSqlDB"),
        _ => throw new Exception("Connection string is not provided")
    } ?? throw new Exception("Connection string is not provided");

    services.AddEfUcContext<EfUcContext>(options =>
    {
        options.DbProvider = provider;
        options.ConnectionString = connectionString;
        options.SeedOptions = configuration.GetSection("IdentitySeedOptions").Get<IdentitySeedOptions>();
    });
}

static void ConfigureIdentity(IServiceCollection services, IConfiguration configuration)
{
    services.AddDefaultIdentity<User>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
    })
        .AddRoles<Role>()
        .AddEfUcStores()
        .AddErrorDescriber<LocalizedIdentityErrorDescriber>();

    services.Configure<LocalizationOptions>(configuration.GetRequiredSection("LocalizationOptions"));
    services.AddSingleton<IIdentityErrorDescriberFactory, IdentityErrorDescriberFactory>();
}

static void ConfigureMappings(IServiceCollection services)
{
    services.AddAutoMapper(typeof(UserMappingProfile));
}

static void ConfigurePolicies(IServiceCollection services)
{
    services.AddAuthorization(options =>
    {
        options.AddPolicy(Policy.NotOnSelf, policyBuilder =>
            policyBuilder.AddRequirements(new NotOnSelfRequirement()));
    });

    services.AddAuthorization(options =>
    {
        options.AddPolicy(Policy.NotOnOwner, policyBuilder =>
            policyBuilder.AddRequirements(new NotOnOwnerRequirement()));
    });

    services.AddAuthorization(options =>
    {
        options.AddPolicy(Policy.NotOwner, policyBuilder =>
            policyBuilder.AddRequirements(new NotOwnerRequirement()));
    });

    services.AddTransient<IAuthorizationHandler, NotOnSelfHandler>();
    services.AddTransient<IAuthorizationHandler, NotOnOwnerHandler>();
    services.AddTransient<IAuthorizationHandler, NotOwnerHandler>();
}