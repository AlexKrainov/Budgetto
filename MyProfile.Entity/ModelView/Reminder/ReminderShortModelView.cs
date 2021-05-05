using System;

namespace MyProfile.Entity.ModelView.Reminder
{
    public class ReminderShortModelView
    {
        public long ReminderID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CssIcon { get; set; }
        public DateTime? DateReminder { get; set; }
        public bool IsRepeat { get; set; }
        public bool IsDone { get; set; }
    }
}
