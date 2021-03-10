using Email.Service;
using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Service;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Send all [IsReady] Notification to user who are connect (online) on the web site
    /// </summary>
    public class NotificationOnMailTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;
        private int items;

        public NotificationOnMailTask(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();

                base.BaseExecute(repository, TaskType.NotificationMailTask, _execute);
            }
            return Task.CompletedTask;
        }

        public int _execute()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var emailService = scope.ServiceProvider.GetRequiredService<NotificationEmailService>();

                try
                {
                    var notifications = repository.GetAll<MyProfile.Entity.Model.Notification>(x =>
                          x.IsMail
                          && x.IsReady == true
                          && x.IsSentOnMail == false
                          && x.User.IsConfirmEmail)
                       .Select(x => new NotificationViewModel
                       {
                           NotificationID = x.ID,
                           NotificationTypeID = x.NotificationTypeID,
                           UserID = x.UserID,
                           Email = x.User.Email,
                           Name = x.LimitID != null ? x.Limit.Name : x.ReminderID != null ? x.Reminder.Title : "",

                           //Limit
                           Total = x.Total,
                           SpecificCulture = x.User.Currency.SpecificCulture,
                           LimitID = x.LimitID,

                           //Reminder
                           ExpirationDateTime = x.ExpirationDateTime,
                           Icon = x.Icon,
                       })
                       .ToList();

                    foreach (var notification in notifications)
                    {
                        try
                        {
                            bool isSent = emailService.SendNotification(notification).Result;

                            if (isSent)
                            {
                                var dbNotification = repository.GetAll<Entity.Model.Notification>(x => x.ID == notification.NotificationID)
                                                       .FirstOrDefault();
                                dbNotification.IsSentOnMail = true;
                                dbNotification.IsDone = dbNotification.IsSite == dbNotification.IsSentOnSite && dbNotification.IsMail == dbNotification.IsSentOnMail && dbNotification.IsTelegram == dbNotification.IsSentOnTelegram;
                                repository.Save();
                                items += 1;
                            }
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
                catch (Exception ex)
                {
                    repository.Create(new ErrorLog
                    {
                        ErrorText = ex.Message,
                        CurrentDate = DateTime.Now.ToUniversalTime(),
                        Where = "NotificationOnTelegramTask._execute",
                    }, true);
                }
                return items;
            }

        }



    }
}
