using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using MudBlazor.Services;
using Radzen;
using System.Reflection;
using WebClient.Identity;
using WebClient.LanguageResources;
using WebClient.RequestHttp;
using WebClient.Service.AppConfigs;
using WebClient.Service.AppHistories;
using WebClient.Service.Departments;
using WebClient.Service.JS;
using WebClient.Service.Positions;
using WebClient.Service.PostCategories;
using WebClient.Service.Posts;
using WebClient.Service.Roles;
using WebClient.Service.Services;
using WebClient.Service.ServiceTypes;
using WebClient.Service.Upload;
using WebClient.Service.UserCompanies;
using WebClient.Service.Users;
using WebClient.Service.WebBanners;
using WebClient.Service.WebMenus;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddMudServices();

RequestClient.Initialize(builder.Configuration);

//service
builder.Services.AddSingleton<JsonStringLocalizer>();

builder.Services.AddScoped<RoleManagerService, RoleManagerService>();
builder.Services.AddTransient<UserManagerService,UserManagerService>();
builder.Services.AddTransient<UploadService, UploadService>();
builder.Services.AddTransient<PositionService,PositionService>();
builder.Services.AddTransient<DepartmentService,DepartmentService>();
builder.Services.AddTransient<ServiceService>();
builder.Services.AddTransient<ServiceTypeService>();
builder.Services.AddTransient<AppConfigService>();
builder.Services.AddScoped<DownloadFileService>();
builder.Services.AddScoped<TooltipService>();
builder.Services.AddScoped<ClipboardService>();
builder.Services.AddScoped<DialogService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddTransient<PostCategoryService>();
builder.Services.AddTransient<WebBannerService>();
builder.Services.AddTransient<WebMenuService>();
builder.Services.AddTransient<JsService>();
builder.Services.AddTransient<PostService>();
builder.Services.AddTransient<AppHistoryService>();
builder.Services.AddTransient<UserCompanyService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider , ApiAuthenticationStateProvider >();


//note:
var assembly = Assembly.GetExecutingAssembly();
var types = assembly.GetTypes().Where(t => typeof(IValidator).IsAssignableFrom(t) && t.IsClass);
foreach (var type in types)
{
    builder.Services.AddTransient( type);
}



builder.Services
    .AddBlazorise(options =>
    {
        options.Immediate = true;
    })
    .AddBootstrapProviders()
    .AddFontAwesomeIcons();


builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


var provider = new FileExtensionContentTypeProvider();
// Add new MIME type mappings
provider.Mappings[".res"] = "application/octet-stream";
provider.Mappings[".pexe"] = "application/x-pnacl";
provider.Mappings[".nmf"] = "application/octet-stream";
provider.Mappings[".mem"] = "application/octet-stream";
provider.Mappings[".wasm"] = "application/wasm";

app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
    ContentTypeProvider = provider
});

app.UseCors(policy => policy.AllowAnyMethod().AllowAnyHeader().SetIsOriginAllowed(origin => true).AllowCredentials());

app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();