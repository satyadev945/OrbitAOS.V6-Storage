using OrbitAOS.V6.Application;
using OrbitAOS.V6.Application.Common;
using OrbitAOS.V6.Application.Interfaces;
using OrbitAOS.V6.Application.Services;
using OrbitAOS.V6.Infrastructure;
using OrbitAOS.V6.Filters;
using OrbitAOS.V6.Middleware;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Infrastructure layer (Database, Identity, Repositories)
builder.Services.AddInfrastructure(builder.Configuration);

// Application layer (AutoMapper, FluentValidation, MediatR)
builder.Services.AddApplication();

// Application layer (Business logic, Services)
builder.Services.AddScoped<ISampleService, SampleService>();

// FluentValidation
builder.Services.AddFluentValidationAutoValidation()
    .AddFluentValidationClientsideAdapters();

// MVC Controllers with Views
builder.Services.AddControllersWithViews(options =>
{
    // Add global exception filter
    options.Filters.Add<GlobalExceptionFilter>();
    // Add validation filter
    options.Filters.Add<ValidationFilter>();
});

// Response compression
builder.Services.AddResponseCompression();

// Razor Pages (for Identity UI)
builder.Services.AddRazorPages();

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

// Custom middleware
app.UseRequestLogging();

// Response compression
app.UseResponseCompression();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
