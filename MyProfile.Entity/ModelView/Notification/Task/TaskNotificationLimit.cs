using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Notification.Task
{
    public class TaskNotificationLimit
    {
        public int NotificationID { get; set; }
        public int? LimitID { get; set; }
        public decimal LimitMoney { get; set; }
        public List<int> SectionIDs { get; set; }
        public int PeriodTypeID { get; set; }
        public Guid UserID { get; set; }
        public bool IsAllowCollectiveBudget { get; set; }
    }

    public class TaskNotificationReminder
    {
        public int NotificationID { get; set; }
        public int ReminderDateID { get; set; }
        public bool IsRepeat { get; set; }
        public Guid UserID { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public int UTCOffsetMinutes { get; set; }
    }
}
