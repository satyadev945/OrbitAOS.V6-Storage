using OrbitAOS.Infrastructure;
using OrbitAOS.Web.Filters;
using OrbitAOS.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register Infrastructure layer (includes DbContext, Identity, Repositories, Services)
builder.Services.AddInfrastructure(connectionString);

// Add MVC controllers with views and global exception filter
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<GlobalExceptionFilter>();
});

// Add session support (replaces legacy System.Web.SessionState)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add response caching middleware (replaces legacy OutputCache attribute)
builder.Services.AddResponseCaching();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // HSTS: 30 days default. Adjust for production scenarios.
    // See https://aka.ms/aspnetcore-hsts
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Custom request logging middleware (replaces legacy IHttpModule)
app.UseRequestLogging();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();
app.UseResponseCaching();

// Map default MVC controller route (replaces App_Start/RouteConfig.cs)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Map Razor Pages (required for Identity UI scaffolding)
app.MapRazorPages();

app.Run();
