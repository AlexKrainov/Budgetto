using Common.Service;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.ModelView.Notification.Task;
using MyProfile.Entity.Repository;
using MyProfile.Hubs;
using MyProfile.Limit.Service;
using MyProfile.Notification.Service;
using MyProfile.Reminder.Service;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Проверяем не подошло ли время рассылки для пользователя 
    /// </summary>
    public class CheckerSystemMailingTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;
        private int _checkLimits = 0;

        public CheckerSystemMailingTask(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var systemMailingService = scope.ServiceProvider.GetRequiredService<SystemMailingService>();

                base.BaseExecute(repository, TaskType.SystemMailing, systemMailingService.CheckSystemMailings);
            }
            return Task.CompletedTask;
        }

        private int checkLimits()
        {
            return _checkLimits;
        }
    }
}
