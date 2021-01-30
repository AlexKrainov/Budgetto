using MyProfile.Entity.Model;
using MyProfile.Entity.ModelView.Reminder;
using System;
using System.Collections.Generic;

namespace MyProfile.Entity.ModelView.BudgetView
{
    public class TableViewModel
    {
    }
    public class Cell : ICloneable
    {
        /// <summary>
        ///  Format 36 903,35 ₽
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Format 36903.35
        /// </summary>
        public decimal NaturalValue { get; set; }
        public bool IsShow { get; set; } = true;
        public TemplateColumnType TemplateColumnType { get; set; }
        public int dateCounter { get; set; }
        public bool IsWeekend { get; set; }
        public bool IsHoliday { get; set; }
        public DateTime CurrentDate { get; set; }
        public List<ReminderCell> Reminders { get; set; } = new List<ReminderCell>();

        /// <summary>
        /// Too long, because boxing and unboxing
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public Cell CloneObject()
        {
            return new Cell
            {
                Value = this.Value,
                CurrentDate = this.CurrentDate,
                dateCounter = this.dateCounter,
                IsHoliday = this.IsHoliday,
                IsShow = this.IsShow,
                IsWeekend = this.IsWeekend,
                NaturalValue = this.NaturalValue,
                TemplateColumnType = this.TemplateColumnType,
                Reminders = this.Reminders,
            };
        }
    }
    public class FooterCell : Cell
    {
    }

    public class ReminderCell
    {
        public string CssIcon { get; set; }
        public int Count { get; set; }
        public string Titles { get; set; }
        public bool IsRepeat { get; set; }
        public bool IsDone { get; set; }
    }

}
