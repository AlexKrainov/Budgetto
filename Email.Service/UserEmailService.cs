using Email.Service.EmailEnvironment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Email.Service
{
    public class UserEmailService
    {
        private IBaseRepository _repository;
        private IEmailSender _emailSender;
        private IHostingEnvironment hostingEnvironment;
        private IHttpContextAccessor httpContextAccessor;

        public UserEmailService(
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

        public async Task<int> ConfirmEmail(User user)
        {
            string body = string.Empty;
            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = (int)MailTypeEnum.ConfirmEmail,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = user.ID,
                Email = user.Email,
            };
            string confirmUrl = @"https://" + httpContextAccessor.HttpContext.Request.Host.Value.ToString() + @"/Identity/Account/ConfirmEmail?id=" + mailLog.ID;

            try
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\ConfirmEmail.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("${Link}", confirmUrl);

                await _emailSender.SendEmailAsync(user.Email, "Подтверждение почты", body);
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

        public async Task<int> ResetPassword(User user)
        {
            string body = string.Empty;
            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = (int)MailTypeEnum.ResetPassword,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = user.ID,
                Email = user.Email,
            };
            string confirmUrl = @"https://" + httpContextAccessor.HttpContext.Request.Host.Value.ToString() + @"/Identity/Account/ResetPassword2?id=" + mailLog.ID;

            try
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\ResetPassword.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("${Link}", confirmUrl);

                await _emailSender.SendEmailAsync(user.Email, "Сброс пароля", body);
            }
            catch (Exception ex)
            {
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;
            }

            await _repository.CreateAsync(mailLog, true);
            return 1;

        }

        public async Task<Guid> ResetPassword_Complete(Guid id)
        {
            var mailLog = await _repository.GetAll<MailLog>(x => x.ID == id).FirstOrDefaultAsync();

            mailLog.CameDateTime = DateTime.Now.ToUniversalTime();

            if ((mailLog.UserID == null || mailLog.UserID == Guid.Empty)
                && !string.IsNullOrEmpty(mailLog.Email))
            {
                mailLog.UserID = await _repository.GetAll<User>(x => x.Email == mailLog.Email).Select(x => x.ID).FirstOrDefaultAsync();
            }

            await _repository.UpdateAsync(mailLog, true);


            return mailLog.UserID ?? Guid.Empty;
        }
    }
}
