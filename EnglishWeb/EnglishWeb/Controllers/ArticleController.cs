using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EnglishWeb.Core.Models;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using EnglishWeb.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EnglishWeb.Controllers
{
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

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = UserRoles.Teacher)]
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
        [HttpGet("[controller]/Create/{success?}")]
        public IActionResult Create(bool? success)
        {
            if (success == true)
                ViewBag.Success = true;

            return View();
        }

        [Authorize(Roles = UserRoles.Teacher)]
        [HttpPost]
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