using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Service
{
    public class NotificationUpdater
    {
        private BaseRepository repository;

        public NotificationUpdater(BaseRepository repository)
        {
            this.repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notifications"></param>
        /// <param name="objectID">Limit, Reminder and etc.</param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<bool> CreateOrUpdate<T>(IEnumerable<NotificationUserViewModel> notifications, int objectID, Guid userID)
        {
            var now = DateTime.Now;
            bool anyChanges = false;

            #region Delete
            if (notifications.Any(x => x.IsDeleted))
            {
                var notificationForDelete = notifications.Where(x => x.IsDeleted).Select(x => x.ID).ToList();
                foreach (var id in notificationForDelete)
                {
                    repository.Delete<Notification>(id);
                }

            }
            #endregion

            #region Create

            if (notifications.Any(x => x.ID < 0))
            {
                List<Notification> newNotifications = new List<Notification>();

                foreach (var notification in notifications.Where(x => x.ID < 0))
                {
                    var newNotification = new Notification
                    {
                        UserID = userID,
                        Icon = notification.Icon,
                        IsMail = notification.IsMail,
                        IsSite = notification.IsSite,
                        IsTelegram = notification.IsTelegram,
                        LastChangeDateTime = now,

                        Total = notification.Price,

                        ExpirationDateTime = notification.ExpirationDateTime,
                    };

                    if (typeof(T) == typeof(Limit))
                    {
                        newNotification.LimitID = objectID;
                        newNotification.NotificationTypeID = (int)NotificationType.Limit;
                    }
                    else if (typeof(T) == typeof(Reminder))
                    {
                        newNotification.ReminderID = objectID;
                        newNotification.NotificationTypeID = (int)NotificationType.Reminder;
                    }

                    newNotifications.Add(newNotification);
                    anyChanges = true;
                }
                repository.CreateRange(newNotifications);
            }
            #endregion
            await repository.SaveAsync();

            #region Update 

            if (notifications.Any(x => x.IsDeleted == false && x.ID > 0))
            {
                foreach (var notification in notifications.Where(x => x.IsDeleted == false && x.ID > 0))
                {
                    //we are tring to find any changes
                    var dbNotification = repository.GetAll<Notification>(x =>
                        x.ID == notification.ID
                        && (x.IsMail != notification.IsMail
                            || x.IsTelegram != notification.IsTelegram
                            || x.IsSite != notification.IsSite
                            || x.Total != notification.Price
                            || x.ExpirationDateTime != notification.ExpirationDateTime)
                    ).FirstOrDefault();

                    if (dbNotification != null)
                    {
                        dbNotification.IsMail = notification.IsMail;
                        dbNotification.IsSite = notification.IsSite;
                        dbNotification.IsTelegram = notification.IsTelegram;
                        dbNotification.Icon = notification.Icon;
                        dbNotification.LastChangeDateTime = now;
                        dbNotification.IsReady = false;
                        dbNotification.IsSent = false;
                        dbNotification.IsDone = false;
                        dbNotification.IsRead = false;
                        dbNotification.ReadDateTime = null;

                        if (typeof(T) == typeof(Limit))
                        {
                            dbNotification.Total = notification.Price;
                        }
                        else if (typeof(T) == typeof(Reminder))
                        {
                            dbNotification.ExpirationDateTime = notification.ExpirationDateTime;
                        }

                        await repository.SaveAsync();
                        anyChanges = true;
                    }
                }
            }

            #endregion
            return anyChanges;
        }
    }
}
