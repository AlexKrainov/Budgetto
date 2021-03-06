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

                base.BaseExecute(repository, TaskType.NotificationLimitSiteTask, _execute);
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
                    var limitNotifications = repository.GetAll<MyProfile.Entity.Model.Notification>(x =>
                          x.LimitID != null
                          && x.IsReady == true
                          && x.IsSent == false)
                       .Select(x => new NotificationViewModel
                       {
                           NotificationID = x.ID,
                           NotificationTypeID = x.NotificationTypeID,
                           Total = x.Total,
                           //UserID = x.UserID,
                           UserConnectionIDs = x.User.UserConnect.HubConnects
                                .Select(y => y.ConnectionID)
                                .ToList(),
                           IsRead = false,
                           ReadyDateTime = x.IsReadyDateTime,
                           Name = x.LimitID != null ? x.Limit.Name : x.ReminderID != null ? x.Reminder.Title : "",
                           //Hidden
                           SpecificCulture = x.User.Currency.SpecificCulture,
                       })
                       .ToList();

                    foreach (var notification in limitNotifications)
                    {
                        notificationService.GetMessage(notification);

                        foreach (var connectionID in notification.UserConnectionIDs)
                        {
                            try
                            {
                                var client = notificationHub.Clients.Client(connectionID);
                                client.SendCoreAsync("Receive", new object[] { notification });
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                        var dbNotification = repository.GetAll<Entity.Model.Notification>(x => x.ID == notification.NotificationID)
                            .FirstOrDefault();
                        dbNotification.IsSent = true;
                        dbNotification.IsDone = true;
                        repository.Save();
                        items += 1;
                    }
                }
                catch (Exception ex)
                {

                }
                return items;
            }

        }



    }
}
