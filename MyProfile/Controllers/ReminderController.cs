using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Identity;
using MyProfile.Reminder.Service;
using MyProfile.User.Service;
using MyProfile.UserLog.Service;

namespace MyProfile.Controllers
{
    [Authorize]
    public class ReminderController : Controller
    {
        private ReminderService reminderService;
        private UserLogService userLogService;

        public ReminderController(ReminderService reminderService, UserLogService userLogService)
        {
            this.reminderService = reminderService;
            this.userLogService = userLogService;
        }

        [HttpGet]
        public async Task<IActionResult> LoadReminders(DateTime? currentDate, int? month, int? year)
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Reminder_Part);

            if (currentDate != null)
            {
                return Json(new
                {
                    IsOk = true,
                    data = await reminderService.GetRimindersByDate(currentDate.Value),
                    currentDate = currentDate
                });
            }
            else //if (month != null && year != null)
            {
                currentDate = new DateTime(year ?? DateTime.Now.Year, month ?? 1, 1);
                var finishDate = new DateTime(year ?? DateTime.Now.Year, month ?? 12, DateTime.DaysInMonth(year ?? DateTime.Now.Year, month ?? 12), 23, 59, 59);

                return Json(new
                {
                    IsOk = true,
                    data = await reminderService.GetRimindersByDate(currentDate.Value, finishDate),
                    currentDate = currentDate,
                    dateTimeFinish = finishDate
                });
            }
            //return Json(new { IsOk = true, data = await reminderService.GetRimindersByDate(currentDate.Value) });
        }



        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] ReminderEditModelView reminder)
        {
            var result = await reminderService.CreateOrUpdate(reminder);
            return Json(new { IsOk = result.Item1, data = reminder, message = result.Item2 });
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] ReminderEditModelView reminder)
        {
            var result = await reminderService.RemoveOrRecovery(reminder, isDelete: true);
            return Json(new { IsOk = true, data = reminder, isDeleted = true });
        }

        [HttpPost]
        public async Task<IActionResult> Recovery([FromBody] ReminderEditModelView reminder)
        {
            var result = await reminderService.RemoveOrRecovery(reminder, isDelete: false);
            return Json(new { IsOk = result, data = reminder, isRecovery = true });
        }

    }
}
