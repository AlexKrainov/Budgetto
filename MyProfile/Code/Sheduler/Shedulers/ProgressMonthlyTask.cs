using Microsoft.Extensions.DependencyInjection;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Progress.Service;
using Quartz;
using System.Threading.Tasks;

namespace MyProfile.Code.Sheduler.Shedulers
{
    /// <summary>
    /// Copy to ProgressLog old progress object
    /// </summary>
    public class ProgressMonthlyTask : BaseTaskJob, IJob
    {
        private IServiceScopeFactory _scopeFactory;

        public ProgressMonthlyTask(IServiceScopeFactory scopeFactory) :
            base(scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task Execute(IJobExecutionContext context)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<BaseRepository>();
                var progressService = scope.ServiceProvider.GetRequiredService<ProgressService>();

                base.BaseExecute(repository, TaskType.ProgressMonthly, progressService.CopyToHistory);
            }
            return Task.CompletedTask;
        }
    }
}
