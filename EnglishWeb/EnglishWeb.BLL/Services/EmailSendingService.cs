using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using EnglishWeb.BLL.Services.Abstract;
using Microsoft.Extensions.Configuration;

namespace EnglishWeb.BLL.Services
{
    public class EmailSendingService : IEmailSendingService
    {
        private readonly SmtpClient _smtpClient;
        private readonly IConfiguration _configuration;

        public NetworkCredential Credential { get; set; }

        public EmailSendingService(IConfiguration configuration)
        {
            _configuration = configuration;
            _smtpClient = new SmtpClient(configuration["Host"], int.Parse(configuration["Port"]))
            {
                EnableSsl = true
            };
            Credential = new NetworkCredential(configuration["Email"], configuration["Password"]);
            _smtpClient.Credentials = Credential;
        }

        public async Task<bool> SendAsync(string emailTo, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage(_configuration["Email"], emailTo, subject, message)
                {
                    IsBodyHtml = true
                };

                await _smtpClient.SendMailAsync(mailMessage);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
