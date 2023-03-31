using System.Globalization;
using Data.Db;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using UserControl.Access;

var builder = WebApplication.CreateBuilder(args);

ConfigureDatabase(builder);

builder.Services.AddControllersWithViews();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotSelf", policyBuilder =>
        policyBuilder.AddRequirements(new NotSelfUserRequirement()));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotPrimeAdmin", policyBuilder =>
        policyBuilder.AddRequirements(new NotPrimeAdminRequirement()));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("NotPrimeAdminUser", policyBuilder =>
        policyBuilder.AddRequirements(new NotPrimeAdminUserRequirement()));
});

builder.Services.AddTransient<IAuthorizationHandler, NotSelfUserHandler>();
builder.Services.AddTransient<IAuthorizationHandler, NotPrimeAdminHandler>();
builder.Services.AddTransient<IAuthorizationHandler, NotPrimeAdminUserHandler>();


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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

    builder.Services.AddDefaultIdentity<IdentityUser>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;
        options.Password.RequireUppercase = false;
    })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>();

    builder.Services.AddScoped<UserProfileProvider>();
}