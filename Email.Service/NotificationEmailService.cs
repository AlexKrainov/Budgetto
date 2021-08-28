using Email.Service.EmailEnvironment;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.UserLog.Service;
using System;
using System.Globalization;
using System.IO;
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

            switch ((NotificationType)notification.NotificationTypeID)
            {
                case NotificationType.Limit:
                    mailLog.MailTypeID = (int)MailTypeEnum.NotificationLimit;
                    mailLog.Code = notification.LimitID ?? 0;
                    break;
                case NotificationType.Reminder:
                    mailLog.MailTypeID = (int)MailTypeEnum.NotificationReminder;
                    mailLog.Code = notification.ReminderDateID ?? 0;
                    break;
                case NotificationType.SystemMailing:
                    mailLog.MailTypeID = (int)MailTypeEnum.NotificationSystemMailing;
                    mailLog.Comment = Enum.GetName(typeof(SystemMailingType), notification.SystemMailngType);
                    //mailLog.Code
                    break;
                default:
                    break;
            }

            try
            {
                string body = string.Empty;

                switch ((NotificationType)notification.NotificationTypeID)
                {
                    case NotificationType.Limit:
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
                    case NotificationType.Reminder:

                        using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\notification\\Reminder.html"))
                        {
                            body = reader.ReadToEnd();
                        }

                        body = body.Replace("${ReminderName}", notification.Name);
                        body = body.Replace("${ReminderDate}", notification.ExpirationDateTime.Value.AddMinutes(notification.ReminderUTCOffsetMinutes).ToString("dd MM yyyy HH:mm"));

                        await _emailSender.SendEmailAsync(notification.Email, "Напоминание: " + notification.Name, body);
                        break;
                    case NotificationType.SystemMailing:

                        switch (notification.SystemMailngType)
                        {
                            case SystemMailingType.FeedbackMonth:
                                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\notification\\SystemMailing_FeedbackMonth.html"))
                                {
                                    body = reader.ReadToEnd();
                                }

                                await _emailSender.SendEmailAsync(notification.Email, "Нам важно ваше мнение", body);
                                break;
                            case SystemMailingType.StatisticsWeek:
                                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\notification\\SystemMailing_StatisticsWeek.html"))
                                {
                                    body = reader.ReadToEnd();
                                }

                                body = body.Replace("${ReminderName}", notification.Name);
                                body = body.Replace("${ReminderDate}", notification.ExpirationDateTime.Value.AddMinutes(notification.ReminderUTCOffsetMinutes).ToString("dd MM yyyy HH:mm"));

                                await _emailSender.SendEmailAsync("ialexkrainov2@gmail.com", "StatisticsWeek: " + notification.Name, body);
                                break;
                            case SystemMailingType.StatisticsMonth:
                                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\notification\\SystemMailing_StatisticsMonth.html"))
                                {
                                    body = reader.ReadToEnd();
                                }

                                body = body.Replace("${ReminderName}", notification.Name);
                                body = body.Replace("${ReminderDate}", notification.ExpirationDateTime.Value.AddMinutes(notification.ReminderUTCOffsetMinutes).ToString("dd MM yyyy HH:mm"));

                                await _emailSender.SendEmailAsync("ialexkrainov2@gmail.com", "StatisticsMonth: " + notification.Name, body);
                                break;
                            case SystemMailingType.NotActive1DayAfterRegistration:
                            case SystemMailingType.NotActive2DaysAfterRegistration:
                            case SystemMailingType.NotActive3DaysAfterRegistration:
                            case SystemMailingType.NotActive4DaysAfterRegistration:
                            case SystemMailingType.NotActive5DaysAfterRegistration:
                            case SystemMailingType.NotActive6DaysAfterRegistration:
                            case SystemMailingType.NotActive7DaysAfterRegistration:
                                using (StreamReader reader = new StreamReader(hostingEnvironment.WebRootPath + @"\\template\\notification\\SystemMailing_NotActiveDaysAfterRegistration.html"))
                                {
                                    body = reader.ReadToEnd();
                                }
                                body = body.Replace("${ReminderName}", notification.Name);
                                body = body.Replace("${ReminderDate}", notification.ExpirationDateTime.Value.AddMinutes(notification.ReminderUTCOffsetMinutes).ToString("dd MM yyyy HH:mm"));

                                await _emailSender.SendEmailAsync("ialexkrainov2@gmail.com", "NotActive1DayAfterRegistration: " + notification.Name, body);
                                break;
                            default:
                                break;
                        }

                        //await _emailSender.SendEmailAsync(notification.Email, "FeedbackMonth: " + notification.Name, body);
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
