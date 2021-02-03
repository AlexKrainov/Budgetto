using Common.Service;
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
    public class CurrencyHistoryTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;

        public CurrencyHistoryTask(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var currencyService = scope.ServiceProvider.GetRequiredService<CurrencyService>();

                base.BaseExecute(repository, TaskType.CurrencyHistoryTask, currencyService.HistoryLoad);
            }
            return Task.CompletedTask;
        }
    }
}
