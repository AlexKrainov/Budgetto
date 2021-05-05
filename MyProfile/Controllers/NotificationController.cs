using Microsoft.AspNetCore.Mvc;
using MyProfile.Identity;
using MyProfile.Notification.Service;
using System.Collections.Generic;

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
            var notifications = notificationService.GetLastNotification(skip, take, x =>
                    x.IsReady
                    && x.IsSentOnSite
                    && x.UserID == UserInfo.Current.ID);
            return Json(new { isOk = true, notifications });
        }

        [HttpPost]
        public IActionResult SetFlagRead([FromBody] List<long> IDs)
        {
            notificationService.SetRead(IDs);
            return Json(new { isOk = true });
        }
    }
}
