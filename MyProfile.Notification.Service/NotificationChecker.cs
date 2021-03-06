using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Notification.Task;
using MyProfile.Entity.Repository;
using MyProfile.Limit.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MyProfile.Notification.Service
{
    public class NotificationChecker
    {
        private BaseRepository repository;
        private LimitService limitService;

        public NotificationChecker(BaseRepository repository,
            LimitService limitService)
        {
            this.repository = repository;
            this.limitService = limitService;
        }

        public int CheckLimits()
        {
            var now = DateTime.Now;
            int _checkLimits = 0;
            try
            {
                var limitNotifications = repository.GetAll<MyProfile.Entity.Model.Notification>(x =>
                        x.NotificationTypeID == (int)NotificationType.Limit
                        && x.LimitID != null
                        && x.IsReady == false
                        && x.IsDone == false)
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
                     .ToList();

                DateTime start = new DateTime(now.Year, now.Month, 01, 00, 00, 00);
                DateTime finish = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month), 23, 59, 59);

                foreach (var taskNotificationLimit in limitNotifications)
                {
                    _checkLimits += 1;
                    limitService.CheckLimit(start, finish, taskNotificationLimit).Wait();
                }
            }
            catch (Exception ex)
            {

            }

            return _checkLimits;
        }
    }
}
