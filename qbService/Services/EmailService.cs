using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace qbService.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            this._configuration = configuration;
        }
        public Task SendEmailSystem(string userEmail, string Subject, string Body)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_configuration["email"], "QuickBooks Services");
                    message.To.Add(new MailAddress(userEmail));
                    //message.CC.Add(new MailAddress("cc@email.com", "CC Name"));
                    //message.Bcc.Add(new MailAddress("bcc@email.com", "BCC Name"));
                    message.Subject = Subject;
                    message.IsBodyHtml = true;
                    message.Body = Body;

                    using (var client = new SmtpClient("smtp.gmail.com"))
                    {
                        client.Port = 587;
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_configuration["email"], _configuration["emailPass"]);
                        client.EnableSsl = true;

                        client.Send(message);
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return Task.CompletedTask;
        }
    }
}
