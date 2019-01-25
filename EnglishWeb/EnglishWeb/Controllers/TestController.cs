using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EnglishWeb.Core.Models;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnglishWeb.Controllers
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public TestController(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("My")]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                ModelState.AddModelError("UserError", "User was not found");

                return View();
            }

            return View(_mapper.Map<List<Test>, List<TestViewModel>>(user.Tests));
        }

        [HttpGet("Create/{success?}")]
        public IActionResult Create(bool? success)
        {
            if (success == true)
                ViewBag.Success = true;

            return View();
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateTestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("ModelError", "Invalid model");

                return View();
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                ModelState.AddModelError("UserError", "User was not found");

                return View();
            }

            var test = _mapper.Map<CreateTestViewModel, Test>(model);

            test.User = user;
            test.UserId = user.Id;
            user.Tests.Add(test);

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
                return RedirectToAction(nameof(TestController.Create), "Test", new {success = true});
            return RedirectToAction(nameof(TestController.Create), "Test", new { success = false });
        }
    }
}