using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShopGiay.Data;
using ShopGiay.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------------- DATABASE ----------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// ------------ IDENTITY CHO ADMIN -------------
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ------------ AUTHENTICATION CHO KHÁCH HÀNG (THÊM COOKIE SCHEME RIÊNG) ------------
// Identity đã có scheme riêng, chỉ cần THÊM scheme "Customer", KHÔNG ghi đè default
builder.Services.AddAuthentication()
    .AddCookie("Customer", options =>
    {
        options.LoginPath = "/Customers/Login";
        options.LogoutPath = "/Customers/Signout";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

// ------------ HASH PASSWORD CHO KHÁCH HÀNG ------------
builder.Services.AddSingleton<IPasswordHasher<Khachhang>, PasswordHasher<Khachhang>>();

// ------------ HTTP CONTEXT ACCESSOR ------------
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Session cho gi? hàng + login khách hàng
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.IdleTimeout = TimeSpan.FromDays(7);
});

var app = builder.Build();

// ----------------- INIT ROLE ADMIN -----------------
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    
    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    var admin = await userManager.FindByEmailAsync("admin@shop.com");
    if (admin == null)
    {
        admin = new IdentityUser
        {
            UserName = "admin@shop.com",
            Email = "admin@shop.com",
            EmailConfirmed = true
        };

        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

// ---------------- MIDDLEWARE PIPELINE ----------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();        
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customers}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
