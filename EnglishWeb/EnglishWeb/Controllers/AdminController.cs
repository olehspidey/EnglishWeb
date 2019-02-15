using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnglishWeb.Core.Models;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishWeb.Controllers
{
    [Route("[controller]")]
    [Authorize(Roles = UserRoles.Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public AdminController(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        [HttpGet("Users")]
        public async Task<IActionResult> Users()
            => View(_mapper.Map<List<User>, List<ShortUserViewModel>>(await _userManager
                .Users
                .Take(20)
                .ToListAsync())
               );

        [HttpPost("Activate/{id}")]
        public async Task<IActionResult> Activate(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            user.IsActive = true;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Users), "Admin");
        }

        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());

            if (user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Users), "Admin");
        }
    }
}