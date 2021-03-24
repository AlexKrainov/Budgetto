using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification.Task;
using MyProfile.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyProfile.Limit.Service
{
    using Notification = MyProfile.Entity.Model.Notification;
    public partial class LimitService
    {

        /// <summary>
        /// if limitID is empty we are check notification by UserID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int CheckLimitNotifications()
        {
            return CheckLimitNotificationsAsync().Result;
        }
        /// <summary>
        /// if limitID is empty we are check notification by UserID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<int> CheckLimitNotificationsAsync(int? limitID = null, Guid? userID = null)
        {
            int _checkLimits = 0;
            var predicate = PredicateBuilder.True<Notification>();

            predicate = predicate.And(x =>
                            x.NotificationTypeID == (int)NotificationType.Limit
                            && x.IsReady == false
                            && x.IsDone == false);

            if (limitID != null)
            {
                predicate = predicate.And(x => x.LimitID == limitID);
            }
            else if (userID != null)
            {
                predicate = predicate.And(x => x.UserID == userID);
            }
            else
            {
                predicate = predicate.And(x => x.LimitID != null);
            }

            try
            {
                var limitNotifications = await repository.GetAll(predicate)
                          .Select(x => new TaskNotificationLimit
                          {
                              NotificationID = x.ID,
                              LimitID = x.LimitID,
                              PeriodTypeID = x.Limit.PeriodTypeID,
                              LimitMoney = x.Total ?? 0,
                              UserID = x.UserID,
                              IsAllowCollectiveBudget = x.User.IsAllowCollectiveBudget,
                              SectionIDs = x.Limit.SectionGroupLimits
                                  .Select(y => y.BudgetSectionID)
                                  .ToList(),
                          })
                          .ToListAsync();

                foreach (var taskNotificationLimit in limitNotifications)
                {
                    bool isReady = await CheckLimit(taskNotificationLimit);

                    if (isReady)
                    {
                        _checkLimits += 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Guid? userSessionID = null;
                if (userID != null)
                {
                    userSessionID = UserInfo.Current.UserSessionID;
                }

                await repository.CreateAsync(new ErrorLog
                {
                    ErrorText = ex.Message,
                    CurrentDate = DateTime.Now,
                    Where = "LimitNotificationService.CheckNotifications",
                    UserSessionID = userSessionID
                }, true);
            }

            return _checkLimits;
        }


        public async Task<bool> CheckLimit(TaskNotificationLimit notificationLimit)
        {
            var now = DateTime.Now;
            bool isThis = false;
            DateTime start = new DateTime(),
                finish = new DateTime();

            if (notificationLimit.PeriodTypeID == (int)PeriodTypesEnum.Month)
            {
                start = new DateTime(now.Year, now.Month, 01, 00, 00, 00);
                finish = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59);

                isThis = finish.Month == now.Month && finish.Year == now.Year;// month/year
            }
            else if (notificationLimit.PeriodTypeID == (int)PeriodTypesEnum.Year)
            {
                start = new DateTime(now.Year, 1, 01, 00, 00, 00);
                finish = new DateTime(now.Year, 12, 31, 23, 59, 59);
                isThis = finish.Year == now.Year;// month/year
            }

            var filter = new Entity.ModelView.CalendarFilterModels
            {
                StartDate = start,
                EndDate = finish,
                Sections = notificationLimit.SectionIDs,
                UserID = notificationLimit.UserID,
                IsConsiderCollection = notificationLimit.IsAllowCollectiveBudget
            };

            if (filter.IsConsiderCollection)
            {
                filter.Sections.AddRange(await sectionService.GetCollectionSectionIDsBySectionID(filter.Sections));
            }

            var totalSpended = await budgetRecordService.GetTotalSpendsForLimitByFilter(filter);
            var leftMoneyToSpend = notificationLimit.LimitMoney - totalSpended;

            if (isThis && leftMoneyToSpend < 0)
            {
                //"Вы превысили лимит на";
                var notification = repository.GetAll<Notification>(x => x.ID == notificationLimit.NotificationID)
                                .FirstOrDefault();
                notification.IsReady = true;
                notification.IsReadyDateTime = now;
                repository.Save(); //??
                return true;
            }
            return false;
        }


        /// <summary>
        ///  Reset all notification what need every month (1st 10:00):
        /// </summary>
        /// <returns></returns>
        public int NotificationsReset()
        {
            int items = 0;

            try
            {
                var notifications = repository.GetAll<Notification>(x => x.NotificationTypeID == (int)NotificationType.Limit)
                        .ToList();

                for (int i = 0; i < notifications.Count; i++)
                {
                    notifications[i].IsSentOnMail = false;
                    notifications[i].IsSentOnSite = false;
                    notifications[i].IsSentOnTelegram = false;

                    notifications[i].IsDone = false;
                    notifications[i].IsRead = false;
                    notifications[i].IsReady = false;
                    notifications[i].IsReadyDateTime = null;
                    notifications[i].ReadDateTime = null;
                }

                repository.Save();
                items = notifications.Count;
            }
            catch (Exception ex)
            {
                 repository.Create(new ErrorLog
                {
                    ErrorText = ex.Message,
                    CurrentDate = DateTime.Now,
                    Where = "LimitNotificationService.NotificationsReset",
                }, true);
            }

            return items;
        }
    }
}
