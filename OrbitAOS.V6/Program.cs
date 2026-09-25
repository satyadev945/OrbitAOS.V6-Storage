using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using OrbitAOS.V6.Data;
using WebOptimizer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

// -----------------------------------------------------------------------
// cr-dotnet-1016: ResponseCompression — Gzip/Brotli for AWS Elastic Beanstalk/ECS
// -----------------------------------------------------------------------
// Enable ASP.NET Core ResponseCompression middleware with both Brotli and Gzip
// providers to reduce egress bandwidth costs on AWS-hosted ASP.NET applications.
// Brotli is preferred (higher compression ratio); Gzip is the fallback for clients
// that do not advertise Brotli support via the Accept-Encoding request header.
// HTML, CSS, JavaScript, JSON, XML, plain-text, and SVG MIME types are all
// compressed, covering Razor-rendered HTML responses and API payloads alike.
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[]
    {
        "text/html",
        "text/css",
        "application/javascript",
        "application/json",
        "application/xml",
        "text/plain",
        "image/svg+xml"
    });
});
builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest;
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.SmallestSize;
});

// Register AWS ElastiCache (Redis) as the distributed cache provider for async/non-blocking
// data retrieval in Razor view controllers (cr-dotnet-1000 remediation).
// The connection string is read from the ELASTICACHE_REDIS_CONNECTION_STRING environment variable
// at runtime in AWS, falling back to the appsettings.json value for local development.
var elastiCacheConnectionString =
    Environment.GetEnvironmentVariable("ELASTICACHE_REDIS_CONNECTION_STRING")
    ?? builder.Configuration["ElastiCache:ConnectionString"]
    ?? "localhost:6379";

var elastiCacheInstanceName =
    builder.Configuration["ElastiCache:InstanceName"] ?? "OrbitAOS_";

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = elastiCacheConnectionString;
    options.InstanceName = elastiCacheInstanceName;
});

// -----------------------------------------------------------------------
// cr-dotnet-1011: WebOptimizer — Bundle & Minify CSS/JS for AWS CloudFront
// -----------------------------------------------------------------------
// LigerShark WebOptimizer bundles and minifies CSS and JavaScript assets at
// startup, producing optimised bundles that are served with content-hash
// fingerprinted URLs.  AWS CloudFront is configured to cache these bundles
// at the edge using the long-lived Cache-Control headers set below, which
// dramatically reduces cloud egress costs and improves page-load times.
//
// Bundle paths must match the <link> / <script> tags in _Layout.cshtml.
// The tag helpers provided by WebOptimizer automatically append a ?v=<hash>
// query string so CloudFront treats each unique bundle as an immutable asset.
builder.Services.AddWebOptimizer(pipeline =>
{
    // --- CSS bundles ---
    // Bundle all Bootstrap CSS into a single minified file.
    pipeline.AddCssBundle(
        "/css/bootstrap.min.css",
        "lib/bootstrap/dist/css/bootstrap.css");

    // Bundle the application's own stylesheet(s).
    pipeline.AddCssBundle(
        "/css/site.min.css",
        "css/site.css");

    // --- JavaScript bundles ---
    // Bundle jQuery into a single minified file.
    pipeline.AddJavaScriptBundle(
        "/js/jquery.min.js",
        "lib/jquery/dist/jquery.js");

    // Bundle Bootstrap JS (includes Popper).
    pipeline.AddJavaScriptBundle(
        "/js/bootstrap.bundle.min.js",
        "lib/bootstrap/dist/js/bootstrap.bundle.js");

    // Bundle jQuery Validation and unobtrusive validation together.
    pipeline.AddJavaScriptBundle(
        "/js/jquery.validate.bundle.min.js",
        "lib/jquery-validation/dist/jquery.validate.js",
        "lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.js");

    // Bundle the application's own scripts.
    pipeline.AddJavaScriptBundle(
        "/js/site.min.js",
        "js/site.js");
});

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

// cr-dotnet-1016: UseResponseCompression must be placed at the top of the middleware
// pipeline — before UseHttpsRedirection, UseStaticFiles, UseWebOptimizer, and UseRouting —
// so that ALL responses (Razor HTML pages, API JSON, static assets) are compressed before
// being written to the response stream.  This reduces AWS egress bandwidth costs.
app.UseResponseCompression();

app.UseHttpsRedirection();

// WebOptimizer middleware must be registered BEFORE UseStaticFiles so that
// bundle requests are intercepted and served from the in-memory pipeline
// rather than falling through to the raw wwwroot files.
app.UseWebOptimizer();

// Configure Static File Middleware with Cache-Control headers for AWS CloudFront CDN.
// Versioned assets (files with a fingerprint/hash in the URL query string or path) receive
// a long-lived immutable cache directive so CloudFront and browsers cache them aggressively.
// All other static files receive a shorter max-age to allow periodic revalidation.
// CloudFront respects these Cache-Control headers and forwards them to edge caches,
// reducing origin load and cloud bandwidth costs (cr-dotnet-1010 remediation).
var contentTypeProvider = new FileExtensionContentTypeProvider();

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = contentTypeProvider,
    OnPrepareResponse = ctx =>
    {
        var headers = ctx.Context.Response.Headers;
        var path = ctx.File.Name;

        // Determine whether this is a cache-busted / fingerprinted asset.
        // ASP.NET Core's tag helpers append a ?v=<hash> query string to versioned assets.
        // Additionally, files whose names contain a content hash (e.g. site.abc123.min.js)
        // are treated as immutable.
        var requestPath = ctx.Context.Request.Path.Value ?? string.Empty;
        var hasVersionQuery = ctx.Context.Request.Query.ContainsKey("v");
        var isVersionedPath = requestPath.Contains(".min.", StringComparison.OrdinalIgnoreCase)
                              || requestPath.Contains("-", StringComparison.OrdinalIgnoreCase);

        if (hasVersionQuery || isVersionedPath)
        {
            // Versioned / fingerprinted assets: cache for 1 year, mark immutable.
            // CloudFront will cache these at the edge for the full TTL.
            headers["Cache-Control"] = "public, max-age=31536000, immutable";
        }
        else
        {
            // Non-versioned assets: cache for 1 hour, allow revalidation.
            // CloudFront will respect this TTL and revalidate with the origin after expiry.
            headers["Cache-Control"] = "public, max-age=3600, must-revalidate";
        }
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
