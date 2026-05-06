using Microsoft.Extensions.Configuration;
using OrderPoint.Application.Interfaces.Email;
using OrderPoint.Application.Services.EmailTemplates;
using OrderPoint.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OrderPoint.Application.Services.Email
{
    public class EmailAppService : IEmailAppService
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailTemplatesRepository _EmailTemplatesRepository;
        public EmailAppService(IConfiguration configuration , IEmailTemplatesRepository emailTemplatesRepository)
        {
            _configuration = configuration;
            _EmailTemplatesRepository = emailTemplatesRepository;
        }


        public async Task SendEmailAsync(string email, string ccEmail , string bccEmail, string subject,  string message)
        {
            var settingList = await _EmailTemplatesRepository.GetSystemSettingList();
            var query = settingList.Item2.ToList();

          
            var sMTPSetting = query.Where(l=>l.Type== "SMTP").ToDictionary(s => s.Key, s => s.Value);

            string Port = sMTPSetting["Port"];
            string FromEmail = sMTPSetting["Mail From"];
            string Password = sMTPSetting["Password"];
            string adminEmail = sMTPSetting["Admin Email"];
            string Host = sMTPSetting["Host"];//
            string UserName = sMTPSetting["Username"];

            //string FromEmail= _configuration["EmailCredential:FromEmail"];
            //string Host = _configuration["EmailCredential:Host"];
            //string UserName = _configuration["EmailCredential:UserName"];
            //string Password = _configuration["EmailCredential:Password"];
            //string Port = _configuration["EmailCredential:Port"];

            MailMessage mail = new MailMessage();
            mail.To.Add(email);
            mail.From = new MailAddress(FromEmail);
             
            if (!string.IsNullOrWhiteSpace(bccEmail))
            {
                foreach (var e in bccEmail.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    mail.Bcc.Add(new MailAddress(e.Trim()));
                }
            }

            if (!string.IsNullOrEmpty(ccEmail))
            {
                string[] ccid = ccEmail.Split(',');
                foreach (var item in ccid)
                {
                    mail.CC.Add(new MailAddress(item));
                }
            }
            mail.Subject = subject;
            mail.Body = message;
            mail.IsBodyHtml = true;

            //SmtpClient smtp = new SmtpClient(Host, 587);
            //SmtpClient smtp = new SmtpClient(Host, Convert.ToInt32(Port));
            //smtp.EnableSsl = true;
            //smtp.UseDefaultCredentials = false;
            //smtp.Credentials = new System.Net.NetworkCredential(UserName, Password);
            SmtpClient smtp = new SmtpClient(Host, Convert.ToInt32(Port))
            {
                EnableSsl = true,
                 
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(UserName, Password)
            };
            try
            {

                smtp.Send(mail);
            }
            catch (Exception e)
            {

            }
        }
    }
}
