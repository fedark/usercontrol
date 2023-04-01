using System.Globalization;
using Data.Db;
using Data.Models;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using UserControl.Access;
using UserControl.Services;
using UserControl.ViewModels.MappingProfiles;

var builder = WebApplication.CreateBuilder(args);

ConfigureDatabase(builder);
ConfigureIdentity(builder.Services);
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
    var supportedCultures = new CultureInfo[] { new("en"), new("ru") };

    options.DefaultRequestCulture = new RequestCulture("en");
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

static void ConfigureDatabase(WebApplicationBuilder builder)
{
    if (!Enum.TryParse<AppDbProvider>(builder.Configuration.GetValue("DbProvider", nameof(AppDbProvider.SqlServer)), out var provider))
    {
        provider = AppDbProvider.SqlServer;
    }

    var connectionString = builder.Configuration.GetConnectionString("Default");

    builder.Services.AddDbContext<AppDbContext>(options =>
    {
        _ = provider switch
        {
            AppDbProvider.SqlServer => options.UseSqlServer(connectionString ??= builder.Configuration.GetConnectionString("UserControlLocalDB"),
                b => b.MigrationsAssembly("Data.SqlServerMigrations")),
            AppDbProvider.PostgreSql => options.UseNpgsql(connectionString ??= builder.Configuration.GetConnectionString("UserControlPostgreSqlDB"),
                b => b.MigrationsAssembly("Data.PostgreSqlMigrations")),
            AppDbProvider.Sqlite => options.UseSqlite(connectionString ??= builder.Configuration.GetConnectionString("UserControlSqliteDB"),
                b => b.MigrationsAssembly("Data.SqliteMigrations")),
            AppDbProvider.ContainerSqlServer => options.UseSqlServer(connectionString ??= builder.Configuration.GetConnectionString("UserControlContainerSqlServerDB"),
                b => b.MigrationsAssembly("Data.SqlServerMigrations")),
            AppDbProvider.ContainerPostgreSql => options.UseNpgsql(connectionString ??= builder.Configuration.GetConnectionString("UserControlContainerPostgreSqlDB"),
                b => b.MigrationsAssembly("Data.PostgreSqlMigrations")),

            _ => throw new Exception($"Database provider '{provider}' is not supported")
        };
    });

    builder.Services.Configure<InitialDbSettings>(builder.Configuration.GetRequiredSection(nameof(InitialDbSettings)));

    builder.Services.AddScoped<UserProfileProvider>();
}

static void ConfigureIdentity(IServiceCollection services)
{
    services.AddDefaultIdentity<User>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
    })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<AppDbContext>();
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