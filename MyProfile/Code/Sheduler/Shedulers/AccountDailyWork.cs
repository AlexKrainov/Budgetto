using Microsoft.Extensions.DependencyInjection;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using Quartz;
using System;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Обновление данных счетов, например начисление процентов по вкладам
    /// </summary>
    public class AccountDailyWork : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;
        private int _checkLimits = 0;

        public AccountDailyWork(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now;
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                base.BaseExecute(repository, TaskType.AccountDailyWork, accountService.CheckDepositForInterest);
            }
            return Task.CompletedTask;
        }
    }
}
