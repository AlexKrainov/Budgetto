using Microsoft.Extensions.DependencyInjection;
using MyProfile.Common;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using Quartz;
using System;
using System.Linq;
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
            var now = TimeZoner.GetCurrentDateTimeUTC();
            var task = repository.GetAll<SchedulerTask>(x => x.ID == (int)taskType).FirstOrDefault();

            if (task != null)
            {
                task.LastStart = now;
                try
                {
                    CronExpression cronExpression = new CronExpression(task.CronExpression);
                    task.NextStart = cronExpression.GetNextValidTimeAfter(now).GetValueOrDefault().DateTime;
                }
                catch (Exception ex) { }

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

                        if (taskType != TaskType.NotificationSiteTask)
                        {
                            repository.Create(new SchedulerTaskLog
                            {
                                TaskID = (int)taskType,
                                ChangedItems = changedCounts,
                                Start = now,
                                End = TimeZoner.GetCurrentDateTimeUTC(),
                            }, true);
                        }
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
                            End = TimeZoner.GetCurrentDateTimeUTC(),
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
