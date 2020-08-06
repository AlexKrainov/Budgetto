﻿using System;

namespace MyProfile.Entity.ModelView.Reminder
{
    public class ReminderEditModelView
    {
        public bool isShowForFilter;
        public bool isDeleted;

        public int ID { get; set; }
        public DateTime DateCreate { get; set; }
        public DateTime DateEdit { get; set; }
        public DateTime? DateReminder { get; set; }
        public string Description { get; set; }
        public bool IsRepeat { get; set; }
        public string RepeatEvery { get; set; }
        public string Title { get; set; }
        public string CssIcon { get; set; }
    }
}
