using System.Threading.Tasks;
using AutoMapper;
using EnglishWeb.Core.Models;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using IdentityRole = EnglishWeb.Core.Models.DomainModels.IdentityRole;

namespace EnglishWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpGet]
        public IActionResult Login() => View();

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUserVIewModel model)
        {
            if (!ModelState.IsValid)
                return View();

            var mapperUser = _mapper.Map<RegisterUserVIewModel, User>(model);

            if (model.UserRole == UserRoles.User)
                mapperUser.IsActive = true;

            var createdResult = await _userManager.CreateAsync(mapperUser, model.Password);

            if (!createdResult.Succeeded)
            {
                ModelState.AddModelError("Register", "Can't create user");
                return View();
            }

            if (!await _roleManager.RoleExistsAsync(model.UserRole))
                await _roleManager.CreateAsync(new IdentityRole(model.UserRole));

            await _userManager.AddToRoleAsync(mapperUser, UserRoles.User);
            if (model.UserRole == UserRoles.User)
                await _signInManager.SignInAsync(mapperUser, false);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Model", "Invalid model");

                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("UserError", "User was not found");

                return View();
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                ModelState.AddModelError("PasswordError", "Incorrect password");

                return View();
            }

            await _signInManager.SignInAsync(user, false);

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
    }
}