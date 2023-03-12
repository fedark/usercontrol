using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserControl.Access;
using UserControl.Data;
using UserControl.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Default") ??
    builder.Configuration.GetConnectionString("UserControlLocalDB");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

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

builder.Services.AddTransient<IAuthorizationHandler, NotSelfUserHandler>();
builder.Services.AddTransient<IAuthorizationHandler, NotPrimeAdminHandler>();

builder.Services.AddTransient<DefaultUserProfileProvider>();

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
