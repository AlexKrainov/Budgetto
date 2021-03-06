using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Limit.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace MyProfile.Notification.Service
{
    public class NotificationService
    {
        private BaseRepository repository;
        private LimitService limitService;

        public NotificationService(BaseRepository repository,
            LimitService limitService)
        {
            this.repository = repository;
            this.limitService = limitService;
        }

        public int CheckLimits()
        {
            return (new NotificationChecker(repository, limitService)).CheckLimits();
        }

        public List<NotificationViewModel> GetLastNotification(int skip, int take)
        {
            var currentUser = UserInfo.Current;
            var notifications = repository.GetAll<Entity.Model.Notification>(x =>
                x.IsReady && x.IsSent && x.UserID == currentUser.ID)
                 .Select(x => new NotificationViewModel
                 {
                     NotificationID = x.ID,
                     NotificationTypeID = x.NotificationTypeID,
                     Total = x.Total,
                     ExpirationDateTime = x.ExpirationDateTime,
                     IsRead = x.IsRead,
                     Name = x.LimitID != null ? x.Limit.Name : x.ReminderID != null ? x.Reminder.Title : "",
                     //Hidden
                     SpecificCulture = x.User.Currency.SpecificCulture,
                 })
                 .Skip(skip)
                 .Take(take)
                 .ToList();

            foreach (var notification in notifications)
            {
                GetMessage(notification);
            }

            return notifications;
        }

        public void SetRead(List<int> ids)
        {
            var now = DateTime.Now;
            var notifications = repository.GetAll<Entity.Model.Notification>(x => x.IsRead == false && x.IsSent && ids.Contains(x.ID)).ToList();

            for (int i = 0; i < notifications.Count; i++)
            {
                notifications[i].IsRead = true;
                notifications[i].ReadDateTime = now;
            }

            repository.Save();
        }

        public void GetMessage(NotificationViewModel notification)
        {
            NumberFormatInfo numberFormatInfo = new CultureInfo(notification.SpecificCulture, false).NumberFormat;
            numberFormatInfo.CurrencyDecimalDigits = 0;

            switch (notification.NotificationTypeID)
            {
                case (int)NotificationType.Limit:
                    notification.Title = $"Лимит '{ notification.Name }'";
                    notification.Message = "Цена достигла <strong>" + (notification.Total ?? 0).ToString("C", numberFormatInfo) + "</strong>";
                    notification.Color = "bg-danger";
                    notification.Icon = "lnr lnr-frame-expand";
                    break;
                case (int)NotificationType.Reminder:
                    break;
                default:
                    break;
            }

        }
    }


    public class LimitNotificationCheker
    {
        public PeriodTypesEnum PeriodType { get; internal set; }
        public decimal LimitMoney { get; internal set; }
        public decimal? Total { get; internal set; }
        public int NotificationID { get; internal set; }
        public List<int> SectionIDs { get; internal set; }
    }
}
