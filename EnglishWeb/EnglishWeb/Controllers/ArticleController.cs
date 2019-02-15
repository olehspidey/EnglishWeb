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
                ModelState.AddModelError("ArticleError", "Incorrect article");

                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            var article = await _articleRepository.GetByIdAsync(id);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user == null)
                return RedirectToAction(nameof(HomeController.Error), "Home");

            if (article == null)
                return RedirectToAction(nameof(HomeController.Error), "Home");

            ViewBag.CanEdit = article.UserId == user.Id;

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
            => View();

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var article = await _articleRepository.GetByIdAsync(id);

            if(article == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            return View(_mapper.Map<Article, EditArticleViewModel>(article));
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

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpPost("Edit")]
        public async Task<IActionResult> EditArticle(EditArticleViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Edit", model);

            var article = await _articleRepository.GetByIdAsync(model.Id);

            if (article == null)
            {
                ModelState.AddModelError("ArticleError", "Article don't exist");

                return View("Edit", model);
            }

            article.Language = model.Language;
            article.Name = model.Name;
            article.Text = model.Text;
            article.Type = model.Type;

            await _articleRepository.UpdateAsync(article);

            return RedirectToAction(nameof(Index), new {id = model.Id});
        }

        [Authorize(Roles = UserRoles.Admin)]
        [HttpPost("Delete/{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var article = await _articleRepository.GetByIdAsync(id);

            if (article == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            await _articleRepository.DeleteAsync(article);

            return
                RedirectToAction(nameof(List));
        }
    }
}