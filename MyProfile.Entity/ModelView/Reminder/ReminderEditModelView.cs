using MyProfile.Entity.ModelView.Notification;
using System;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView.Reminder
{
    public enum RepeatEveryType
    {
        Undefined = 0,
        Day,
        Week,
        Month,
        Year
    }

    public class ReminderEditModelView
    {
        public bool isShowForFilter = true;
        public bool isDeleted;
        public bool isWasRepeat;

        public int ID { get; set; }
        public DateTime? DateReminder { get; set; }
        public string Description { get; set; }
        public bool IsRepeat { get; set; }
        public string RepeatEvery { get; set; }
        public string Title { get; set; }
        public string CssIcon { get; set; }
        public string DateReminderString { get; set; }
        public string RepeatEveryName { get; set; }
        public DateTime OldDateReminder { get; set; }
        public int OffSetClient { get; set; }
        public string TimeZoneClient { get; set; }
        public int? OlzonTZID { get; set; }
        public List<NotificationUserViewModel> Notifications { get; set; }
        public int ReminderDateID { get; set; }
    }
}
