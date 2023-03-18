using Data.Db;
using Data.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserControl.Access;
using UserControl.Services;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Add services to the container.
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

builder.Services.AddTransient<UserProfileProvider>();
builder.Services.AddTransient<AdminRoleManager>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
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
