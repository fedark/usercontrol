using System.Globalization;
using Data.Db;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UserControl.Access;
using UserControl.Localization;
using UserControl.Services;
using UserControl.ViewModels.MappingProfiles;

var builder = WebApplication.CreateBuilder(args);

ConfigureDatabase(builder.Services, builder.Configuration);
ConfigureIdentity(builder.Services, builder.Configuration);
ConfigureMappings(builder.Services);

builder.Services.AddControllersWithViews()
    .AddMvcLocalization(options => options.ResourcesPath = "Resources");

ConfigurePolicies(builder.Services);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRequestLocalization(options =>
{
    var localizationOptions = app.Services.GetRequiredService<IOptions<LocalizationOptions>>();
    var supportedCultures = localizationOptions.Value.SupportedCultures.Select(c => new CultureInfo(c)).ToList();

    options.DefaultRequestCulture = new RequestCulture(localizationOptions.Value.DefaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapDefaultControllerRoute();

app.Run();

static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
{
    if (!Enum.TryParse<AppDbProvider>(configuration.GetValue("DbProvider", nameof(AppDbProvider.SqlServer)), out var provider))
    {
        provider = AppDbProvider.SqlServer;
    }

    var connectionString = configuration.GetConnectionString("Default");

    services.AddDbContext<AppDbContext>(options =>
    {
        _ = provider switch
        {
            AppDbProvider.SqlServer => options.UseSqlServer(connectionString ??= configuration.GetConnectionString("UserControlLocalDB"),
                b => b.MigrationsAssembly("Data.SqlServerMigrations")),
            AppDbProvider.PostgreSql => options.UseNpgsql(connectionString ??= configuration.GetConnectionString("UserControlPostgreSqlDB"),
                b => b.MigrationsAssembly("Data.PostgreSqlMigrations")),
            AppDbProvider.Sqlite => options.UseSqlite(connectionString ??= configuration.GetConnectionString("UserControlSqliteDB"),
                b => b.MigrationsAssembly("Data.SqliteMigrations")),
            AppDbProvider.ContainerSqlServer => options.UseSqlServer(connectionString ??= configuration.GetConnectionString("UserControlContainerSqlServerDB"),
                b => b.MigrationsAssembly("Data.SqlServerMigrations")),
            AppDbProvider.ContainerPostgreSql => options.UseNpgsql(connectionString ??= configuration.GetConnectionString("UserControlContainerPostgreSqlDB"),
                b => b.MigrationsAssembly("Data.PostgreSqlMigrations")),

            _ => throw new Exception($"Database provider '{provider}' is not supported")
        };
    });

    services.Configure<InitialDbSettings>(configuration.GetRequiredSection(nameof(InitialDbSettings)));

    services.AddScoped<UserProfileProvider>();
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
        .AddEntityFrameworkStores<AppDbContext>()
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