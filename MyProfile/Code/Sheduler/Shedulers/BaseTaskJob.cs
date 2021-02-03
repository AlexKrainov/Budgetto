using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyProfile.Budget.Service;
using MyProfile.Common;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using Quartz;
using System;
using System.Linq;
using System.Threading.Tasks;
using TaskStatus = MyProfile.Entity.Model.TaskStatus;

namespace MyProfile.Code.Sheduler.Shedulers
{

    public class BaseTaskJob
    {
        private IServiceScopeFactory _scopeFactory;

        public BaseTaskJob(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        internal void BaseExecute(BaseRepository repository, TaskType taskType, Func<int> _service)
        {
            var now = TimeZoner.GetCurrentDateTimeWithRusTimeZone();

            var task = repository.GetAll<SchedulerTask>(x => x.ID == (int)taskType).FirstOrDefault();

            if (task != null)
            {
                task.LastStart = now;

                if (task.TaskStatus == Enum.GetName(typeof(TaskStatus), TaskStatus.New)
                    || task.TaskStatus == Enum.GetName(typeof(TaskStatus), TaskStatus.InProcess))
                {
                    if (task.TaskStatus == Enum.GetName(typeof(TaskStatus), TaskStatus.New))
                    {
                        task.FirstStart = now;
                    }

                    task.TaskStatus = Enum.GetName(typeof(TaskStatus), TaskStatus.InProcess);

                    try
                    {
                        int changedCounts = _service();

                        repository.Create(new SchedulerTaskLog
                        {
                            TaskID = (int)taskType,
                            ChangedItems = changedCounts,
                            Start = now,
                            End = TimeZoner.GetCurrentDateTimeWithRusTimeZone(),
                        }, true);
                    }
                    catch (Exception ex)
                    {
                        task.TaskStatus = Enum.GetName(typeof(TaskStatus), TaskStatus.Error);
                        task.Comment = ex.Message;

                        repository.Create(new SchedulerTaskLog
                        {
                            TaskID = (int)taskType,
                            Comment = ex.Message,
                            ChangedItems = -1,
                            Start = now,
                            End = TimeZoner.GetCurrentDateTimeWithRusTimeZone(),
                        }, true);
                    }
                }
                else
                {
                    repository.Save();
                }
            }
            else
            {
                //??
            }
        }
    }
}
