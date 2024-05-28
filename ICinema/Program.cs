using ICinema.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(options =>
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
    //IConfigurationSection googleAuthSection = builder.Configuration.GetSection("Google");
    //options.ClientId = googleAuthSection["ClientId"] ?? "";
    //options.ClientSecret = googleAuthSection["ClientSecret"] ?? "";

    options.ClientId = "583090746385-f5utupjicbdnuf6l0oocn6ds2i6fs21d.apps.googleusercontent.com";
    options.ClientSecret = "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";

    options.Scope.Add("profile");
    options.Scope.Add("email");
});

builder.Services.AddDistributedMemoryCache();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<CinemaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ZhenyaServerConnection")));

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

    