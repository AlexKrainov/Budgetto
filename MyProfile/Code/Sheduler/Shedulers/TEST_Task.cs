using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.Hubs;
using MyProfile.Reminder.Service;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Set Done for ReminderDates.IsDone Every day at 1am
    /// </summary>
    public class TEST_Task : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;

        public TEST_Task(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var notificationHub = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>();

                try
                {
                    var hubConnects = repository.GetAll<HubConnect>().ToList();

                    foreach (var connect in hubConnects)
                    {
                        NotificationViewModel notification = new NotificationViewModel
                        {
                            Title = "Hello form server",
                            Message = $"send time to {connect.UserConnect.User.Email } Time : {DateTime.Now.ToString()}",
                        };
                        var client = notificationHub.Clients.Client(connect.ConnectionID);


                        client.SendCoreAsync("Receive", new object[] { notification });
                    }
                }
                catch (Exception ex)
                {

                }
                //base.BaseExecute(repository, TaskType.Test, test);
            }
            return Task.CompletedTask;
        }

        public int test()
        {
            return 1;
        }
    }
}
