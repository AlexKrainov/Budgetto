using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.Hubs;
using MyProfile.Notification.Service;
using MyProfile.Reminder.Service;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Send all [IsReady] Notification to user who are connect (online) on the web site
    /// </summary>
    public class NotificationOnSiteTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;
        private int items;

        public NotificationOnSiteTask(IServiceScopeFactory scopeFactory) :
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
                var notificationHub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();
                var notificationService = scope.ServiceProvider.GetRequiredService<NotificationService>();

                try
                {
                    var notifications = notificationService.GetLastNotification(0, 100,
                        x => x.IsSite
                           && x.IsReady == true
                           && x.IsSentOnSite == false);

                    foreach (var notification in notifications)
                    {
                        //notificationService.GetMessage(notification);

                        foreach (var connectionID in notification.UserConnectionIDs)
                        {
                            try
                            {
                                var client = notificationHub.Clients.Client(connectionID);
                                client.SendCoreAsync("Receive", new object[] { notification });
                            }
                            catch (Exception ex)
                            {
                                repository.Create(new ErrorLog
                                {
                                    ErrorText = ex.Message,
                                    CurrentDate = DateTime.Now.ToUniversalTime(),
                                    Where = "NotificationOnSiteTask._execute.Receive",
                                }, true);
                            }
                        }
                        var dbNotification = repository.GetAll<Entity.Model.Notification>(x => x.ID == notification.NotificationID)
                            .FirstOrDefault();
                        dbNotification.IsSentOnSite = true;
                        dbNotification.IsDone = dbNotification.IsSite == dbNotification.IsSentOnSite && dbNotification.IsMail == dbNotification.IsSentOnMail && dbNotification.IsTelegram == dbNotification.IsSentOnTelegram;
                        repository.Save();
                        items += 1;
                    }
                }
                catch (Exception ex)
                {
                    repository.Create(new ErrorLog
                    {
                        ErrorText = ex.Message,
                        CurrentDate = DateTime.Now.ToUniversalTime(),
                        Where = "NotificationOnSiteTask._execute",
                    }, true);
                }
                return items;
            }

        }



    }
}
