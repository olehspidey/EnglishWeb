using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EnglishWeb.Core.Models;
using EnglishWeb.Core.Models.DomainModels;
using EnglishWeb.Core.Models.ViewModels;
using EnglishWeb.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EnglishWeb.Controllers
{
    [Route("[controller]")]
    public class TestController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IRepository<Test> _testsRepository;
        private readonly IRepository<TestAnswer> _answersRepository;
        private readonly IRepository<PassedTest> _passedTestsRepository;
        private readonly IMapper _mapper;

        public TestController(UserManager<User> userManager,
            IMapper mapper,
            IRepository<Test> testsRepository,
            IRepository<TestAnswer> answersRepository,
            IRepository<PassedTest> ressedTestsRepository)
        {
            _userManager = userManager;
            _mapper = mapper;
            _testsRepository = testsRepository;
            _answersRepository = answersRepository;
            _passedTestsRepository = ressedTestsRepository;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = UserRoles.UserTeacherAdmin)]
        public async Task<IActionResult> Index(Guid? id)
        {
            if (id == null)
            {
                ModelState.AddModelError("ModelError", "Invalid id");

                return View();
            }

            var test = await _testsRepository.GetByIdAsync(id);

            if (test == null)
            {
                ModelState.AddModelError("ModelError", "Test was not found");

                return View();
            }

            return View(_mapper.Map<Test, TestViewModel>(test));
        }

        [HttpGet("Image/{answerId}")]
        public async Task<IActionResult> Image(Guid answerId)
        {
            var answer = await _answersRepository.GetByIdAsync(answerId);

            if (answer == null)
                return BadRequest("Image was not found");

            return File(answer.Image, "image/jpeg");
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

        [HttpGet("ChooseType")]
        [Authorize(Roles = UserRoles.Teacher)]
        public IActionResult ChooseType(TestType testType)
        {
            switch (testType)
            {
                case TestType.Image:
                    return RedirectToAction(nameof(CreateImage), "Test");
                case TestType.Input:
                    return RedirectToAction(nameof(CreateInput), "Test");
                case TestType.Radio:
                    return RedirectToAction(nameof(CreateRadioAndInput), "Test");
            }

            return View();
        }

        [HttpGet("CreateRadioAndInput")]
        [Authorize(Roles = UserRoles.Teacher)]
        public IActionResult CreateRadioAndInput()
        {
            ViewBag.TestType = TestType.Radio;

            return View();
        }

        [HttpGet("CreateImage")]
        [Authorize(Roles = UserRoles.Teacher)]
        public IActionResult CreateImage()
        {
            ViewBag.TestType = TestType.Image;

            return View();
        }

        [HttpGet("CreateInput")]
        [Authorize(Roles = UserRoles.Teacher)]
        public IActionResult CreateInput()
        {
            ViewBag.TestType = TestType.Input;

            return View("CreateRadioAndInput");
        }

        [HttpPost("Create")]
        [Authorize(Roles = UserRoles.Teacher)]
        public async Task<IActionResult> Create([FromForm] CreateTestViewModel model)
        {
            try
            {
                model.Questions = JsonConvert.DeserializeObject<List<QuestionViewModel>>(model.QStringified);
            }
            catch (Exception)
            {
                return BadRequest("Invalid model. Invalid questions");
            }

            if (model.Type == TestType.Image)
            {
                if (model.Images == null)
                    return BadRequest("Invalid model. Please add all images");

                var index = 0;

                model.Questions.ForEach(question =>
                {
                    question.Answers.ForEach(async answer =>
                    {
                        using (var stream = model.Images[index].OpenReadStream())
                        {
                            answer.Image = new byte[stream.Length];
                            await stream.ReadAsync(answer.Image);
                        }
                        index++;
                    });
                });
            }

            if (!ModelState.IsValid)
                return BadRequest("Invalid model");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return BadRequest("User was not found");

            var test = _mapper.Map<CreateTestViewModel, Test>(model);

            test.User = user;
            test.UserId = user.Id;
            user.Tests.Add(test);

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
                return Json("Success");

            Response.StatusCode = (int)HttpStatusCode.BadRequest;
            return Json("Can't save");
        }

        [HttpPost("Pass")]
        [Authorize(Roles = UserRoles.UserTeacherAdmin)]
        public async Task<IActionResult> Pass(PassTestViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Inalid model");

            var test = await _testsRepository.GetByIdAsync(model.Id);

            if (test == null)
                return BadRequest("Test was not found");

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return BadRequest("User was not found");

            var result = (0, 0, 0);

            if (model.Type == TestType.Radio || model.Type == TestType.Image)
                result = GetPassRadioResult(test.Questions, model.AnswersId);
            if (model.Type == TestType.Input)
                result = GetPassInputResult(test.Questions, model.Answers);

            var existedTest = user.PassedTests.FirstOrDefault((passedTest => passedTest.TestId == test.Id));

            if (existedTest != null)
            {
                existedTest.FalseAnswersCount = result.Item1;
                existedTest.TrueAnswersCount = result.Item2;

                await _passedTestsRepository.UpdateAsync(existedTest);
            }
            else
                await _passedTestsRepository.InsertAsync(PassedTest.CreateFromTest(result.Item2, result.Item1, test, user));

            return Json(new PassedTestResultViewModel
            {
                FalseCount = result.Item1,
                TrueCount = result.Item2,
                QuestionsCount = result.Item3
            });
        }

        private static (int, int, int) GetPassRadioResult(List<Question> questions, List<Guid> answersId)
        {
            var falseCount = 0;
            var trueCount = 0;

            questions.ForEach(question =>
            {
                var trueAnswers = question.Answers.Where(answer => answer.IsTrue);
                var userAnswer = trueAnswers.FirstOrDefault(answer => answersId.Contains(answer.Id));

                if (userAnswer == null)
                    falseCount++;
                else
                    trueCount++;
            });

            return (falseCount, trueCount, questions.Count);
        }

        private static (int, int, int) GetPassInputResult(List<Question> questions, List<string> answers)
        {
            var falseCount = 0;
            var trueCount = 0;

            questions.ForEach(question =>
            {
                var trueAnswers = question.Answers.Where(answer => answer.IsTrue);
                var userAnswer = trueAnswers.FirstOrDefault(answer => answers.Contains(answer.Text));

                if (userAnswer == null)
                    falseCount++;
                else
                    trueCount++;
            });

            return (falseCount, trueCount, questions.Count);
        }
    }
}