using Common.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification;
using MyProfile.Entity.ModelView.Reminder;
using MyProfile.Entity.Repository;
using MyProfile.Identity;
using MyProfile.Notification.Service;
using MyProfile.UserLog.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Reminder.Service
{
    using Reminder = MyProfile.Entity.Model.Reminder;
    using ReminderDate = MyProfile.Entity.Model.ReminderDate;
    using Notification = MyProfile.Entity.Model.Notification;
    public partial class ReminderService
    {
        private IBaseRepository repository;
        private UserLogService userLogService;
        private CommonService commonService;
        private NotificationService notificationService;

        public ReminderService(IBaseRepository baseRepository, UserLogService userLogService, CommonService commonService, NotificationService notificationService)
        {
            this.repository = baseRepository;
            this.userLogService = userLogService;
            this.commonService = commonService;
            this.notificationService = notificationService;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentDate"></param>
        /// <param name="dateFinish">For period</param>
        /// <returns></returns>
        public async Task<IList<ReminderEditModelView>> GetRimindersByDate(DateTime currentDate, DateTime? dateFinish = null)
        {
            var moscowOlzonTZID = commonService.GetTimeZones().FirstOrDefault(x => x.OlzonTZName == "Europe/Moscow").OlzonTZID;
            var currenUserID = UserInfo.Current.ID;
            var expression = PredicateBuilder.True<ReminderDate>();
            expression = expression.And(x => x.Reminder.UserID == currenUserID
                    && x.Reminder.IsDeleted == false);

            var start = currentDate.ToUniversalTime();
            var finish = start.AddDays(1);

            if (dateFinish != null)
            {
                finish = dateFinish.Value.ToUniversalTime();
            }

            expression = expression.And(x => x.DateReminder >= start && x.DateReminder <= finish);

            var reminders = await repository
                .GetAll<ReminderDate>(expression)
                 .Select(x => new ReminderEditModelView
                 {
                     ID = x.ReminderID,
                     ReminderDateID = x.ID,
                     DateReminder = x.DateReminder,
                     Description = x.Reminder.Description,
                     IsRepeat = x.Reminder.IsRepeat,
                     RepeatEvery = x.Reminder.RepeatEvery,
                     Title = x.Reminder.Title,
                     CssIcon = x.Reminder.CssIcon,
                     isShowForFilter = true,
                     isDeleted = false,
                     isWasRepeat = x.Reminder.ReminderDates.Count() > 1,
                     OffSetClient = x.Reminder.OlsonTZID != null ? x.Reminder.OlsonTZ.TimeZone.UTCOffsetMinutes : 180,
                     OlzonTZID = x.Reminder.OlsonTZID,
                     Notifications = x.Notifications != null ?
                        x.Notifications.Select(y => new NotificationUserViewModel
                        {
                            ID = y.ID,
                            IsMail = y.IsMail,
                            IsSite = y.IsSite,
                            IsTelegram = y.IsTelegram,
                            ExpirationDateTime = y.ExpirationDateTime, // error -> .HasValue && x.Reminder.OlsonTZID != null ? y.ExpirationDateTime.Value.AddMinutes(x.Reminder.OlsonTZ.TimeZone.UTCOffsetMinutes) : y.ExpirationDateTime.Value,
                            IsRepeat = y.IsRepeat
                        }).ToList() : null
                 })
                 .OrderBy(x => x.DateReminder)
                 .ToListAsync();

            for (int i = 0; i < reminders.Count; i++)
            {
                if (reminders[i].OlzonTZID == null)//old version
                {
                    reminders[i].DateReminder = reminders[i].OldDateReminder = reminders[i].DateReminder.Value.AddHours(10);// set 10:00
                    reminders[i].OlzonTZID = moscowOlzonTZID;
                }
                else
                {
                    reminders[i].DateReminder = reminders[i].OldDateReminder = reminders[i].DateReminder.Value.AddMinutes(reminders[i].OffSetClient);
                }

                reminders[i].DateReminderString = reminders[i].DateReminder.Value.ToString("dd.MM.yyyy");
                reminders[i].RepeatEveryName = LocalizatoinRepeat(reminders[i].RepeatEvery);

                for (int y = 0; y < reminders[i].Notifications.Count; y++)
                {
                    reminders[i].Notifications[y].ExpirationDateTime = reminders[i].Notifications[y].ExpirationDateTime.Value.AddMinutes(reminders[i].OffSetClient);
                }
            }

            return reminders;
        }

        public int SetDoneForAllPastRemindersDate()
        {
            var now = DateTime.Now.ToUniversalTime();

            var reminderDates = repository.GetAll<ReminderDate>(x => x.Reminder.IsDeleted == false
                    && x.IsDone == false
                    && x.DateReminder.Date <= now.Date)
                .ToList();

            for (int i = 0; i < reminderDates.Count; i++)
            {
                reminderDates[i].IsDone = true;

                if (i % 500 == 0)
                {
                    repository.Save();
                }
            }

            repository.Save();

            return reminderDates.Count;
        }

        public async Task<bool> CreateOrUpdate(ReminderEditModelView newReminder)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();
            var newDateReminder = newReminder.DateReminder.Value.ToUniversalTime();
            bool isNotificationChange = false;

            try
            {
                if (newReminder.ID > 0)//Update
                {
                    DateTime? dateRemoveFrom = null;
                    bool isCreateCurrentReminderDate = false;
                    var oldReminder = await repository.GetAll<Reminder>(x => x.ID == newReminder.ID
                            && x.UserID == currentUser.ID
                            && x.IsDeleted == false)
                        .FirstOrDefaultAsync();

                    if (newReminder.IsRepeat == false)
                    {
                        newReminder.RepeatEvery = null;
                    }

                    if (!oldReminder.ReminderDates.Any(x => x.DateReminder == newDateReminder))
                    {
                        if (newReminder.IsRepeat == false && oldReminder.IsRepeat == false)
                        {
                            oldReminder.ReminderDates.FirstOrDefault(x => x.ID == newReminder.ReminderDateID).IsDone = now > newDateReminder;
                            oldReminder.ReminderDates.FirstOrDefault(x => x.ID == newReminder.ReminderDateID).DateReminder = newDateReminder;

                            await notificationService.CreateOrUpdate<ReminderDate>(newReminder.Notifications, newReminder.ReminderDateID, currentUser.ID);
                            isNotificationChange = true;
                        }
                        else if (oldReminder.IsRepeat && newReminder.IsRepeat)
                        {
                            if ((newReminder.DateReminder - newReminder.OldDateReminder).Value.TotalMilliseconds != 0)
                            {
                                //await repository.DeleteAsync(await repository.GetAll<ReminderDate>(x => x.ReminderID == newReminder.ID
                                //                      && x.Reminder.UserID == currentUser.ID
                                //                      && x.Reminder.IsDeleted == false
                                //                      && x.ID == newReminder.ReminderDateID).FirstOrDefaultAsync(), true);
                                oldReminder.ReminderDates.FirstOrDefault(x => x.ID == newReminder.ReminderDateID).IsDone = now > newDateReminder;
                                oldReminder.ReminderDates.FirstOrDefault(x => x.ID == newReminder.ReminderDateID).DateReminder = newDateReminder;

                                await notificationService.CreateOrUpdate<ReminderDate>(newReminder.Notifications, newReminder.ReminderDateID, currentUser.ID);
                                isNotificationChange = true;
                            }
                            isCreateCurrentReminderDate = true;
                        }
                        else if (newReminder.IsRepeat != oldReminder.IsRepeat)
                        {
                            if (newReminder.IsRepeat)
                            {
                                oldReminder.ReminderDates.FirstOrDefault().DateReminder = newDateReminder;
                                oldReminder.ReminderDates.FirstOrDefault().IsDone = now > newDateReminder;
                            }
                            else //if newReminder.IsRepeat == false
                            {
                                var tmp = oldReminder.ReminderDates.FirstOrDefault(x => x.DateReminder.ToUniversalTime() == newReminder.OldDateReminder.ToUniversalTime());
                                tmp.DateReminder = newReminder.DateReminder ?? newReminder.OldDateReminder.Date;
                                tmp.IsDone = now.Date > newReminder.DateReminder.Value;

                                dateRemoveFrom = newReminder.OldDateReminder.ToUniversalTime();
                            }
                        }
                    }

                    if (newReminder.IsRepeat == false && oldReminder.IsRepeat)
                    {
                        //remove until reminder.DateReminder
                        await repository.DeleteRangeAsync(await repository.GetAll<ReminderDate>(x => x.ReminderID == newReminder.ID
                            && x.Reminder.UserID == currentUser.ID
                            && x.Reminder.IsDeleted == false
                            && x.DateReminder > (dateRemoveFrom ?? newDateReminder)).ToListAsync(), true);
                    }
                    else if (newReminder.IsRepeat && oldReminder.IsRepeat == false)
                    {
                        var tmp = oldReminder.ReminderDates.ToList();
                        tmp.AddRange(GetReminderDates(newReminder, currentUser.ID, isCreateCurrentReminderDate: false));
                        oldReminder.ReminderDates = tmp;
                        if (newReminder.Notifications != null && newReminder.Notifications.Count() > 0)
                        {
                            isNotificationChange = true;
                        }
                    }
                    else if (newReminder.IsRepeat
                        && oldReminder.IsRepeat
                        && newReminder.RepeatEvery != oldReminder.RepeatEvery)
                    {
                        await repository.DeleteRangeAsync(await repository.GetAll<ReminderDate>(x => x.ReminderID == newReminder.ID
                            && x.Reminder.UserID == currentUser.ID
                            && x.Reminder.IsDeleted == false
                            && x.DateReminder > newDateReminder).ToListAsync(), true);

                        var tmp = oldReminder.ReminderDates.ToList();
                        tmp.AddRange(GetReminderDates(newReminder, currentUser.ID, isCreateCurrentReminderDate: isCreateCurrentReminderDate));
                        oldReminder.ReminderDates = tmp;

                        if (newReminder.Notifications != null && newReminder.Notifications.Count() > 0)
                        {
                            isNotificationChange = true;
                        }
                    }

                    oldReminder.Title = newReminder.Title;
                    oldReminder.Description = newReminder.Description;
                    oldReminder.DateReminder = newDateReminder;
                    oldReminder.DateEdit = now;
                    oldReminder.IsRepeat = newReminder.IsRepeat;
                    oldReminder.RepeatEvery = newReminder.RepeatEvery;
                    oldReminder.CssIcon = newReminder.CssIcon ?? "pe-7s-alarm";
                    oldReminder.OffSetClient = newReminder.OffSetClient;
                    oldReminder.TimeZoneClient = newReminder.TimeZoneClient;
                    oldReminder.OlsonTZID = newReminder.OlzonTZID;

                    await repository.UpdateAsync(oldReminder, true);
                    await notificationService.CreateOrUpdate<ReminderDate>(newReminder.Notifications, newReminder.ReminderDateID, currentUser.ID);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Edit);

                    if (isNotificationChange)
                    {
                        await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Notification);
                    }
                }
                else
                {
                    var reminder = new Reminder();

                    reminder.UserID = currentUser.ID;
                    reminder.DateEdit = reminder.DateCreate = now;

                    reminder.Title = newReminder.Title;
                    reminder.Description = newReminder.Description;
                    reminder.DateReminder = newReminder.DateReminder.Value.ToUniversalTime();
                    reminder.IsRepeat = newReminder.IsRepeat;
                    reminder.RepeatEvery = newReminder.RepeatEvery;
                    reminder.CssIcon = newReminder.CssIcon ?? "pe-7s-alarm";
                    reminder.OffSetClient = newReminder.OffSetClient;
                    reminder.TimeZoneClient = newReminder.TimeZoneClient;
                    reminder.OlsonTZID = newReminder.OlzonTZID;
                    reminder.ReminderDates = GetReminderDates(newReminder, currentUser.ID);

                    await repository.CreateAsync(reminder, true);
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Create);
                    if (newReminder.Notifications != null && newReminder.Notifications.Count() > 0)
                    {
                        await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Notification);
                    }
                    newReminder.ID = reminder.ID;
                    var notifications = reminder.ReminderDates.FirstOrDefault(x => x.DateReminder.Date == reminder.DateReminder.Value.Date).Notifications;

                    foreach (var notification in newReminder.Notifications)
                    {
                        notification.ID = notifications.FirstOrDefault(x => x.ExpirationDateTime.Value.Date == notification.ExpirationDateTime.Value.Date).ID;
                    }
                }
            }
            catch (Exception ex)
            {
                var errorID = await userLogService.CreateErrorLogAsync(currentUser.UserSessionID, "ReminderService_CreateOrUpdate", ex);

                if (newReminder.ID > 0)
                {
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Edit, errorLogIDs: new List<int>(errorID));
                }
                else
                {
                    await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Create, errorLogIDs: new List<int>(errorID));
                }

                return false;
            }

            newReminder.DateReminderString = newReminder.DateReminder.Value.ToString("dd.MM.yyyy");
            newReminder.RepeatEveryName = LocalizatoinRepeat(newReminder.RepeatEvery);

            //newReminder.Notifications = repository.GetAll<ReminderDate>(x => x.ID == newReminder.ID)

            return true;
        }

        public IQueryable<ReminderShortModelView> GetRemindersByDateRange(DateTime from, DateTime to, Guid currentUserID)
        {
            from = from.ToUniversalTime();
            to = to.ToUniversalTime();

            return repository
                .GetAll<ReminderDate>(x => x.Reminder.UserID == currentUserID
                && x.Reminder.IsDeleted == false
                && x.DateReminder >= from
                && x.DateReminder <= to)
                .Select(x => new ReminderShortModelView
                {
                    CssIcon = x.Reminder.CssIcon,
                    DateReminder = x.DateReminder,
                    Description = x.Reminder.Description,
                    ReminderID = x.ReminderID,
                    Title = x.Reminder.Title,
                    IsRepeat = x.Reminder.IsRepeat,
                    IsDone = x.IsDone,
                });
        }

        public async Task<bool> RemoveOrRecovery(ReminderEditModelView reminderEdit, bool isDelete)
        {
            var currentUser = UserInfo.Current;
            var now = DateTime.Now.ToUniversalTime();

            var reminder = await repository.GetAll<Reminder>(x => x.ID == reminderEdit.ID
                    && x.UserID == currentUser.ID)
                .FirstOrDefaultAsync();

            reminder.DateEdit = now;
            reminder.IsDeleted = reminderEdit.isDeleted = isDelete;

            try
            {
                await repository.UpdateAsync(reminder, true);
                await userLogService.CreateUserLogAsync(currentUser.UserSessionID, UserLogActionType.Reminder_Delete);

            }
            catch (Exception ex)
            {

                return false;
            }
            return true;
        }

        public List<ReminderDate> GetReminderDates(ReminderEditModelView reminderEdit, Guid userID, bool isCreateCurrentReminderDate = true)
        {
            var now = DateTime.Now.ToUniversalTime();
            var dateReminder = reminderEdit.DateReminder.Value.ToUniversalTime();
            List<ReminderDate> reminderDates = new List<ReminderDate>();
            DateTime? IsReadyDateTime = null;
            DateTime expirationDateTimeUTC;
            List<Notification> notifications;

            if (isCreateCurrentReminderDate)
            {
                var reminderDate = new ReminderDate
                {
                    DateReminder = dateReminder,
                    IsDone = now.Date > dateReminder,
                    ReminderID = reminderEdit.ID
                };

                notifications = new List<Notification>();
                foreach (var notification in reminderEdit.Notifications)
                {
                    IsReadyDateTime = null;
                    if (now >= notification.ExpirationDateTimeUTC)
                    {
                        IsReadyDateTime = now;
                    }

                    notifications.Add(new Notification
                    {
                        IsMail = notification.IsMail,
                        IsSite = notification.IsSite,
                        IsTelegram = notification.IsTelegram,
                        ExpirationDateTime = notification.ExpirationDateTimeUTC,
                        IsRepeat = notification.IsRepeat,
                        NotificationTypeID = (int)NotificationType.Reminder,
                        UserID = userID,
                        LastChangeDateTime = now,

                        IsReady = now >= notification.ExpirationDateTimeUTC,
                        IsReadyDateTime = IsReadyDateTime
                    });
                }
                reminderDate.Notifications = notifications;

                reminderDates.Add(reminderDate);
            }

            if (reminderEdit.IsRepeat && reminderEdit.DateReminder != null)
            {
                reminderEdit.DateReminder = reminderEdit.DateReminder.Value.ToUniversalTime();
                if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Day))
                {
                    for (int i = 1; i < 765; i++)
                    {
                        dateReminder = reminderEdit.DateReminder.Value.AddDays(i);

                        //reminderDates.Add(new ReminderDate
                        //{
                        //    DateReminder = dateReminder,
                        //    IsDone = now.Date > dateReminder,
                        //    ReminderID = reminderEdit.ID,
                        //});

                        var reminderDate = new ReminderDate
                        {
                            DateReminder = dateReminder,
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID
                        };

                        notifications = new List<Notification>();
                        foreach (var notification in reminderEdit.Notifications)
                        {
                            if (notification.IsRepeat)
                            {
                                expirationDateTimeUTC = notification.ExpirationDateTimeUTC.Value.AddDays(i);
                                IsReadyDateTime = null;
                                if (now >= expirationDateTimeUTC)
                                {
                                    IsReadyDateTime = now;
                                }

                                notifications.Add(new Notification
                                {
                                    IsMail = notification.IsMail,
                                    IsSite = notification.IsSite,
                                    IsTelegram = notification.IsTelegram,
                                    ExpirationDateTime = expirationDateTimeUTC,
                                    IsRepeat = notification.IsRepeat,
                                    NotificationTypeID = (int)NotificationType.Reminder,
                                    UserID = userID,
                                    LastChangeDateTime = now,

                                    IsReady = now >= expirationDateTimeUTC,
                                    IsReadyDateTime = IsReadyDateTime
                                });
                            }
                        }
                        reminderDate.Notifications = notifications;

                        reminderDates.Add(reminderDate);
                    }
                }
                else
                if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Week))
                {
                    for (int i = 1; i < 100; i++)
                    {
                        dateReminder = reminderEdit.DateReminder.Value.AddDays(i * 7);

                        var reminderDate = new ReminderDate
                        {
                            DateReminder = dateReminder,
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID
                        };

                        notifications = new List<Notification>();
                        foreach (var notification in reminderEdit.Notifications)
                        {
                            if (notification.IsRepeat)
                            {
                                expirationDateTimeUTC = notification.ExpirationDateTimeUTC.Value.AddDays(i * 7);
                                IsReadyDateTime = null;
                                if (now >= expirationDateTimeUTC)
                                {
                                    IsReadyDateTime = now;
                                }

                                notifications.Add(new Notification
                                {
                                    IsMail = notification.IsMail,
                                    IsSite = notification.IsSite,
                                    IsTelegram = notification.IsTelegram,
                                    ExpirationDateTime = expirationDateTimeUTC,
                                    IsRepeat = notification.IsRepeat,
                                    NotificationTypeID = (int)NotificationType.Reminder,
                                    UserID = userID,
                                    LastChangeDateTime = now,

                                    IsReady = now >= expirationDateTimeUTC,
                                    IsReadyDateTime = IsReadyDateTime
                                });
                            }
                        }
                        reminderDate.Notifications = notifications;

                        reminderDates.Add(reminderDate);
                    }
                }
                else if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Month))
                {
                    for (int i = 1; i < 24; i++)
                    {
                        dateReminder = reminderEdit.DateReminder.Value.AddMonths(i);

                        var reminderDate = new ReminderDate
                        {
                            DateReminder = dateReminder,
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID
                        };

                        notifications = new List<Notification>();
                        foreach (var notification in reminderEdit.Notifications)
                        {
                            if (notification.IsRepeat)
                            {
                                expirationDateTimeUTC = notification.ExpirationDateTimeUTC.Value.AddMonths(i);
                                IsReadyDateTime = null;
                                if (now >= expirationDateTimeUTC)
                                {
                                    IsReadyDateTime = now;
                                }

                                notifications.Add(new Notification
                                {
                                    IsMail = notification.IsMail,
                                    IsSite = notification.IsSite,
                                    IsTelegram = notification.IsTelegram,
                                    ExpirationDateTime = expirationDateTimeUTC,
                                    IsRepeat = notification.IsRepeat,
                                    NotificationTypeID = (int)NotificationType.Reminder,
                                    UserID = userID,
                                    LastChangeDateTime = now,

                                    IsReady = now >= expirationDateTimeUTC,
                                    IsReadyDateTime = IsReadyDateTime
                                });
                            }
                        }
                        reminderDate.Notifications = notifications;

                        reminderDates.Add(reminderDate);

                    }
                }
                else if (reminderEdit.RepeatEvery == Enum.GetName(typeof(RepeatEveryType), RepeatEveryType.Year))
                {
                    for (int i = 1; i < 4; i++)
                    {
                        dateReminder = reminderEdit.DateReminder.Value.AddYears(i);

                        var reminderDate = new ReminderDate
                        {
                            DateReminder = dateReminder,
                            IsDone = now.Date > dateReminder,
                            ReminderID = reminderEdit.ID
                        };

                        notifications = new List<Notification>();
                        foreach (var notification in reminderEdit.Notifications)
                        {
                            if (notification.IsRepeat)
                            {
                                expirationDateTimeUTC = notification.ExpirationDateTimeUTC.Value.AddYears(i);
                                IsReadyDateTime = null;
                                if (now >= expirationDateTimeUTC)
                                {
                                    IsReadyDateTime = now;
                                }

                                notifications.Add(new Notification
                                {
                                    IsMail = notification.IsMail,
                                    IsSite = notification.IsSite,
                                    IsTelegram = notification.IsTelegram,
                                    ExpirationDateTime = expirationDateTimeUTC,
                                    IsRepeat = notification.IsRepeat,
                                    NotificationTypeID = (int)NotificationType.Reminder,
                                    UserID = userID,
                                    LastChangeDateTime = now,

                                    IsReady = now >= expirationDateTimeUTC,
                                    IsReadyDateTime = IsReadyDateTime
                                });
                            }
                        }
                        reminderDate.Notifications = notifications;

                        reminderDates.Add(reminderDate);
                    }
                }
            }

            return reminderDates;
        }

        private string LocalizatoinRepeat(string repeatEvery)
        {
            switch (repeatEvery)
            {
                case "Day":
                    return "День";
                case "Week":
                    return "Неделя";
                case "Month":
                    return "Месяц";
                case "Year":
                    return "Год";
                default:
                    return "";
            }
        }
    }
}
