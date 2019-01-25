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
        [Authorize(Roles = UserRoles.Admin)]
        public async Task<IActionResult> Users()
            => View(_mapper.Map<List<User>, List<ShortUserViewModel>>(await _userManager
                .Users
                .Take(20)
                .ToListAsync()));
    }
}