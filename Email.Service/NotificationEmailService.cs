using Common.Service;
using Email.Service.EmailEnvironment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.UserLog.Service;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Email.Service
{
    public class NotificationEmailService
    {
        private IBaseRepository repository;
        private IEmailSender _emailSender;
        private IHostingEnvironment hostingEnvironment;
        private IHttpContextAccessor httpContextAccessor;
        private UserLogService userLogService;

        public NotificationEmailService(
            IBaseRepository repository,
            IEmailSender emailSender,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor,
            UserLogService userLogService)
        {
            this.repository = repository;
            this._emailSender = emailSender;
            this.hostingEnvironment = hostingEnvironment;
            this.httpContextAccessor = httpContextAccessor;
            this.userLogService = userLogService;
        }

        public async Task<Guid> ConfirmEmail(User user, Guid userSessionID, MailTypeEnum mailTypeEnum, string returnUrl = null)
        {
            // user.Email = "ialexkrainov2@gmail.com";
            string body = string.Empty;
            string templatePath = "";
            Random random = new Random();

            switch (mailTypeEnum)
            {
                case MailTypeEnum.Registration:
                    templatePath = @"\\template\\RegistrationAccount.html";
                    break;
                case MailTypeEnum.EmailUpdate:
                case MailTypeEnum.ResendByUser:
                    templatePath = @"\\template\\EmailUpdate.html";
                    break;
                default:
                    break;
            }

            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                MailTypeID = (int)mailTypeEnum,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = user.ID,
                Email = user.Email,
                Code = (random.Next(1000, 9999)),
            };

            try
            {
                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + templatePath))
                {
                    body = reader.ReadToEnd();
                }
                body = body.Replace("${Code}", mailLog.Code.ToString());
                body = body.Replace("${link}", $"https://{PublishSettings.SiteName}/Identity/Account/Login?id={userSessionID}&email={mailLog.Email}&mid={mailLog.ID}{(string.IsNullOrEmpty(returnUrl) ? "" : "&ReturnUrl=" + returnUrl)}");

                await _emailSender.SendEmailAsync(user.Email, "Подтверждение почты", body);
            }
            catch (Exception ex)
            {
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;

                await repository.CreateAsync(mailLog, true);
                await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_ConfirmEmail, comment: ex.Message);
                return Guid.Empty;
            }

            await repository.CreateAsync(mailLog, true);
            await userLogService.CreateUserLogAsync(userSessionID, UserLogActionType.Email_ConfirmEmail);
            return mailLog.ID;
        }

        public async Task<bool> SendNotification(NotificationViewModel notification)
        {
            MailLog mailLog = new MailLog
            {
                ID = Guid.NewGuid(),
                IsSuccessful = true,
                SentDateTime = DateTime.Now.ToUniversalTime(),
                UserID = notification.UserID,
                Email = notification.Email,
            };
            switch (notification.NotificationTypeID)
            {
                case (int)NotificationType.Limit:
                    mailLog.MailTypeID = (int)MailTypeEnum.NotificationLimit;
                    mailLog.Code = notification.LimitID ?? 0;
                    break;
                case (int)NotificationType.Reminder:
                    mailLog.MailTypeID = (int)MailTypeEnum.NotificationReminder;
                    mailLog.Code = notification.ReminderDateID ?? 0;
                    break;
                default:
                    break;
            }

            try
            {
                string body = string.Empty;

                switch (notification.NotificationTypeID)
                {
                    case (int)NotificationType.Limit:
                        NumberFormatInfo numberFormatInfo = new CultureInfo("ru-RU", false).NumberFormat;
                        numberFormatInfo.CurrencyDecimalDigits = 0;

                        using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\notification\\Limit.html"))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("${LimitName}", notification.Name);
                        body = body.Replace("${LimitPrice}", notification.Total?.ToString("C", numberFormatInfo));

                        await _emailSender.SendEmailAsync(notification.Email, "Уведомление по лимиту: " + notification.Name, body);
                        break;
                    case (int)NotificationType.Reminder:

                        using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\notification\\Reminder.html"))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("${ReminderName}", notification.Name);
                        body = body.Replace("${ReminderDate}", notification.ExpirationDateTime.Value.AddMinutes(notification.ReminderUTCOffsetMinutes).ToString("dd MM yyyy HH:mm"));

                        await _emailSender.SendEmailAsync(notification.Email, "Напоминание: " + notification.Name, body);
                        break;
                    default:
                        break;
                }
                await repository.CreateAsync(mailLog);

                return true;
            }
            catch (Exception ex)
            {
                mailLog.IsSuccessful = false;
                mailLog.Comment = ex.Message;

                await repository.CreateAsync(mailLog);
                await repository.CreateAsync(new ErrorLog
                {
                    ErrorText = ex.Message,
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Where = "NotificationEmailService.SendNotification",
                    Comment = "NotificationEmailService"
                }, true);
            }
            return false;
        }
    }
}
