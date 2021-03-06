using Microsoft.Extensions.DependencyInjection;
using MyProfile.Code.Hubs;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using Quartz;
using System;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Reset all cachback balance every month on the 1st, at noon
    /// </summary>
    public class ResetHubConnectTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;

        public ResetHubConnectTask(IServiceScopeFactory scopeFactory)
            : base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.Now.ToUniversalTime();

            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                HubManager hubManager = new HubManager(repository);

                base.BaseExecute(repository, TaskType.ResetHubConnectTask, hubManager.ResetAllHubConnects);
            }
            return Task.CompletedTask;
        }
    }
}
