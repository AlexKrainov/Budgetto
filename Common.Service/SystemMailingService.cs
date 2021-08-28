using MyProfile.Common;
using MyProfile.Entity.Model;
using MyProfile.Entity.Repository;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Service
{
    public class SystemMailingService
    {
        private IBaseRepository repository;

        public SystemMailingService(IBaseRepository repository)
        {
            this.repository = repository;
        }

        public int CheckSystemMailings()
        {
            var now = TimeZoner.GetCurrentDateTimeUTC();
            var systemMailings = repository.GetAll<SystemMailing>(x => x.IsActive).ToList();
            int checkerItems = 0;

            #region Check new system mailing for user
            List<Notification> newNotifications = new List<Notification>();

            var users = repository.GetAll<User>(x => x.IsDeleted != true)
                 .Select(x => new
                 {
                     UserID = x.ID,
                     Notifications = x.Notifications
                         .Where(y => y.NotificationTypeID == (int)NotificationType.SystemMailing)
                         .Select(y => new
                         {
                             y.ID,
                             y.SystemMailingID,
                             y.ExpirationDateTime
                         })
                         .ToList()
                 })
                 .ToList();

            foreach (var userNotifications in users)
            {
                foreach (var systemMailing in systemMailings)
                {
                    if (systemMailing.IsActive)
                    {
                        if (!userNotifications.Notifications.Any(x => systemMailing.ID == x.SystemMailingID)) //create
                        {
                            var notification = new Notification
                            {
                                ExpirationDateTime = null,
                                SystemMailingID = systemMailing.ID,
                                UserID = userNotifications.UserID,
                                IsMail = systemMailing.IsMail,
                                IsSite = systemMailing.IsSite,
                                IsTelegram = systemMailing.IsTelegram,
                                IsDone = false,
                                NotificationTypeID = (int)NotificationType.SystemMailing,
                                LastChangeDateTime = now,
                            };
                            switch ((SystemMailingType)systemMailing.ID)
                            {
                                case SystemMailingType.FeedbackMonth:
                                case SystemMailingType.StatisticsWeek:
                                case SystemMailingType.StatisticsMonth:
                                    CronExpression cronExpression = new CronExpression(systemMailing.CronExpression);
                                    notification.ExpirationDateTime = cronExpression.GetNextValidTimeAfter(now).GetValueOrDefault().DateTime;
                                    break;
                                case SystemMailingType.NotActive1DayAfterRegistration:
                                case SystemMailingType.NotActive2DaysAfterRegistration:
                                case SystemMailingType.NotActive3DaysAfterRegistration:
                                case SystemMailingType.NotActive4DaysAfterRegistration:
                                case SystemMailingType.NotActive5DaysAfterRegistration:
                                case SystemMailingType.NotActive6DaysAfterRegistration:
                                case SystemMailingType.NotActive7DaysAfterRegistration:
                                //case SystemMailingType.NotEnterRecords2Days:
                                //case SystemMailingType.NotEnterRecords3Days:
                                //case SystemMailingType.NotEnterRecords4Days:
                                    notification.ExpirationDateTime = now.AddMinutes(systemMailing.TotalMinutes ?? 0);
                                    break;
                                default:
                                    break;
                            }

                            if (notification.ExpirationDateTime != null)
                            {
                                newNotifications.Add(notification);
                            }
                        }
                        else //update
                        {
                            //var notification = repository.GetAll<Notification>(x =>
                            //    x.UserID == userNotifications.UserID
                            //    && x.SystemMailingID == systemMailing.ID
                            //    && x.NotificationTypeID == (int)NotificationType.SystemMailing
                            //    && (x.IsTelegram != systemMailing.IsTelegram
                            //        || x.IsSite != systemMailing.IsSite 
                            //        || x.IsMail != systemMailing.IsMail))
                            //.FirstOrDefault();

                            //if (notification != null)
                            //{
                            //    notification.IsMail = systemMailing.IsMail;
                            //    notification.IsSite = systemMailing.IsSite;
                            //    notification.IsTelegram = systemMailing.IsTelegram;
                            //    repository.Update(notification, true);
                            // }
                        }
                    }
                    else if (systemMailing.IsActive == false)
                    {
                        //var notification = repository.GetAll<Notification>(x =>
                        //        x.UserID == userNotifications.UserID
                        //        && x.SystemMailingID == systemMailing.ID
                        //        && x.NotificationTypeID == (int)NotificationType.SystemMailing
                        //        && (x.IsTelegram || x.IsSite || x.IsMail))
                        //    .FirstOrDefault();

                        //if (notification != null)
                        //{
                        //    notification.IsMail = false;
                        //    notification.IsSite = false;
                        //    notification.IsTelegram = false;
                        //    repository.Update(notification, true);
                        //}
                    }
                }

                if (newNotifications.Count == 500)
                {
                    repository.CreateRange(newNotifications, true);
                    newNotifications = new List<Notification>();
                }
            }
            if (newNotifications.Count > 0)
            {
                repository.CreateRange(newNotifications, true);
            }

            #endregion


            //var notify = repository.GetAll<Notification>(x => x.ID == 1301).FirstOrDefault();
            //notify.ExpirationDateTime = now.AddMonths(-1);
            //repository.Save();

            #region Checker notifications

            var _systemMailing = systemMailings.FirstOrDefault(x => x.ID == (int)SystemMailingType.FeedbackMonth);

            #region SystemMailingType.FeedbackMonth
            if (_systemMailing != null)
            {
                var notifications = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.SystemMailing
                    && x.SystemMailingID == _systemMailing.ID
                    && x.IsDone == false
                    && x.IsReady == false
                    && (x.IsMail || x.IsSite || x.IsTelegram)
                    && x.ExpirationDateTime <= now);

                foreach (var notification in notifications)
                {
                    notification.IsReady = true;
                    notification.IsReadyDateTime = now;
                    notification.LastChangeDateTime = now;
                    checkerItems += 1;
                }
                repository.Save();

                //Update IsDone notifications
                notifications = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.SystemMailing
                   && x.SystemMailingID == _systemMailing.ID
                   && x.IsDone == true
                   && x.IsReady == true
                   && (x.IsMail || x.IsSite || x.IsTelegram)
                   && x.ExpirationDateTime <= now);

                CronExpression cronExpression = new CronExpression(_systemMailing.CronExpression);
                var expirationDateTime = cronExpression.GetNextValidTimeAfter(now).GetValueOrDefault().DateTime;

                foreach (var notification in notifications)
                {
                    notification.IsDone = false;
                    notification.IsReady = false;
                    notification.IsReadyDateTime = null;
                    notification.ExpirationDateTime = expirationDateTime;
                    notification.IsSentOnMail = notification.IsSentOnSite = notification.IsSentOnTelegram = false;
                    notification.LastChangeDateTime = now;
                    checkerItems += 1;
                }
                repository.Save();
            }
            #endregion

            #region SystemMailingType.NotActive1DayAfterRegistration ...

            var _systemMailings = systemMailings.Where(x =>
                 x.ID == (int)SystemMailingType.NotActive1DayAfterRegistration
                 || x.ID == (int)SystemMailingType.NotActive2DaysAfterRegistration
                 || x.ID == (int)SystemMailingType.NotActive3DaysAfterRegistration
                 || x.ID == (int)SystemMailingType.NotActive4DaysAfterRegistration
                 || x.ID == (int)SystemMailingType.NotActive5DaysAfterRegistration
                 || x.ID == (int)SystemMailingType.NotActive6DaysAfterRegistration
                 || x.ID == (int)SystemMailingType.NotActive7DaysAfterRegistration);

            var _users = repository.GetAll<User>(x => x.IsDeleted != true && x.DateCreate >= (now.AddDays(-8)))
                .Select(x => new
                {
                    x.ID,
                    x.DateCreate,
                    LastUserSession = x.UserSessions
                        .Select(y => y.EnterDate)
                        .OrderByDescending(y => y)
                        .FirstOrDefault()
                })
                .ToList();

            foreach (var user in _users)
            {
                foreach (var systemMailing in _systemMailings)
                {
                    if (user.LastUserSession.AddMinutes(systemMailing.TotalMinutes ?? 0) <= now)
                    {
                        var notification = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.SystemMailing
                           && x.UserID == user.ID
                           && x.SystemMailingID == systemMailing.ID
                           && x.IsDone == false
                           && x.IsReady == false
                           && (x.IsMail || x.IsSite || x.IsTelegram)
                           && x.ExpirationDateTime <= now)
                            .FirstOrDefault();

                        if (notification != null)
                        {
                            notification.IsReady = true;
                            notification.IsReadyDateTime = now;
                            notification.LastChangeDateTime = now;

                            checkerItems += 1;
                            repository.Save();
                        }
                    }
                }
            }

            #endregion

            #region SystemMailingType.StatisticsWeek, StatisticsMonth

            _systemMailings = systemMailings.Where(x =>
               x.ID == (int)SystemMailingType.StatisticsWeek
               || x.ID == (int)SystemMailingType.StatisticsMonth);

            foreach (var systemMailing in _systemMailings)
            {
                var notifications = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.SystemMailing
                    && x.SystemMailingID == systemMailing.ID
                    && x.IsDone == false
                    && x.IsReady == false
                    && (x.IsMail || x.IsSite || x.IsTelegram)
                    && x.ExpirationDateTime <= now
                    && x.User.UserSessions
                        .Select(y => y.EnterDate)
                        .OrderByDescending(y => y)
                        .FirstOrDefault() >= now.AddDays(-14));

                foreach (var notification in notifications)
                {
                    notification.IsReady = true;
                    notification.IsReadyDateTime = now;
                    notification.LastChangeDateTime = now;
                    checkerItems += 1;
                }
                repository.Save();

                //Update IsDone notifications
                notifications = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.SystemMailing
                   && x.SystemMailingID == systemMailing.ID
                   && x.IsDone == true
                   && x.IsReady == true
                   && (x.IsMail || x.IsSite || x.IsTelegram)
                   && x.ExpirationDateTime <= now);

                CronExpression cronExpression = new CronExpression(systemMailing.CronExpression);
                var expirationDateTime = cronExpression.GetNextValidTimeAfter(now).GetValueOrDefault().DateTime;

                foreach (var notification in notifications)
                {
                    notification.IsDone = false;
                    notification.IsReady = false;
                    notification.IsReadyDateTime = null;
                    notification.ExpirationDateTime = expirationDateTime;
                    notification.IsSentOnMail = notification.IsSentOnSite = notification.IsSentOnTelegram = false;
                    notification.LastChangeDateTime = now;
                    checkerItems += 1;
                }
                repository.Save();
            }
            #endregion

            //#region SystemMailingType.NotEnterRecords2Days ...

            //var notEnterRecords2Days_systemMailing = systemMailings.FirstOrDefault(x =>
            //     x.ID == (int)SystemMailingType.NotEnterRecords2Days);

            //var notEnterRecords3Days_systemMailing = systemMailings.FirstOrDefault(x =>
            //     x.ID == (int)SystemMailingType.NotEnterRecords3Days
            //     );

            //var notEnterRecords4Days_systemMailing = systemMailings.FirstOrDefault(x =>
            //    x.ID == (int)SystemMailingType.NotEnterRecords4Days);

            //var notEnterUsers = repository.GetAll<User>(x => x.IsDeleted != true)
            //     .Select(x => new
            //     {
            //         x.ID,
            //         LastUserRecord = x.BudgetRecords
            //             .Select(y => y.DateTimeCreate)
            //             .OrderByDescending(y => y)
            //             .FirstOrDefault(),
            //         LastUserSession = x.UserSessions
            //             .Select(y => y.EnterDate)
            //             .OrderByDescending(y => y)
            //             .FirstOrDefault()
            //     })
            //     .Where(x => x.LastUserRecord >= now.AddDays(-5)
            //        && x.LastUserRecord <= now.AddHours(-48))
            //     .ToList();

            //foreach (var user in notEnterUsers)
            //{
            //    if (user.LastUserRecord >= now.AddMinutes((notEnterRecords2Days_systemMailing.TotalMinutes ?? 0) - 1)
            //        && user.LastUserRecord <= now.AddMinutes((notEnterRecords3Days_systemMailing.TotalMinutes ?? 0) - 1))
            //    {

            //    }
            //    //Update IsDone notifications
            //    var _notifications = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.SystemMailing
            //       && x.UserID == user.ID
            //       && x.SystemMailingID == notEnterRecords2Days_systemMailing.ID
            //       && x.IsDone == true
            //       && x.IsReady == true
            //       && (x.IsMail || x.IsSite || x.IsTelegram)
            //       && x.ExpirationDateTime <= now);

            //    foreach (var notification in _notifications)
            //    {
            //        notification.IsDone = false;
            //        notification.IsReady = false;
            //        notification.IsReadyDateTime = null;
            //        notification.ExpirationDateTime = now.AddMinutes(systemMailing.TotalMinutes ?? 0);
            //        notification.IsSentOnMail = notification.IsSentOnSite = notification.IsSentOnTelegram = false;
            //        notification.LastChangeDateTime = now;
            //        checkerItems += 1;
            //    }
            //    repository.Save();

            //    if (user.LastUserRecord.AddMinutes(systemMailing.TotalMinutes ?? 0) <= now)
            //    {
            //        var notification = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.SystemMailing
            //           && x.SystemMailingID == systemMailing.ID
            //           && x.IsDone == false
            //           && x.IsReady == false
            //           && (x.IsMail || x.IsSite || x.IsTelegram)
            //           && x.ExpirationDateTime <= now)
            //            .FirstOrDefault();

            //        if (notification != null)
            //        {
            //            notification.IsReady = true;
            //            notification.IsReadyDateTime = now;
            //            notification.LastChangeDateTime = now;

            //            checkerItems += 1;
            //            repository.Save();
            //        }
            //    }

            //}

            //#endregion

            #endregion

            return checkerItems;
        }

        //private DateTime? getFutureExpirationDate(DateTime now, DateTime futureDate, CronExpression cronExpression)
        //{
        //    if (now > dateTime)
        //    {
        //        futureDate = cronExpression.GetNextValidTimeAfter(now).GetValueOrDefault().DateTime;
        //        return getFutureExpirationDate(dateTime, cronExpression)
        //    }
        //    return dateTime;
        //}
    }
}
