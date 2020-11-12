using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Identity;
using MyProfile.Reminder.Service;
using MyProfile.User.Service;

namespace MyProfile.Controllers
{
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
        public async Task<IActionResult> LoadReminders(DateTime currentDate)
        {
            await userLogService.CreateUserLogAsync(UserInfo.Current.UserSessionID, UserLogActionType.Reminder_Part);

            return Json(new { IsOk = true, data = await reminderService.GetRimindersByDate(currentDate) });
        }



        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]ReminderEditModelView reminder)
        {
            var result = await reminderService.CreateOrUpdate(reminder);
            return Json(new { IsOk = result, data = reminder });
        }

        [HttpPost]
        public async Task<IActionResult> Remove([FromBody] ReminderEditModelView reminder)
        {
            var result = await reminderService.RemoveOrRecovery(reminder, true);
            return Json(new { IsOk = true, data = reminder, isDeleted = true });
        }

        [HttpPost]
        public async Task<IActionResult> Recovery([FromBody] ReminderEditModelView reminder)
        {
            var result = await reminderService.RemoveOrRecovery(reminder, false);
            return Json(new { IsOk = true, data = reminder, isRecovery = true });
        }

    }
}
