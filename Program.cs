using FiDa.Database;
using Microsoft.EntityFrameworkCore;
using Auth0.AspNetCore.Authentication;

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FiDaDatabase>();
    dbContext.Database.MigrateAsync().Wait();
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
    pattern: "{controller=Dashboard}/{action=Index}");

app.Run();
