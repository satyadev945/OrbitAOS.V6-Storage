using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OrbitAOS.V6.Models;
using System.Diagnostics;
using System.Text;

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
            // Async non-blocking cache retrieval via AWS ElastiCache (Redis)
            var cacheKey = "home_index_data";
            var cachedData = await _cache.GetAsync(cacheKey);
            if (cachedData == null)
            {
                // Cache miss: set a placeholder value with a sliding expiration
                var dataToCache = Encoding.UTF8.GetBytes("index");
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(5)
                };
                await _cache.SetAsync(cacheKey, dataToCache, cacheOptions);
                _logger.LogInformation("Cache miss for key: {CacheKey}. Data stored in ElastiCache.", cacheKey);
            }
            else
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}.", cacheKey);
            }

            return View();
        }

        public async Task<IActionResult> Privacy()
        {
            // Async non-blocking cache retrieval via AWS ElastiCache (Redis)
            var cacheKey = "home_privacy_data";
            var cachedData = await _cache.GetAsync(cacheKey);
            if (cachedData == null)
            {
                var dataToCache = Encoding.UTF8.GetBytes("privacy");
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromMinutes(10)
                };
                await _cache.SetAsync(cacheKey, dataToCache, cacheOptions);
                _logger.LogInformation("Cache miss for key: {CacheKey}. Data stored in ElastiCache.", cacheKey);
            }
            else
            {
                _logger.LogInformation("Cache hit for key: {CacheKey}.", cacheKey);
            }

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            // Async non-blocking error handling with ElastiCache-backed distributed cache
            var requestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            var cacheKey = $"error_{requestId}";
            var cachedData = await _cache.GetAsync(cacheKey);
            if (cachedData == null)
            {
                var dataToCache = Encoding.UTF8.GetBytes(requestId);
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };
                await _cache.SetAsync(cacheKey, dataToCache, cacheOptions);
            }

            return View(new ErrorViewModel { RequestId = requestId });
        }
    }
}
