using Common.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyProfile.Areas.Admin.Models;
using MyProfile.Budget.Service;
using MyProfile.Code.Hubs;
using MyProfile.Code.Sheduler.Shedulers;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using MyProfile.Reminder.Service;
using MyProfile.Tag.Service;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SchedulerTaskController : Controller
    {
        private BaseRepository repository;
        private CommonService commonService;
        private IMemoryCache cache;
        private AccountService accountService;
        private BaseTaskJob baseTaskJob;
        private CurrencyService currencyService;
        private ReminderService reminderService;
        private HubManager hubManager;
        private LimitService limitService;

        public SchedulerTaskController(BaseRepository repository,
            CommonService commonService,
            CurrencyService currencyService,
            IMemoryCache cache,
            AccountService accountService,
            BaseTaskJob baseTaskJob,
            ReminderService reminderService,
            HubManager hubManager,
            LimitService limitService)
        {
            this.repository = repository;
            this.commonService = commonService;
            this.cache = cache;
            this.accountService = accountService;
            this.baseTaskJob = baseTaskJob;
            this.currencyService = currencyService;
            this.reminderService = reminderService;
            this.hubManager = hubManager;
            this.limitService = limitService;

            if (UserInfo.Current.UserTypeID != (int)UserTypeEnum.Admin)
            {
                this.Redirect("/Home/Month");
            }
        }
        public IActionResult List()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetData([FromBody] SchedulerTaskFilter filter)
        {
            var now = DateTime.Now;
            var data = repository.GetAll<SchedulerTask>()
                .Select(x => new SchedulerTaskModel
                {
                    ID = x.ID,
                    Name = x.Name,
                    FirstStart = x.FirstStart,
                    LastStart = x.LastStart,
                    NextStart = x.NextStart,
                    TaskStatus = x.TaskStatus,
                    CronComment = x.CronComment,
                    CronExpression = x.CronExpression,
                    Comment = x.Comment
                })
                .ToList();

            foreach (var item in data)
            {
                if (item.FirstStart.HasValue)
                {
                    item.FirstStart = item.FirstStart.Value.AddHours(3);
                }
                if (item.LastStart.HasValue)
                {
                    item.LastStart = item.LastStart.Value.AddHours(3);
                }

                if (item.NextStart.HasValue)
                {
                    item.NextStart = item.NextStart.Value.AddHours(3);
                    item.IsMissed = now > item.NextStart;
                }
            }

            return Json(new { isOk = true, data = data });
        }

        [HttpGet]
        public JsonResult SetNewStatus(int taskID, int newStatusID)
        {
            var task = repository.GetAll<SchedulerTask>(x => x.ID == taskID).FirstOrDefault();

            task.TaskStatus = Enum.GetName(typeof(Entity.Model.TaskStatus), (Entity.Model.TaskStatus)newStatusID);

            try
            {
                repository.Update(task, true);
            }
            catch (Exception ex)
            {
                return Json(new { isOk = false });
            }
            return Json(new { isOk = true, newTaskStatus = task.TaskStatus });
        }

        [HttpGet]
        public JsonResult ForceStart(TaskType taskID)
        {

            switch (taskID)
            {
                case TaskType.AccountRemoveCachback:
                    baseTaskJob.BaseExecute(repository, TaskType.AccountRemoveCachback, accountService.ResetAllCahbacks);
                    break;
                case TaskType.SetDoneToReminderDates:
                    baseTaskJob.BaseExecute(repository, TaskType.SetDoneToReminderDates, reminderService.SetDoneForAllPastRemindersDate);
                    break;
                case TaskType.CurrencyHistoryTask:
                    baseTaskJob.BaseExecute(repository, TaskType.CurrencyHistoryTask, currencyService.HistoryLoad);
                    break;
                case TaskType.ResetHubConnectTask:
                    baseTaskJob.BaseExecute(repository, TaskType.ResetHubConnectTask, hubManager.ResetAllHubConnects);
                    break;
                case TaskType.NotificationLimitCheckerTask:
                    baseTaskJob.BaseExecute(repository, TaskType.NotificationLimitCheckerTask, limitService.CheckLimitNotifications);
                    break;
                case TaskType.NotificationSiteTask:
                    break;
                case TaskType.NotificationTelegramTask:
                    break;
                case TaskType.NotificationMailTask:
                    break;
                case TaskType.NotificationReminderCheckerTask:
                    baseTaskJob.BaseExecute(repository, TaskType.NotificationReminderCheckerTask, reminderService.CheckReminderNotifications);
                    break;
                case TaskType.NotificationReset:
                    baseTaskJob.BaseExecute(repository, TaskType.NotificationReset, limitService.NotificationsReset);
                    break;
                case TaskType.AccountDailyWork:
                    baseTaskJob.BaseExecute(repository, TaskType.AccountDailyWork, accountService.AccountDailyWork);
                    break;
                case TaskType.Test:
                    break;
                default:
                    break;
            }
            return Json(new { isOk = true });
        }
    }
}
