using Auth0.AspNetCore.Authentication;
using FiDa.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database context
builder.Services.AddDbContext<FiDaDatabase>(options => options.UseSqlServer(config.GetConnectionString("FiDaDatabase")));

// Auth0 
builder.Services.AddAuth0WebAppAuthentication(options =>
{
    options.Domain = config["Auth0:Domain"];
    options.ClientId = config["Auth0:ClientId"];
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<FiDaDatabase>();

    try
    {
        logger.LogInformation("Applying migrations...");
        await dbContext.Database.MigrateAsync();
        logger.LogInformation("Migrations applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}",
    defaults: new { controller = "Dashboard" });

app.MapControllerRoute(
    name: "pcloud",
    pattern: "pcloud/{action=Index}/{id?}",
    defaults: new { controller = "PCloud" });

app.MapControllerRoute(
    name: "dropbox",
    pattern: "dropbox/{action=Index}/{id?}",
    defaults: new { controller = "Dropbox" });

app.Run();
