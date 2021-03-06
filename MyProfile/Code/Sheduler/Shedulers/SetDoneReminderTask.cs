using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Reminder.Service;
using Quartz;
using System;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Set Done for ReminderDates.IsDone Every day at 1am
    /// </summary>
    public class SetDoneReminderTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;

        public SetDoneReminderTask(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var reminderService = scope.ServiceProvider.GetRequiredService<ReminderService>();

                base.BaseExecute(repository, TaskType.SetDoneToReminderDates, reminderService.SetDoneForAllPastRemindersDate);
            }
            return Task.CompletedTask;
        }
    }
}
