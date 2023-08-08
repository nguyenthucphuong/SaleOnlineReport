using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SaleOnline.Models;
using SaleOnline.Models.Components;
//using static SaleOnline.Areas.User.Controllers.HomeController;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<SaleOnline1Context>(o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("ketnoi"));
});

//builder.Services.AddScoped<CategoryService>();

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Add Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<SaleOnline1Context>()
    .AddDefaultTokenProviders();

//builder.Services.AddIdentity<User, IdentityRole>()
//    .AddEntityFrameworkStores<SaleOnline1Context>()
//    .AddDefaultTokenProviders();


// Configure login path
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/taikhoan/dangnhap";
});
builder.Services.AddRazorPages();
//builder.Services.AddScoped<CustomAuthorizeFilter>(sp => new CustomAuthorizeFilter("Admin,User"));

// Add Session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});




var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();




//app.MapControllerRoute(
//    name: "AdminArea",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
//pattern: "{controller=Home}/{action=Index}/{name?}");




//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "custom",
//        pattern: "Home",
//        defaults: new { controller = "Home", action = "Index" }
//    );
//});


//app.MapControllerRoute(
//    name: "AdminArea",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "AdminArea",
//    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
