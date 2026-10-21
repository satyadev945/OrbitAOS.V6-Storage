using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OrbitAOS.V6.Models;
using System.Diagnostics;
using System.Text.Json;

namespace OrbitAOS.V6.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDistributedCache _cache;

        public HomeController(ILogger<HomeController> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<IActionResult> Index()
        {
            // Async non-blocking data retrieval backed by AWS ElastiCache (Redis)
            var cacheKey = "home:index:data";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData == null)
            {
                _logger.LogInformation("Cache miss for key '{CacheKey}'. Fetching data.", cacheKey);
                // Simulate async data retrieval (replace with actual async data source)
                var data = new { Message = "Welcome to OrbitAOS", Timestamp = DateTime.UtcNow };
                cachedData = JsonSerializer.Serialize(data);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(2)
                };
                await _cache.SetStringAsync(cacheKey, cachedData, cacheOptions);
            }
            else
            {
                _logger.LogInformation("Cache hit for key '{CacheKey}'.", cacheKey);
            }

            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            // Async non-blocking data retrieval backed by AWS ElastiCache (Redis)
            var cacheKey = "home:privacy:data";
            string? cachedData = await _cache.GetStringAsync(cacheKey);

            if (cachedData == null)
            {
                _logger.LogInformation("Cache miss for key '{CacheKey}'. Fetching data.", cacheKey);
                var data = new { Page = "Privacy Policy", Timestamp = DateTime.UtcNow };
                cachedData = JsonSerializer.Serialize(data);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };
                await _cache.SetStringAsync(cacheKey, cachedData, cacheOptions);
            }
            else
            {
                _logger.LogInformation("Cache hit for key '{CacheKey}'.", cacheKey);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            // Async non-blocking error view rendering backed by AWS ElastiCache (Redis)
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var cacheKey = $"home:error:{requestId}";

            string? cachedError = await _cache.GetStringAsync(cacheKey);
            if (cachedError == null)
            {
                _logger.LogWarning("Rendering error view for RequestId '{RequestId}'.", requestId);
                var errorData = new ErrorViewModel { RequestId = requestId };
                cachedError = JsonSerializer.Serialize(errorData);

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };
                await _cache.SetStringAsync(cacheKey, cachedError, cacheOptions);
            }

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
