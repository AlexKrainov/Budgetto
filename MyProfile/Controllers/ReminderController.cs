using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Reminder.Service;

namespace MyProfile.Controllers
{
    public class ReminderController : Controller
    {
        private ReminderService reminderService;

        public ReminderController(ReminderService reminderService)
        {
            this.reminderService = reminderService;
        }

        [HttpGet]
        public async Task<IActionResult> LoadReminders(DateTime currentDate)
        {
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
