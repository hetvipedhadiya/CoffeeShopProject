using FormDemo.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FormDemo.Controllers
{
    [CheckAccess]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Project()
        {
            return View();
        }
        public IActionResult EmpProject()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult createCookie()
        {
            string key = "cookie demo";
            string value = "hello world";
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(1);
           Response.Cookies.Append(key,value, cookieOptions);
            return View("Index");
        }
        public IActionResult readCookie()
        {
            string key = "read cookie";
            var ans = Request.Cookies[key];

            return View();
        }

        public IActionResult removeCookie()
        {
            string key = "cookie remove demo";
            string value = string.Empty;
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Append(key, value, cookieOptions);
            return View("Index");
        }
    }
}
