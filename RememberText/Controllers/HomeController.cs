using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using RememberText.Data;
using RememberText.Models;
using RememberText.RTTools;
using System.Diagnostics;

namespace RememberText.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCache _cache;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _cache = cache;
        }

        public IActionResult Index()
        {
            string ipAddress = "None";
            if (!_cache.TryGetValue("83.248.115.115", out ipAddress))
            {
                
            }

            ViewBag.CacheIpAddress = ipAddress;

            ViewBag.ipadress = ValidateHelpers.getIPAddress(_httpContextAccessor);

            return View(TestData.IpAddresses);
        }

        public IActionResult Introduction()
        {
            ViewBag.ipadress = ValidateHelpers.getIPAddress(_httpContextAccessor);
            return View(TestData.IpAddresses);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
