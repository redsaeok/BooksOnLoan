using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BooksOnLoan.Data;
using BooksOnLoan.Components;
using BooksOnLoan.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options =>
    {
        options.Stores.MaxLengthForKeys = 128;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()    
    .AddDefaultUI()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>()
    .AddRoleManager<RoleManager<IdentityRole>>();    

builder.Services.AddControllersWithViews();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddCircuitOptions(options => options.DetailedErrors = true); // for debugging razor components

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAntiforgery();

app.UseAuthentication();  // we must authentication before we can authorize
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate();

    //var userMgr = services.GetRequiredService<UserManager<CustomUser>>();
    //var roleMgr = services.GetRequiredService<RoleManager<IdentityRole>>();

    //IdentitySeedData.Initialize(context, userMgr, roleMgr).Wait();
}

app.MapRazorComponents<App>()
.AddInteractiveServerRenderMode();
app.Run();
