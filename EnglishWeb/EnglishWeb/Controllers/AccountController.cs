using EnglishWeb.Core.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace EnglishWeb.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterUserVIewModel model)
        {
            return View(model);
        }
    }
}