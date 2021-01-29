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
    public class ResetCachbackAccountTask : IJob
    {
        private IServiceScopeFactory _scopeFactory;

        public ResetCachbackAccountTask(IServiceScopeFactory scopeFactory)
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

                var task = repository.GetAll<ShedulerTask>(x => x.ID == (int)TaskType.AccountRemoveCachback).FirstOrDefault();

                if (task != null)
                {
                    task.LastStart = now;

                    if (task.TaskStatus == Enum.GetName(typeof(TaskStatus), TaskStatus.New)
                        || task.TaskStatus == Enum.GetName(typeof(TaskStatus), TaskStatus.InProcess))
                    {
                        task.TaskStatus = Enum.GetName(typeof(TaskStatus), TaskStatus.InProcess);

                        try
                        {
                            int changedCounts = accountService.ResetAllCahbacks();

                            repository.Create(new ShedulerTaskLog
                            {
                                TaskID = (int)TaskType.AccountRemoveCachback,
                                ChangedItems = changedCounts,
                                Start = now,
                                End = DateTime.Now.ToUniversalTime(),
                            }, true);
                        }
                        catch (Exception ex)
                        {
                            task.TaskStatus = Enum.GetName(typeof(TaskStatus), TaskStatus.Error);
                            task.Comment = ex.Message;

                            repository.Create(new ShedulerTaskLog
                            {
                                TaskID = (int)TaskType.AccountRemoveCachback,
                                Comment = ex.Message,
                                ChangedItems = -1,
                                Start = now,
                                End = DateTime.Now.ToUniversalTime(),
                            }, true);
                        }
                    }
                    else
                    {
                        repository.Save();
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
