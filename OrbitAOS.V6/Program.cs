using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using Microsoft.AspNetCore.StaticFiles;
using WebOptimizer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configure AWS ElastiCache (Redis) as distributed cache for async non-blocking data retrieval
var redisConnectionString = builder.Configuration.GetConnectionString("RedisConnection")
    ?? Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING")
    ?? "localhost:6379";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = redisConnectionString;
    options.InstanceName = builder.Configuration["Redis:InstanceName"] ?? "OrbitAOS_";
});

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// -----------------------------------------------------------------------
// ResponseCompression: Enable Gzip and Brotli compression for Razor HTML
// responses to reduce AWS egress bandwidth costs on Elastic Beanstalk/ECS.
// Brotli is preferred by modern browsers; Gzip is the fallback.
// -----------------------------------------------------------------------
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/html",
        "text/plain",
        "text/css",
        "application/javascript",
        "application/json",
        "image/svg+xml"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.SmallestSize;
});

// -----------------------------------------------------------------------
// WebOptimizer: Bundle and minify CSS/JS assets for AWS CloudFront delivery
// LigerShark.WebOptimizer.Core produces optimized, fingerprinted bundles
// that CloudFront caches at the edge, reducing cloud egress costs.
// -----------------------------------------------------------------------
builder.Services.AddWebOptimizer(pipeline =>
{
    // Bundle and minify all site CSS into a single optimized bundle
    pipeline.AddCssBundle("/css/bundle.min.css",
        "lib/bootstrap/dist/css/bootstrap.min.css",
        "css/site.css");

    // Bundle and minify all site JS into a single optimized bundle
    pipeline.AddJavaScriptBundle("/js/bundle.min.js",
        "lib/jquery/dist/jquery.min.js",
        "lib/bootstrap/dist/js/bootstrap.bundle.min.js",
        "js/site.js");

    // Bundle and minify validation scripts into a separate bundle
    pipeline.AddJavaScriptBundle("/js/validation.min.js",
        "lib/jquery-validation/dist/jquery.validate.min.js",
        "lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js");
}, minifyJavaScript: true, minifyCss: true, enableCaching: true, enableMemoryCache: true, enableDiskCache: false, allowEmptyBundle: false);

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

// UseResponseCompression must be placed early in the pipeline — before
// UseStaticFiles and UseRouting — so that all responses (HTML, JSON, etc.)
// are compressed before being sent to the client, reducing AWS egress costs.
app.UseResponseCompression();

// WebOptimizer middleware must be registered before UseStaticFiles so that
// bundled/minified asset requests are intercepted and served with fingerprinted URLs.
app.UseWebOptimizer();

// Configure Static File Middleware with Cache-Control headers for AWS CloudFront CDN
// Versioned/fingerprinted assets (js, css, images) use long-lived immutable caching
// Non-versioned assets use a shorter max-age to allow periodic revalidation
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        var headers = ctx.Context.Response.Headers;
        var contentType = ctx.Context.Response.ContentType ?? string.Empty;

        // Long-lived immutable cache for versioned static assets (JS, CSS, images, fonts)
        if (contentType.Contains("javascript") || contentType.Contains("css") ||
            contentType.Contains("image") || contentType.Contains("font"))
        {
            // 1 year cache, immutable — CloudFront will cache these at the edge
            headers["Cache-Control"] = "public, max-age=31536000, immutable";
        }
        else
        {
            // 1 hour cache for other static assets; CloudFront respects this TTL
            headers["Cache-Control"] = "public, max-age=3600, must-revalidate";
        }

        // Vary header ensures CloudFront caches separate copies per encoding
        headers["Vary"] = "Accept-Encoding";
    }
});

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
