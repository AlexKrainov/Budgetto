using LinqKit;
using Microsoft.EntityFrameworkCore;
using MyProfile.Entity.Model;
using System;
using System.Threading.Tasks;

namespace MyProfile.Reminder.Service
{
    using Notification = MyProfile.Entity.Model.Notification;
    public partial class ReminderService
    {
        public int CheckReminderNotifications()
        {
            return CheckReminderNotificationsAsync().Result;
        }
        public async Task<int> CheckReminderNotificationsAsync() //Guid? userID = null)
        {
            int _checkReminders = 0;
            var now = DateTime.Now.ToUniversalTime();
            var predicate = PredicateBuilder.True<Notification>();

            var taskSheduler = await repository.GetAll<SchedulerTask>(x =>
                x.TaskType == Enum.GetName(typeof(TaskType), TaskType.NotificationReminderCheckerTask))
                .FirstOrDefaultAsync();
            DateTime lastStart = (taskSheduler.LastStart ?? DateTime.Now.ToUniversalTime().AddDays(-1)).AddMinutes(-1);

            predicate = predicate.And(x =>
                            x.NotificationTypeID == (int)NotificationType.Reminder
                            && x.IsReady == false
                            && x.IsDone == false
                            && x.ExpirationDateTime >= lastStart
                            && x.ExpirationDateTime <= now
                            && x.ReminderDateID != null 
                            && x.ReminderDate.Reminder.IsDeleted != true);

            //if (userID != null)
            //{
            //    predicate = predicate.And(x => x.UserID == userID);
            //}
            //else
            //{
            //    predicate = predicate.And(x => x.ReminderDateID != null && x.ReminderDate.Reminder.IsDeleted != true);
            //}

            try
            {
                var notifications = await repository.GetAll(predicate)
                          .ToListAsync();

                foreach (var notification in notifications)
                {
                    notification.IsReady = true;
                    notification.IsReadyDateTime = now;
                    _checkReminders += 1;
                }
                taskSheduler.LastStart = now;
                await repository.SaveAsync();
            }
            catch (Exception ex)
            {
                Guid? userSessionID = null;
                //if (userID != null)
                //{
                //    userSessionID = UserInfo.Current?.UserSessionID;
                //}

                await repository.CreateAsync(new ErrorLog
                {
                    ErrorText = ex.Message,
                    CurrentDate = DateTime.Now.ToUniversalTime(),
                    Where = "ReminderNotificationService.CheckReminderNotifications",
                    UserSessionID = userSessionID
                }, true);
            }
            return _checkReminders;
        }
    }
}
