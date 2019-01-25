using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EnglishWeb.Core.Models;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using EnglishWeb.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EnglishWeb.Controllers
{
    [Route("[controller]")]
    public class ArticleController : Controller
    {
        private readonly IRepository<Article> _articleRepository;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public ArticleController(IRepository<Article> articleRepository, IMapper mapper, UserManager<User> userManager)
        {
            _articleRepository = articleRepository;
            _mapper = mapper;
            _userManager = userManager;
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var articles = await _articleRepository
                .Table
                .Take(20)
                .ToListAsync();

            return View(_mapper.Map<List<Article>, List<ArticleViewModel>>(articles));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.UserTeacherAdmin)]
        public async Task<IActionResult> Index(Guid? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("ArticleError", "Icorrect article");

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var article = await _articleRepository.GetByIdAsync(id);

            if (article == null)
            {
                ModelState.AddModelError("ArticleError", "Article was not found");

                return View();
            }

            return View(_mapper.Map<Article, ArticleViewModel>(article));
        }

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpGet("My")]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                ModelState.AddModelError("UserError", "User was not found");

                return View();
            }

            return View(_mapper.Map<List<Article>, List<ArticleViewModel>>(user.Articles));
        }

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpGet("Create/{success?}")]
        public IActionResult Create(bool? success)
        {
            if (success == true)
                ViewBag.Success = true;

            return View();
        }

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpPost("Create")]
        public async Task<IActionResult> Create(CreateArticleViewModel model)
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

            var article = _mapper.Map<CreateArticleViewModel, Article>(model);

            article.User = user;
            article.UserId = user.Id;

            var insertRes = await _articleRepository.InsertAsync(article);

            if (insertRes >= 0)
                return RedirectToAction(nameof(ArticleController.Create), "Article", new {success = true});

            return RedirectToAction(nameof(ArticleController.Create), "Article", new { success = false });
        }
    }
}