using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EnglishWeb.BLL.Services.Abstract;
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
    [Authorize]
    public class CabinetController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailSendingService _emailSendingService;
        private readonly IRepository<Chat> _chatRepository;
        private readonly IRepository<Message> _messagesRepository;

        public CabinetController(UserManager<User> userManager,
            IEmailSendingService emailSendingService,
            IRepository<Chat> chatRepository,
            IRepository<Message> messagesRepository)
        {
            _userManager = userManager;
            _emailSendingService = emailSendingService;
            _chatRepository = chatRepository;
            _messagesRepository = messagesRepository;
        }

        [HttpGet("{tab:int}/{isSentRes?}/{sentTo?}/{chatId?}")]
        public async Task<IActionResult> Index(int tab = 1, bool isSentRes = false, Guid? sentTo = null, Guid? chatId = null)
        {
            if (tab > 3)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            if (isSentRes)
                ViewBag.IsSentToken = true;

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var isTeacher = User.IsInRole(UserRoles.Teacher);

            if (tab == 2 && sentTo != null && !isSentRes)
            {
                var desUser = await _userManager.FindByIdAsync(sentTo.ToString());
                
                Chat chat = null;

                if (chatId == null && !isTeacher)
                    chat = await _chatRepository.Table.FirstOrDefaultAsync(c => c.UserDestinationId == sentTo && c.UserOwnerId == user.Id);
                if(chat == null && chatId != null)
                   chat = await _chatRepository
                       .Table
                       .FirstOrDefaultAsync(c => c.Id == chatId);
                else if(chat == null)
                {
                    chat = new Chat
                    {
                        Messages = new List<Message>(),
                        Name = $"{user.UserName} {user.LastName} - {desUser.Name} {desUser.LastName}",
                        UserDestinationId = desUser.Id,
                        UserOwner = user,
                        UserOwnerId = user.Id
                    };
                    await _chatRepository.InsertAsync(chat);
                }

                var messages = chat
                    .Messages
                    .Select(message => new MessageViewModel
                {
                    DestinationId = message.UserDestinationId,
                    OwnerId = message.UserOwnerId,
                    Text = message.Body,
                    CurrentUserId = user.Id,
                    CreateTime = message.CreateTime
                })
                    .OrderBy(model => model.CreateTime)
                    .ToList();

                RouteData.Values["chatId"] = chat.Id;

                return View(messages);
            }

            if (tab == 2 && sentTo == null && !isSentRes)
            {
                var chats = await _chatRepository
                    .Table
                    .Where(chat => isTeacher ? chat.UserDestinationId == user.Id : chat.UserOwnerId == user.Id)
                    .Select(chat => new ChatViewModel
                    {
                        Id = chat.Id,
                        Name = chat.Name,
                        UserDestinationId = chat.UserDestinationId
                    })
                    .ToListAsync();

                return View(chats);
            }

            return View();
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(ResetPasViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("Model", "Invalid model");

                return RedirectToAction("Index", new { tab = 1, isSentRes = true });
            }

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if(user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var resetPasResult = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (!resetPasResult.Succeeded)
                ModelState.AddModelError("UserError", "Can't reset password");

            return RedirectToAction("Index", new {tab = 1, isSentRes = false});
        }

        [HttpPost("SendChangePasToken")]
        public async Task<IActionResult> SendChangePasToken(string redirect)
        {
            if (!ModelState.IsValid)
                return Redirect(redirect);

            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var resetPasToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailSendingService.SendAsync(user.Email,
                "Reset password token",
                resetPasToken);

            return Redirect(redirect);
        }

        [HttpPost("SentMessage")]
        public async Task<IActionResult> SendMessage(SendMessageViewModel model)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var destUser = await _userManager.FindByIdAsync(model.ToId.ToString());

            if(destUser == null)
                return RedirectToAction(nameof(HomeController.NotFound), "Home");

            var chat = await _chatRepository
                .Table
                .FirstOrDefaultAsync(c => c.Id == model.ChatId);

            if (chat == null)
            {
                chat = new Chat
                {
                    Name = $"{user.UserName} {user.LastName} - {destUser.Name} {destUser.LastName}",
                    Messages = new List<Message>(),
                    UserDestinationId = destUser.Id,
                    UserOwnerId = user.Id
                };

                await _chatRepository.InsertAsync(chat);
            }

            await _messagesRepository.InsertAsync(new Message
            {
                Name = $"{user.UserName} {user.LastName} - {destUser.Name} {destUser.LastName}",
                Body = model.Text,
                Chat = chat,
                ChatId = chat.Id,
                UserOwnerId = user.Id,
                UserDestinationId = destUser.Id
            });

            return RedirectToAction("Index", new {tab = 2, isSentRes = false, sentTo = destUser.Id, chatId = chat.Id});
        }
    }
}