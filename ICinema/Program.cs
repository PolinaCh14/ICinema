using DotNetEnv;
using ICinema.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.LoginPath = new PathString("/Account/Login");
    })
    .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
    {
        options.ClientId = Environment.GetEnvironmentVariable("ClientId") ?? "";
        options.ClientSecret = Environment.GetEnvironmentVariable("ClientSecret") ?? "";

        options.Scope.Add("profile");
        options.Scope.Add("email");
    });

builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CinemaContext>(options => 
    options.UseSqlServer(Environment.GetEnvironmentVariable("DbConnectionString")));

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

    