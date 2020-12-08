using Email.Service.EmailEnvironment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
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
        private UserLogService userLogService;

        public UserEmailService(
            IBaseRepository repository,
            IEmailSender emailSender,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            UserLogService userLogService)
        {
            this._repository = repository;
            this._emailSender = emailSender;
            this.hostingEnvironment = hostingEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.userLogService = userLogService;
        }

        public async Task<Guid> ConfirmEmail(User user, Guid userSessionID, bool isResend = false)
        {
            // user.Email = "ialexkrainov2@gmail.com";
            string body = string.Empty;
            Random random = new Random();

            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = isResend ? (int)MailTypeEnum.ConfirmEmail : (int)MailTypeEnum.ResendByUser,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = user.ID,
                Email = user.Email,
                Code = (random.Next(1000, 9999)),
            };
            //string confirmUrl = @"https://" + httpContextAccessor.HttpContext.Request.Host.Value.ToString() + @"/Identity/Account/ConfirmEmail?id=" + mailLog.ID;

            try
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\CodeEmail.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("${Code}", mailLog.Code.ToString());

                await _emailSender.SendEmailAsync(user.Email, "Подтверждение почты", body);
            }
            catch (Exception ex)
            {
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;

                await _repository.CreateAsync(mailLog, true);
                await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_ConfirmEmail, comment: ex.Message);
                return Guid.Empty;
            }

            await _repository.CreateAsync(mailLog, true);
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_ConfirmEmail);
            return mailLog.ID;
        }

        public async Task<int> ConfirmEmail_Complete(Guid id, Guid userSessionID)
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
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_ConfirmEmailComplete);

            return 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isResend"></param>
        /// <returns>if the this site cannot send email, retun guid.empty</returns>
        public async Task<Guid> LoginConfirmation(User user, Guid userSessionID, bool isResend = false)
        {
            // user.Email = "ialexkrainov2@gmail.com";
            string body = string.Empty;
            Random random = new Random();

            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = isResend ? (int)MailTypeEnum.Login : (int)MailTypeEnum.ResendByUser,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = user.ID,
                Email = user.Email,
                Code = (random.Next(1000, 9999)),
            };
            //string confirmUrl = @"https://" + httpContextAccessor.HttpContext.Request.Host.Value.ToString() + @"/Identity/Account/ConfirmEmail?id=" + mailLog.ID;

            try
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\CodeEmail.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("${Code}", mailLog.Code.ToString());

                await _emailSender.SendEmailAsync(user.Email, "Подтверждение входа", body);
            }
            catch (Exception ex)
            {
                //
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;
                await _repository.CreateAsync(mailLog, true);
                await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_LoginConfirmation, comment: ex.Message);
                return Guid.Empty;
            }

            await _repository.CreateAsync(mailLog, true);
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_LoginConfirmation);
            return mailLog.ID;
        }



        public async Task<Guid> RecoveryPassword(User user, Guid userSessionID, bool isResend = false)
        {
            //user.Email = "ialexkrainov2@gmail.com";
            string body = string.Empty;
            Random random = new Random();

            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = isResend ? (int)MailTypeEnum.ResetPassword : (int)MailTypeEnum.ResendByUser,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = user.ID,
                Email = user.Email,
                Code = (random.Next(1000, 9999)),
            };
            //string confirmUrl = @"https://" + httpContextAccessor.HttpContext.Request.Host.Value.ToString() + @"/Identity/Account/ResetPassword2?id=" + mailLog.ID;

            try
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\CodeEmail.html"))
                {
                    body = reader.ReadToEnd();
                }

                body = body.Replace("${Code}", mailLog.Code.ToString());

                await _emailSender.SendEmailAsync(user.Email, "Сброс пароля", body);
            }
            catch (Exception ex)
            {
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;
                await _repository.CreateAsync(mailLog, true);
                await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_RecoveryPassword, ex.Message);

                return Guid.Empty;
            }

            await _repository.CreateAsync(mailLog, true);
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_RecoveryPassword);

            return mailLog.ID;
        }

        public async Task<Guid> CheckCode(Guid mailLogID, int code, Guid userSessionID)
        {
            var mailLog = await _repository.GetAll<MailLog>(x => x.ID == mailLogID).FirstOrDefaultAsync();

            if (mailLog.Code == code)
            {
                mailLog.CameDateTime = DateTime.Now.ToUniversalTime();

                if ((mailLog.UserID == null || mailLog.UserID == Guid.Empty)
                    && !string.IsNullOrEmpty(mailLog.Email))
                {
                    mailLog.UserID = await _repository.GetAll<User>(x => x.Email == mailLog.Email).Select(x => x.ID).FirstOrDefaultAsync();
                }

                await _repository.UpdateAsync(mailLog, true);
                await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_CheckCodeOk);
                return mailLog.UserID ?? Guid.Empty;
            }
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_CheckCodeNotOk);
            return Guid.Empty;
        }

        public async Task<bool> CheckCode(string email, int code, Guid userSessionID)
        {
            var mailLog = await _repository.GetAll<MailLog>(x => x.Email == email)
                .OrderByDescending(t => t.SentDateTime)
                .FirstOrDefaultAsync();

            if (mailLog != null && mailLog.Code == code)
            {
                mailLog.CameDateTime = DateTime.Now.ToUniversalTime();

                if ((mailLog.UserID == null || mailLog.UserID == Guid.Empty)
                    && !string.IsNullOrEmpty(mailLog.Email))
                {
                    mailLog.UserID = await _repository.GetAll<User>(x => x.Email == mailLog.Email).Select(x => x.ID).FirstOrDefaultAsync();
                }

                await _repository.UpdateAsync(mailLog, true);
                await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_CheckCodeOk);
                return true;
            }
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_CheckCodeNotOk);
            return false;
        }

        public async Task<Guid> CancelLastEmail(Guid emailID, Guid userSessionID)
        {
            var mailLog = await _repository.GetAll<MailLog>(x => x.ID == emailID).FirstOrDefaultAsync();

            mailLog.CameDateTime = DateTime.Now.ToUniversalTime();

            if ((mailLog.UserID == null || mailLog.UserID == Guid.Empty)
                && !string.IsNullOrEmpty(mailLog.Email))
            {
                mailLog.UserID = await _repository.GetAll<User>(x => x.Email == mailLog.Email).Select(x => x.ID).FirstOrDefaultAsync();
            }

            await _repository.UpdateAsync(mailLog, true);
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_CheckCodeNotOk);

            return mailLog.UserID ?? Guid.Empty;
        }
    }
}
