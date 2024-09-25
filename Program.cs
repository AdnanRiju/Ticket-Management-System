using CompanyManagement.Models;
using CompanyManagement.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MiniErp.Service.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


var configuration = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
var config = configuration.Build();
AppSetting.ConnectionString = config.GetConnectionString("SystemConnection");
builder.Services.AddDbContext<CMScontext>(options => options.UseSqlServer(AppSetting.ConnectionString, opt => opt.EnableRetryOnFailure(2, TimeSpan.FromSeconds(2), null)), ServiceLifetime.Scoped);
builder.Services.AddScoped<CMScontext, CMScontext>();
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<CompanyService, CompanyService>();
builder.Services.AddScoped<RoleService, RoleService>();
builder.Services.AddScoped<MenuItemService, MenuItemService>();
builder.Services.AddScoped<RoleMenuService, RoleMenuService>();
builder.Services.AddScoped<UserService, UserService>();
builder.Services.AddSession(o => {
    o.IdleTimeout = TimeSpan.FromSeconds(1800);
});
builder.Services.AddMvc(options =>
{
    options.EnableEndpointRouting = false;
}).AddJsonOptions(jsonoption =>
{
    jsonoption.JsonSerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(300);
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
