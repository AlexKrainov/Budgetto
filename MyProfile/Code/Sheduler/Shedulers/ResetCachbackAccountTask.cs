using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyProfile.Budget.Service;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskStatus = MyProfile.Entity.Model.TaskStatus;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Reset all cachback balance every month on the 1st, at noon
    /// </summary>
    public class ResetCachbackAccountTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;

        public ResetCachbackAccountTask(IServiceScopeFactory scopeFactory)
            :base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now.ToUniversalTime();

            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var accountService = scope.ServiceProvider.GetRequiredService<AccountService>();

                base.BaseExecute(repository, TaskType.AccountRemoveCachback, accountService.ResetAllCahbacks);
            }
            return Task.CompletedTask;
        }
    }
}
