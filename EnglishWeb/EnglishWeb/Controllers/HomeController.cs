﻿using System.Diagnostics;
using AutoMapper;
using EnglishWeb.Core.Models.DomainModels;
using Microsoft.AspNetCore.Mvc;
using EnglishWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace EnglishWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public HomeController(UserManager<User> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
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
