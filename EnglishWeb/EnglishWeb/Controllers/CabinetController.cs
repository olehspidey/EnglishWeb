using System.Threading.Tasks;
using EnglishWeb.BLL.Services.Abstract;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnglishWeb.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class CabinetController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSendingService _emailSendingService;

        public CabinetController(UserManager<User> userManager, IEmailSendingService emailSendingService)
        {
            _userManager = userManager;
            _emailSendingService = emailSendingService;
        }

        [HttpGet("{tab:int}/{isSentRes?}")]
        public IActionResult Index(int tab = 1, bool isSentRes = false)
        {
            if (tab > 3)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            if (isSentRes)
                ViewBag.IsSentToken = true;

            return View();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Model", "Invalid model");

                return RedirectToAction("Index", new { tab = 1, isSentRes = true });
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var resetPasResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!resetPasResult.Succeeded)
                ModelState.AddModelError("UserError", "Can't reset password");

            return RedirectToAction("Index", new {tab = 1, isSentRes = false});
        }

        [HttpPost("SendChangePasToken")]
        public async Task<IActionResult> SendChangePasToken(string redirect)
        {
            if (!ModelState.IsValid)
                return Redirect(redirect);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var resetPasToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSendingService.SendAsync(user.Email,
                "Reset password token",
                resetPasToken);

            return Redirect(redirect);
        }
    }
}