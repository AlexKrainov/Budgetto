using System;
using System.Collections.Generic;
using System.Text;

namespace MyProfile.Entity.ModelView.Reminder
{
    public class ReminderShortModelView
    {
        public int ReminderID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CssIcon { get; set; }
        public DateTime? DateReminder { get; set; }
        public bool IsRepeat { get; set; }
    }
}
