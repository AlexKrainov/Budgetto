using Email.Service;
using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.Limit.Service;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Service;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Reset all notification what need every month (1st 10:00):
    /// Limit
    /// </summary>
    public class NotificationReset : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;
        private int items;

        public NotificationReset(IServiceScopeFactory scopeFactory) :
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
                var limitService = scope.ServiceProvider.GetRequiredService<LimitService>();

                base.BaseExecute(repository, TaskType.NotificationReset, limitService.NotificationsReset);
                return items;
            }

        }



    }
}
