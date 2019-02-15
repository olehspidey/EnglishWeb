using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using EnglishWeb.Models;

namespace EnglishWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
            => View();

        public IActionResult Privacy()
            => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public new IActionResult NotFound()
            => View();
    }
}
