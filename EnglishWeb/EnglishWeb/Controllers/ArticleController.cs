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

        //[HttpGet("List")]
        //public async Task<IActionResult> List()
        //{
        //    var articles = await _articleRepository
        //        .Table
        //        .Take(20)
        //        .ToListAsync();

        //    return View(_mapper.Map<List<Article>, List<ArticleViewModel>>(articles));
        //}

        // todo issue
        [HttpGet("List")]
        public async Task<IActionResult> List(Language language, ArticleType type, string query)
        {
            var articles = _articleRepository
                .Table
                .Where(a => a.Language == language && a.Type == type);

            if (!string.IsNullOrWhiteSpace(query))
                articles = articles
                    .Where(a => a.User.Name.Contains(query) || a.User.LastName.Contains(query));

            RouteData.Values["language"] = language;
            RouteData.Values["type"] = type;
            RouteData.Values["query"] = query;

            return View(_mapper.Map<List<Article>, List<ArticleViewModel>>(await articles.ToListAsync()));
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Index(Guid? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("ArticleError", "Icorrect article");

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var article = await _articleRepository.GetByIdAsync(id);

            if (article == null)
                return RedirectToAction(nameof(HomeController.Error), "Home");

            return View(_mapper.Map<Article, ArticleViewModel>(article));
        }

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpGet("My")]
        public async Task<IActionResult> My()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return RedirectToAction(nameof(HomeController.Error), "Home");

            return View(_mapper.Map<List<Article>, List<ArticleViewModel>>(user.Articles));
        }

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpGet("Create")]
        public IActionResult Create()
        {
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
                return RedirectToAction(nameof(HomeController.Error), "Home");

            var article = _mapper.Map<CreateArticleViewModel, Article>(model);

            article.User = user;
            article.UserId = user.Id;

            var insertRes = await _articleRepository.InsertAsync(article);

            if (insertRes >= 0)
            {
                ViewBag.Success = true;
                return RedirectToAction(nameof(Create), "Article");
            }

            ViewBag.Success = true;
            return RedirectToAction(nameof(Create), "Article");
        }
    }
}