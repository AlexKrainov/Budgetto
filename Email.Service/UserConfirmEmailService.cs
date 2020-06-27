using Email.Service.EmailEnvironment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Email.Service
{
    public class UserConfirmEmailService
    {
        private IBaseRepository _repository;
        private IEmailSender _emailSender;
        private IHostingEnvironment hostingEnvironment;
        private IHttpContextAccessor httpContextAccessor;

        public UserConfirmEmailService(
            IBaseRepository repository,
            IEmailSender emailSender,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            this._repository = repository;
            this._emailSender = emailSender;
            this.hostingEnvironment = hostingEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<int> ConfirmEmail(bool isNewUser)
        {
            string body = string.Empty;
            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = (int)MailTypeEnum.ConfirmEmail,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = UserInfo.Current.ID,
            };
            string confirmUrl = @"https://" + httpContextAccessor.HttpContext.Request.Host.Value.ToString() + @"//Identity/Account/ConfirmEmail?id=" + mailLog.ID;

            try
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\ConfirmEmail.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("${Link}", confirmUrl);

                await _emailSender.SendEmailAsync(UserInfo.Current.Email, "Подтверждение почты", body);
            }
            catch (Exception ex)
            {
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;
            }

            await _repository.CreateAsync(mailLog, true);
            return 1;
        }

        public async Task<int> ConfirmEmail_Complete(Guid id)
        {
            var user = UserInfo.Current;
            var mailLog = await _repository.GetAll<MailLog>(x => x.ID == id).FirstOrDefaultAsync();

            mailLog.CameDateTime = DateTime.Now.ToUniversalTime();
            await _repository.UpdateAsync(mailLog, true);

            var dbUser = await _repository.GetAll<User>(x => x.ID == user.ID).FirstOrDefaultAsync();
            dbUser.IsConfirmEmail = true;
            await _repository.UpdateAsync(dbUser, true);

            user.IsConfirmEmail = true;
            await UserInfo.AddOrUpdate_Authenticate(user);

            return 1;
        }
    }
}
