using Microsoft.AspNetCore.Identity;
using System.Net;
using Website.LanguageResources;
using Website.Middleware;
using Website.Models;
using Website.Services.AppConfigs;
using Website.Services.Auth;
using Website.Services.Menus;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Config options
builder.Services.Configure<RemoteAPIConfig>(builder.Configuration.GetSection("RemoteServices"));
builder.Services.Configure<UrlConfig>(builder.Configuration.GetSection("UrlConfigs"));
builder.Services.Configure<NewsInHomepageConfig>(builder.Configuration.GetSection("NewsInHomepage"));

builder.Services.Configure<IdentityOptions>(options =>
{
    //Default settings
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// Services
builder.Services.AddSingleton<JsonStringLocalizer>();

builder.Services.AddTransient<IMenuService, MenuService>();
builder.Services.AddTransient<IAppConfigService, AppConfigService>();
builder.Services.AddTransient<IAuthService, AuthService>();


builder.Services
    .AddHttpClient("Website", c =>
    {
        c.BaseAddress = new Uri(builder.Configuration.GetSection("RemoteServices").GetValue<string>("BaseUrl"));
        c.DefaultRequestHeaders.Add("Accept", "application/json");
    })
    .ConfigurePrimaryHttpMessageHandler(handler => new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip });

builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStatusCodePagesWithReExecute("/Home/Error");
app.UseExceptionHandler("/Home/Error");

app.UseStaticFiles();

app.UseOnlineUsers();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
    endpoints.MapControllerRoute(name: "default2", pattern: "{controller=Home}/{action=Index}");
    endpoints.MapControllerRoute(name: "error", pattern: "{controller=Home}/{action=Error}");
    endpoints.MapControllerRoute(name: "postList", pattern: "{controller=Post}/{action=Index}/{slug}_n_{id}");
    endpoints.MapControllerRoute(name: "postDetail", pattern: "{controller=Post}/{action=PostDetail}/post/{slug}_dt_{id}");
    endpoints.MapControllerRoute(name: "introPostDetail", pattern: "{controller=Post}/{action=PostDetail}/post/{slug}_a_{id}");
    endpoints.MapControllerRoute(name: "serviceList", pattern: "{controller=Service}/{action=Index}/{slug}_p_{id}");
    endpoints.MapControllerRoute(name: "serviceDetail", pattern: "{controller=Service}/{action=ServiceDetail}/serviceDetail/{slug}_pdt_{id}");
	endpoints.MapControllerRoute(name: "doctorList", pattern: "{controller=Doctor}/{action=DoctorDetail}/doctor/{slug}_sf_{id}");

	endpoints.MapControllerRoute(name: "auth-sign-in", pattern: "{controller=Auth}/{action=SignInAsync}/auth/sign-in");
	endpoints.MapControllerRoute(name: "auth-sign-out", pattern: "{controller=Auth}/{action=SignInAsync}/auth/sign-out");
	endpoints.MapControllerRoute(name: "auth-sign-up", pattern: "{controller=Auth}/{action=SignInAsync}/auth/sign-up");
});

app.Run();
