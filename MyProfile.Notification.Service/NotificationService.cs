using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Progress.Service;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MyProfile.Notification.Service
{
    using Notification = Entity.Model.Notification;
    public class NotificationService
    {
        private BaseRepository repository;
        private ProgressService progressService;

        public NotificationService(BaseRepository repository,
            ProgressService progressService)
        {
            this.repository = repository;
            this.progressService = progressService;
        }

        public List<NotificationViewModel> GetLastNotification(int skip, int take, Expression<Func<Notification, bool>> expression = null)
        {
            var notifications = repository.GetAll(expression)
                 .Select(x => new NotificationViewModel
                 {
                     NotificationID = x.ID,
                     NotificationTypeID = x.NotificationTypeID,
                     IsRead = x.IsRead,
                     ReadDateTime = x.ReadDateTime,
                     ReadyDateTime = x.IsReadyDateTime,
                     UserUTCOffsetMinutes = x.User.OlsonTZID != null ? x.User.OlsonTZ.TimeZone.UTCOffsetMinutes : 180,
                     UserConnectionIDs = x.User.UserConnect.HubConnects
                                .Select(y => y.ConnectionID)
                                .ToList(),

                     Name = x.LimitID != null
                                    ? x.Limit.Name
                                    : x.ReminderDateID != null
                                        ? x.ReminderDate.Reminder.Title
                                        : x.TelegramAccountID != null
                                            ? x.TelegramAccount.TelegramID.ToString()
                                            : "",

                     TelegramName = x.TelegramAccountID != null ? x.TelegramAccount.LastName != null ? x.TelegramAccount.FirstName + " " + x.TelegramAccount.LastName : x.TelegramAccount.FirstName : null,
                     TelegramUserName = x.TelegramAccountID != null ? x.TelegramAccount.Username : null,
                     //Limit
                     Total = x.Total,
                     SpecificCulture = x.User.Currency.SpecificCulture,

                     //Reminder
                     ExpirationDateTime = x.ExpirationDateTime,
                     ReminderUTCOffsetMinutes = x.ReminderDateID != null ? x.ReminderDate.Reminder.OlsonTZ.TimeZone.UTCOffsetMinutes : 0,
                     Icon = x.ReminderDateID != null ? x.ReminderDate.Reminder.CssIcon : null,
                 })
                 .OrderByDescending(x => x.ReadyDateTime)
                 .Skip(skip)
                 .Take(take)
                 .ToList();

            for (int i = 0; i < notifications.Count; i++)
            {
                if (notifications[i].ReadyDateTime.HasValue)
                {
                    notifications[i].ReadyDateTime = notifications[i].ReadyDateTime.Value.AddMinutes(notifications[i].UserUTCOffsetMinutes);
                }

                GetMessage(notifications[i]);
            }

            return notifications;
        }

        public void SetRead(List<long> ids)
        {
            var now = DateTime.Now.ToUniversalTime();
            var notifications = repository.GetAll<Entity.Model.Notification>(x => x.IsRead == false && x.IsSentOnSite && ids.Contains(x.ID)).ToList();

            for (int i = 0; i < notifications.Count; i++)
            {
                notifications[i].IsRead = true;
                notifications[i].ReadDateTime = now;
            }

            repository.Save();
        }

        public void GetMessage(NotificationViewModel notification)
        {
            switch (notification.NotificationTypeID)
            {
                case (int)NotificationType.Limit:
                    NumberFormatInfo numberFormatInfo = new CultureInfo(notification.SpecificCulture, false).NumberFormat;
                    numberFormatInfo.CurrencyDecimalDigits = 0;

                    notification.Title = $"По лимиту '{ notification.Name }' достигнуто значение<strong> " + (notification.Total ?? 0).ToString("C", numberFormatInfo) + "</strong>";
                    notification.Message = "Лимит";
                    notification.Color = "bg-danger";
                    notification.Icon = "lnr lnr-frame-expand";
                    notification.NotifyType = "warning";
                    break;
                case (int)NotificationType.Reminder:
                    DateTime expirationDateTime = notification.ExpirationDateTime.Value.AddMinutes(notification.ReminderUTCOffsetMinutes);

                    notification.Title = "Напоминание";
                    notification.Message = $"'{ notification.Name }' <strong>" + expirationDateTime.ToString("dd MM yyyy HH:mm") + "</strong>";
                    notification.Color = "bg-primary";
                    //notification.Icon = "lnr lnr-frame-expand";
                    notification.NotifyType = "success";
                    break;
                case (int)NotificationType.Telegram:
                    notification.Title = $"Telegram уведомления";
                    notification.Message = $"Ваш аккаунт <b>{ (string.IsNullOrEmpty(notification.TelegramUserName) ? notification.TelegramName + " " + notification.Name : notification.TelegramUserName + " " + notification.Name) }</b> подключен к Telegram-боту <b>Budgetto_bot</b>. Теперь вы можете получать все уведомления в Telegram.";
                    notification.Color = "bg-primary";
                    notification.Icon = "fab fa-telegram-plane";
                    notification.NotifyType = "success";
                    break;
                default:
                    break;
            }

        }


        /// <summary>
        /// Create, update and delete notifications
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="notifications"></param>
        /// <param name="objectID">Limit, Reminder and etc.</param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<bool> CreateOrUpdate<T>(List<NotificationUserViewModel> notifications, long objectID, Guid userID)
        {
            var now = DateTime.Now.ToUniversalTime();
            bool anyChanges = false;
            var currentUser = UserInfo.Current;

            #region Delete
            if (notifications.Any(x => x.IsDeleted))
            {
                var notificationForDelete = notifications.Where(x => x.IsDeleted).Select(x => x.ID).ToList();
                foreach (var id in notificationForDelete)
                {
                    repository.Delete<Notification>(x => x.ID == id);
                }
                await repository.SaveAsync();
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
                        //Icon = notification.Icon,
                        IsMail = notification.IsMail,
                        IsSite = notification.IsSite,
                        IsTelegram = notification.IsTelegram,
                        LastChangeDateTime = now,

                        Total = notification.Price,
                    };

                    if (typeof(T) == typeof(Limit))
                    {
                        newNotification.LimitID = objectID;
                        newNotification.NotificationTypeID = (int)NotificationType.Limit;
                    }
                    else if (typeof(T) == typeof(ReminderDate))
                    {
                        newNotification.ReminderDateID = objectID;
                        newNotification.NotificationTypeID = (int)NotificationType.Reminder;
                        newNotification.ExpirationDateTime = notification.ExpirationDateTime.Value.ToUniversalTime();
                        newNotification.IsRepeat = notification.IsRepeat;
                    }

                    newNotifications.Add(newNotification);
                    anyChanges = true;
                    notification.ID = newNotification.ID;
                }
                repository.CreateRange(newNotifications);
                await repository.SaveAsync();

                #region Progress

                if (currentUser.IsCompleteIntroductoryProgress == false)
                {
                    await progressService.CompleteProgressItemType(currentUser.ID, ProgressTypeEnum.Introductory, ProgressItemTypeEnum.CreateNotification);
                }

                #endregion
            }
            #endregion


            #region Update 

            if (notifications.Any(x => x.IsDeleted == false && x.ID > 0))
            {
                foreach (var notification in notifications.Where(x => x.IsDeleted == false && x.ID > 0))
                {
                    DateTime? expirationDateTime = null;
                    if (notification.ExpirationDateTime.HasValue)
                    {
                        expirationDateTime = notification.ExpirationDateTime.Value.ToUniversalTime();
                    }

                    //we are tring to find any changes
                    var dbNotification = repository.GetAll<Notification>(x =>
                        x.ID == notification.ID
                        && (x.IsMail != notification.IsMail
                            || x.IsTelegram != notification.IsTelegram
                            || x.IsSite != notification.IsSite
                            || x.Total != notification.Price
                            || x.ExpirationDateTime != expirationDateTime)
                    ).FirstOrDefault();

                    if (dbNotification != null)
                    {
                        dbNotification.IsMail = notification.IsMail;
                        dbNotification.IsSite = notification.IsSite;
                        dbNotification.IsTelegram = notification.IsTelegram;
                        //dbNotification.Icon = notification.Icon;
                        dbNotification.LastChangeDateTime = now;
                        dbNotification.IsReady = false;
                        dbNotification.IsSentOnSite = false;
                        dbNotification.IsSentOnTelegram = false;
                        dbNotification.IsSentOnMail = false;
                        dbNotification.IsDone = false;
                        dbNotification.IsRead = false;
                        dbNotification.ReadDateTime = null;
                        dbNotification.IsReadyDateTime = null;

                        dbNotification.Total = notification.Price;

                        if (notification.ExpirationDateTime.HasValue)
                        {
                            dbNotification.ExpirationDateTime = notification.ExpirationDateTimeUTC;
                            dbNotification.IsRepeat = notification.IsRepeat;
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


    public class LimitNotificationCheker
    {
        public PeriodTypesEnum PeriodType { get; internal set; }
        public decimal LimitMoney { get; internal set; }
        public decimal? Total { get; internal set; }
        public int NotificationID { get; internal set; }
        public List<int> SectionIDs { get; internal set; }
    }
}
