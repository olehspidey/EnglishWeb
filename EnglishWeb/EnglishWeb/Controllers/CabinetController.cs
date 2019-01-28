using System.Threading.Tasks;
using EnglishWeb.BLL.Services.Abstract;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using EnglishWeb.Extensions;
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

        [HttpGet("{tab:int}")]
        public IActionResult Index(int tab = 1)
        {
            if (tab > 3)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            return View();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var resetPasResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!resetPasResult.Succeeded)
                ModelState.AddModelError("UserError", "Can't reset password");

            return PartialView("_ResetPasswordPartial");
        }

        [HttpPost("SendChangePasToken")]
        public async Task<IActionResult> SendChangePasToken()
        {
            if (!ModelState.IsValid)
                return PartialView("_SentChangePasToken");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var resetPasToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSendingService.SendAsync(user.Email,
                "Reset email token",
                resetPasToken);

            return PartialView("_ResetPasswordPartial");
        }
    }
}