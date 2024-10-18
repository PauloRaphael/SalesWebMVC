using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMVC.Data;

var builder = WebApplication.CreateBuilder(args);

// Register the DbContext
builder.Services.AddDbContext<SalesWebMVCContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("SalesWebMVCContext"),
        new MySqlServerVersion(new Version(8, 0, 25)), // Specify the MySQL server version
        mysqlOptions => mysqlOptions.MigrationsAssembly("SalesWebMVC")
    ));

// Register SeedingService
builder.Services.AddScoped<SeedingService>();

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Add seed logic after app building
using (var scope = app.Services.CreateScope())
{ 
    var services = scope.ServiceProvider;

    // Fetch the SeedingService and call Seed
    var seedingService = services.GetRequiredService<SeedingService>();
    seedingService.Seed();
}

// Configure the HTTP request pipeline 
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
