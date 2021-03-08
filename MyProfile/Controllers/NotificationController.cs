using Microsoft.AspNetCore.Mvc;
using MyProfile.Notification.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace MyProfile.Controllers
{
    public class NotificationController : Controller
    {
        private NotificationService notificationService;

        public NotificationController(NotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public IActionResult GetLast(int skip, int take)
        {
            var notifications = notificationService.GetLastNotification(skip, take);
            return Json(new { isOk = true, notifications });
        }

        [HttpPost]
        public IActionResult SetFlagRead([FromBody] List<int> IDs)
        {
            notificationService.SetRead(IDs);
            return Json(new { isOk = true });
        }
    }
}
