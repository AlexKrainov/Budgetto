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
    public class NotificationOnTelegramTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;
        private int items;

        public NotificationOnTelegramTask(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();

                base.BaseExecute(repository, TaskType.NotificationSiteTask, _execute);
            }
            return Task.CompletedTask;
        }

        public int _execute()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var telegramService = scope.ServiceProvider.GetRequiredService<TelegramService>();

                try
                {
                    var notifications = repository.GetAll<MyProfile.Entity.Model.Notification>(x =>
                          x.IsTelegram
                          && x.IsReady == true
                          && x.IsSentOnTelegram == false)
                       .Select(x => new NotificationViewModel
                       {
                           NotificationID = x.ID,
                           NotificationTypeID = x.NotificationTypeID,
                           Total = x.Total,
                           ExpirationDateTime = x.ExpirationDateTime,
                           //UserID = x.UserID,
                           TelegramAccounts = x.User.UserConnect.TelegramAccounts
                                .Where(y => y.Status == TelegramStatus.Connected)
                                .Select(y => new TelegramAccountModelView
                                {
                                    ChatID = y.ChatUsers.FirstOrDefault().ChatID,
                                    TelegramID = y.TelegramID.ToString()
                                })
                                .ToList(),
                           IsRead = false,
                           ReadyDateTime = x.IsReadyDateTime,
                           Name = x.LimitID != null ? x.Limit.Name : x.ReminderID != null ? x.Reminder.Title : "",

                           //Hidden
                           SpecificCulture = x.User.Currency.SpecificCulture,
                       })
                       .ToList();

                    foreach (var notification in notifications)
                    {
                        try
                        {
                            bool isSent = telegramService.SendNotification(notification).Result;

                            if (isSent)
                            {
                                var dbNotification = repository.GetAll<Entity.Model.Notification>(x => x.ID == notification.NotificationID)
                                                       .FirstOrDefault();
                                dbNotification.IsSentOnTelegram = true;
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
